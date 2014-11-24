using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;  //为SQLite数据库服务
using System.Reflection;         //程序集反射以及类型获取
using System.IO;   //读写文件流

namespace SuperHelper_SQLite
{
    // SH_Query类的分部定义之一
    // 这里定义的是直接访问数据库的ExecuteQuery方法
    public partial class SH_Query<T>
    {
        /// <summary>
        /// SuperHelper_SQLServer内部方法；
        /// 根据数据库连接字符串、sql语句、访问数据库的方式以及SQLiteParameter数组，执行访问数据库操作，
        /// 并返回T类型的List集合
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="queryStr">表示sql语句的字符串</param>
        /// <param name="param">可变的SQLiteParameter参数数组</param>
        /// <returns>以T类型的List集合的方式返回查询结果集</returns>
        private static List<T> ExecuteQuery(string connStr, string queryStr,  params SQLiteParameter[] param)
        {
            try
            {
                Type return_type = typeof(T);

                //定义T类型集合，用于返回查询结果
                List<T> lst_obj = new List<T>();

                //链接数据库
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(queryStr, conn))
                    {
                        conn.Open();


                        if (param != null)
                        {
                            cmd.Parameters.AddRange(param);
                        }

                        //执行查询语句
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            //如果查询返回结果中含有行的话，执行下面语句
                            if (reader.HasRows)
                            {
                                //如果return_type是用户自定义的class类型，则以该类的公共字段和公共属性为基准，从结果集中匹配列并获取其值
                                #region 如果return_type类型是用户自定义的class类型
                                if (BaseMethods.GetParamType(return_type) == ParamType.UserClass)
                                {
                                    //1、定义字典，键存储该类与查询结果集匹配的公共字段或者属性名，值存储结果集中对应的列索引
                                    Dictionary<string, int> dic_Properties = new Dictionary<string, int>();
                                    Dictionary<string, int> dic_Fields = new Dictionary<string, int>();

                                    #region 关于对用户自定义class类型中用户自定义class类型成员的支持的测试
                                    Dictionary<string, string> dic_userClassMember = new Dictionary<string, string>(); //key用于存储与该成员对应的结果集中的列名，value用于存储用户自定义class类型的成员的名字
                                    #endregion

                                    #region 2、获取return_type类的所有公共属性、公共字段和结果集的所有列名,并进行匹配
                                    //2、获取return_type类的所有公共属性、公共字段和结果集的所有列名,并进行匹配
                                    PropertyInfo[] properties = return_type.GetProperties();
                                    FieldInfo[] fields = return_type.GetFields();

                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        for (int j = 0; j < properties.Length; j++)
                                        {
                                            if (reader.GetName(i) == properties[j].Name)
                                            {
                                                dic_Properties.Add(properties[j].Name, i);
                                                break;
                                                //Type t=properties[]
                                            }

                                            #region 关于对用户自定义class类型中用户自定义class类型成员的支持的测试
                                            //将结果集列名中含有“.”的列赋值给用户自定义class类型属性
                                            string[] reader_FieldName = reader.GetName(i).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);   //先对结果集中列名用“.”进行分割，如果reader_FieldName的数量大于1，则表明该列名含有“.”
                                            if (reader_FieldName.Length > 1)
                                            {
                                                //如果分割后的第一个字符串符合该类的某一个属性名
                                                if (reader_FieldName[0] == properties[j].Name)
                                                {
                                                    //同时该属性类型为用户自定义类型，则将其添加到待处理的字符串集合lst_userClassProper
                                                    if (BaseMethods.GetParamType(properties[j].PropertyType) == ParamType.UserClass)
                                                    {
                                                        dic_userClassMember.Add(reader.GetName(i), properties[j].Name);
                                                    }
                                                }
                                            }
                                            #endregion

                                        }
                                        for (int k = 0; k < fields.Length; k++)
                                        {
                                            if (reader.GetName(i) == fields[k].Name)
                                            {
                                                dic_Fields.Add(fields[k].Name, i);
                                                break;
                                            }
                                            #region 关于对用户自定义class类型中用户自定义class类型成员的支持的测试
                                            //将结果集列名中含有“.”的列赋值给用户自定义class类型字段
                                            string[] reader_FieldName = reader.GetName(i).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);   //先对结果集中列名用“.”进行分割，如果reader_FieldName的数量大于1，则表明该列名含有“.”
                                            if (reader_FieldName.Length > 1)
                                            {
                                                //如果分割后的第一个字符串符合该类的某一个公共字段名
                                                if (reader_FieldName[0] == fields[k].Name)
                                                {
                                                    //同时该字段类型为用户自定义类型，则将其添加到待处理的字符串集合lst_userClassProper
                                                    if (BaseMethods.GetParamType(fields[k].FieldType) == ParamType.UserClass)
                                                    {
                                                        dic_userClassMember.Add(reader.GetName(i), fields[k].Name);
                                                    }
                                                }
                                            }
                                            #endregion
                                        }

                                    }
                                    #endregion

                                    #region 3、遍历结果集数据行
                                    //3、遍历结果集数据行
                                    while (reader.Read())
                                    {
                                        //4、创建return_type类型的对象实例
                                        object obj = Activator.CreateInstance(return_type);
                                        //T t = new T();
                                        #region 5、遍历字典dic_Properties，为特定的对象实例的公共属性赋值
                                        //5、遍历字典dic_Properties，为特定的对象实例的公共属性赋值
                                        foreach (KeyValuePair<string, int> item in dic_Properties)
                                        {
                                            //这里要考虑结果集中有可能存在空值
                                            if (reader[item.Value] == DBNull.Value)
                                            {
                                                object o = null;
                                                return_type.GetProperty(item.Key).SetValue(obj, o, null);
                                            }
                                            else
                                            {
                                                return_type.GetProperty(item.Key).SetValue(obj, reader[item.Value], null);
                                            }

                                        }
                                        #endregion

                                        #region 6、遍历字典dic_Fields，为特定的对象实例的公共字段赋值
                                        //6、遍历字典dic_Fields，为特定的对象实例的公共字段赋值
                                        foreach (KeyValuePair<string, int> item in dic_Fields)
                                        {
                                            //这里要考虑结果集中有可能存在空值
                                            if (reader[item.Value] == DBNull.Value)
                                            {
                                                object o = null;
                                                return_type.GetField(item.Key).SetValue(obj, o);
                                            }
                                            else
                                            {
                                                return_type.GetField(item.Key).SetValue(obj, reader[item.Value]);
                                            }
                                        }
                                        #endregion

                                        #region 关于对用户自定义class类型中用户自定义class类型成员的支持的测试
                                        //遍历dic_userClassMember集合，为用户自定义class类型成员赋值
                                        foreach (KeyValuePair<string, string> userClassMember in dic_userClassMember)
                                        {
                                            if (return_type.GetProperty(userClassMember.Value) != null)
                                            {
                                                return_type.GetProperty(userClassMember.Value).SetValue(obj, BaseMethods.GetUserClassMemberValue(return_type, userClassMember.Key, reader[userClassMember.Key]), null);
                                            }
                                            else if (return_type.GetField(userClassMember.Value) != null)
                                            {
                                                return_type.GetField(userClassMember.Value).SetValue(obj, BaseMethods.GetUserClassMemberValue(return_type, userClassMember.Key, reader[userClassMember.Key]));
                                            }

                                        }
                                        #endregion

                                        //7、将添加好的对象实例obj添加到lst_obj中
                                        lst_obj.Add((T)obj);
                                    }
                                    #endregion

                                }
                                #endregion

                                //如果return_type是非用户自定义class类型，则将结果集中的数据按顺序添加到lst_obj集合中
                                #region 如果return_type类型是非用户自定义class类型
                                if (BaseMethods.GetParamType(return_type) != ParamType.UserClass)
                                {
                                    //读取结果集中的数据
                                    while (reader.Read())
                                    {
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            lst_obj.Add((T)reader[i]);
                                        }
                                    }
                                }
                                #endregion
                            }
                        } //dispose reader
                    } //dispose cmd
                } //dispose conn

                return lst_obj;
            }
            catch (Exception ex)
            {

                throw new Exception("来自" + HelperVar.DLL_Name + ".SH_Query<T>.ExecuteQuery方法的错误：" + ex.Message);
            }
        }

    }
}
