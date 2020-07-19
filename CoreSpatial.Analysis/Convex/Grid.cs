using CoreSpatial.BasicGeometrys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreSpatial.Analysis.Convex
{
    class Grid
    {
        private Dictionary<int, Dictionary<int, List<Coordinate>>> _cells;
        private double _reverseCellSize;
        private double _cellSize;

        public Grid(IEnumerable<Coordinate> points, double cellSize)
        {
            _cells = new Dictionary<int, Dictionary<int, List<Coordinate>>>();

            _cellSize = cellSize;
            _reverseCellSize = 1 / cellSize;

            var pointList = points.ToList();
            foreach (var point in pointList)
            {
                var x = CoordToCellNum(point.X);
                var y = CoordToCellNum(point.Y);
                if (!_cells.ContainsKey(x))
                {
                    _cells.Add(x, new Dictionary<int, List<Coordinate>>());;
                }
                if (!_cells[x].ContainsKey(y))
                {
                    _cells[x].Add(y, new List<Coordinate>());
                }

                _cells[x][y].Add(point);
            }
        }

        private int CoordToCellNum(double x)
        {
            return (int)Math.Truncate(x * this._reverseCellSize);
        }

        internal double[] ExtendBbox(double[] bbox, int scaleFactor)
        {
            return new double[]
            {
                bbox[0] - (scaleFactor * this._cellSize),
                bbox[1] - (scaleFactor * this._cellSize),
                bbox[2] + (scaleFactor * this._cellSize),
                bbox[3] + (scaleFactor * this._cellSize)
            };
        }

        internal List<Coordinate> RangePoints(double[] bbox)
        {
            var tlCellX = this.CoordToCellNum(bbox[0]);
            var tlCellY = this.CoordToCellNum(bbox[1]);
            var brCellX = this.CoordToCellNum(bbox[2]);
            var brCellY = this.CoordToCellNum(bbox[3]);
            var points = new List<Coordinate>();

            for (var x = tlCellX; x <= brCellX; x++)
            {
                for (var y = tlCellY; y <= brCellY; y++)
                {
                    points.Add(new Coordinate(x,y));
                }
            }

            return points;
        }

        internal List<Coordinate> RemovePoint(Coordinate point)
        {
            var cellX = CoordToCellNum(point.X);
            var cellY = CoordToCellNum(point.Y);
            var cell = _cells[cellX][cellY];

            cell.Remove(point);

            return cell;
            
            //Coordinate pointIdxInCell = null;

            //foreach (var item in cell)
            //{
            //    if (item == point)
            //    {
            //        pointIdxInCell = item;
            //        break;
            //    }
            //}

            //cell.Remove(pointIdxInCell);
            //return cell;
        }
    }
}
