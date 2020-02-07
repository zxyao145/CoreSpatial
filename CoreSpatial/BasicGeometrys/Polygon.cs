using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoreSpatial.BasicGeometrys
{
    public class Polygon : IBasicGeometry
    {
        #region 构造函数

        public Polygon([NotNull]PolyLine edge, params PolyLine[] holes):this(edge,holes.ToList())
        {
        }
        
        public Polygon([NotNull]PolyLine edge, IEnumerable<PolyLine> holes = null)
        {
            _lines = new List<PolyLine>();
            AddEdge(edge);
            if (holes != null)
            {
                foreach (var hole in holes)
                {
                    AddHole(hole);
                }
            }
        }

        internal Polygon([NotNull]IList<PolyLine> lines) : this(lines[0], lines.Skip(1).ToList())
        {
        }

        #endregion

        private List<PolyLine> _lines;

        /// <summary>
        /// 添加边界
        /// </summary>
        /// <param name="line"></param>
        public void AddEdge(PolyLine line)
        {
            line.SafeCloseLine();
            line.Clockwise();
            _lines.Add(line);
        }

        /// <summary>
        /// 添加洞
        /// </summary>
        /// <param name="line"></param>
        public void AddHole(PolyLine line)
        {
            line.SafeCloseLine();
            line.Counterclockwise();
            _lines.Add(line);
        }

        /// <summary>
        /// 边集合
        /// </summary>
        public IEnumerable<PolyLine> PolyLines
        {
            get => _lines;
            internal set => _lines = value.ToList();
        }

        #region 接口实现

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public GeometryType GeometryType => GeometryType.Polygon;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<Coordinate> Coordinates
        {
            get
            {
                var list = new List<Coordinate>();
                foreach (var polyLine in _lines)
                {
                    list.AddRange(polyLine.Coordinates);
                }

                return list;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PartsNum => _lines.Count;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PointsNum => _lines.Sum(e => e.PartsNum);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnvelope Envelope
        {
            get
            {
                if (PartsNum > 0)
                {
                    var envelope = _lines[0].Envelope;

                    for (int i = 1; i < PartsNum; i++)
                    {
                        envelope.Update(_lines[i].Envelope);
                    }

                    return envelope;
                }

                return null;
            }
        }

        #endregion
    }
}