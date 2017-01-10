using UnityEngine;
using System.Collections.Generic;

public static partial class Helper
{
    public partial class Line
    {
        public Vector3 p1;
        public Vector3 p2;

        public Line(Vector3 p1, Vector3 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Ray ToRay()
        {
            return new Ray(this.p1, this.p2 - this.p1);
        }

        public bool Intersect(out Vector3 Intersect, Line l)
        {
            return Line.Intersect(out Intersect, this.p1, this.p2, l.p1, l.p2);
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            return Line.ClosestPoint(this.p1, this.p2, point);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="closest">the closest point on line 2 to this line</param>
        /// <param name="l2">the other line</param>
        /// <returns>true, if a point is found</returns>
        public bool ClosestPoint(out Vector3 closest, Line l2)
        {
            Vector3 closest_a;
            return Line.ClosestPoints(out closest_a, out closest, this.p1, this.p2, l2.p1, l2.p2);
        }

        public float Distance(Vector3 point)
        {
            return Vector3.Distance(point, this.ClosestPoint(point));
        }

        public float Distance(Line l2)
        {
            float dist;
            if (this.Distance(out dist, l2))
            {
                return dist;
            }
            return -1;
        }

        public bool Distance(out float result, Line l2)
        {
            Vector3 closest_this;
            Vector3 closest_l2;
            if (Line.ClosestPoints(out closest_this, out closest_l2, this.p1, this.p2, l2.p1, l2.p2))
            {
                result = Vector3.Distance(closest_this, closest_l2);
                return true;
            }
            result = -1;
            return false;
        }

        public partial class Segment : Line
        {
            public Segment(Vector3 p1, Vector3 p2) : base(p1, p2) { }

            public bool Intersect(out Vector3 Intersect, Vector3 p1, Vector3 p2)
            {
                return Line.Intersect(out Intersect, this.p1, this.p2, p1, p2) && this.Contains(Intersect);
            }

            public new bool Intersect(out Vector3 Intersect, Line l)
            {
                return Line.Intersect(out Intersect, this.p1, this.p2, l.p1, l.p2) && this.Contains(Intersect);
            }

            public bool Intersect(out Vector3 Intersect, Segment s)
            {
                if (Line.Intersect(out Intersect, this.p1, this.p2, s.p1, s.p2))
                {
                    return this.Contains(Intersect) && s.Contains(Intersect);
                }
                return false;
            }

            public bool Contains(Vector3 point)
            {
                return Segment.Contains(this.p1, this.p2, point);
            }

            public new Vector3 ClosestPoint(Vector3 point)
            {
                return Segment.ClosestPoint(this.p1, this.p2, point);
            }
        }
    }

}
