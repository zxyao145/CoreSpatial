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
        void RemoveAt(int index);
        void Remove(IFeature feature);
        void Clear();
        int IndexOf(IFeature feature);
    }
}
