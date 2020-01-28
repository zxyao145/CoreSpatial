# CoreSpatial

### 用途
CoreSpatial是基于.NET Core v3.1实现的用以读写shapefile的类库，支持投影文件.prj的读写。
在使用.net core v3.1进行开发的过程中需要读取shapefile，但是并未找到相关的类库，遂自己写了个。
部分内容借鉴于DotSpatial。

### 项目信息
+ .NET Core v3.1
+ 目前版本为 v0.1.0
+ 支持shapefile的读写
+ 使用ProjNet支持对.prj文件的读写


### 使用方法
读:
```c#
IFeatureSet fs = FeatureSet.Open(shpPath);
...
```
写：
```c#
fs.Save(shpPath);
```
类似于DotSpatial，详见CoreSpatial.Test。

### 注意事项
+ 暂不能读取M值、Z值shp文件

### 开发者
zxyao145

### 许可协议
有关许可条款的更多信息，请参阅LICENSE.txt。
