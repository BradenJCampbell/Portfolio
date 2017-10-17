using BCUnity;
using System;
using UnityEngine;

public struct GameEngineWorldBounds
{
    public GameEngineWorldBounds(GameEngineWorld World)
    {
        this._world = World;
    }

    public GameEngineWorld World
    {
        get
        {
            return this._world;
        }
    }

    public SmartVector TopLeft
    {
        get
        {
            return this.World.ScreenRatioToWorldPosition(-1, -1);
        }
    }

    public SmartVector TopRight
    {
        get
        {
            return this.World.ScreenRatioToWorldPosition(1, -1);
        }
    }

    public SmartVector BottomLeft
    {
        get
        {
            return this.World.ScreenRatioToWorldPosition(-1, 1);
        }
    }

    public SmartVector BottomRight
    {
        get
        {
            return this.World.ScreenRatioToWorldPosition(1, 1);
        }
    }

    public float Left
    {
        get
        {
            return this.TopLeft.x;
        }
    }

    public float Right
    {
        get
        {
            return this.TopRight.x;
        }
    }

    public float Top
    {
        get
        {
            return this.TopLeft.y;
        }
    }

    public float Bottom
    {
        get
        {
            return this.BottomLeft.y;
        }
    }

    public override string ToString()
    {
        return this.World.ScreenRatioToWorldPosition(-1, -1) + " => " + this.World.ScreenRatioToWorldPosition(1, 1);
    }

    private GameEngineWorld _world;
}

[System.Serializable]
public class GameEngineWorld
{
    public Camera GameCamera;
    public SmartVector Up = SmartVector.Up;
    public SmartVector Forward = SmartVector.Forward;
    public SmartVector Right = SmartVector.Right;

    public GameEngineWorldBounds Bounds
    {
        get
        {
            return new GameEngineWorldBounds(this);
        }
    }

    public bool Changed
    {
        get
        {
            return (!this._assesed || this.Up.Distance(this._up) != 0 || this.Forward.Distance(this._forward) != 0 || this.Right.Distance(this._right) != 0);
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

    public bool ValidScreenPoint(float x, float y)
    {
        return x >= 0 && y >= 0 && x <= Screen.width && y <= Screen.height;
    }

    public SmartVector Rotate(float angle, SmartVector sv)
    {
        this._assess();
        return sv.RotateClone(this.Forward, angle);
    }

    public bool ScreenToWorldPosition(float x, float y, out SmartVector output)
    {
        if (GameEngine.GameWorld.ValidScreenPoint(x, y))
        {
            float world_dist;
            Ray pos_ray = this.GameCamera.ScreenPointToRay(new Vector2(x, y));
            if (this.Plane.Raycast(pos_ray, out world_dist))
            {
                Vector3 pos = pos_ray.GetPoint(world_dist);
                output = SmartVector.CreateWorldPoint(pos.x, pos.y, pos.z);
                return true;
            }
        }
        output = SmartVector.Zero;
        return false;
    }

    public SmartVector ScreenRatioToWorldPosition(float x, float y)
    {
       SmartVector ret;
        float screen_x = ((float)0.5 + x / 2) * Screen.width;
        float screen_y = ((float)0.5 + y / 2) * Screen.height;
        this.ScreenToWorldPosition(screen_x, screen_y, out ret);
        return ret;
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
        Vector3 v = (Vector3)wv;
        //  get the distance to the plane
        float distance = this._plane.GetDistanceToPoint(v);
        if (distance == 0)
        {
            return wv;
        }
        Vector3 norm = this._plane.normal.normalized * distance;
        Vector3 a = v + norm;
        Vector3 b = v - norm;
        float a_dist = Mathf.Abs(this._plane.GetDistanceToPoint(a));
        float b_dist = Mathf.Abs(this._plane.GetDistanceToPoint(b));
        if (a_dist < b_dist)
        {
            return SmartVector.CreateWorldPoint(a.x, a.y, a.z);
        }
        return SmartVector.CreateWorldPoint(b.x, b.y, b.z);
    }

    public float PlanarAngle(SmartVector sv)
    {
        this._assess();
        float ret = MathHelper.PlanarAngle((Vector3)this._up, (Vector3)this._right, (Vector3)sv.World, false);
        if (ret < 0)
        {
            return 360 + ret;
        }
        return ret;
    }

    public bool Place(Transform Target, SmartVector Start, SmartVector End, float width, float height)
    {
        this._assess();
        GameEngine.Debug.TraceLog.Update("GameWorld.Place");
        try
        {
            SmartVector absol = End - Start;
            float angle = this.PlanarAngle(absol);
            SmartVector mid = (Start + (absol / 2));
            Target.position = (Vector3)mid;
            Target.Rotate((Vector3)this.Forward, angle);
            Target.localScale = new Vector3(width, absol.Magnitude, height);
            return true;
        }
        catch (Exception ex)
        {
            GameEngine.Debug.Log("GameWorld.Place " + ex);
            return false;
        }
    }

    private void _assess()
    {
        if (this.Changed)
        {
            this._up = this.Up;
            this._forward = this.Forward;
            this._right = this.Right;
            this._plane = new Plane((Vector3)this.Forward, Vector3.zero);
            this._assesed = true;
        }
    }

    private SmartVector _up;
    private SmartVector _forward;
    private SmartVector _right;
    private bool _assesed;
    private Plane _plane;
}
