using UnityEngine;
using System.Collections.Generic;

public partial class CameraBehaviour : MonoBehaviour {
    public CameraBehaviourBounds DistanceBounds;
    public float MinimumY;
    public bool invertScroll;
    private Vector3 _start_pos;
    private Vector3 _pos;
    private float _downtime;

	// Use this for initialization
	void Start () {
        this._start_pos = this.transform.localPosition;
        this._pos = this.transform.localPosition;
	}
	
    private float distanceDelta
    {
        get {
            float delta = this.transform.parent.localScale.magnitude * (Input.GetAxis("Mouse ScrollWheel") - 0f);
            if (delta != 0)  // a delta of 0 results in a divide-by-zero
            {
                delta = delta / Mathf.Abs(delta);
            }
            if (this.invertScroll)
            {
                return delta * -1;
            }
            return delta;
        }
    }

    public Transform Target
    {
        get { return this.transform.parent; }
    }

    public Vector3 TargetPosition
    {
        get { return this.Target.TransformPoint(Vector3.zero); }
    }

    public Camera BoundCamera
    {
        get { return this.transform.GetComponent<Camera>(); }
    }

    public void ResetPos()
    {
        this._pos = this._start_pos;
    }

    public float Distance
    {
        get { return this._pos.magnitude;  }
        set { this._pos = new Vector3(this._pos.normalized.x * value, this._pos.normalized.y * value, this._pos.normalized.z * value);  }
    }

    public Ray RayFromTargetToCamera
    {
        get { return new Ray(this.TargetPosition, this.Target.TransformPoint(this._pos) - this.TargetPosition);  }
    }

    public void RotatePos(float AboutX, float AboutY)
    {
        this._pos = Quaternion.AngleAxis(AboutX, Vector3.right) * Quaternion.AngleAxis(AboutY, Vector3.up) * this._pos;
    }

    public void DebugRender()
    {
        Color col = Color.magenta;
        float rad = (float)0.5;
        Vector3 WorldPos = this.transform.parent.TransformPoint(this._pos);
        StaticHelper.DebugDrawStar(WorldPos, rad, col);
        Debug.DrawRay(this.RayFromTargetToCamera.origin, this.RayFromTargetToCamera.direction, Color.blue);
    }

    public Vector3 TransformPoint(Vector3 LocalPoint)
    {
        return this.transform.parent.TransformPoint(LocalPoint); 
    }

    public Vector3 InverseTransformPoint(Vector3 WorldPoint)
    {
        return this.transform.parent.InverseTransformPoint(WorldPoint);
    }

    void LateUpdate()
    {
        this.Distance = this.DistanceBounds.Restrict(this.Distance + this.distanceDelta);
        if (Input.GetButton("Fire3"))
        {
            this.RotatePos(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        }
        if (Input.GetButton("Fire1"))
        {
            this.ResetPos();
        }
        Vector3 newPos = this._pos;
        // clamp magnitude
        newPos = newPos.normalized * this.DistanceBounds.Restrict(newPos.magnitude);
        // account for objects in camera line of sight
        /*RaycastHit hit;
        if (Physics.Raycast(this.RayFromTargetToCamera, out hit, this._pos.magnitude, LayerMask.GetMask("Track", "Ground")))
        {
            Debug.Log(hit.collider.gameObject.name + " hit (" + hit.distance + ")");
            newPos = newPos.normalized * (hit.distance - 1);
        }*/
        //  account for near plane collisions
        CameraCollisionHandler collision = new CameraCollisionHandler(this.BoundCamera, this.TransformPoint(this._pos), this.transform.parent, this.Target);
        collision.DebugRender();
        newPos = this.InverseTransformPoint(collision.World.ClosestPoint);
        this.transform.localPosition = newPos;
        this.transform.LookAt(this.transform.parent);
        this.DebugRender();
    }
}

[System.Serializable]
public class CameraBehaviourBounds
{
    public float MinimumDistance;
    public float MaximumDistance;

    public float Restrict (float Value)
    {
        return Mathf.Max(this.MinimumDistance, Mathf.Min(this.MaximumDistance, Value));
    }
}
