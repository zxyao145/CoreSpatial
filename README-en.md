# CoreSpatial

[中文](./README.md)


### Purpose
CoreSpatial is a class library based on .Net core v3.1 to read and write shapefiles. It supports the reading and writing of projection file .prj (support by ProjNet).


### nuget
|  Library   | nuget  |
|  ----  | ----  |
| CoreSpatial | [![NuGet Status](https://img.shields.io/nuget/v/CoreSpatial)](https://www.nuget.org/packages/CoreSpatial) |
| CoreSpatial.Converter | [![NuGet Status](https://img.shields.io/nuget/v/CoreSpatial.Converter?style=plastic)](https://www.nuget.org/packages/CoreSpatial.Converter) |


### Project information
+ .NET Core v3.1
+ Current version v0.1.1
+ Supports the reading and writing of shapefile
+ Using **ProjNet** to support reading and writing .prj files


### Usage
read:
```c#
IFeatureSet fs = FeatureSet.Open(shpPath);
...
```
write：
```c#
fs.Save(shpPath);
```
Corespatial refers to **DotSpatial**. Some of its usage is similar. See CoreSpatial.Test for details.

Convert to GeoJSON (You need to add reference **CoreSpatial.Converter** first):
```c#
fs.ToGeoJSON();
```
See CoreSpatial.Converter.Test for usage, which can be verified at http://geojson.io/.


### Attention
+ Cannot read SHP file of M value and Z value

### Developer
zxyao145

### License
See LICENSE.txt
