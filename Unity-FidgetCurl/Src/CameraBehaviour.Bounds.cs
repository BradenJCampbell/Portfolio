using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class CameraBehaviour : MonoBehaviour
{
    public CameraBounds Bounds
    {
        get
        {
            return new CameraBounds(this, this._camera);
        }
    }

    public partial struct CameraBounds
    {

        public CameraBounds(CameraBehaviour parent, Camera camera)
        {
            this._parent = parent;
            this._camera = camera;
        }

        public CameraBehaviour Parent
        {
            get
            {
                return this._parent;
            }
        }

        public SmartVector TopLeft
        {
            get
            {
                return this.Parent.ScreenRatioToWorldPosition(-1, 1);
            }
        }

        public SmartVector TopRight
        {
            get
            {
                return this.Parent.ScreenRatioToWorldPosition(1, 1);
            }
        }

        public SmartVector BottomLeft
        {
            get
            {
                return this.Parent.ScreenRatioToWorldPosition(-1, -1);
            }
        }

        public SmartVector BottomRight
        {
            get
            {
                return this.Parent.ScreenRatioToWorldPosition(1, -1);
            }
        }

        public float Width
        {
            get
            {
                return UnityEngine.Mathf.Abs(this.Right - this.Left);
            }
        }

        public float Height
        {
            get
            {
                return UnityEngine.Mathf.Abs(this.Top - this.Bottom);
            }
        }

        public float Left
        {
            get
            {
                return this.TopLeft.x;
            }
        }

        public float Right
        {
            get
            {
                return this.TopRight.x;
            }
        }

        public float Top
        {
            get
            {
                return this.TopLeft.y;
            }
        }

        public float Bottom
        {
            get
            {
                return this.BottomLeft.y;
            }
        }

        public bool Project(SmartVector Point, out SmartVector point)
        {
            Vector3 center = (Vector3)this.Parent.ScreenRatioToWorldPosition(0, 0).World;
            Vector3 dir = (Vector3)Point.World - center;
            UnityEngine.Bounds b = new UnityEngine.Bounds(center, new Vector3(this.Width, this.Height, 0));
            Ray r = new Ray(center, dir);
            float dist;
            if (b.IntersectRay(r, out dist))
            {
                Vector3 ret = center - (dir.normalized * dist);
                point = SmartVector.CreateWorldPoint(ret.x, ret.y, ret.z);
                return true;
            }
            point = SmartVector.Empty;
            return false;
        }

        public override string ToString()
        {
            return this.Parent.ScreenRatioToWorldPosition(-1, -1) + " => " + this.Parent.ScreenRatioToWorldPosition(1, 1);
        }

        private CameraBehaviour _parent;
        private Camera _camera;
    }
}