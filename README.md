SuperHelper
===========

  SuperHelper是一个基于.Net平台的非侵入式的的微型ORM框架，可帮助开发者快速简便的访问数据库并对其操作，且部署起来十分简单；只依赖于相应的数据库引擎，开发者可以根据实际项目需要增加引用不同版本的SuperHelper组件，且不会产生冲突。（目前SuperHelper有SQlServer版和SQLite版）

  以下是SuperHelper的概要特点：  
1. 部署十分简单。开发者只需在项目配置文件中为SuperHelper指定一个可用的连接字符串即可完成部署。  
2. 使用灵活。SuperHelper可以在经典三层架构项目、一般处理程序+模板引擎项目、WebForm、MVC架构项目等等都可以使用，项目中如果需要切换使用不同数据库引擎则只需切换相应版本的SuperHelper组件即可。还有，SuperHelper可以与其它ORM框架如微软的EF、NHibernate等混可使用且不会产生任何冲突。同时因为SuperHelper是一个非侵入式的ORM框架，项目不会对SuperHelper有过多的依赖，开发者依然可以把代码很方便的迁移到其他地方。  
3. 使用简单。写好sql语句之后，开发者只需要再写一行代码即可完成访问数据库并返回相应数据实体的操作（SuperHelper还支持对实体类的复杂类型属性或字段赋值）。ps：与一些微型ORM一样，SuperHelper是不支持LinQ的，不过针对SQlServer，SuperHelper支持存储过程的调用。

更多详细信息与用法可参考项目中帮助文档和作者的博客：[http://www.cnblogs.com/fenglibin/](http://www.cnblogs.com/fenglibin/)
