using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PDDLTools.Windows.ResourceDictionary
{
    // Based on https://stackoverflow.com/a/5203530
    public class ArrowLine
    {
        public Path Path { get; internal set; }

        private Point _startPoint;
        public Point StartPoint
        {
            get
            {
                return _startPoint;
            }
            set
            {
                if (_startPoint != value)
                {
                    _startPoint = value;
                    UpdateLine();
                }
            }
        }

        private Point _endPoint;
        public Point EndPoint
        {
            get
            {
                return _endPoint;
            }
            set
            {
                if (_endPoint != value)
                {
                    _endPoint = value;
                    UpdateLine();
                }
            }
        }

        private PathFigure _pathFigure;
        private LineSegment _seg1;
        private LineSegment _seg2;
        private LineSegment _seg3;
        private RotateTransform _transform;
        private LineGeometry _connectorGeometry;

        public void UpdateLine(Point? startPoint = null, Point? endPoint = null)
        {
            if (startPoint != null)
                _startPoint = startPoint.Value;
            if (endPoint != null)
                _endPoint = endPoint.Value;

            double theta = Math.Atan2((_endPoint.Y - _startPoint.Y), (_endPoint.X - _startPoint.X)) * 180 / Math.PI;
            Point p = new Point(_startPoint.X + ((_endPoint.X - _startPoint.X) / 1.35), _startPoint.Y + ((_endPoint.Y - _startPoint.Y) / 1.35));
            _pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            _seg1.Point = lpoint;
            _seg2.Point = rpoint;
            _seg3.Point = p;

            _transform.Angle = theta + 90;
            _transform.CenterX = p.X;
            _transform.CenterY = p.Y;

            _connectorGeometry.StartPoint = _startPoint;
            _connectorGeometry.EndPoint = _endPoint;
        }

        public ArrowLine(Point startPoint, Point endPoint)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((endPoint.Y - startPoint.Y), (endPoint.X - startPoint.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            _pathFigure = new PathFigure();
            Point p = new Point(startPoint.X + ((endPoint.X - startPoint.X) / 1.35), startPoint.Y + ((endPoint.Y - startPoint.Y) / 1.35));
            _pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            _seg1 = new LineSegment();
            _seg1.Point = lpoint;
            _pathFigure.Segments.Add(_seg1);

            _seg2 = new LineSegment();
            _seg2.Point = rpoint;
            _pathFigure.Segments.Add(_seg2);

            _seg3 = new LineSegment();
            _seg3.Point = p;
            _pathFigure.Segments.Add(_seg3);

            pathGeometry.Figures.Add(_pathFigure);
            _transform = new RotateTransform();
            _transform.Angle = theta + 90;
            _transform.CenterX = p.X;
            _transform.CenterY = p.Y;
            pathGeometry.Transform = _transform;
            lineGroup.Children.Add(pathGeometry);

            _connectorGeometry = new LineGeometry();
            _connectorGeometry.StartPoint = startPoint;
            _connectorGeometry.EndPoint = endPoint;
            lineGroup.Children.Add(_connectorGeometry);
            Path = new Path();
            Path.Data = lineGroup;
            Path.StrokeThickness = 2;
            Path.Stroke = Path.Fill = Brushes.White;

            _startPoint = startPoint;
            _endPoint = endPoint;
        }
    }
}
