SuperHelper_SQLite使用注意事项：
    开发者若想使用SQLite数据库，除了要引用SuperHelper_SQLite.dll组件，还需要把名为“System.Data.SQLite.dll”的组件一并引用，同时还要根据解决方案的.NetFramework版本和Windows操作系统的版本（32位/64位）来选择正确版本的组件。
    例如：开发者在64位的操作系统上，使用基于.NetFramework4.0框架的VS解决方案进行开发，则使用文件夹“SQLite-NetFramework4.0-static-binary-bundle-x64-1.0.93.0”中的SQLite.dll组件。（如果开发者已经在项目中引用了合适版本的SQLite.dll组件，则无需重复引用。）

本项目中只提供了两个比较常用的SQLite.dll版本下载，想要更多不同版本的可以到网站http://system.data.sqlite.org自行下载。