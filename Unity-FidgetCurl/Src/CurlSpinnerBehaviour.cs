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

	// Use this for initialization
	void Start () {
        this._last_rotation = 0;
        this._spin_speed = 0;
        this._spin_accel = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (this._spin_dir == CurlSpinDirection.Decelerate)
        {
            this._spin_speed = Mathf.Sign(this._spin_speed) * Mathf.Clamp(Mathf.Abs(this._spin_speed) - Time.deltaTime * this._spin_accel, 0, 900);
        }
        else
        {
            this._spin_speed = Mathf.Clamp(this._spin_speed + (float)this._spin_dir * Time.deltaTime * this._spin_accel, -900, 900);
        }

        this.transform.Rotate(-Vector3.forward, Time.deltaTime * this._spin_speed);
    }

    public bool ClosestPoint(Vector3 worldPoint, out Vector3 result)
    {
        Vector3 curr = worldPoint;
        RaycastHit hit;
        result = Vector3.zero;
        do
        {
            if (!Physics.Raycast(curr, this.transform.position - worldPoint, out hit))
            {
                return false;
            }
            foreach (Collider c in this.transform.GetComponentsInChildren<Collider>())
            {
                if (hit.collider == c)
                {
                    result = hit.point;
                    return true;
                }
            }
        } while (true);
    }

    public bool ClosestPointOnBounds(Vector3 worldPoint, out Vector3 result)
    {
        Vector3 curr;
        bool found = false;
        result = Vector3.zero;
        try
        {
            foreach (Collider c in this.transform.GetComponentsInChildren<Collider>())
            {
                curr = c.ClosestPointOnBounds(worldPoint);
                if (!found || Vector3.Distance(worldPoint, curr) < Vector3.Distance(worldPoint, result))
                {
                    result = curr;
                    found = true;
                }
            }
        }
        catch (Exception ex)
        {
        }
        return found;
    }

    public bool ApplyRotationalForce(Vector3 forceStart, Vector3 forceEnd, float magnitude = 0)
    {
        Vector3 force = forceEnd - forceStart;
        if (magnitude > 0)
        {
            force = force.normalized * magnitude;
        }
        Vector3 near_point;
        Vector3 far_point;
        if (this.ClosestPoint(forceStart, out near_point) && this.ClosestPoint(this.transform.position - forceStart, out far_point))
        //if (this.ClosestPointOnBounds(forceStart, out near_point) && this.ClosestPointOnBounds(this.transform.position - forceStart, out far_point))
        {
            try
            {
                this.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(force / 2, near_point);
                this.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(-force / 2, far_point);
                //StaticHelper.DebugDrawStar(near_point, 5, Color.yellow);
                //Debug.Log("+force " + (force / 2) + " at " + near_point + "\n-force " + (-force / 2) + " at " + far_point);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
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

    private float _last_rotation;
    private float _spin_speed;
    private float _spin_accel;
    private CurlSpinDirection _spin_dir;
}
