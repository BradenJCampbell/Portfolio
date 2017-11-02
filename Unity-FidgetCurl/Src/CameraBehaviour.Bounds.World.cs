using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class CameraBehaviour
{
    public partial struct CameraBounds
    {
        public WorldBounds World
        {
            get
            {
                return new WorldBounds(this._parent, this._camera);
            }
        }

        public struct WorldBounds
        {
            public WorldBounds(CameraBehaviour behaviour, Camera camera)
            {
                this._behaviour = behaviour;
                this._camera = camera;
                this._screen = new ScreenBounds(this._camera);
            }

            public float Width
            {
                get
                {
                    return this.maxX - this.minX;
                }
            }

            public float Height
            {
                get
                {
                    return this.maxY - this.minY;
                }
            }

            public float minX
            {
                get
                {
                    return this.BottomLeft.x;
                }
            }

            public float minY
            {
                get
                {
                    return this.BottomLeft.y;
                }
            }

            public float maxX
            {
                get
                {
                    return this.TopRight.x;
                }
            }

            public float maxY
            {
                get
                {
                    return this.TopRight.y;
                }
            }

            public SmartVector TopLeft
            {
                get
                {
                    return this._screen_to_world(this._screen.Left, this._screen.Top);
                }
            }

            public SmartVector TopRight
            {
                get
                {
                    return this._screen_to_world(this._screen.Right, this._screen.Top);
                }
            }

            public SmartVector BottomLeft
            {
                get
                {
                    return this._screen_to_world(this._screen.Left, this._screen.Bottom);
                }
            }

            public SmartVector BottomRight
            {
                get
                {
                    return this._screen_to_world(this._screen.Right, this._screen.Bottom);
                }
            }

            public override string ToString()
            {
                return "(" + this.minX + ", " + this.minY + ") => (" + this.maxX + ", " + this.maxY + ") = (" + this.Width + ", " + this.Height + ")";
            }

            private SmartVector _screen_to_world(float x, float y)
            {
                SmartVector ret;
                if (this._behaviour.ScreenToWorldPosition(x, y, out ret))
                {
                    return ret;
                }
                return SmartVector.Empty;
            }

            private CameraBehaviour _behaviour;
            private Camera _camera;
            private ScreenBounds _screen;
        }
    }
}

