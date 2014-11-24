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
    /// SH_NonQuery类，包含了一些对数据库进行非查询操作的方法
    /// </summary>
    public partial class SH_NonQuery
    {
        
        /// <summary>
        /// 根据sql语句，object类型List参数集合，访问数据库并执行非查询操作，返回受影响行数；
        /// 注意：sql语句中的参数名要与pam_obj集合中开发者自定义class类型对象中的公共属性或公共字段名称保持一致
        /// </summary>
        /// <param name="sql">SQL语句（非查询命令；多语句情况下最后一句必须是非查询命令）</param>
        /// <param name="pam_obj">object类型List参数集合，集合中可以有开发者自定义class类型对象或系统定义的值类型</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        public static int GetRowCountBySql(string sql, List<object> pam_obj)
        {
            int rowCount = 0;

            //将参数的匹配与处理统一交给BaseMethod_ParamProcessed方法执行，方便以后的维护和管理
            SqlParameter[] param = BaseMethods.BaseMethod_ParamProcessed(sql, pam_obj);

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr();
            #endregion

            //如果param里没有参数，表示sql语句是无参数的，则直接调用ExecuteQuery方法
            if (param.Length == 0)
            {
                rowCount = ExecuteNonQuery(connStr,sql, CommandType.Sql);
            }

            //4、将sql语句、return_type、以及上一步生成的SqlParameter[]作为参数调用DAL_Methods方法，返回return_type类型集合
            rowCount = ExecuteNonQuery(connStr, sql, CommandType.Sql, param);

            return rowCount;
        }


        /// <summary>
        /// 根据指定的数据库连接字符串、sql语句，object类型List参数集合，访问数据库并执行非查询操作，返回受影响行数；
        /// 注意：sql语句中的参数名要与pam_obj集合中开发者自定义class类型对象中的公共属性或公共字段名称保持一致
        /// </summary>
        /// <param name="connStrName">数据库连接字符串的Name属性</param>
        /// <param name="sql">SQL语句（非查询命令；多语句情况下最后一句必须是非查询命令）</param>
        /// <param name="pam_obj">object类型List参数集合，集合中可以有开发者自定义class类型对象或系统定义的值类型</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        public static int GetRowCountBySql(string connStrName,string sql, List<object> pam_obj)
        {
            int rowCount = 0;

            //将参数的匹配与处理统一交给BaseMethod_ParamProcessed方法执行，方便以后的维护和管理
            SqlParameter[] param = BaseMethods.BaseMethod_ParamProcessed(sql, pam_obj);

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr(connStrName);
            #endregion

            //如果param里没有参数，表示sql语句是无参数的，则直接调用ExecuteQuery方法
            if (param.Length == 0)
            {
                rowCount = ExecuteNonQuery(connStrName,sql, CommandType.Sql);
            }

            //4、将sql语句、return_type、以及上一步生成的SqlParameter[]作为参数调用DAL_Methods方法，返回return_type类型集合
            rowCount = ExecuteNonQuery(connStrName,sql, CommandType.Sql, param);

            return rowCount;
        }


        
        /// <summary>
        /// 根据存储过程名称、字典类型的参数列表，访问数据库并执行非查询操作，返回受影响行数
        /// </summary>
        /// <param name="proc">将要访问的数据库中的存储过程的名称</param>
        /// <param name="dic_pam">字典类型的参数集合dic_pam，key对应的是存储过程proc中所需的参数名称，value是参数对应的值</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        public static int GetRowCountByProc(string proc, Dictionary<string, object> dic_pam)
        {
            int rowCount = 0;

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr();
            #endregion


            //如果dic_pam里没有参数，表示存储过程是无参数的，则直接调用ExecuteQuery方法
            if (dic_pam == null || dic_pam.Count == 0)
            {
                rowCount = ExecuteNonQuery(connStr, proc, CommandType.StoredProcedure);
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


            //4、将存储过程、以及上一步生成的SqlParameter[]作为参数调用ExecuteNonQuery方法，返回受影响行数
            rowCount = ExecuteNonQuery(connStr, proc, CommandType.StoredProcedure, param);

            return rowCount;
        }


        /// <summary>
        /// 根据指定的数据库连接字符串、存储过程名称、字典类型的参数列表，访问数据库并执行非查询操作，返回受影响行数
        /// </summary>
        /// <param name="connStrName">数据库连接字符串的Name属性</param>
        /// <param name="proc">将要访问的数据库中的存储过程的名称</param>
        /// <param name="dic_pam">字典类型的参数集合dic_pam，key对应的是存储过程proc中所需的参数名称，value是参数对应的值</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        public static int GetRowCountByProc(string connStrName,string proc, Dictionary<string, object> dic_pam)
        {
            int rowCount = 0;

            #region 获取数据库连接字符串
            string connStr = BaseMethods.GetConstr(connStrName);
            #endregion


            //如果dic_pam里没有参数，表示存储过程是无参数的，则直接调用ExecuteQuery方法
            if (dic_pam == null || dic_pam.Count == 0)
            {
                rowCount = ExecuteNonQuery(connStr, proc, CommandType.StoredProcedure);
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


            //4、将存储过程、以及上一步生成的SqlParameter[]作为参数调用ExecuteNonQuery方法，返回受影响行数
            rowCount = ExecuteNonQuery(connStr, proc, CommandType.StoredProcedure, param);

            return rowCount;
        }


    }
    
}
