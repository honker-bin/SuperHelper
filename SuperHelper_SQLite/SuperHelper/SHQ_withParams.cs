using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;  //正则表达式
using System.Data.SQLite;  //为SQLite数据库服务
using System.IO;     //读写文件流

namespace SuperHelper_SQLite
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
            SQLiteParameter[] param = BaseMethods.BaseMethod_ParamProcessed(sql, pam_obj);

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr();
            #endregion

            //如果param里没有参数，表示sql语句是无参数的，则直接调用ExecuteQuery方法
            if (param.Length == 0)
            {
                result = ExecuteQuery(connStr, sql);
            }

            //将sql语句、以及上一步生成的SQLiteParameter[]作为参数调用ExecuteQuery方法，返回T类型集合
            result = ExecuteQuery(connStr, sql, param);

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
        public static List<T> GetListBySql(string connStrName, string sql, List<object> pam_obj)
        {
            List<T> result = new List<T>();  //用于存储返回结果的list集合

            //将参数的匹配与处理统一交给BaseMethod_ParamProcessed方法执行，方便以后的维护和管理
            SQLiteParameter[] param = BaseMethods.BaseMethod_ParamProcessed(sql, pam_obj);

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr(connStrName);
            #endregion

            //如果param里没有参数，表示sql语句是无参数的，则直接调用ExecuteQuery方法
            if (param.Length == 0)
            {
                result = ExecuteQuery(connStr, sql);
            }

            //将sql语句、以及上一步生成的SQLiteParameter[]作为参数调用ExecuteQuery方法，返回T类型集合
            result = ExecuteQuery(connStr, sql, param);

            return result;
        }


    }
}
