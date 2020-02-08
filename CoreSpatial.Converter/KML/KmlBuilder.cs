using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace CoreSpatial.Converter.KML
{
    internal class KmlBuilder
    {
        private readonly IFeatureSet _featureSet;
        private readonly Dictionary<int, string> _columnNameDict;
        private string _name;

        public KmlBuilder(IFeatureSet featureSet)
        {
            _featureSet = featureSet;
            _columnNameDict = new Dictionary<int, string>();
        }

        public string Build([NotNull] string name)
        {
            _name = name;
            XDocument xDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes")
            );

            XNamespace xns = "http://www.opengis.net/kml/2.2";
            var root = new XElement(xns + "kml",
                new XAttribute("xmlns", xns));

            var documentEle = new XElement("Document");
            documentEle.SetAttributeValue("id", "root_doc");
            root.Add(documentEle);
            var schemaEle = BuildSchema();
            documentEle.Add(schemaEle);
            documentEle.Add(BuildFolder());

            xDoc.Add(root);

            using var wr = new Utf8StringWriter(Encoding.UTF8);
            xDoc.Save(wr);
            return wr.ToString();
        }

        public void BuildKMZ([NotNull] string name, string savePath)
        {
            var text = Build(name);

            if (Path.GetExtension(savePath) != ".kmz")
            {
                throw new ArgumentException("kmzPath must has extension \".kmz\"");
            }

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            try
            {
                using FileStream zipToOpen = new FileStream(savePath, FileMode.CreateNew);
                using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);
                ZipArchiveEntry readmeEntry = archive.CreateEntry("doc.kml");
                using StreamWriter writer = new StreamWriter(readmeEntry.Open());
                writer.Write(text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// 构建字段信息
        /// </summary>
        /// <returns></returns>
        private XElement BuildSchema()
        {
            var columns = _featureSet.AttrTable.Columns;

            var schemaEle = new XElement("Schema");
            schemaEle.SetAttributeValue("name", _name);
            schemaEle.SetAttributeValue("id", _name);

            var columnNameDictKey = 0;
            foreach (DataColumn dataColumn in columns)
            {
                var colName = dataColumn.ColumnName;
                _columnNameDict.Add(columnNameDictKey++, colName);
                var simpleFieldEle = new XElement("SimpleField");
                simpleFieldEle.SetAttributeValue("name", colName);
                simpleFieldEle.SetAttributeValue("type", dataColumn.DataType.Name.ToLower());
                schemaEle.Add(simpleFieldEle);
            }

            return schemaEle;
        }

        /// <summary>
        /// 构建Folder
        /// </summary>
        /// <returns></returns>
        private XElement BuildFolder()
        {
            var folderEle = new XElement("Folder");
            folderEle.SetElementValue("name", _name);
            var features = _featureSet.Features;
            foreach (var feature in features)
            {
                var placemarkEle = BuildPlacemark(feature);
                folderEle.Add(placemarkEle);
            }

            return folderEle;
        }

        /// <summary>
        /// 构建Placemark
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private XElement BuildPlacemark(IFeature feature)
        {
            var placemarkEle = new XElement("Placemark");
            var extendedData = BuildExtendedData(feature);
            var spatial = BuildSpatial(feature);
            placemarkEle.Add(extendedData);
            placemarkEle.Add(spatial);
            return placemarkEle;
        }

        /// <summary>
        /// 构建属性值
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private XElement BuildExtendedData(IFeature feature)
        {
            var extendedDataEle = new XElement("ExtendedData");
            var schemaDataEle = new XElement("SchemaData");
            schemaDataEle.SetAttributeValue("schemaUrl", $"#{_name}");

            var dataRow = feature.DataRow.ItemArray;
            var columnCount = _columnNameDict.Count;
            for (int i = 0; i < columnCount; i++)
            {
                var simpleDataEle = new XElement("SimpleData");
                simpleDataEle.SetAttributeValue("name", _columnNameDict[i]);
                var value = dataRow[i];
                simpleDataEle.SetValue(value);
                schemaDataEle.Add(simpleDataEle);
            }

            extendedDataEle.Add(schemaDataEle);
            return extendedDataEle;
        }

        #region 构建空间信息

        /// <summary>
        /// 构建空间信息
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private XElement BuildSpatial(IFeature feature)
        {
            var basicGeometry = feature.Geometry.BasicGeometry;
            var spatial = KmlUtil.GetCoordinates(basicGeometry);
            if (KmlUtil.IsMultiGeometry(feature.GeometryType))
            {
                var multiGeometry = new XElement("MultiGeometry");
                foreach (var xElement in spatial)
                {
                    multiGeometry.Add(xElement);
                }

                return multiGeometry;
            }
            else
            {
                return spatial.Count > 0 ? spatial[0] : null;
            }
        }

        #endregion
    }
}