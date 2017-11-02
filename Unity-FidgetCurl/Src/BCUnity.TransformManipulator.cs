using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BCUnity
{
    public struct TransformManipulator
    {
        public TransformManipulator(Transform target)
        {
            this._transform = target;
        }

        public void Remove()
        {
            BCUnity.TransformPool.Destroy(this.Transform);
        }

        public void Place(SmartVector Start, SmartVector End, float width = 1, float height = 1)
        {
            Vector3 dir = (Vector3)(End.World - Start.World);
            Vector3 dir_euler = BCUnity.Helper.Math.EulerAngles(dir);
            this.Transform.eulerAngles = dir_euler;
            this.Transform.position = (Vector3)BCUnity.Helper.Math.Scale(Start, End, (float)0.5);
            this.Transform.localScale = new Vector3(width, dir.magnitude, height);
        }

        public SmartVector Position
        {
            get
            {
                if (this.Parent == null)
                {
                    return SmartVector.CreateWorldPoint(
                        this.Transform.position.x, 
                        this.Transform.position.y, 
                        this.Transform.position.z
                    );
                }
                return SmartVector.CreateLocalPoint(
                    this.Transform.localPosition.x, 
                    this.Transform.localPosition.y, 
                    this.Transform.localPosition.z, 
                    this.Parent
                );
            }
            set
            {
                this.Transform.position = (Vector3)value.World;
            }
        }

        public Transform Parent
        {
            get
            {
                return this.Transform.parent;
            }
            set
            {
                this.Transform.SetParent(value);
            }
        }

        public Renderer Renderer
        {
            get
            {
                try
                {
                    return this._transform.GetComponent<Renderer>();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public Color Color
        {
            set
            {
                this.Renderer.material.color = value;
            }
            get
            {
                return this.Renderer.material.color;
            }
        }

        public bool Active
        {
            set
            {
                this.Transform.gameObject.SetActive(value);
            }
            get
            {
                return this.Transform.gameObject.activeInHierarchy;
            }
        }

        public bool Visible
        {
            get
            {
                try
                {
                    return this.Renderer.enabled;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            set
            {
                try
                {
                    this.Renderer.enabled = value;
                }
                catch (Exception ex)
                {

                }
            }
        }

        public int ID
        {
            get
            {
                return this._transform.GetInstanceID();
            }
        }

        public Transform Transform
        {
            get
            {
                return this._transform;
            }
        }

        private Transform _transform;

    }
}
