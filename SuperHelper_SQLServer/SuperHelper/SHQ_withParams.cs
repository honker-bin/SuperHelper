using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;  //正则表达式
using System.Data.SqlClient;   //为mssqlserver数据库服务
using System.IO;     //读写文件流

namespace SuperHelper_SQLServer
{
    /// <summary>
    /// SH_Query泛型类，包含了一些对数据库进行查询操作的方法
    /// </summary>
    /// <typeparam name="T">查询数据库后返回结果集</typeparam>
    public static partial class SH_Query<T>
    {
        /// <summary>
        /// 根据sql语句，object类型List参数集合，查询数据库并返回指定T类型的List集合；
        /// 注意1：sql语句中的参数名要与pam_obj集合中开发者自定义class类型对象中的公共属性或公共字段名称保持一致；
        /// 注意2：若T为开发者自定义class类型，则其公共属性或公共字段名称要与执行sql语句后返回的查询结果集中的列名保持一致
        /// </summary>
        /// <param name="sql">SQL语句（查询命令；多语句情况下最后一句必须是查询命令）</param>
        /// <param name="pam_obj">object类型List参数集合，集合中可以有开发者自定义class类型对象或系统定义的值类型</param>
        /// <returns>以T类型的List集合的方式返回查询结果集</returns>
        public static List<T> GetListBySql(string sql, List<object> pam_obj)
        {
            List<T> result = new List<T>();  //用于存储返回结果的list集合

            //将参数的匹配与处理统一交给BaseMethod_ParamProcessed方法执行，方便以后的维护和管理
            SqlParameter[] param = BaseMethods.BaseMethod_ParamProcessed(sql, pam_obj);

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr();
            #endregion

            //如果param里没有参数，表示sql语句是无参数的，则直接调用ExecuteQuery方法
            if (param.Length == 0)
            {
                result = ExecuteQuery(connStr, sql, CommandType.Sql);
            }

            //将sql语句、以及上一步生成的SqlParameter[]作为参数调用ExecuteQuery方法，返回T类型集合
            result = ExecuteQuery(connStr, sql, CommandType.Sql, param);

            return result;
        }

        /// <summary>
        /// 根据指定的数据库连接字符串、sql语句、object类型List参数集合，查询数据库并返回指定T类型的List集合；
        /// 注意1：sql语句中的参数名要与pam_obj集合中开发者自定义class类型对象中的公共属性或公共字段名称保持一致；
        /// 注意2：若T为开发者自定义class类型，则其公共属性或公共字段名称要与执行sql语句后返回的查询结果集中的列名保持一致
        /// </summary>
        /// <param name="connStrName">数据库连接字符串的Name属性</param>
        /// <param name="sql">SQL语句（查询命令；多语句情况下最后一句必须是查询命令）</param>
        /// <param name="pam_obj">object类型List参数集合，集合中可以有开发者自定义class类型对象或系统定义的值类型</param>
        /// <returns>以T类型的List集合的方式返回查询结果集</returns>
        public static List<T> GetListBySql(string connStrName,string sql, List<object> pam_obj)
        {
            List<T> result = new List<T>();  //用于存储返回结果的list集合

            //将参数的匹配与处理统一交给BaseMethod_ParamProcessed方法执行，方便以后的维护和管理
            SqlParameter[] param = BaseMethods.BaseMethod_ParamProcessed(sql, pam_obj);

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr(connStrName);
            #endregion

            //如果param里没有参数，表示sql语句是无参数的，则直接调用ExecuteQuery方法
            if (param.Length == 0)
            {
                result = ExecuteQuery(connStr, sql, CommandType.Sql);
            }

            //将sql语句、以及上一步生成的SqlParameter[]作为参数调用ExecuteQuery方法，返回T类型集合
            result = ExecuteQuery(connStr, sql, CommandType.Sql, param);

            return result;
        }



        /// <summary>
        /// 根据存储过程名称、字典类型的参数列表，查询数据库并返回指定T类型的集合；
        /// 注意：若T为开发者自定义class类型，则其公共属性或公共字段名称要与执行存储过程后返回的查询结果集中的列名保持一致
        /// </summary>
        /// <param name="proc">将要访问的数据库中的存储过程的名称</param>
        /// <param name="dic_pam">字典类型的参数集合dic_pam，key对应的是存储过程proc中所需的参数名称，value是参数对应的值</param>
        /// <returns>以T类型的List集合的方式返回查询结果集</returns>
        public static List<T> GetListByProc(string proc, Dictionary<string,object> dic_pam)
        {
            List<T> result = new List<T>();  //用于存储返回结果的list集合

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr();
            #endregion

            //如果dic_pam里没有参数，表示存储过程是无参数的，则直接调用ExecuteQuery方法
            if (dic_pam==null||dic_pam.Count==0)
            {
                result = ExecuteQuery(connStr, proc, CommandType.StoredProcedure);
            }

            #region 将字典中的key和value分别转换为SqlParameter的参数名和值
            SqlParameter[] param = new SqlParameter[dic_pam.Count];
            int index = 0;//指示SqlParameter数组中当前操作的索引值
            foreach (KeyValuePair<string,object> item in dic_pam)
            {
                param[index] = new SqlParameter(item.Key, item.Value);
                index += 1;
            }
            #endregion

            //将存储过程以及上一步生成的SqlParameter[]作为参数调用ExecuteQuery方法，返回T类型集合
            result = ExecuteQuery(connStr, proc, CommandType.StoredProcedure, param);

            return result;
        }


        /// <summary>
        /// 根据指定的数据库连接字符串、存储过程名称、字典类型的参数列表，查询数据库并返回指定T类型的集合；
        /// 注意：若T为开发者自定义class类型，则其公共属性或公共字段名称要与执行存储过程后返回的查询结果集中的列名保持一致
        /// </summary>
        /// <param name="connStrName">数据库连接字符串的Name属性</param>
        /// <param name="proc">将要访问的数据库中的存储过程的名称</param>
        /// <param name="dic_pam">字典类型的参数集合dic_pam，key对应的是存储过程proc中所需的参数名称，value是参数对应的值</param>
        /// <returns>以T类型的List集合的方式返回查询结果集</returns>
        public static List<T> GetListByProc(string connStrName, string proc, Dictionary<string, object> dic_pam)
        {
            List<T> result = new List<T>();  //用于存储返回结果的list集合

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr(connStrName);
            #endregion

            //如果dic_pam里没有参数，表示存储过程是无参数的，则直接调用ExecuteQuery方法
            if (dic_pam == null || dic_pam.Count == 0)
            {
                result = ExecuteQuery(connStr, proc, CommandType.StoredProcedure);
            }

            #region 将字典中的key和value分别转换为SqlParameter的参数名和值
            SqlParameter[] param = new SqlParameter[dic_pam.Count];
            int index = 0;//指示SqlParameter数组中当前操作的索引值
            foreach (KeyValuePair<string, object> item in dic_pam)
            {
                param[index] = new SqlParameter(item.Key, item.Value);
                index += 1;
            }
            #endregion

            //将存储过程以及上一步生成的SqlParameter[]作为参数调用ExecuteQuery方法，返回T类型集合
            result = ExecuteQuery(connStr, proc, CommandType.StoredProcedure, param);

            return result;
        }

    }
}
