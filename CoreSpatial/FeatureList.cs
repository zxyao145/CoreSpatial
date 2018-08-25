using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial
{
    public class FeatureList:IFeatureList
    {
        public FeatureList(IFeatureSet featureSet)
        {
            _featureSet = featureSet;
            _features = new List<IFeature>();
        }

        private IFeatureSet _featureSet;

        private List<IFeature> _features;
        
        public void Add(IFeature feature)
        {
            _features.Add(feature);
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
