using CoreSpatial.Analysis.Convex;
using CoreSpatial.Analysis.Tin;
using CoreSpatial.Analysis.Tyson;
using CoreSpatial.BasicGeometrys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreSpatial.Analysis
{
    public static class FeatureSetExtensions
    {
        /// <summary>
        /// Voronoi  
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="envelope"></param>
        /// <returns></returns>
        public static IFeatureSet Tyson(this IFeatureSet featureSet, double[] envelope)
        {
            if (featureSet is null)
            {
                throw new ArgumentNullException(nameof(featureSet));
            }

            var tyson = new TysonAnalysis();
            return tyson.Analysis(featureSet, envelope);
        }

        /// <summary>
        /// create tin
        /// </summary>
        /// <param name="featureSet"></param>
        /// <returns></returns>
        public static IFeatureSet Tin(this IFeatureSet featureSet)
        {
            if (featureSet is null)
            {
                throw new ArgumentNullException(nameof(featureSet));
            }

            var tinBuilder = new TinBuilder();
            return tinBuilder.Build(featureSet);
        }

        /// <summary>
        /// get convex hull
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="concavity"></param>
        /// <returns></returns>
        public static IFeatureSet Convex(this IFeatureSet featureSet, int concavity = 20)
        {
            var features = featureSet.Features;
            List<Coordinate> coordinates = new List<Coordinate>();
            foreach (var item in features)
            {
                coordinates.AddRange(item.Geometry.Coordinates);
            }

            return (new ConvexAnalysis()).Build(coordinates, concavity);
        }
    }
}
