using BCUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CurlSpinnerBehaviour : MonoBehaviour {

    [System.Serializable]
    public struct CurlSpinnerDebugOptions
    {
        public bool ClosestPoint;
        public bool Reflect;
        public bool Forces;
    }

    public CurlSpinnerDebugOptions DebugOptions;
    public bool FarMirrorsNear = false;
    public float SpinSpeedMultiplier = (float)1.0;
    public float ThrowSpeedMultiplier = (float)1.0;

    public Transform Body
    {
        get
        {
            if (this._spin_body == null)
            {
                this._spin_body = this.transform.FindChild("Sphere").transform;
            }
            return this._spin_body;
        }
    }

    public Rigidbody PhysicsBody
    {
        get
        {
            if (this._physics_body == null)
            {
                this._physics_body = this.transform.GetComponentInChildren<Rigidbody>();
            }
            return this._physics_body;
        }
    }

    public float Mass
    {
        get
        {
            return this.PhysicsBody.mass;
        }
    }

    public SmartVector WorldPosition
    {
        get
        {
            return SmartVector.CreateWorldPoint(
                this.PhysicsBody.transform.position.x, 
                this.PhysicsBody.transform.position.y, 
                this.PhysicsBody.transform.position.z
            );
        }
    }

    public SmartVector Velocity
    {
        get
        {
            return SmartVector.CreateLocalVector(
                this.PhysicsBody.velocity.x,
                this.PhysicsBody.velocity.y,
                this.PhysicsBody.velocity.z,
                this.PhysicsBody.transform
            );
        }
    }

    public float SpinSpeed
    {
        get
        {
            return Mathf.Abs(this.PhysicsBody.angularVelocity.z);
        }
    }

    public PieceSet Pieces
    {
        get
        {
            return this._pieces;
        }
    }

    protected DebugMessagesContainer error_writer
    {
        get
        {
            if (this._errors == null)
            {
                this._errors = new DebugMessagesContainer();
            }
            return this._errors;
        }
    }

    public DebugMessages Errors
    {
        get
        {
            return this.error_writer.Reader;
        }
    }

    public float Score
    {
        get
        {
            return this._score;
        }
    }

    // Use this for initialization
    void Start () {
        GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.Start");
        this._last_rand = Time.fixedTime;
        this._pieces = new CurlSpinnerBehaviour.PieceSet(this, this.PhysicsBody);
        if (this.PhysicsBody == null)
        {
            this.error_writer.Add("no rigidbody");
        }
        Transform capsule = this.transform.FindChild("Capsule");
        capsule.gameObject.SetActive(false);
        this.Pieces.Add(20, capsule, SmartVector.CreateWorldPoint(0, 8, 0), 2);
        this.Pieces.Add(20, capsule, SmartVector.CreateWorldPoint(0, -8, 0), 2);
        this.Pieces.Add(20, capsule, SmartVector.CreateWorldPoint(8, 0, 0), 2);
        this.Pieces.Add(20, capsule, SmartVector.CreateWorldPoint(-8, 0, 0), 2);
    }

    // Update is called once per frame
    void FixedUpdate () {
        GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.FixedUpdate");
        this._last_velocity = SmartVector.CreateWorldDirection(
            this.PhysicsBody.velocity.x, 
            this.PhysicsBody.velocity.y, 
            this.PhysicsBody.velocity.z
        );
        //this._random_pieces();
    }

    private void _random_pieces()
    {
        GameEngine.Debug.TraceLog.Update("CurlSpinner.random_pieces");
        if (this.PhysicsBody.velocity.magnitude == 0 && Time.fixedTime >= this._last_rand + 2)
        {
            Vector3 rand = UnityEngine.Random.insideUnitCircle.normalized * 10;
            this.Pieces.Remove(0);
            SmartVector local = this.LocalPoint(rand.x, rand.y, rand.z);
            this.Pieces.Add(20, this.transform.FindChild("Capsule"), local, 1);
            this._last_rand = Time.fixedTime;
        }
    }

    public void Reset()
    {
        this.transform.position = Vector3.zero;
        this.PhysicsBody.velocity = Vector3.zero;
        this.PhysicsBody.angularVelocity = Vector3.zero;
        this.Body.rotation = Quaternion.identity;
    }

    public void OnCollisionEnter(Collision collision)
    {
        GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.OnCollisionEnter");
        SmartVector collision_normal = SmartVector.CreateWorldDirection(
                collision.contacts[0].normal.x,
                collision.contacts[0].normal.y,
                collision.contacts[0].normal.z
        );
        Bumper bump = GameEngine.Bumpers.Lookup(collision.transform);
        if (bump != null)
        {
            this._score += this.SpinSpeed * bump.ScoreValue;
        }
        SmartVector reflect = GameEngine.GameWorld.Reflect(collision_normal, this._last_velocity).Normalized * this._last_velocity.Magnitude;
        if (this.DebugOptions.Reflect)
        {
            GameEngine.Debug.Line(SmartVector.Zero, collision_normal * 15, Color.yellow, 2);
            GameEngine.Debug.Log("SpinnerReflect.Normal " + collision_normal);
            GameEngine.Debug.Line(SmartVector.Zero, this._last_velocity * 15, Color.red, 2);
            GameEngine.Debug.Log("SpinnerReflect.Velocity " + this._last_velocity);
            GameEngine.Debug.Line(SmartVector.Zero, reflect * 15, Color.green, 2);
            GameEngine.Debug.Log("SpinnerReflect.Reflect " + reflect);
        }
        this.PhysicsBody.velocity = (Vector3)reflect;
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
        GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.recursive_line_cast");
        SmartVector curr = Start;
        SmartVector micro_shift = (End - Start).Normalized * (float)0.0001;
        for (int loops = 1; this.line_cast(curr, End, out hit); loops++)
        {
            //GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.recursive_line_cast[" + loops + ", " + hit.transform.gameObject.name + "(" + hit.transform.gameObject.GetInstanceID() + ")]");
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
        GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.ClosestPoint");
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
                GameEngine.Debug.Point(result, Color.green);
                GameEngine.Debug.Point(worldPoint, Color.red);
            }
        }
        return found;
    }

    public bool ApplyDirectionalForce(SmartVector direction, float magnitude = 0)
    {
        GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.ApplyDirectionalForce");
        if (this.PhysicsBody == null)
        {
            return false;
        }
        if (magnitude > 0)
        {
            direction = direction.Normalized * magnitude;
        }
        direction *= this.ThrowSpeedMultiplier;
        this.PhysicsBody.AddForce((Vector3)direction.World, ForceMode.Impulse);
        if (this.DebugOptions.Forces)
        {
            GameEngine.Debug.Line(this.WorldPosition, this.WorldPosition + direction, Color.green, 2);
        }
        return true;
    }

    public bool ApplyRotationalForce(SmartVector forceStart, SmartVector forceEnd, float magnitude = 0)
    {
        GameEngine.Debug.TraceLog.Update("CurlSpinBehaviour.ApplyRotationalForce");
        SmartVector force = forceEnd - forceStart;
        if (magnitude > 0)
        {
            force = force.Normalized * magnitude;
        }
        SmartVector near_point;
        SmartVector far_point;
        SmartVector force_offset = forceStart - this.WorldPosition;
        bool near_exists = this.ClosestPoint(this.WorldPosition + force_offset, out near_point);
        bool far_exists;
        if (!near_exists)
        {
            this.error_writer.Add("ApplyRotationalForce: no near point for " + forceStart + " spinner world position at " + this.WorldPosition);
        }
        if (this.FarMirrorsNear)
        {
            SmartVector near_offset = near_point - this.WorldPosition;
            far_point = this.WorldPosition - near_offset;
            far_exists = true;
        }
        else
        {
            far_exists = this.ClosestPoint(this.WorldPosition - force_offset, out far_point);
        }
        if (!far_exists)
        {
            this.error_writer.Add("ApplyRotationalForce: no far point for " + forceStart + " spinner world position at " + this.WorldPosition);
        }
        if (this.PhysicsBody == null)
        {
            return false;
        }
        if (near_exists && far_exists)
        {
            SmartVector near_force = this.SpinSpeedMultiplier * force / 2;
            SmartVector far_force = this.SpinSpeedMultiplier * -force / 2;
            if (this.DebugOptions.Forces)
            {
                GameEngine.Debug.Line(near_point, near_point + (20 * near_force.Normalized), Color.cyan, 2);
                GameEngine.Debug.Line(far_point, far_point + (20 * far_force.Normalized), Color.cyan, 2);
            }
            this.PhysicsBody.AddForceAtPosition((Vector3)near_force, (Vector3)near_point);
            this.PhysicsBody.AddForceAtPosition((Vector3)far_force, (Vector3)far_point);
            return true;
        }
        return false;
    }

    private float _score;
    private Rigidbody _physics_body;
    private Transform _spin_body;
    private DebugMessagesContainer _errors;
    private SmartVector _last_velocity;
    private PieceSet _pieces;
    private float _last_rand;
}
