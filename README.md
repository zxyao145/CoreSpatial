# CoreSpatial

[English](./README-en.md)


### 用途
CoreSpatial是基于.NET Core v3.1实现的用以读写shapefile的类库，支持投影文件.prj的读写。


### nuget
|  类库   | nuget  |
|  ----  | ----  |
| CoreSpatial | [![NuGet Status](https://img.shields.io/nuget/v/CoreSpatial)](https://www.nuget.org/packages/CoreSpatial) |
| CoreSpatial.Converter | [![NuGet Status](https://img.shields.io/nuget/v/CoreSpatial.Converter?style=plastic)](https://www.nuget.org/packages/CoreSpatial.Converter) |


### 项目信息
+ .NET Core v3.1
+ 目前版本为 v0.1.1
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
CoreSpatial参考于DotSpatial，部分用法类似，详见 CoreSpatial.Test。

转为GeoJSON（需要先引用 **CoreSpatial.Converter**）：
```c#
fs.ToGeoJSON();
```
使用方法见 CoreSpatial.Converter.Test，可在http://geojson.io/进行验证


### 注意事项
+ 暂不能读取M值、Z值shp文件


### 开发者
zxyao145


### 许可协议
有关许可条款的更多信息，请参阅LICENSE.txt。
