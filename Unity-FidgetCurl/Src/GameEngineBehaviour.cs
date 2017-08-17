﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameEngineBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {


    public GameEngineWorld World;
    public Camera GameCamera;
    public CurlSpinnerBehaviour CurlSpinner;

    // Use this for initialization
    void Start()
    {
        this._dragging = false;
        this._last_drag = Vector2.zero;
        this._last_drag_frame = Time.frameCount;
        float world_dist;
        Ray pos_ray = new Ray(this.GameCamera.transform.position, this.World.Plane.normal);
        if (this.World.Plane.Raycast(pos_ray, out world_dist))
        {
            this._world_center = pos_ray.GetPoint(world_dist);
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        /*
        if (this.IsDragging)
        {
            this.CurlSpinner.Spin(CurlSpinDirection.Clockwise);
        }
        else
        {
            this.CurlSpinner.Spin(CurlSpinDirection.Decelerate);
        }
        */
        /*
        Debug.Log(this.World.PlanarAngle(this._last_drag));
        if (this.IsDragFrame)
        { 
            this.CurlSpinner.Spin(this.DragAngle * 50);
        }
        else
        {
            this.CurlSpinner.Spin(CurlSpinDirection.Decelerate);
        }
        */
        if (this.DragRay.direction.magnitude > 0 && !this.CurlSpinner.ApplyRotationalForce(this.DragRay.origin, this.DragRay.origin + this.DragRay.direction, this.DragRay.direction.magnitude * 1000))
        {
            Debug.Log("not spinning - rigidbody problems?");
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

    public bool IsDragFrame
    {
        get
        {
            return this.IsDragging && Mathf.Abs(Time.frameCount - this._last_drag_frame) < 2;
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

    public Vector3 Drag
    {
        get
        {
            if (this.IsDragging)
            {
                return this._drag_dir.direction;
            }
            return Vector3.zero;
        }
    }

    public Ray DragRay
    {
        get
        {
            if (this.IsDragging)
            {
                return this._drag_dir;
            }
            return new Ray(Vector3.zero, Vector3.zero);
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
            this._drag_dir = new Ray(this._last_drag, worldTouch - this._last_drag);
            this._drag_angle = this.World.PlanarAngle(worldTouch) - this.World.PlanarAngle(this._last_drag);
            this._last_drag = worldTouch;
            this._last_drag_frame = Time.frameCount;
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
    private Ray _drag_dir;
    private float _drag_angle;
    private float _last_drag_frame;
}
