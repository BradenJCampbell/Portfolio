using BCUnity;
using System;
using UnityEngine;


[System.Serializable]
public partial class GameEngineWorld
{
    public SmartVector Up = SmartVector.Up;
    public SmartVector Forward = SmartVector.Forward;
    public SmartVector Right = SmartVector.Right;


    public bool Changed
    {
        get
        {
            return (!this._assesed || this.Up.Distance(this._plane.Up) != 0 || this.Forward.Distance(this._plane.Normal) != 0 || this.Right.Distance(this._plane.Right) != 0);
        }
    }

    public UnityEngine.Plane Plane
    {
        get
        {
            this._assess();
            return this._plane.UnityPlane;
        }
    }


    public SmartVector Rotate(float angle, SmartVector sv)
    {
        this._assess();
        return sv.RotateClone(this.Forward, angle);
    }

    public SmartVector Reflect(SmartVector Normal, SmartVector sv)
    {
        this._assess();
        SmartVector ret = this.ClosestPoint(sv.Reflect(Normal));
        return ret;
    }

    public SmartVector ClosestPoint(SmartVector wv)
    {
        this._assess();
        return this._plane.ClosestPoint(wv);
    }

    public float PlanarAngle(SmartVector sv)
    {
        this._assess();
        float ret = this._plane.PlanarAngle(sv);
        if (ret < 0)
        {
            return 360 + ret;
        }
        return ret;
    }

    public SmartVector RatioPosition(float x, float y)
    {
        return SmartVector.CreateWorldPoint(this.Bounds.Left + x * (this.Bounds.Right - this.Bounds.Left), this.Bounds.Bottom + y * (this.Bounds.Top - this.Bounds.Bottom), 0);
    }

    public bool Place(Transform Target, SmartVector Start, SmartVector End, float width, float height)
    {
        this._assess();
        GameEngine.Debug.TraceLog.Update("GameWorld.Place");
        return this._plane.Place(Target, Start, End, width, height);
    }

    public bool Raycast(Ray r, out float distance)
    {
        return this._plane.UnityPlane.Raycast(r, out distance);
    }

    public bool Raycast(Ray r, out SmartVector point)
    {
        float world_dist;
        if (GameEngine.GameWorld.Plane.Raycast(r, out world_dist))
        {
            Vector3 pos = r.GetPoint(world_dist);
            point = SmartVector.CreateWorldPoint(pos.x, pos.y, pos.z);
            return true;
        }
        point = SmartVector.Empty;
        return false;
    }

    private void _assess()
    {
        if (this.Changed)
        {
            this._plane = new BCUnity.Plane(this.Up, this.Right, -this.Forward);
            this._assesed = true;
        }
    }

    private bool _assesed;
    private BCUnity.Plane _plane;
}
