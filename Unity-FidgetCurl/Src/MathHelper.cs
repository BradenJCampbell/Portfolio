using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper {
    public static float PlanarAngle(Vector3 PlaneUp, Vector3 PlaneRight, Vector3 Vector, bool validate = true)
    {
        if (validate)
        {
            Plane p = new Plane(PlaneUp, PlaneRight, Vector3.zero);
            if (p.GetDistanceToPoint(Vector) != 0)
            {
                return float.NaN;
            }
        }
        float polar_angle = Vector3.Angle(PlaneUp, Vector);
        if (Vector3.Angle(PlaneRight, Vector) > 90)
        {
            return -polar_angle;
        }
        return polar_angle;
    }
}
