using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BCUnity
{
    public class Plane
    {
        public SmartVector Up
        {
            get
            {
                return this._up;
            }
        }

        public SmartVector Normal
        {
            get
            {
                return this._normal;
            }
        }

        public SmartVector Right
        {
            get
            {
                return this._right;
            }
        }

        public UnityEngine.Plane UnityPlane
        {
            get
            {
                return this._plane;
            }
        }

        public Plane() : this(SmartVector.Up, SmartVector.Right, SmartVector.Forward)
        {

        }

        public Plane(SmartVector Up, SmartVector Right, SmartVector Normal)
        {
            this._up = Up;
            this._normal = Normal;
            this._right = Right;
            this._plane = new UnityEngine.Plane((Vector3)this.Normal, Vector3.zero);
        }

        public SmartVector Rotate(float angle, SmartVector sv)
        {
            return sv.RotateClone(this.Normal, angle);
        }

        public SmartVector Reflect(SmartVector Normal, SmartVector sv)
        {
            SmartVector ret = this.ClosestPoint(sv.Reflect(Normal));
            return ret;
        }

        public SmartVector ClosestPoint(SmartVector wv)
        {
            Vector3 v = (Vector3)wv;
            //  get the distance to the plane
            float distance = this._plane.GetDistanceToPoint(v);
            if (distance == 0)
            {
                return wv;
            }
            Vector3 norm = this._plane.normal.normalized * distance;
            Vector3 a = v + norm;
            Vector3 b = v - norm;
            float a_dist = Mathf.Abs(this._plane.GetDistanceToPoint(a));
            float b_dist = Mathf.Abs(this._plane.GetDistanceToPoint(b));
            if (a_dist < b_dist)
            {
                return SmartVector.CreateWorldPoint(a.x, a.y, a.z);
            }
            return SmartVector.CreateWorldPoint(b.x, b.y, b.z);
        }

        public float PlanarAngle(SmartVector sv)
        {
            return BCUnity.Helper.Math.PlanarAngle((Vector3)this._up, (Vector3)this._right, (Vector3)sv.World, false);
        }

        public bool Place(Transform Target, SmartVector Start, SmartVector End, float width, float height)
        {
            try
            {
                SmartVector absol = End - Start;
                float angle = this.PlanarAngle(absol);
                SmartVector mid = (Start + (absol / 2));
                Target.position = (Vector3)mid;
                Target.Rotate((Vector3)this.Normal, angle);
                Target.localScale = new Vector3(width, absol.Magnitude, height);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private SmartVector _up;
        private SmartVector _normal;
        private SmartVector _right;
        private UnityEngine.Plane _plane;
    }
}
