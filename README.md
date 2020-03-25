# CoreSpatial

[English](./README-en.md)


### 用途
CoreSpatial是基于.NET Standard v2.1实现的用以读写shapefile的类库，支持投影文件.prj的读写。


### nuget
|  类库   | nuget  |
|  ----  | ----  |
| CoreSpatial | [![NuGet Status](https://img.shields.io/nuget/v/CoreSpatial)](https://www.nuget.org/packages/CoreSpatial) |
| CoreSpatial.Converter | [![NuGet Status](https://img.shields.io/nuget/v/CoreSpatial.Converter?style=plastic)](https://www.nuget.org/packages/CoreSpatial.Converter) |


### 项目信息
+ .NET Standard v2.1
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

创建新的shapefile:
```c#
void CreatePoint()
{
	IFeatureSet fs = new FeatureSet(FeatureType.Point);
	var point1 = new GeoPoint(133, 30);
	var point2 = new GeoPoint(133, 32);
	var point3 = new GeoPoint(134, 30);

	var feature1 = new Feature(new Geometry(point1));
	var feature2 = new Feature(new Geometry(point2));
	var feature3 = new Feature(new Geometry(point3));

	fs.Features.Add(feature1);
	fs.Features.Add(feature2);
	fs.Features.Add(feature3);
	fs.Crs = Crs.Wgs84Gcs;

	var dataTable = new DataTable();
	dataTable.Columns.Add("名称", typeof(string));
	dataTable.Columns.Add("id", typeof(int));
	var row1 = dataTable.NewRow();
	var row2 = dataTable.NewRow();
	var row3 = dataTable.NewRow();

	row1[0] = "点1";
	row1[1] = 1;

	row2[0] = "点2";
	row2[1] = 2;
	
	row3[0] = "点3";
	row3[1] = 3;

	dataTable.Rows.Add(row1);
	dataTable.Rows.Add(row2);
	dataTable.Rows.Add(row3);

	fs.AttrTable = dataTable;

	fs.Save("../createNew/point.shp");
}

void CreateMultiPoint()
{
	IFeatureSet fs = new FeatureSet(FeatureType.MultiPoint);

	var point1 = new GeoPoint(133, 30);
	var point2 = new GeoPoint(133, 32);
	var point3 = new GeoPoint(134, 30);

	var point4 = new GeoPoint(134, 34);
	var point5 = new GeoPoint(135, 35);

	var multiPoint1 = new MultiPoint(new List<GeoPoint>()
	{
		point1,point2,point3
	});

	var multiPoint2 = new MultiPoint();
	multiPoint2.Points.Add(point4);
	multiPoint2.Points.Add(point5);
	

	var feature1 = new Feature(new Geometry(multiPoint1));
	var feature2 = new Feature(new Geometry(multiPoint2));

	fs.Features.Add(feature1);
	fs.Features.Add(feature2);
	fs.Crs = Crs.Wgs84Gcs;

	var dataTable = new DataTable();
	dataTable.Columns.Add("名称", typeof(string));
	dataTable.Columns.Add("id", typeof(int));
	var row1 = dataTable.NewRow();
	var row2 = dataTable.NewRow();

	row1[0] = "多点1";
	row1[1] = 1;

	row2[0] = "多点2";
	row2[1] = 2;

	dataTable.Rows.Add(row1);
	dataTable.Rows.Add(row2);

	fs.AttrTable = dataTable;

	fs.Save("../createNew/multipoint.shp");
}

void CreatePolyLine()
{
	
	IFeatureSet fs = new FeatureSet(FeatureType.PolyLine);

	var point1 = new GeoPoint(133, 30);
	var point2 = new GeoPoint(133, 32);
	var point3 = new GeoPoint(134, 30);

	var point4 = new GeoPoint(134, 34);
	var point5 = new GeoPoint(135, 35);

	//line ring
	var polyLine1 = new PolyLine(new List<GeoPoint>()
	{
		point1,point2,point3,point1
	});
	var isLineRing = polyLine1.IsLineRing;

	var polyLine2 = new PolyLine(new List<GeoPoint>()
	{
		point4,point5
	});
	
	var feature1 = new Feature(new Geometry(polyLine1));
	var feature2 = new Feature(new Geometry(polyLine2));

	fs.Features.Add(feature1);
	fs.Features.Add(feature2);
	fs.Crs = Crs.Wgs84Gcs;

	var dataTable = new DataTable();
	dataTable.Columns.Add("名称", typeof(string));
	dataTable.Columns.Add("id", typeof(int));
	var row1 = dataTable.NewRow();
	var row2 = dataTable.NewRow();

	row1[0] = "线1";
	row1[1] = 1;

	row2[0] = "线2";
	row2[1] = 2;

	dataTable.Rows.Add(row1);
	dataTable.Rows.Add(row2);

	fs.AttrTable = dataTable;

	fs.Save("../createNew/polyline.shp");
}

void CreateMultiPolyLine()
{
	IFeatureSet fs = new FeatureSet(FeatureType.PolyLine);

	var point1 = new GeoPoint(132, 30);
	var point2 = new GeoPoint(136, 30);
	var point3 = new GeoPoint(134, 28);
	var point4 = new GeoPoint(134, 32);

	var polyLine1 = new PolyLine(new List<GeoPoint>()
	{
		point1,point2
	});

	var polyLine2 = new PolyLine(new List<GeoPoint>()
	{
		point3,point4
	});

	var multiPolyLine1 = new MultiPolyLine(new List<PolyLine>()
	{
		polyLine1,polyLine2
	});
   
	var feature1 = new Feature(new Geometry(multiPolyLine1));

	fs.Features.Add(feature1);
	fs.Crs = Crs.Wgs84Gcs;

	var dataTable = new DataTable();
	dataTable.Columns.Add("名称", typeof(string));
	dataTable.Columns.Add("id", typeof(int));
	var row1 = dataTable.NewRow();

	row1[0] = "十字多线";
	row1[1] = 1;

	dataTable.Rows.Add(row1);

	fs.AttrTable = dataTable;

	fs.Save("../createNew/multiPolyLine.shp");
}

void CreatePolygon()
{
	IFeatureSet fs = new FeatureSet(FeatureType.Polygon);

	var point1 = new GeoPoint(132, 30);
	var point2 = new GeoPoint(136, 30);
	var point3 = new GeoPoint(132, 35);
	
	var edge = new PolyLine(new List<GeoPoint>()
	{
		point1, point2, point3, point1
	});
	
	var polygon = new Polygon(edge);

	//edge，Clockwise
	var point4 = new GeoPoint(140, 30);
	var point5 = new GeoPoint(140, 35);
	var point6 = new GeoPoint(150, 35);
	var point7 = new GeoPoint(150, 30);
	var edge2 = new PolyLine(new List<GeoPoint>()
	{
		point4, point5, point6, point7, point4
	});

	//points sketch 
	//              .(point9)
	//
	//  .(point8)          .(point10)
	//
	var point8 = new GeoPoint(142, 31);
	var point9 = new GeoPoint(145, 33);
	var point10 = new GeoPoint(148, 31);

	//hole, need Counterclockwise,
	//if Counterclockwise, sequence should be:
	//point8, point10, point9
	//It will be handled inside the library.
	//It will be a line ring inside the library too
	var hole = new PolyLine(new List<GeoPoint>()
	{
		point8,point9,point10
	});
	var polygon2 = new Polygon(edge2, hole);


	var feature1 = new Feature(new Geometry(polygon));
	var feature2 = new Feature(new Geometry(polygon2));

	fs.Features.Set(new List<IFeature>()
	{
		feature1,
		feature2
	});
	fs.Crs = Crs.Wgs84Gcs;


	var dataTable = new DataTable();
	dataTable.Columns.Add("名称", typeof(string));
	dataTable.Columns.Add("id", typeof(int));
	var row1 = dataTable.NewRow();
	var row2 = dataTable.NewRow();

	row1[0] = "简单面";
	row1[1] = 1;

	row2[0] = "带洞面";
	row2[1] = 2;

	dataTable.Rows.Add(row1);
	dataTable.Rows.Add(row2);

	fs.AttrTable = dataTable;

	fs.Save("../createNew/polygon.shp");
}

//for using GB2312
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

CreatePoint();
CreateMultiPoint();
CreatePolyLine();
CreateMultiPolyLine();
CreatePolygon();
```
CoreSpatial参考于DotSpatial，部分用法类似，详见 CoreSpatial.Test。


转为GeoJSON（需要先引用 **CoreSpatial.Converter**）：
```c#
fs.ToGeoJSON();
```


转为KML/KMZ（需要先引用 **CoreSpatial.Converter**）：
```c#
fs.ToKML(kmlName);
fs.ToKMZ(kmlName, KmzSavePath);
```

使用方法见 CoreSpatial.Converter.Test，可在http://geojson.io/校验GeoJSON。


### 注意事项
+ 暂不能读取M值、Z值shp文件

### 参考 
[DotSpatial](https://github.com/DotSpatial/DotSpatial "DotSpatial")

[NetTopologySuite.IO.ShapeFile](https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile "NetTopologySuite.IO.ShapeFile")


### 开发者
zxyao145


### 许可协议
有关许可条款的更多信息，请参阅LICENSE.txt。
