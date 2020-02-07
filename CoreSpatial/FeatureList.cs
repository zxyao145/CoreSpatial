using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace CoreSpatial
{
    public class FeatureList : IFeatureList
    {
        public FeatureList(IFeatureSet featureSet)
        {
            _featureSet = featureSet;
            _features = new List<IFeature>();
        }

        private IFeatureSet _featureSet;

        private List<IFeature> _features;

        public int Count => _features.Count;

        public IFeature this[int index]
        {
            get => _features[index];
            set => _features[index] = value;
        }

        public void Add(IFeature feature)
        {
            var fe = ((Feature)feature);
            fe.ParentFeatureSet = _featureSet;
            _features.Add(fe);
        }

        public void AddRange(IEnumerable<IFeature> features)
        {
            var newFeatures = features.Select(e =>
            {
                var fe = (Feature)e;
                fe.ParentFeatureSet = _featureSet;
                return fe;
            });
            _features.AddRange(newFeatures);
        }

        public void Set(IEnumerable<IFeature> features)
        {
            _features.Clear();
            AddRange(features);
        }

        public void RemoveAt(int index)
        {
            _features.RemoveAt(index);
            _featureSet.AttrTable.Rows.RemoveAt(index);
        }

        public void Remove(IFeature feature)
        {
            var index = _features.IndexOf(feature);
            RemoveAt(index);
        }

        public void Clear()
        {
            _featureSet.AttrTable.Rows.Clear();
            _features.Clear();
        }

        public int IndexOf(IFeature feature)
        {
            return _features.IndexOf(feature);
        }


        public IEnumerator<IFeature> GetEnumerator()
        {
            return _features.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
