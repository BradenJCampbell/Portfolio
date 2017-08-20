using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameEngineBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    public int FrameFlex = 0;
    public GameEngineWorld World;
    public Camera GameCamera;
    public CurlSpinnerBehaviour CurlSpinner;

    // Use this for initialization
    void Start()
    {
        this._dragging = false;
        this._path = new PathTracker();
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (this.IsDragging)
        {
            this._path.Capture(this._last_drag);
        }
        if (this._path.LastMovement != null && this._path.LastMovement.Magnitude > 0)
        {
            float rotationAngle = Vector3.Angle(this._path.LastMovement.Start.Position - this.transform.position, this._path.LastMovement.End.Position - this.transform.position);
            CurlSpinDirection rotationSign = (CurlSpinDirection)Mathf.Sign(rotationAngle);
            this.CurlSpinner.Spin(rotationSign);
            //if (!this.CurlSpinner.ApplyRotationalForce(this._path.LastMovement.Start.Position, this._path.LastMovement.End.Position, this._path.LastMovement.Magnitude * 150000))
            {
                //Debug.Log("not spinning - rigidbody problems?");
            }
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

    public CurlSpinDirection RotationDirection(Vector3 pivot, Vector3 start, Vector3 end)
    {
        float planar_start = MathHelper.PlanarAngle(this.World.Up, this.World.Right, start - pivot);
        float planar_end = MathHelper.PlanarAngle(this.World.Up, this.World.Right, end - pivot);
        return (CurlSpinDirection)Mathf.Sign(planar_end - planar_start);
    }

    public bool IsDragFrame
    {
        get
        {
            return this.IsDragging && Time.frameCount - this._path.LastMovementFrame <= this.FrameFlex;
        }
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

    public void OnDrag(PointerEventData pev)
    {
        //  the position in world space of the touch
        Vector3 worldTouch;
        if (this.WorldPosition(pev.position, out worldTouch))
        {
            this._last_drag = worldTouch;
            this.IsDragging = true;
        }
    }

    public void OnPointerDown(PointerEventData pev)
    {
        this.OnDrag(pev);
    }

    public void OnPointerUp(PointerEventData pev)
    {
        //this.OnDrag(pev);
        if (this._path.LastMovement != null)
        {
            this.CurlSpinner.ApplyDirectionalForce(this._path.LastMovement.Direction, this._path.LastMovement.Magnitude * 500);
        }
        this.IsDragging = false;
    }

    private bool _dragging;
    private Vector3 _last_drag;
    private PathTracker _path;
}
