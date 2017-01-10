using UnityEngine;
using System.Collections.Generic;

public partial class TrackBehaviour
{ 
	public static partial class TrackCalc
    {
        private static void _init()
        {
            if (TrackCalc._needs_init)
            {
                TrackCalc._normal = Vector3.up;
                TrackCalc._level = new Plane(TrackCalc._normal, 0);
                TrackCalc._needs_init = false;
            }
        }

        private static bool _pivot_point_equilateral(out Vector3 pivot, Vector3 a, Vector3 b, Vector3 c)
        {
            DebugStatement dbs = new DebugStatement();
            if (TrackCalc.IsParallel(b - a, c - b))
            {
                // vector a->b and vector b->c are in the same direction, no possible pivot exists 
                pivot = Vector3.zero;
                return false;
            }
            //  specify b as the origin, and shift
            Vector3 first = a - b;
            Vector3 second = c - b;
            float theta = Vector3.Angle(first, second);
            Vector3 hyp_a = c - a; // hypotenuse vector with origin at a
            Vector3 hyp_c = a - c;  // hypotenuse vector with origin at c
            // calculate the adjacent sides of the triangle
            Vector3 adj_a = Vector3.RotateTowards(hyp_a, first, (float)0.5 * theta * Mathf.Deg2Rad, hyp_a.magnitude + first.magnitude);
            Vector3 adj_c = Vector3.RotateTowards(hyp_c, second, (float)0.5 * theta * Mathf.Deg2Rad, hyp_c.magnitude + second.magnitude);
            return StaticHelper.Intersect(out pivot, a, adj_a, c, adj_c);
        }

        private static Vector3 _curve_point_adjust(Vector3 a, Vector3 b, Vector3 c, Vector3 point)
        {
            Vector3 pivot;
            if (TrackCalc.PivotPoint(out pivot, a, b, c))
            {
                Vector3 intersect;
                if (StaticHelper.IntersectSegment(out intersect, pivot, point - pivot, a, b - a))
                {
                    return intersect;
                }
                if (StaticHelper.IntersectSegment(out intersect, pivot, point - pivot, b, c - b))
                {
                    return intersect;
                }
            }
            return point;
        }

        private static Vector3 _curve_point_adjust(Vector3 Point, Vector3 Pivot, Plane Level)
        {
            //  find the closest match line on the level plane
            Vector3 start_level = Vector3.ProjectOnPlane(Point, Level.normal);
            Vector3 end_level = Vector3.ProjectOnPlane(Pivot, Level.normal);
            Vector3 intersect;
            if (StaticHelper.IntersectSegment(out intersect, Pivot, Point - Pivot, start_level, end_level - start_level))
            {
                return intersect;
            }
            return Point;
        }

        private static Vector3 _curve_point(Vector3 start, Vector3 end, float ratio)
        {
            return StaticHelper.RotateTowardsRatio(start, end, ratio);
        }

        private static TrackPathLine _curve_section(Vector3 start, Vector3 end, float ratio_start, float ratio_end, Vector3 offset)
        {
            return new TrackPathLine(offset + TrackCalc._curve_point(start, end, ratio_start), offset + TrackCalc._curve_point(start, end, ratio_end));
        }

        private static bool _curve_section(out TrackPathLine line, Vector3 a, Vector3 b, Vector3 c, float ratio_start, float ratio_end)
        {
            Vector3 pivot;
            if (TrackCalc.PivotPoint(out pivot, a, b, c))
            {
                Vector3 start = a - pivot;
                Vector3 end = c - pivot;
                Vector3 raw_p1 = pivot + TrackCalc._curve_point(start, end, ratio_start);
                Vector3 raw_p2 = pivot + TrackCalc._curve_point(start, end, ratio_end);
                //Vector3 adj_p1 = TrackCalc._curve_point_adjust(a, b, c, raw_p1);
                Vector3 adj_p1 = TrackCalc._curve_point_adjust(raw_p1, pivot, TrackCalc.WorldLevel);
                //Vector3 adj_p2 = TrackCalc._curve_point_adjust(a, b, c, raw_p2);
                Vector3 adj_p2 = TrackCalc._curve_point_adjust(raw_p2, pivot, TrackCalc.WorldLevel);
                line = new TrackPathLine(adj_p1, adj_p2);
                return true;
            }
            line = new TrackPathLine(Vector3.zero, Vector3.zero);
            return false;
        }

        private static bool _curve_doubled(ref List<TrackPathLine> container, Vector3 a, Vector3 b, Vector3 c, float segment_length)
        {
            //  calculate pivot point
            Vector3 pivot;
            if (TrackCalc.PivotPoint(out pivot, a, b, c))
            {
                // shift to origin
                Vector3 start = a - pivot;
                Vector3 mid = b - pivot;
                Vector3 end = c - pivot;
                //  calculate the curved distance between points
                float theta = Vector3.Angle(start, end);
                float circum = Mathf.PI * (start.magnitude + end.magnitude);  // 2 PI r   where r = (start + end) / 2
                float dist = circum * theta / 360;
                //  calculate the number of sections
                float count = Mathf.CeilToInt(dist / segment_length);
                float ratio_inc = 1 / count;
                float ratio_inc_half = (float)0.5 / count;
                //  add a piece at start
                container.Add(new TrackPathLine(a - ((b - a).normalized * segment_length), a + ((b - a).normalized * segment_length)));
                for (int i = 0; i < count; i++)
                {
                    TrackPathLine line;
                    if (i > 0)
                    {
                        float ratio_start = (i * ratio_inc) - ratio_inc_half;
                        float ratio_end = (i * ratio_inc) + ratio_inc_half;
                        //  add the "backpedal" section
                        if (TrackCalc._curve_section(out line, a, b, c, ratio_start, ratio_end))
                        {
                            container.Add(line);
                        }
                        else
                        {
                            container.Add(TrackCalc._curve_section(start, end, ratio_start, ratio_end, pivot));
                        }
                    }
                    //  add the current section
                    container.Add(TrackCalc._curve_section(start, end, (i * ratio_inc), (i + 1) * ratio_inc, pivot));
                }
                //  add a piece at end
                container.Add(new TrackPathLine(c - ((c - b).normalized * segment_length), c + ((c - b).normalized * segment_length)));
                return true;
            }
            return false;
        }

        private static bool _curve(ref List<TrackPathLine> container, Vector3 a, Vector3 b, Vector3 c, float segment_length)
        {
            //  calculate pivot point
            Vector3 pivot;
            if (TrackCalc.PivotPoint(out pivot, a, b, c))
            {
                // shift to origin
                Vector3 start = a - pivot;
                Vector3 mid = b - pivot;
                Vector3 end = c - pivot;
                //  calculate the curved distance between points
                float theta = Vector3.Angle(start, end);
                float circum = Mathf.PI * (start.magnitude + end.magnitude);  // 2 PI r   where r = (start + end) / 2
                float dist = circum * theta / 360;
                //  calculate the number of sections
                float count = Mathf.CeilToInt(dist / segment_length);
                Vector3 curr = TrackCalc._curve_point(start, end, 0);
                Vector3 next;
                for (int i = 1; i <= count; i++)
                {
                    //  determine the curve direction for this section
                    Vector3 curve_dir = TrackCalc._curve_point(start, end, (i + (float)0.5) / count);
                    //  calculate the next point
                    next = TrackCalc._curve_point(start, end, i / count);
                    container.Add(new TrackPathLine(pivot + curr, pivot + next));
                    curr = next;
                }
                return true;
            }
            return false;
        }

        private static bool _curve(ref List<TrackPathLine> container, Vector3 a, Vector3 b, Vector3 c, float segment_length, float level_magnitude, Plane level_plane)
        {
            //  calculate pivot point
            Vector3 pivot;
            if (TrackCalc.PivotPoint(out pivot, a, b, c))
            {
                //  shift to origin
                Vector3 start = a - pivot;
                Vector3 mid = b - pivot;
                Vector3 end = c - pivot;
                //  calculate the levels for endpoints
                Vector3 start_level = TrackCalc._level_vector(level_plane, start, level_magnitude);
                Vector3 end_level = TrackCalc._level_vector(level_plane, end, level_magnitude);
                //  apply levels
                start = start + start_level;
                end = end + end_level;
                //  calculate the curved distance between points
                float theta = Vector3.Angle(start, end);
                float circum = Mathf.PI * (start.magnitude + end.magnitude);  // 2 PI r   where r = (start + end) / 2
                float dist = circum * theta / 360;
                //  calculate the number of sections
                float count = Mathf.CeilToInt(dist / segment_length);
                Vector3 curr = TrackCalc._curve_point(start, end, 0);
                Vector3 next;
                for (int i = 1; i <= count; i++)
                {
                    //  determine the curve direction for this section
                    Vector3 curve_dir = TrackCalc._curve_point(start, end, (i - (float)0.5) / count);
                    //  calculate the level for this section
                    Vector3 section_level = TrackCalc._level_vector(level_plane, curve_dir, level_magnitude);
                    //  calculate the next point
                    next = TrackCalc._curve_point(start, end, i / count);
                    container.Add(new TrackPathLine(pivot + curr - section_level, pivot + next - section_level));
                    curr = next;
                }

            }
            return false;
        }

        private static bool _intermediate_section(ref List<TrackPathLine> container, Vector3 a, Vector3 b, Vector3 c, float segment_length, float level_magnitude, Plane level_plane)
        {
            Vector3 dir = StaticHelper.BuildVector(c - a, segment_length / 2);
            container.Add(new TrackPathLine(b - dir, b + dir));
            return true;
        }

        private static bool _straight(ref List<TrackPathLine> container, Vector3 start, Vector3 end, float length = 1)
        {
            int count = Mathf.CeilToInt((end - start).magnitude / length);
            Vector3 offset = (end - start) / count;
            Vector3 curr = start;
            for (int i = 1; i <= count; i++)
            {
                Vector3 next = curr + offset;
                container.Add(new TrackPathLine(curr, next));
                curr = next;
            }
            return true;
        }

        private static Vector3 _level_vector(Plane level, Vector3 vector, float magnitude)
        {
            return StaticHelper.BuildVector(Vector3.ProjectOnPlane(vector, level.normal), magnitude);
        }

    }
}
