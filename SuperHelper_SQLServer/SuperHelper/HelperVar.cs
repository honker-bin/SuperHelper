using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperHelper_SQLServer
{
    /// <summary>
    /// 在HelperVar类里，定义了一些全局的字段或属性
    /// </summary>
    internal class HelperVar
    {
        /// <summary>
        /// 内部使用的字段；
        /// 用于存储本组件的名称
        /// </summary>
        internal static readonly string DLL_Name = "SuperHelper_SQLServer";


        /// <summary>
        /// 内部使用的字段；
        /// 用于存储解决方案config文件中数据库连接字符串的Name属性的值，默认为SuperHelper_SQLServer
        /// </summary>
        internal static string conStrName = "SuperHelper_SQLServer";
    }
}
