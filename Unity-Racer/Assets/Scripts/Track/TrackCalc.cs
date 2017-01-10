using UnityEngine;
using System.Collections.Generic;

public partial class TrackBehaviour
{
    public static partial class TrackCalc
    {
        private static bool _needs_init = true;
        private static Plane _level;
        private static Vector3 _normal;
        public static readonly float RoadBreadth = (float)0.1;

        public static Vector3 WorldNormal
        {
            get { TrackCalc._init(); return TrackCalc._normal; }
        }

        public static Plane WorldLevel
        {
            get { TrackCalc._init(); return TrackCalc._level; }
        }

        public static Vector3 Normal(Vector3 Start, Vector3 End)
        {
            return StaticHelper.ClosestPerpendicular(End - Start, TrackCalc.WorldNormal).normalized;
        }

        public static Vector3 Level(Vector3 Start, Vector3 End)
        {
            Vector3 original = End - Start;
            Vector3 level = Vector3.Cross(original, TrackCalc.Normal(Start, End)).normalized;
            return level;
        }

        public static Vector3 CurveDirection(Vector3 a, Vector3 b, Vector3 c)
        {
            //  normalize vectors
            Vector3 first = b - a;
            Vector3 second = b - c;
            //  calculate direction
            return (first + second).normalized;
        }

        public static bool Straight(ref List<TrackPathLine> container, Vector3 start, Vector3 end, float length = 1)
        {
            return TrackCalc._straight(ref container, start, end, length);
        }

        public static bool Rail(ref List<TrackPathLine> container, Vector3 a, Vector3 b, Vector3 c, float rail_length)
        {
            Vector3 pivot;
            if (TrackCalc.PivotPoint(out pivot, a, b, c))
            {
                return TrackCalc._curve(ref container, a, b, c, rail_length);
            }
            return TrackCalc._straight(ref container, a, c, rail_length);
        }

        public static bool Road(ref List<TrackPathLine> container, Vector3 a, Vector3 b, Vector3 c, float road_width)
        {
            Vector3 pivot;
            if (TrackCalc.PivotPoint(out pivot, a, b, c))
            {
                //  this is the dirty way to distinguish how to render the curve
                //  (there were problems with curves being calculated wrong)
                //if (Mathf.Abs(TrackCalc.WorldLevel.GetDistanceToPoint(TrackCalc.CurveDirection(a, b, c))) > 0.0001)
                {
                    //return TrackCalc._curve(ref container, a, b, c, TrackCalc.RoadBreadth);
                }
                //  curve pivots around y-axis
                //return TrackCalc._curve(ref container, a, b, c, TrackCalc.RoadBreadth, road_width / 2, TrackCalc.WorldLevel);
                return TrackCalc._curve_doubled(ref container, a, b, c, TrackCalc.RoadBreadth);
            }
            return TrackCalc._straight(ref container, a, c, road_width);
        }

        public static bool Connector(ref List<TrackPathLine> container, Vector3 a, Vector3 b, Vector3 c, float road_width)
        {
            return TrackCalc._intermediate_section(ref container, a, b, c, TrackCalc.RoadBreadth, road_width, TrackCalc.WorldLevel);
        }

        public static float Angle(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 first = a - b;
            Vector3 second = c - b;
            Vector3 mid = first + second;
            return Vector3.Angle(first, mid) + Vector3.Angle(mid, second);
        }

        public static bool PivotPoint(out Vector3 pivot, Vector3 a, Vector3 b, Vector3 c)
        {
            if (TrackCalc.IsParallel(b - a, c - b))
            {
                // vector a->b and vector b->c are in the same direction, no possible pivot exists 
                pivot = Vector3.zero;
                return false;
            }
            if (TrackCalc.Angle(a, b, c) > 90)
            {
                return TrackCalc._pivot_point_equilateral(out pivot, a, b, c);
            }
            return StaticHelper.Intersect(out pivot, c, a - b, a, c - b);
        }

        public static bool IsParallel(Vector3 a, Vector3 b)
        {
            return a.normalized == b.normalized;
        }


    }
}
