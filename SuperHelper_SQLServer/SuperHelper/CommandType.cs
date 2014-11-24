using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperHelper_SQLServer
{
    /// <summary>
    /// SuperHelper内部使用的枚举类型，
    /// 表示执行访问数据库命令的方式：存储过程、sql语句
    /// </summary>
     enum CommandType
    {
         /// <summary>
         /// 使用存储过程执行访问数据库
         /// </summary>
         StoredProcedure,
         /// <summary>
         /// 使用sql语句执行访问数据库
         /// </summary>
         Sql
    }

}
