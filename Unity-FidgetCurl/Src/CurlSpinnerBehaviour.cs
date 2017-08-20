using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurlSpinDirection
{
    Clockwise = 1,
    CounterClockwise = -1,
    Decelerate = 0
}

public class CurlSpinnerBehaviour : MonoBehaviour {

    public float MaximumSpinSpeed = 900;

	// Use this for initialization
	void Start () {
        this._last_rotation = 0;
        this._spin_speed = 0;
        this._spin_accel = 0;
        this._body = this.transform.GetComponentInChildren<Rigidbody>();
        this._spin_body = this.transform.FindChild("Sphere").transform;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (this._spin_dir == CurlSpinDirection.Decelerate)
        {
            this._spin_speed = Mathf.Sign(this._spin_speed) * Mathf.Clamp(Mathf.Abs(this._spin_speed) - Time.deltaTime * this._spin_accel, 0, this.MaximumSpinSpeed);
        }
        else
        {
            this._spin_speed = Mathf.Clamp(this._spin_speed + (float)this._spin_dir * Time.deltaTime * this._spin_accel, -this.MaximumSpinSpeed, this.MaximumSpinSpeed);
        }

        this._spin_body.Rotate(-Vector3.forward, Time.deltaTime * this._spin_speed);
    }

    public bool ClosestPoint(Vector3 worldPoint, out Vector3 result)
    {
        Vector3 curr = worldPoint;
        RaycastHit hit;
        result = Vector3.zero;
        //  check for ray hit on ray from given point to center of spinner
        while(Physics.Raycast(curr, this.transform.position - curr, out hit))
        {
            foreach (Collider c in this.transform.GetComponentsInChildren<Collider>())
            {
                if (hit.collider == c)
                {
                    result = hit.point;
                    return true;
                }
            }
            curr = hit.point;
        }
        curr = worldPoint;
        //  check for ray hit from center of spinner to given point
        //  (useful if given point is within the spinner)
        while (Physics.Raycast(this.transform.position, curr - this.transform.position, out hit))
        {
            foreach (Collider c in this.transform.GetComponentsInChildren<Collider>())
            {
                if (hit.collider == c)
                {
                    result = hit.point;
                    return true;
                }
            }
            curr = hit.point;
        }
        return false;
    }

    public bool ApplyDirectionalForce(Vector3 direction, float magnitude = 0)
    {
        if (this._body == null)
        {
            return false;
        }
        if (magnitude > 0)
        {
            direction = direction.normalized * magnitude;
        }
        this._body.AddForce(direction, ForceMode.Impulse);
        return true;
    }

    public bool ApplyRotationalForce(Vector3 forceStart, Vector3 forceEnd, float magnitude = 0)
    {
        if (this._body == null)
        {
            return false;
        }
        Vector3 force = forceEnd - forceStart;
        if (magnitude > 0)
        {
            force = force.normalized * magnitude;
        }
        Vector3 near_point;
        Vector3 far_point;
        if (this.ClosestPoint(forceStart, out near_point) && this.ClosestPoint(this.transform.position - forceStart, out far_point))
        {
            this._body.AddForceAtPosition(force / 2, near_point);
            this._body.AddForceAtPosition(-force / 2, far_point);
            return true;
        }
        return false;
    }

    public void Spin(float angular_acceleration)
    {
        this._spin_dir = (CurlSpinDirection)Mathf.Sign(angular_acceleration);
        this._spin_accel = Mathf.Abs(angular_acceleration);
    }

    public void Spin(CurlSpinDirection direction, float angular_acceleration = 360)
    {
        this._spin_dir = direction;
        this._spin_accel = Mathf.Abs(angular_acceleration);
    }

    private Transform _spin_body;
    private float _last_rotation;
    private float _spin_speed;
    private float _spin_accel;
    private CurlSpinDirection _spin_dir;
    private Rigidbody _body;
}
