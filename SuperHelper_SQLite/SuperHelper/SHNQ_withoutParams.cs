using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;  //为SQLite数据库服务
using System.Text.RegularExpressions;  //正则表达式
using System.IO;   //读写文件流

namespace SuperHelper_SQLite
{
    ///// <summary>
    ///// SH_NonQuery类的分部定义之一
    ///// </summary>
    public partial class SH_NonQuery
    {

        /// <summary>
        /// 根据sql语句，访问数据库并执行非查询操作，返回受影响行数
        /// </summary>
        /// <param name="sql">SQL语句（非查询命令；多语句情况下最后一句必须是非查询命令）</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        public static int GetRowCountBySql(string sql)
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

            return ExecuteNonQuery(connStr, sql);
        }


        /// <summary>
        /// 根据指定的数据库连接字符串、sql语句，访问数据库并执行非查询操作，返回受影响行数
        /// </summary>
        /// <param name="connStrName">数据库连接字符串的Name属性</param>
        /// <param name="sql">SQL语句（非查询命令；多语句情况下最后一句必须是非查询命令）</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        public static int GetRowCountBySql(string connStrName, string sql)
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

            return ExecuteNonQuery(connStr, sql);
        }

    }
}
