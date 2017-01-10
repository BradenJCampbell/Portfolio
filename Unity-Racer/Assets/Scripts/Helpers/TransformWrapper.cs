using UnityEngine;

public class TransformWrapper
{
    public TransformWrapper(Transform target)
    {
        this._world = new TransformWorldWrapper(target);
    }
    public Transform Target
    {
        set { this._world.Target = value; }
        get { return this._world.Target; }
    }
    public TransformWorldWrapper World
    {
        get { return this._world; }
    }

    private TransformWorldWrapper _world;

    public class TransformWorldWrapper
    {
        public TransformWorldWrapper(Transform Target)
        {
            this.Target = Target;
        }

        public Transform Target
        {
            set { this._target = value; }
            get { return this._target;  }
        }

        public Vector3 Position
        {
            get { return this.t_point(this.Target.localPosition);  }
        }

        public Vector3 Up
        {
            get { return this.t_dir(this.Target.up);  }
        }

        public Vector3 Right
        {
            get { return this.t_dir(this.Target.right); }
        }

        private Vector3 t_dir(Vector3 dir)
        {
            if (this.Target.parent == null)
            {
                return dir;
            }
            return this.Target.parent.TransformDirection(dir);
        }
        private Vector3 t_point(Vector3 pos)
        {
            if (this.Target.parent == null)
            {
                return pos;
            }
            return this.Target.parent.TransformPoint(pos);
        }
        private Transform _target;
    }
}
