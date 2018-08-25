# CoreSpatial

### 用途
CoreSpatial是基于.NET Core2.1实现的用以读取shapefile的类库，暂不能读取坐标投影文件及进行shp文件的写入。部分内容借鉴与DotSpatial

### 项目信息
+ .NET Core2.1
+ 目前版本为 v1.0.0.0


### 使用方法
```
IFeatureSet fs = FeatureSet.Open(shpPath);
...
```
类似于DotSpatial，详见CoreSpatial.Test。

### 注意事项
+ 暂不能读取坐标投影文件
+ 暂不能进行shp文件的写入

### 开发者
zxyao145

### 许可协议
有关许可条款的更多信息，请参阅LICENSE.txt。
