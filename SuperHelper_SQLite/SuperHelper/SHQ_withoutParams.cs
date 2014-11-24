using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;  //为SQLite数据库服务
using System.Text.RegularExpressions;  //正则表达式
using System.IO;   //读写文件流

namespace SuperHelper_SQLite
{

    public partial class SH_Query<T>
    {
        /// <summary>
        /// 根据无参数的sql语句，查询数据库并返回指定T类型的List集合。
        /// 注意：若T为开发者自定义class类型，则其公共属性或公共字段名称要与执行sql语句后返回的查询结果集中的列名保持一致
        /// </summary>
        /// <param name="sql">SQL语句（查询命令；多语句情况下最后一句必须是查询命令）</param>
        /// <returns>以T类型的List集合的方式返回查询结果集</returns>
        public static List<T> GetListBySql(string sql)
        {
            //1、找出sql语句中所有参数

            #region 第一步，找出sql语句中所有参数
            //定义正则表达式，用于匹配出sql语句中所有参数
            string partten = @"[@](\w+)";
            MatchCollection mc = Regex.Matches(sql, partten);
            #endregion

            //异常判断，如果sql语句中有参数
            if (mc.Count > 0)
            {
                throw new Exception(HelperVar.DLL_Name + "组件捕捉到的异常：sql语句中有参数！");
            }

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr();
            #endregion

            return ExecuteQuery(connStr, sql);
        }


        /// <summary>
        /// 根据指定的数据库连接字符串、无参数的sql语句，查询数据库并返回指定T类型的List集合。
        /// 注意：若T为开发者自定义class类型，则其公共属性或公共字段名称要与执行sql语句后返回的查询结果集中的列名保持一致
        /// </summary>
        /// <param name="connStrName">数据库连接字符串的Name属性</param>
        /// <param name="sql">SQL语句（查询命令；多语句情况下最后一句必须是查询命令）</param>
        /// <returns>以T类型的List集合的方式返回查询结果集</returns>
        public static List<T> GetListBySql(string connStrName, string sql)
        {
            //1、找出sql语句中所有参数

            #region 第一步，找出sql语句中所有参数
            //定义正则表达式，用于匹配出sql语句中所有参数
            string partten = @"[@](\w+)";
            MatchCollection mc = Regex.Matches(sql, partten);
            #endregion

            //异常判断，如果sql语句中有参数
            if (mc.Count > 0)
            {
                throw new Exception(HelperVar.DLL_Name + "组件捕捉到的异常：sql语句中有参数！");
            }

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr(connStrName);
            #endregion

            return ExecuteQuery(connStr, sql);
        }


    }

}
