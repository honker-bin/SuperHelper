using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;  //为SQLite数据库服务
using System.Reflection;         //程序集反射以及类型获取
using System.IO;   //读写文件流

namespace SuperHelper_SQLite
{
    ///// <summary>
    ///// SH_NonQuery类的分部定义之一
    ///// </summary>
    public partial class SH_NonQuery
    {

        /// <summary>
        /// SuperHelper_SQLite内部方法；
        /// 根据数据库连接字符串、sql语句以及SQLiteParameter数组，执行访问数据库操作；
        /// 并返回受影响行数
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="nonQueryStr">sql语句</param>
        /// <param name="param">可变的SQLiteParameter参数数组</param>
        /// <returns>执行非查询操作后的受影响行数</returns>
        private static int ExecuteNonQuery(string connStr, string nonQueryStr, params SQLiteParameter[] param)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(nonQueryStr, conn))
                    {
                        conn.Open();


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
                throw new Exception(HelperVar.DLL_Name + "组件捕捉到的异常：" + ex.Message);
            }

        }
    }
}
