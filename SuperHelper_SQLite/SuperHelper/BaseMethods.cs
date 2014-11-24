using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;  //为SQLite数据库服务
using System.Text.RegularExpressions;  //正则表达式
using System.Reflection;         //程序集反射以及类型获取
using System.IO;   //读写文件流
using System.Configuration;  //读取config文件需要

namespace SuperHelper_SQLite
{
    /// <summary>
    /// BaseMethods类，只限SuperHelper_SQLite内部使用，
    /// 这里定义的主要是为SH_Query以及SH_NonQuery类提供基础服务的方法
    /// </summary>
    partial class BaseMethods
    {
        /// <summary>
        /// SuperHelper_SQLite内部方法，
        /// 根据sql语句、object类型List参数集合，生成含有该sql语句中所有参数以及对应的值的SQLiteParameter[]数组
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="pam_obj">object类型List参数集合</param>
        /// <returns>根据sql语句生成的SQLiteParameter[]数组</returns>
        public static SQLiteParameter[] BaseMethod_ParamProcessed(string sql, List<object> pam_obj)
        {
            //1、找出sql语句中所有参数

            #region 第一步，找出sql语句中所有参数
            //定义正则表达式，用于匹配出sql语句中所有参数
            string partten = @"[@](\w+)";
            MatchCollection mc = Regex.Matches(sql, partten);

            #endregion


            //此处要判断sql语句中有没有参数，有参数执行第二、第三步，没有则跳出方法
            if (mc.Count == 0)
            {
                return new SQLiteParameter[0];
            }

            //异常判断，如果sql语句中有参数，但参数集合pam_obj为空时
            if (mc.Count > 0 && pam_obj.Count == 0)
            {
                throw new Exception(HelperVar.DLL_Name + "组件捕捉到的异常：sql语句中有参数，但实参pam_obj为空！");

            }

            //对获取到的sql语句中的参数进行去重复处理，并处理结果存储到一个泛型集合中
            #region 对获取到的sql语句中的参数进行去重复处理，并处理结果存储到一个集合中
            List<string> str_param = new List<string>();
            foreach (Match item in mc)
            {
                if (!str_param.Contains(item.Groups[1].Value))
                {
                    str_param.Add(item.Groups[1].Value);
                }
            }

            SQLiteParameter[] param = new SQLiteParameter[str_param.Count];  //定义用来访问数据库的SQLiteParameter参数数组
            #endregion

            //2、遍历pam_obj集合，提取所有参数和值，包括class类型以及值类型，分别存与一个参数字典和object集合中，等待匹配
            #region 第二步，遍历pam_obj集合，提取所有参数和值

            //定义参数字典和object集合
            Dictionary<string, object> dic = new Dictionary<string, object>();
            List<object> obj = new List<object>();

            for (int i = 0; i < pam_obj.Count; i++)
            {
                //获取该类类型的type
                Type type = pam_obj[i].GetType();

                //如果pam_obj子元素是用户自定义的class类型，则将它的公共字段和属性作为参数名以及参数值存于参数字典dic中
                #region 如果pam_obj子元素是用户自定义的class类型
                if (BaseMethods.GetParamType(type) == ParamType.UserClass)
                {

                    //获取类的所有公共字段
                    FieldInfo[] fields = type.GetFields();
                    //获取类的所有公共属性
                    PropertyInfo[] properties = type.GetProperties();

                    //遍历该类所有公共字段
                    for (int j = 0; j < fields.Length; j++)
                    {
                        //判断参数字典的键中是否含有当前字段名，如果有，则覆盖该键的值
                        if (!dic.ContainsKey(fields[j].Name))
                        {
                            dic.Add(fields[j].Name, fields[j].GetValue(pam_obj[i]));
                        }
                        else
                        {
                            dic[fields[j].Name] = fields[j].GetValue(pam_obj[i]);
                        }
                    }
                    //遍历该类所有公共属性
                    for (int k = 0; k < properties.Length; k++)
                    {
                        //判断参数字典的键中是否含有当前属性名，如果有，则覆盖该键的值
                        if (!dic.ContainsKey(properties[k].Name))
                        {
                            dic.Add(properties[k].Name, properties[k].GetValue(pam_obj[i], null));
                        }
                        else
                        {
                            dic[properties[k].Name] = properties[k].GetValue(pam_obj[i], null);
                        }
                    }
                }
                #endregion
                //如果pam_obj子元素是非用户自定义class类型，则直接将它添加到参数集合obj中
                #region 如果pam_obj子元素是非用户自定义class类型
                else if (BaseMethods.GetParamType(type) != ParamType.UserClass)
                {
                    //如果pam_obj子元素是值类型，则直接将它添加到参数集合obj中
                    obj.Add(pam_obj[i]);
                }
                #endregion

            }
            #endregion


            //3、遍历SQL语句中的所有参数，优先从参数字典中匹配相应的参数名，获取对应的参数值，若没有找到，则从object数组中按索引顺序提取子元素作为参数值（若sql语句中有n个参数在字典中没有找到，而object数组数量小于n的话，抛出异常）
            #region 第三步，遍历SQL语句中的所有参数
            //SQLiteParameter[] param = new SQLiteParameter[mc.Count];  //定义用来访问数据库的SQLiteParameter参数数组
            int index_param = 0;        //指示局部变量param的当前索引值
            int index_obj = 0;      //指示object参数集合的当前索引值
            foreach (string str in str_param)
            {
                //先从参数字典中匹配相应的参数名，获取对应的参数值
                if (dic.ContainsKey(str))
                {
                    //这里需要注意：上面正则表达式第一组匹配项获取到的参数名是不带@的字符串，所以这里需要补上
                    param[index_param] = new SQLiteParameter("@" + str, dic[str]);
                }
                //若没有找到，则从object数组中按索引顺序提取子元素作为参数值
                else
                {
                    try//若sql语句中有n个参数在字典中没有找到，而object参数集合数量小于n的话，抛出异常
                    {
                        //这里需要注意：上面正则表达式第一组匹配项获取到的参数名是不带@的字符串，所以这里需要补上
                        param[index_param] = new SQLiteParameter("@" + str, obj[index_obj]);
                        index_obj += 1;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(HelperVar.DLL_Name + "组件捕捉到的异常：" + ex.Message);
                    }
                }
                index_param += 1;
            }
            #endregion

            return param;
        }

        /// <summary>
        /// SuperHelper_SQLite内部方法；
        /// 根据泛型类型参数type获取其ParamType的类型
        /// </summary>
        /// <param name="type">泛型类型参数</param>
        /// <returns>ParamType的枚举类型</returns>
        public static ParamType GetParamType(Type type)
        {
            #region 旧版本的Type类型判断方法（弃用）
            ////获取Type的类型全名，区分系统类型与用户自定义类型
            //MatchCollection mc = Regex.Matches(type.FullName, @"^System[.](\w+)");

            ////匹配到数据表明该类是系统提供的类型
            //if (mc.Count > 0)
            //{
            //    if (type.Name == "String")
            //    {
            //        return ParamType.String;
            //    }

            //    if (type.IsClass)
            //    {
            //        return ParamType.OtherSysClass;
            //    }

            //    if (type.IsValueType)
            //    {
            //        return ParamType.SysValue;
            //    }
            //}
            //else
            //{
            //    return ParamType.UserClass;
            //}

            //return ParamType.UnKown;
            #endregion

            //定义返回结果result，并赋初值
            ParamType result = ParamType.UnKown;

            //获取定义参数type类型的模块名称
            string str_Module = (type.Module).ScopeName;

            //用正则表达式检查匹配参数type的含命名空间的全称是否以"System."开头
            bool isSystem = Regex.IsMatch(type.FullName, @"^System[.](\w+)");

            //判断参数type为何种类型
            if (isSystem == true && str_Module == "CommonLanguageRuntimeLibrary")
            {
                if (type.IsClass == true)
                {
                    result = ParamType.OtherSysClass;
                }
                else if (type.Name == "String")
                {
                    result = ParamType.String;
                }
                else if (type.IsValueType)
                {
                    result = ParamType.SysValue;
                }
                else
                {
                    result = ParamType.UnKown;
                }
            }
            else
            {
                result = ParamType.UserClass;
            }

            return result;
        }

        /// <summary>
        /// SuperHelper_SQLite内部方法；
        /// 返回Name属性为HelperVar类中conStrName字段值的数据库连接字符串
        /// </summary>
        /// <returns>数据库连接字符串</returns>
        public static string GetConstr()
        {

            #region 1、判断项目配置文件中<connectionStrings>节点中是否有Name属性为HelperVar.conStrName的值的连接字符串
            //1、判断项目配置文件中<connectionStrings>节点中是否有Name属性为HelperVar.conStrName的值的连接字符串
            if (ConfigurationManager.ConnectionStrings[HelperVar.conStrName] == null)
            {
                throw new Exception(HelperVar.DLL_Name + "组件捕捉到的异常：找不到连接字符串！请为该组件设置一个Name属性为\"" + HelperVar.conStrName + "\"的连接字符串");
            }
            #endregion

            //2、若已存在，则直接使用该字符串
            return ConfigurationManager.ConnectionStrings[HelperVar.conStrName].ConnectionString;
        }

        /// <summary>
        /// SuperHelper_SQLite内部方法；
        /// 根据参数查找相应Name属性值的连接字符串，并返回
        /// </summary>
        /// <param name="conStrName">表示要返回的连接字符串的Name属性值</param>
        /// <returns>数据库连接字符串</returns>
        public static string GetConstr(string conStrName)
        {

            #region 1、判断项目配置文件中<connectionStrings>节点中是否有Name属性为conStrName的值的连接字符串
            //1、判断项目配置文件中<connectionStrings>节点中是否有Name属性为conStrName的值的连接字符串
            if (ConfigurationManager.ConnectionStrings[conStrName] == null)
            {
                throw new Exception(HelperVar.DLL_Name + "组件捕捉到的异常：找不到连接字符串！请为该组件设置一个Name属性为\"" + conStrName + "\"的连接字符串");
            }
            #endregion

            //2、若已存在，则直接使用该字符串
            return ConfigurationManager.ConnectionStrings[conStrName].ConnectionString;
        }

        /// <summary>
        /// SuperHelper_SQLite内部方法，
        /// 根据某类型对象的泛型类型，用户自定义class类型的公共成员名对该类某个公共成员进行赋值，并返回该成员类型的对象
        /// </summary>
        /// <param name="type">某类型对象的泛型类型，该泛型类型的对象含有需要赋值的class类型的属性</param>
        /// <param name="userClassMemberName">需要赋值的class类型的公共成员名称，如："A.Name"，A是上述type类型对象的公共成员之一，Name则是A类型对象的公共成员</param>
        /// <param name="userClassMemberValue">需要赋给指定成员的值，如："A.Name"，userClassMemberValue就是要赋给A对象的成员Name的值</param>
        /// <returns>已被赋值的class类型的公共成员对象，如："A.Name"，则返回A类型对象</returns>
        internal static object GetUserClassMemberValue(Type type, string userClassMemberName, object userClassMemberValue)
        {
            //1、将待赋值的名称用“.”分割
            string[] str_Names = userClassMemberName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            object obj_result = null;

            //2、根据指定的公共成员名称实例化一个对象并为其赋值
            #region 如果该公共成员是属性的话，则执行如下代码
            if (type.GetProperty(str_Names[0]) != null)
            {
                Type t_Property = type.GetProperty(str_Names[0]).PropertyType;   //参考例子：这里是为了获取type类型中属性A的泛型类型
                obj_result = Activator.CreateInstance(t_Property);  //根据属性A的泛型类型反射获取A类型对象

                //Type t_Proper_Member = null;
                //如果"A.Name"中的Name是A的属性，则执行下面操作
                if (t_Property.GetProperty(str_Names[1]) != null)
                {
                    //这里是为了获取t_Property类型中属性Name的泛型类型
                    Type t_Proper_Proper = t_Property.GetProperty(str_Names[1]).PropertyType;

                    //如果A的公共成员Name不是用户自定义的class类型，则为Name赋值
                    if (GetParamType(t_Proper_Proper) != ParamType.UserClass)
                    {
                        t_Property.GetProperty(str_Names[1]).SetValue(obj_result, userClassMemberValue, null);
                    }
                    else//如果A的公共成员Name也是一个class类型对象，则执行递归，直到获取到其被赋值的对象为止
                    {
                        //获取userClassMemberName字符串中从Name开始的那一段字符串（就是舍去"A.Name.abcd..."字符串中的"A."）
                        int index = userClassMemberName.IndexOf('.');
                        string str = userClassMemberName.Substring(index + 1);

                        obj_result = GetUserClassMemberValue(t_Property, str, userClassMemberValue);
                    }
                }
                //如果"A.Name"中的Name是A的字段，则执行下面操作
                else if (t_Property.GetField(str_Names[1]) != null)
                {
                    //这里是为了获取t_Property类型中字段Name的泛型类型
                    Type t_Proper_Field = t_Property.GetField(str_Names[1]).FieldType;

                    //如果A的公共成员Name不是用户自定义的class类型，则为Name赋值
                    if (GetParamType(t_Proper_Field) != ParamType.UserClass)
                    {
                        t_Property.GetField(str_Names[1]).SetValue(obj_result, userClassMemberValue);
                    }
                    else//如果A的公共成员Name也是一个class类型对象，则执行递归，直到获取到其被赋值的对象为止
                    {
                        //获取userClassMemberName字符串中从Name开始的那一段字符串（就是舍去"A.Name.abcd..."字符串中的"A."）
                        int index = userClassMemberName.IndexOf('.');
                        string str = userClassMemberName.Substring(index + 1);

                        obj_result = GetUserClassMemberValue(t_Property, str, userClassMemberValue);
                    }
                }

            }
            #endregion

            #region 如果该公共成员是字段的话，则执行如下代码
            else if (type.GetField(str_Names[0]) != null)
            {
                Type t_Field = type.GetField(str_Names[0]).FieldType;   //参考例子：这里是为了获取type类型中字段A的泛型类型
                obj_result = Activator.CreateInstance(t_Field);  //根据属性A的泛型类型反射获取A类型对象

                //如果"A.Name"中的Name是A的属性，则执行下面操作
                if (t_Field.GetProperty(str_Names[1]) != null)
                {
                    //这里是为了获取t_Field类型中属性Name的泛型类型
                    Type t_Field_Proper = t_Field.GetProperty(str_Names[1]).PropertyType;

                    //如果A的公共成员Name不是用户自定义的class类型，则为Name赋值
                    if (GetParamType(t_Field_Proper) != ParamType.UserClass)
                    {
                        t_Field.GetProperty(str_Names[1]).SetValue(obj_result, userClassMemberValue, null);
                    }
                    else//如果A的公共成员Name也是一个class类型对象，则执行递归，直到获取到其被赋值的对象为止
                    {
                        //获取userClassMemberName字符串中从Name开始的那一段字符串（就是舍去"A.Name.abcd..."字符串中的"A."）
                        int index = userClassMemberName.IndexOf('.');
                        string str = userClassMemberName.Substring(index + 1);

                        obj_result = GetUserClassMemberValue(t_Field, str, userClassMemberValue);
                    }
                }
                //如果"A.Name"中的Name是A的字段，则执行下面操作
                else if (t_Field.GetField(str_Names[1]) != null)
                {
                    //这里是为了获取t_Property类型中字段Name的泛型类型
                    Type t_Field_Field = t_Field.GetField(str_Names[1]).FieldType;

                    //如果A的公共成员Name不是用户自定义的class类型，则为Name赋值
                    if (GetParamType(t_Field_Field) != ParamType.UserClass)
                    {
                        t_Field.GetField(str_Names[1]).SetValue(obj_result, userClassMemberValue);
                    }
                    else//如果A的公共成员Name也是一个class类型对象，则执行递归，直到获取到其被赋值的对象为止
                    {
                        //获取userClassMemberName字符串中从Name开始的那一段字符串（就是舍去"A.Name.abcd..."字符串中的"A."）
                        int index = userClassMemberName.IndexOf('.');
                        string str = userClassMemberName.Substring(index + 1);

                        obj_result = GetUserClassMemberValue(t_Field, str, userClassMemberValue);
                    }
                }
            }
            #endregion

            return obj_result;
        }

    }
}
