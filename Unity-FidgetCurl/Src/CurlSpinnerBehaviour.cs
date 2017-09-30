using BCUnity;
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

public partial class CurlSpinnerBehaviour : MonoBehaviour {

    [System.Serializable]
    public struct CurlSpinnerDebugOptions
    {
        public bool ClosestPoint;
        public bool Reflect;
    }

    public CurlSpinnerDebugOptions DebugOptions;
    public float MaximumSpinSpeed = 900;

    public Transform Body
    {
        get
        {
            return this._spin_body;
        }
    }

    public float Mass
    {
        get
        {
            return this._physics_body.mass;
        }
    }

    public SmartVector WorldPosition
    {
        get
        {
            return SmartVector.CreateWorldPoint(
                this._physics_body.transform.position.x, 
                this._physics_body.transform.position.y, 
                this._physics_body.transform.position.z
            );
        }
    }

    public SmartVector Velocity
    {
        get
        {
            return SmartVector.CreateLocalVector(
                this._physics_body.velocity.x,
                this._physics_body.velocity.y,
                this._physics_body.velocity.z,
                this._physics_body.transform
            );
        }
    }

    public PieceSet Pieces
    {
        get
        {
            return this._pieces;
        }
    }

    public DebugMessages Errors
    {
        get
        {
            if (this._errors == null)
            {
                this._errors = new DebugMessagesContainer();
            }
            return this._errors.Reader;
        }
    }

    // Use this for initialization
    void Start () {
        GameEngine.TraceLog.Update("CurlSpinBehaviour.Start");
        this._last_rand = Time.fixedTime;
        this._physics_body = this.transform.GetComponentInChildren<Rigidbody>();
        this._spin_body = this.transform.FindChild("Sphere").transform;
        this._pieces = new CurlSpinnerBehaviour.PieceSet(this, this._physics_body);
        if (this._errors == null)
        {
            this._errors = new DebugMessagesContainer();
        }
        if (this._physics_body == null)
        {
            this._errors.Add("no rigidbody");
        }
        Transform capsule = this.transform.FindChild("Capsule");
        capsule.gameObject.SetActive(false);
        this.Pieces.Add(0, capsule, SmartVector.CreateWorldPoint(0, 8, 0), 2);
        this.Pieces.Add(0, capsule, SmartVector.CreateWorldPoint(0, -8, 0), 2);
        this.Pieces.Add(0, capsule, SmartVector.CreateWorldPoint(8, 0, 0), 2);
        this.Pieces.Add(0, capsule, SmartVector.CreateWorldPoint(-8, 0, 0), 2);
    }

    // Update is called once per frame
    void FixedUpdate () {
        GameEngine.TraceLog.Update("CurlSpinBehaviour.FixedUpdate");
        this._last_velocity = this._physics_body.velocity;
        //this._random_pieces();
    }

    private void _random_pieces()
    {
        GameEngine.TraceLog.Update("CurlSpinner.random_pieces");
        if (this._physics_body.velocity.magnitude == 0 && Time.fixedTime >= this._last_rand + 2)
        {
            Vector3 rand = UnityEngine.Random.insideUnitCircle.normalized * 10;
            this.Pieces.Remove(0);
            SmartVector local = this.LocalPoint(rand.x, rand.y, rand.z);
            this.Pieces.Add(200, this.transform.FindChild("Capsule"), local, 1);
            this._last_rand = Time.fixedTime;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameEngine.TraceLog.Update("CurlSpinBehaviour.OnCollisionEnter");
        SmartVector velocity = SmartVector.CreateWorldDirection(
            this._last_velocity.x, 
            this._last_velocity.y, 
            this._last_velocity.z
        );
        SmartVector collision_normal = SmartVector.CreateWorldDirection(
                collision.contacts[0].normal.x,
                collision.contacts[0].normal.y,
                collision.contacts[0].normal.z
        );
        SmartVector reflect = GameEngine.GameWorld.Reflect(collision_normal, velocity).Normalized * velocity.Magnitude;
        if (this.DebugOptions.Reflect)
        {
            GameEngine.DebugLine(SmartVector.Zero, collision_normal * 15, Color.yellow);
            GameEngine.DebugLog("SpinnerReflect.Normal " + collision_normal);
            GameEngine.DebugLine(SmartVector.Zero, velocity * 15, Color.red);
            GameEngine.DebugLog("SpinnerReflect.Velocity " + velocity);
            GameEngine.DebugLine(SmartVector.Zero, reflect * 15, Color.green);
            GameEngine.DebugLog("SpinnerReflect.Reflect " + reflect);
        }
        this._physics_body.velocity = (Vector3)reflect;
    }

    public SmartVector LocalPoint(float x, float y, float z = 0)
    {
        return SmartVector.CreateLocalPoint(x, y, z, this.Body);
    }

    protected bool line_cast(SmartVector Start, SmartVector End, out RaycastHit hit)
    {
        Vector3 origin = (Vector3)Start.World;
        Vector3 direction = (Vector3)End.World - (Vector3)Start.World;
        return Physics.Raycast(origin, direction.normalized, out hit, direction.magnitude);
    }
    
    protected bool recursive_line_cast(SmartVector Start, SmartVector End, out RaycastHit hit)
    {
        GameEngine.TraceLog.Update("CurlSpinBehaviour.recursive_line_cast");
        SmartVector curr = Start;
        SmartVector micro_shift = (End - Start).Normalized * (float)0.0001;
        for (int loops = 1; this.line_cast(curr, End, out hit); loops++)
        {
            //GameEngine.TraceLog.Update("CurlSpinBehaviour.recursive_line_cast[" + loops + ", " + hit.transform.gameObject.name + "(" + hit.transform.gameObject.GetInstanceID() + ")]");
            foreach (Collider c in this.transform.GetComponentsInChildren<Collider>())
            {
                if (hit.collider == c)
                {
                    return true;
                }
            }
            curr = SmartVector.CreateWorldPoint(hit.point.x, hit.point.y, hit.point.z) + micro_shift;
        }
        return false;
    }

    public bool ClosestPoint(SmartVector worldPoint, out SmartVector result)
    {
        GameEngine.TraceLog.Update("CurlSpinBehaviour.ClosestPoint");
        RaycastHit hit;
        result = SmartVector.Zero;
        bool found = false;
        //  check for ray hit on ray from given point to center of spinner
        found = this.recursive_line_cast(worldPoint, this.WorldPosition, out hit);
        //  check for ray hit from center of spinner to given point
        //  (useful if given point is within the spinner)
        found = found || this.recursive_line_cast(this.WorldPosition, worldPoint, out hit);
        if (found)
        {
            result = SmartVector.CreateWorldPoint(hit.point.x, hit.point.y, hit.point.z);
            if (this.DebugOptions.ClosestPoint)
            {
                GameEngine.DebugPoint(result, Color.green);
                GameEngine.DebugPoint(worldPoint, Color.red);
            }
        }
        return found;
    }

    public bool ApplyDirectionalForce(SmartVector direction, float magnitude = 0)
    {
        GameEngine.TraceLog.Update("CurlSpinBehaviour.ApplyDirectionalForce");
        if (this._physics_body == null)
        {
            return false;
        }
        if (magnitude > 0)
        {
            direction = direction.Normalized * magnitude;
        }
        this._physics_body.AddForce((Vector3)direction.World, ForceMode.Impulse);
        return true;
    }

    public bool ApplyRotationalForce(SmartVector forceStart, SmartVector forceEnd, float magnitude = 0)
    {
        GameEngine.TraceLog.Update("CurlSpinBehaviour.ApplyRotationalForce");
        SmartVector force = forceEnd - forceStart;
        if (magnitude > 0)
        {
            force = force.Normalized * magnitude;
        }
        SmartVector near_point;
        SmartVector far_point;
        SmartVector near_offset = forceStart - this.WorldPosition;
        bool near_exists = this.ClosestPoint(this.WorldPosition + near_offset, out near_point);
        bool far_exists = this.ClosestPoint(this.WorldPosition - near_offset, out far_point);
        if (!near_exists)
        {
            this._errors.Add("ApplyRotationalForce: no near point for " + forceStart + " spinner world position at " + this.WorldPosition);
        }
        if (!far_exists)
        {
            this._errors.Add("ApplyRotationalForce: no far point for " + forceStart + " spinner world position at " + this.WorldPosition);
        }
        if (this._physics_body == null)
        {
            return false;
        }
        if (near_exists && far_exists)
        {
            this._physics_body.AddForceAtPosition((Vector3)force / 2, (Vector3)near_point);
            this._physics_body.AddForceAtPosition(-(Vector3)force / 2, (Vector3)far_point);
            return true;
        }
        return false;
    }

    private Rigidbody _physics_body;
    private Transform _spin_body;
    private DebugMessagesContainer _errors;
    private Vector3 _last_velocity;
    private PieceSet _pieces;
    private float _last_rand;
}
