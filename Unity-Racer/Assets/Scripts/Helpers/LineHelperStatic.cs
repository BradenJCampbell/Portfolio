using UnityEngine;
using System.Collections.Generic;

public static partial class Helper
{
    public static readonly float VectorErrorAllowance = (float)0.00001;

    public partial class Line
    {
        public static Ray ToRay(Vector3 p1, Vector3 p2)
        {
            return new Ray(p1, p2 - p1);
        }

        public static bool Intersect(out Vector3 intersect, Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            return Math3d.LineLineIntersection(out intersect, a1, a2 - a1, b2, b2 - b1);
        }

        public static Vector3 ClosestPoint(Vector3 p1, Vector3 p2, Vector3 given)
        {
            //  presume p1 is the origin
            Vector3 line = p2 - p1;
            Vector3 shifted_point = given - p1;
            //  project shifted onto line
            Vector3 projected_point = Vector3.Project(shifted_point, line);
            //  shift back to previous coordinate system
            return p2 + projected_point;
        }

        public static bool ClosestPoints(out Vector3 closest_a, out Vector3 closest_b, Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            return Math3d.ClosestPointsOnTwoLines(out closest_a, out closest_b, a1, a2 - a1, b1, b2 - b1);
        }

        public static bool Contains(Vector3 p1, Vector3 p2, Vector3 point)
        {
            float a = Vector3.Distance(p1, p2);
            float b = Vector3.Distance(p1, point);
            float c = Vector3.Distance(p2, point);
            return side_check(a, b, c) || side_check(b, a, c) || side_check(c, a, b);
        }

        //  check if b + c = a
        protected static bool side_check(float a, float b, float c)
        {
            return Mathf.Abs(a - b - c) <= VectorErrorAllowance;
        }

        public partial class Segment : Line
        {
            public static new Vector3 ClosestPoint(Vector3 p1, Vector3 p2, Vector3 point)
            {
                //  find closest point on entire line
                Vector3 abs_closest = Line.ClosestPoint(p1, p2, point);
                //  determine if point is on segment
                if (Segment.Contains(p1, p1 - p2, abs_closest))
                {
                    return abs_closest;
                }
                else if (Vector3.Distance(p1, abs_closest) < Vector3.Distance(p2, abs_closest))
                {
                    return p1;
                }
                else
                {
                    return p2;
                }
            }

            public static new bool Contains(Vector3 p1, Vector3 p2, Vector3 point)
            {
                return side_check(Vector3.Distance(p1, p2), Vector3.Distance(p1, point), Vector3.Distance(p2, point));
            }

            public static new bool Intersect(out Vector3 Intersect, Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
            {
                if (Line.Intersect(out Intersect, a1, a2, b1, b2))
                {
                    return Segment.Contains(a1, a2, Intersect) && Segment.Contains(b1, b2, Intersect);
                }
                return false;
            }
        }
    }

}
