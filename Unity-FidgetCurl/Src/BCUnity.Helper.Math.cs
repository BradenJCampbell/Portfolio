using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BCUnity
{
    namespace Helper
    {
        public class Math
        {
            public static SmartVector Scale(SmartVector Start, SmartVector End, float multiplier)
            {
                return Start + (multiplier * (End - Start));
            }

            public static Vector3 EulerAngles(Vector3 v)
            {
                Quaternion rot = Quaternion.FromToRotation(v, Vector3.up);
                return rot.eulerAngles;
                /*
                float about_x = PlanarAngle(Vector3.forward, Vector3.up, v, false);
                float about_y = PlanarAngle(Vector3.right, Vector3.forward, v, false);
                float about_z = PlanarAngle(Vector3.up, Vector3.right, v, false);
                return new Vector3(about_x, about_y, about_z);
                */
            }

            public static float PlanarAngle(Vector3 PlaneUp, Vector3 PlaneRight, Vector3 Vector, bool validate = true)
            {
                if (validate)
                {
                    UnityEngine.Plane p = new UnityEngine.Plane(PlaneUp, PlaneRight, Vector3.zero);
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
    }
}
