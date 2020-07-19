using CoreSpatial.Analysis.Tin;
using CoreSpatial.Analysis.Tyson;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial.Analysis
{
    public static class FeatureSetExtensions
    {
        public static IFeatureSet Tyson(this IFeatureSet featureSet, double[] envelope)
        {
            if (featureSet is null)
            {
                throw new ArgumentNullException(nameof(featureSet));
            }

            var tyson = new TysonAnalysis();
            return tyson.Analysis(featureSet, envelope);
        }

        public static IFeatureSet Tin(this IFeatureSet featureSet)
        {
            if (featureSet is null)
            {
                throw new ArgumentNullException(nameof(featureSet));
            }

            var tinBuilder = new TinBuilder();
            return tinBuilder.Build(featureSet);
        }
    }
}
