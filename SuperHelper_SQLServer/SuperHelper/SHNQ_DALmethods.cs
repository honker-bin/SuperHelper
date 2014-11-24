using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;  //为mssqlserver数据库服务
using System.Reflection;         //程序集反射以及类型获取
using System.IO;   //读写文件流

namespace SuperHelper_SQLServer
{
    ///// <summary>
    ///// SH_NonQuery类的分部定义之一
    ///// </summary>
    public partial class SH_NonQuery
    {
        
        /// <summary>
        /// SuperHelper_SQLServer内部方法；
        /// 根据数据库连接字符串、存储过程名称或者sql语句、访问数据库的方式以及SqlParameter数组，执行访问数据库操作；
        /// 并返回受影响行数
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="nonQueryStr">表示存储过程名称/sql语句的字符串</param>
        /// <param name="commandType">表示执行访问数据库的方式</param>
        /// <param name="param">可变的SqlParameter参数数组</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        private static int ExecuteNonQuery(string connStr, string nonQueryStr, CommandType commandType, params SqlParameter[] param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand(nonQueryStr, conn))
                    {
                        conn.Open();

                        #region 处理存储过程和sql语句执行过程中的不同
                        if (commandType == CommandType.StoredProcedure)
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        }
                        #endregion

                        if (param != null)
                        {
                            cmd.Parameters.AddRange(param);
                        }
                        return cmd.ExecuteNonQuery();

                        //Console.WriteLine(count);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(HelperVar.DLL_Name+"组件捕捉到的异常：" + ex.Message);
            }

        }
    }
}
