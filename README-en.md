# CoreSpatial

[中文](./README.md)

### Purpose
CoreSpatial is a class library based on .Net core v3.1 to read and write shapefiles. It supports the reading and writing of projection file .prj (support by ProjNet).

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
Similar to **DotSpatial**, more see CoreSpatial.Test.

### Attention
+ Cannot read SHP file of M value and Z value

### Developer
zxyao145

### License
See LICENSE.txt
