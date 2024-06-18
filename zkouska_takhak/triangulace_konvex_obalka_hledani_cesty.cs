using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska_takhak
{
    public class triangulace_konvex_obalka_hledani_cesty : Form
    {
        private List<Point> points;
        private List<Triangle> triangles;
        private List<Point> convexHull;
        private Timer timer;

        public triangulace_konvex_obalka_hledani_cesty()
        {
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(this.MainForm_Paint);
            this.MouseDown += new MouseEventHandler(this.MainForm_MouseDown);
            this.points = new List<Point>();
            this.triangles = new List<Triangle>();
            this.convexHull = new List<Point>();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawPoints(g);
            DrawTriangles(g);
            DrawConvexHull(g);
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                points.Add(e.Location);
                this.Invalidate();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (points.Count > 2)
            {
                convexHull = ConvexHull(points);
                triangles = GreedyTriangulation(convexHull);
                this.Invalidate();
            }
        }

        private void DrawPoints(Graphics g)
        {
            foreach (var point in points)
            {
                g.FillEllipse(Brushes.Black, point.X - 2, point.Y - 2, 4, 4);
            }
        }

        private void DrawTriangles(Graphics g)
        {
            foreach (var triangle in triangles)
            {
                g.DrawPolygon(Pens.Blue, triangle.GetPoints());
            }
        }

        private void DrawConvexHull(Graphics g)
        {
            if (convexHull.Count > 1)
            {
                g.DrawPolygon(Pens.Red, convexHull.ToArray());
            }
        }

        private List<Point> ConvexHull(List<Point> points)
        {
            // Implementace algoritmu Graham Scan
            List<Point> sortedPoints = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            Stack<Point> hull = new Stack<Point>();

            foreach (var point in sortedPoints)
            {
                while (hull.Count > 1 && CrossProductLength(hull.ElementAt(1), hull.Peek(), point) <= 0)
                {
                    hull.Pop();
                }
                hull.Push(point);
            }

            int lowerHullCount = hull.Count;
            for (int i = sortedPoints.Count - 2; i >= 0; i--)
            {
                var point = sortedPoints[i];
                while (hull.Count > lowerHullCount && CrossProductLength(hull.ElementAt(1), hull.Peek(), point) <= 0)
                {
                    hull.Pop();
                }
                hull.Push(point);
            }

            hull.Pop(); // Remove the last point because it's the same as the first one
            return hull.ToList();
        }

        private double CrossProductLength(Point a, Point b, Point c)
        {
            double BAx = b.X - a.X;
            double BAy = b.Y - a.Y;
            double CAx = c.X - a.X;
            double CAy = c.Y - a.Y;
            return BAx * CAy - BAy * CAx;
        }

        private List<Triangle> GreedyTriangulation(List<Point> points)
        {
            List<Triangle> result = new List<Triangle>();
            for (int i = 1; i < points.Count - 1; i++)
            {
                result.Add(new Triangle(points[0], points[i], points[i + 1]));
            }
            return result;
        }

     
    }

    public class Triangle
    {
        public Point A { get; set; }
        public Point B { get; set; }
        public Point C { get; set; }

        public Triangle(Point a, Point b, Point c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public Point[] GetPoints()
        {
            return new Point[] { A, B, C };
        }
    }
}
