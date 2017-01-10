using UnityEngine;
using System.Collections.Generic;

public static class StaticHelper
{
    public static int ModularIndex(int count, int index)
    {
        if (index < 0)
        {
            index += Mathf.CeilToInt(-1 * (float)index / (float)count) * count;
        }
        return index % count;
    }

    public static Vector3[] AllRayCastHits(Vector3 Origin, Vector3 Direction, float MaxDistance = 0)
    {
        if (MaxDistance <= 0)
        {
            MaxDistance = Direction.magnitude;
        }
        Vector3 curr_origin = Origin;
        RaycastHit hit;
        List<Vector3> ret = new List<Vector3>();
        while (MaxDistance > 0 && Physics.Raycast(curr_origin, Direction, out hit, MaxDistance))
        {
            curr_origin = hit.point;
            MaxDistance -= hit.distance;
            ret.Add(hit.point);
        }
        return ret.ToArray();
    }

    /// <summary>
    /// executes a raycast, but from point a to point b, instead of origin to origin + direction
    /// </summary>
    public static bool RaycastPoint(out RaycastHit hit, Vector3 From, Vector3 To, LayerMask CollisionLayersMask)
    {
        return Physics.Raycast(From, To - From, out hit, (To - From).magnitude, CollisionLayersMask);
    }

    public static Vector3 BuildVector(Vector3 Direction, float Magnitude)
    {
        return Direction.normalized * Magnitude;
    }

    public static Vector3 ToOrigin(Vector3 vector, Vector3 origin)
    {
        return vector - origin;
    }

    public static void DebugDrawStar(Vector3 WorldPosition, float Radius, Color Colour)
    {

        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(-1, -1, -1), Radius), Colour);
        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(-1, -1, 1), Radius), Colour);
        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(-1, 1, -1), Radius), Colour);
        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(-1, 1, 1), Radius), Colour);
        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(1, -1, -1), Radius), Colour);
        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(1, -1, 1), Radius), Colour);
        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(1, 1, -1), Radius), Colour);
        Debug.DrawRay(WorldPosition, BuildVector(new Vector3(1, 1, 1), Radius), Colour);
    }


    static public Vector3 ClosestPoint(Vector3 Target, params Vector3[] Points)
    {
        if (Points.Length > 0)
        {
            Vector3 shortest_point = Points[0];
            for (int i = 1; i < Points.Length; i++)
            {
                if (Vector3.Distance(Target, Points[i]) < Vector3.Distance(Target, shortest_point))
                {
                    shortest_point = Points[i];
                }
            }
            return shortest_point;
        }
        return Target;
    }

    static public Vector3 ClosestPerpendicular(Vector3 Target, Vector3 QuasiNormal)
    {
        if (Vector3.Angle(Target, QuasiNormal) < 90)
        {
            Target *= -1;
        }
        return BuildVector(Vector3.RotateTowards(Target, QuasiNormal, 90 * Mathf.Deg2Rad * Mathf.PI, Target.magnitude + QuasiNormal.magnitude), Target.magnitude);
    }

    static public Vector3 RotateTowardsMagnitude(Vector3 start, Vector3 end, float radians_angle, float magnitude)
    {
        return StaticHelper.BuildVector(Vector3.RotateTowards(start, end, radians_angle, float.MaxValue), magnitude);
    }

    static public Vector3 RotateTowardsRatio(Vector3 start, Vector3 end, float ratio)
    {
        float angle = ratio * Mathf.Deg2Rad * Vector3.Angle(start, end);
        float magnitude = start.magnitude + (ratio * (end.magnitude - start.magnitude));
        return StaticHelper.BuildVector(Vector3.RotateTowards(start, end, angle, float.MaxValue), magnitude);
    }

    static public float AnglePlane(Vector3 vector, Plane plane)
    {
        return AnglePlane(vector, plane.normal);
    }
    static public float AnglePlane(Vector3 vector, Vector3 plane_normal)
    {
        return Vector3.Angle(vector, Vector3.ProjectOnPlane(vector, plane_normal));
    }
    // had troubles with the Math3d intersect, so this is a "dirty workaround" that does the job
    public static bool Intersect(out Vector3 intersect, Vector3 origin1, Vector3 direction1, Vector3 origin2, Vector3 direction2)
    {
        float mult = 5000;  // only seems to work with this multiplier; no clue why
        return Math3d.LineLineIntersection(out intersect, origin1, mult * direction1.normalized, origin2, mult * direction2.normalized);
    }
    public static bool Intersect(out Vector3 intersect, Ray Line1, Ray Line2)
    {
        return Intersect(out intersect, Line1.origin, Line1.direction, Line2.origin, Line2.direction);
    }
    public static bool IntersectSegment(out Vector3 intersect, Vector3 origin1, Vector3 direction1, Vector3 origin2, Vector3 direction2)
    {
        if (StaticHelper.Intersect(out intersect, origin1, direction1, origin2, direction2))
        {
            return Helper.Line.Segment.Contains(origin1, direction1, intersect) && Helper.Line.Segment.Contains(origin2, direction2, intersect);
        }
        return false;
    }
}

