using CoreSpatial.BasicGeometrys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreSpatial.Analysis.Tyson
{
    internal class TysonAnalysis
    {
        const double EnvelopeEpsilon = 0.01;

        public FeatureSet Analysis(IFeatureSet featureSet, double[] envelope = null)
        {
            if (featureSet.FeatureType != FeatureType.Point)
            {
                return null;
            }

            var geoPoints =
                featureSet
                    .Features
                    .Select(e => (GeoPoint)(e
                        .Geometry
                        .BasicGeometry)
                    )
                .ToList();
            if (geoPoints.Count == 0)
            {
                return null;
            }

            var firstPoint = geoPoints[0];
            envelope ??= new double[4]
            {
                firstPoint.X, firstPoint.Y, firstPoint.X, firstPoint.Y
            };

            if (envelope.Length != 4)
            {
                throw new Exception(nameof(envelope) + "的长度必须为4");
            }

            foreach (var geoPoint in geoPoints)
            {
                if (geoPoint.X < envelope[0])
                {
                    envelope[0] = geoPoint.X;
                }
                else if (geoPoint.X > envelope[2])
                {
                    envelope[2] = geoPoint.X;
                }

                if (geoPoint.Y < envelope[1])
                {
                    envelope[1] = geoPoint.Y;
                }
                else if (geoPoint.Y > envelope[3])
                {
                    envelope[3] = geoPoint.Y;
                }
            }

            envelope[0] -= EnvelopeEpsilon;
            envelope[1] -= EnvelopeEpsilon;
            envelope[2] += EnvelopeEpsilon;
            envelope[3] += EnvelopeEpsilon;

            var tysons = VoronoiBuilder
                .BuildCellWithIndex(geoPoints, envelope);

            var originTables = featureSet.AttrTable;
            var originRows = originTables.Rows;

            var polygonFeatureSet = new FeatureSet(FeatureType.Polygon);
            var attrTable = originTables.Clone();
            var attrTableRows = attrTable.Rows;
            var cols = attrTable.Columns.Count;

            foreach (var polygon in tysons)
            {
                IGeometry geometry = new Geometry(polygon.Value);
                polygonFeatureSet.Features.Add(new CoreSpatial.Feature(geometry));
                var newRow = attrTable.NewRow();
                var oldRow = originRows[polygon.Key];
                for (int i = 0; i < cols; i++)
                {
                    newRow[i] = oldRow[i];
                }
                attrTableRows.Add(newRow);
            }

            polygonFeatureSet.AttrTable = attrTable;

            return polygonFeatureSet;
        }
    }
}
