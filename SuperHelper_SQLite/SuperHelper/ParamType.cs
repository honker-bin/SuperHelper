using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperHelper_SQLite
{
    /// <summary>
    /// SuperHelper_SQLite内部使用的枚举类型，
    /// 表示某参数的类型，主要用于区分用户自定义的class类型和其他系统提供的类型
    /// </summary>
    enum ParamType
    {
        /// <summary>
        /// 用户自定义的class类型
        /// </summary>
        UserClass,
        /// <summary>
        /// 系统提供的字符串类型
        /// </summary>
        String,
        /// <summary>
        /// 系统提供的其他的类类型
        /// </summary>
        OtherSysClass,
        /// <summary>
        /// 系统提供的值类型
        /// </summary>
        SysValue,
        /// <summary>
        /// 未知类型
        /// </summary>
        UnKown
    }
}
