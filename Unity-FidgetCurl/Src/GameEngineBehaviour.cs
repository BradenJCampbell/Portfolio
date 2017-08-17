using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameEngineBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    [System.Serializable]
    public class GameEngineWorld
    {
        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        public bool Changed
        {
            get
            {
                return (!this._assesed || Vector3.Distance(this.up, this._up) != 0 || Vector3.Distance(this.forward, this._forward) != 0 || Vector3.Distance(this.right, this._right) != 0);
            }
        }

        public Plane Plane
        {
            get
            {
                this._assess();
                return this._plane;
            }
        }

        public float PlanarAngle(Vector3 v)
        {
            return MathHelper.PlanarAngle(this._up, this._right, v, false);
        }

        private void _assess()
        {
            if (this.Changed)
            {
                this._up = this.up;
                this._forward = this.forward;
                this._right = this.right;
                this._plane = new Plane(this._forward, Vector3.zero);
                this._assesed = true;
            }
        }
        private Vector3 _up;
        private Vector3 _forward;
        private Vector3 _right;
        private bool _assesed;
        private Plane _plane;
    }

    public GameEngineWorld World;
    public Camera GameCamera;
    public CurlSpinnerBehaviour CurlSpinner;

    // Use this for initialization
    void Start()
    {
        this._dragging = false;
        this._last_drag = Vector2.zero;
        float world_dist;
        Ray pos_ray = new Ray(this.GameCamera.transform.position, this.World.Plane.normal);
        if (this.World.Plane.Raycast(pos_ray, out world_dist))
        {
            this._world_center = pos_ray.GetPoint(world_dist);
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (this.IsDragging)
        {
            this.CurlSpinner.Spin(CurlSpinDirection.Clockwise);
        }
        else
        {
            this.CurlSpinner.Spin(CurlSpinDirection.Decelerate);
        }
    }

    public bool WorldPosition(Vector2 ScreenPosition, out Vector3 output)
    {
        float world_dist;
        Ray pos_ray = this.GameCamera.ScreenPointToRay(ScreenPosition);
        if (this.World.Plane.Raycast(pos_ray, out world_dist))
        {
            output = pos_ray.GetPoint(world_dist);
            return true;
        }
        output = Vector3.zero;
        return false;
    }

    public bool IsDragging
    {
        protected set
        {
            this._dragging = value;
        }
        get
        {
            return this._dragging;
        }
    }

    public Vector3 Drag
    {
        get
        {
            if (this.IsDragging)
            {
                return this._drag_dir;
            }
            return Vector3.zero;
        }
    }

    public float DragAngle
    {
        get
        {
            if (this.IsDragging)
            {
                return this._drag_angle;
            }
            return 0;
        }
    }

    public void OnDrag(PointerEventData pev)
    {
        //  the position in world space of the touch
        Vector3 worldTouch;
        if (this.WorldPosition(pev.position, out worldTouch))
        {
            this._drag_dir = worldTouch - this._last_drag;
            this._drag_angle = this.World.PlanarAngle(worldTouch) - this.World.PlanarAngle(this._last_drag);
            this._last_drag = worldTouch;
        }
        this.IsDragging = true;
    }

    public void OnPointerDown(PointerEventData pev)
    {
        this.OnDrag(pev);
    }

    public void OnPointerUp(PointerEventData pev)
    {
        this.OnDrag(pev);
        this.IsDragging = false;
    }

    private bool _dragging;
    private Vector3 _last_drag;
    private Vector3 _world_center;
    private Vector3 _drag_dir;
    private float _drag_angle;
}
