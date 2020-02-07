using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial
{
    public interface IFeatureList: IEnumerable<IFeature>
    {
        int Count { get; }
        IFeature this[int index] { get; set; }

        void Add(IFeature feature);
        void AddRange(IEnumerable<IFeature> features);
        void RemoveAt(int index);
        void Remove(IFeature feature);
        void Set(IEnumerable<IFeature> features);
        void Clear();
        int IndexOf(IFeature feature);
    }
}
