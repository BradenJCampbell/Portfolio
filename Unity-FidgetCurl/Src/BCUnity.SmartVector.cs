using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BCUnity
{
    public enum SmartVectorType
    {
        Point,
        Vector,
        Direction
    }

    public struct SmartVector
    {
        public static SmartVector CreateWorldVector(float x, float y, float z)
        {
            return CreateLocalVector(x, y, z, null);
        }

        public static SmartVector CreateLocalVector(float x, float y, float z, Transform Space)
        {
            return new BCUnity.SmartVector(x, y, z, SmartVectorType.Vector, Space);
        }

        public static SmartVector CreateWorldPoint(float x, float y, float z)
        {
            return CreateLocalPoint(x, y, z, null);
        }

        public static SmartVector CreateLocalPoint(float x, float y, float z, Transform Space)
        {
            return new BCUnity.SmartVector(x, y, z, SmartVectorType.Point, Space);
        }

        public static SmartVector CreateWorldDirection(float x, float y, float z)
        {
            return CreateLocalDirection(x, y, z, null);
        }

        public static SmartVector CreateLocalDirection(float x, float y, float z, Transform Space)
        {
            return new BCUnity.SmartVector(x, y, z, SmartVectorType.Direction, Space);
        }

        public static SmartVector Zero
        {
            get
            {
                return new SmartVector(Vector3.zero);
            }
        }

        public static SmartVector Up
        {
            get
            {
                return new SmartVector(Vector3.up);
            }
        }

        public static SmartVector Right
        {
            get
            {
                return new SmartVector(Vector3.right);
            }
        }

        public static SmartVector Forward
        {
            get
            {
                return new SmartVector(Vector3.forward);
            }
        }

        public static explicit operator Vector2(SmartVector sv)
        {
            return (Vector2)sv._pos;
        }

        public static explicit operator Vector3(SmartVector sv)
        {
            return sv._pos;
        }

        public static SmartVector operator +(SmartVector sv1, SmartVector sv2)
        {
            return new BCUnity.SmartVector(sv1.World._pos + sv2.World.Localize(sv1)._pos, sv1.Type, sv1.Space);
        }

        public static SmartVector operator -(SmartVector sv1, SmartVector sv2)
        {
            return new BCUnity.SmartVector(sv1.World._pos - sv2.World.Localize(sv1)._pos, sv1.Type, sv1.Space);
        }

        public static SmartVector operator -(SmartVector sv)
        {
            return -1 * sv;
        }

        public static SmartVector operator *(SmartVector sv, float f)
        {
            return new SmartVector(sv._pos * f, sv.Type, sv.Space);
        }

        public static SmartVector operator *(float f, SmartVector sv)
        {
            return new SmartVector(sv._pos * f, sv.Type, sv.Space);
        }

        public static SmartVector operator /(SmartVector sv, float f)
        {
            return new SmartVector(sv._pos / f, sv.Type, sv.Space);
        }

        private SmartVector(Vector3 Vector, SmartVectorType Type = SmartVectorType.Vector, Transform Space = null)
        {
            this._pos = Vector;
            this._type = Type;
            this._space = Space;
        }

        private SmartVector(float x, float y, float z, SmartVectorType Type = SmartVectorType.Vector, Transform Space = null) : this(new Vector3(x, y, z), Type, Space)
        {

        }

        public bool IsWorld
        {
            get
            {
                return this._space == null;
            }
        }

        public bool IsLocal
        {
            get
            {
                return this._space != null;
            }
        }

        public SmartVector Normalized
        {
            get
            {
                return new SmartVector(this._pos.normalized, this.Type, this.Space);
            }
        }

        public float Magnitude
        {
            get
            {
                return this._pos.magnitude;
            }
        }

        public SmartVector World
        {
            get
            {
                if (this.IsWorld)
                {
                    return this;
                }
                Vector3 ret;
                switch (this._type)
                {
                    case SmartVectorType.Point:
                        ret = this._space.TransformPoint(this._pos);
                        break;
                    case SmartVectorType.Direction:
                        ret = this._space.TransformDirection(this._pos);
                        break;
                    case SmartVectorType.Vector:
                        ret = this._space.TransformVector(this._pos);
                        break;
                    default:
                        ret = Vector3.zero;
                        break;
                }
                return new SmartVector(ret, this._type, null);
            }
        }

        public Vector3 Vector3
        {
            get
            {
                return (Vector3)this._pos;
            }
        }

        public Vector2 Vector2
        {
            get
            {
                return (Vector2)this._pos;
            }
        }

        public float x
        {
            set
            {
                this._pos.x = value;
            }
            get
            {
                return this._pos.x;
            }
        }

        public float y
        {
            set
            {
                this._pos.y = value;
            }
            get
            {
                return this._pos.y;
            }
        }

        public float z
        {
            set
            {
                this._pos.z = value;
            }
            get
            {
                return this._pos.z;
            }
        }

        public Transform Space
        {
            get
            {
                return this._space;
            }
        }

        public SmartVectorType Type
        {
            get
            {
                return this._type;
            }
        }

        public SmartVector Clone
        {
            get
            {
                return new SmartVector(this._pos, this.Type, this.Space);
            }
        }

        public SmartVector Reflect(SmartVector Normal)
        {
            return new SmartVector(_local_v(Vector3.Reflect(this.World._pos, Normal.World._pos), this.Space, this.Type), this.Type, this.Space);
        }

        public float Angle(SmartVector sv2)
        {
            SmartVector loc = sv2.World.Localize(this);
            return Vector3.Angle(this._pos, sv2._pos);
        }

        public void Rotate(SmartVector Axis, float Angle)
        {
            Vector3 result = Quaternion.AngleAxis(Angle, Axis.World._pos) * this.World._pos;
            this._pos = _local_v(result, this.Space, this.Type);
        }

        public SmartVector RotateClone(SmartVector Axis, float Angle)
        {
            SmartVector ret = this.Clone;
            ret.Rotate(Axis, Angle);
            return ret;
        }

        public float Distance(SmartVector sv2)
        {
            return Vector3.Distance(this.World._pos, sv2.World._pos);
        }

        public SmartVector Localize(SmartVector sv)
        {
            return this.Localize(sv.Space, sv.Type);
        }

        public SmartVector Localize(Transform Space, SmartVectorType Type)
        {
            if (Space == null)
            {
                return this.World;
            }
            Vector3 ret;
            switch (Type)
            {
                case SmartVectorType.Point:
                    ret = Space.InverseTransformPoint(this.World._pos);
                    break;
                case SmartVectorType.Direction:
                    ret = Space.InverseTransformDirection(this.World._pos);
                    break;
                case SmartVectorType.Vector:
                    ret = Space.InverseTransformVector(this.World._pos);
                    break;
                default:
                    ret = Vector3.zero;
                    break;
            }
            return new SmartVector(ret, Type, Space);
        }

        public SmartVector LocalizePoint(Transform Space)
        {
            return this.Localize(Space, SmartVectorType.Point);
        }

        public SmartVector LocalizeDirection(Transform Space)
        {
            return this.Localize(Space, SmartVectorType.Direction);
        }

        public SmartVector LocalizeVector(Transform Space)
        {
            return this.Localize(Space, SmartVectorType.Vector);
        }

        public override string ToString()
        {
            if (this.IsWorld)
            {
                return "World " + this._pos;
            }
            return "Local " + this._pos + "  " + this.World;
        }

        private static Vector3 _world_v(Vector3 Local, Transform Space, SmartVectorType Type)
        {
            if (Space == null)
            {
                return Local;
            }
            switch (Type)
            {
                case SmartVectorType.Point:
                    return Space.TransformPoint(Local);
                case SmartVectorType.Direction:
                    return Space.TransformDirection(Local);
                case SmartVectorType.Vector:
                    return Space.TransformVector(Local);
                default:
                    return Vector3.zero;
            }
        }

        private static Vector3 _local_v(Vector3 World, Transform Space, SmartVectorType Type)
        {
            if (Space == null)
            {
                return World;
            }
            switch (Type)
            {
                case SmartVectorType.Point:
                    return Space.InverseTransformPoint(World);
                case SmartVectorType.Direction:
                    return Space.InverseTransformDirection(World);
                case SmartVectorType.Vector:
                    return Space.InverseTransformVector(World);
                default:
                    return Vector3.zero;
            }
        }

        private Vector3 _pos;
        private Transform _space;
        private SmartVectorType _type;

    }
}