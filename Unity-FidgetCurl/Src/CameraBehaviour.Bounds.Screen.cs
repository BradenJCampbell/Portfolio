using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class CameraBehaviour : MonoBehaviour
{
    public partial struct CameraBounds
    {
        public ScreenBounds Screen
        {
            get
            {
                return new ScreenBounds(this._camera);
            }
        }

        public struct ScreenBounds
        {
            public ScreenBounds(Camera camera)
            {
                this._camera = camera;
            }

            public bool Contains(float x, float y)
            {
                return this.minX <= x && x <= this.maxX && this.minY <= y && y <= this.maxY;
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
                    return this._camera.rect.min.x * UnityEngine.Screen.width;
                }
            }

            public float maxX
            {
                get
                {
                    return this._camera.rect.max.x * UnityEngine.Screen.width;
                }
            }

            public float minY
            {
                get
                {
                    return this._camera.rect.min.y * UnityEngine.Screen.height;
                }
            }

            public float maxY
            {
                get
                {
                    return this._camera.rect.max.y * UnityEngine.Screen.height;
                }
            }

            public float Left
            {
                get
                {
                    return this.minX;
                }
            }

            public float Right
            {
                get
                {
                    return this.maxX;
                }
            }

            public float Bottom
            {
                get
                {
                    return this.minY;
                }
            }

            public float Top
            {
                get
                {
                    return this.maxY;
                }
            }

            public override string ToString()
            {
                return "(" + this.minX + ", " + this.minY + ") => (" + this.maxX + ", " + this.maxY + ") = (" + this.Width + ", " + this.Height + ")";
            }

            private Camera _camera;
        }
    }
}