using UnityEngine;

[System.Serializable]
public class GameEngineWorld
{
    public Vector3 Up = Vector3.up;
    public Vector3 Forward = Vector3.forward;
    public Vector3 Right = Vector3.right;

    public bool Changed
    {
        get
        {
            return (!this._assesed || Vector3.Distance(this.Up, this._up) != 0 || Vector3.Distance(this.Forward, this._forward) != 0 || Vector3.Distance(this.Right, this._right) != 0);
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
        this._assess();
        float ret = MathHelper.PlanarAngle(this._up, this._right, v, false);
        if (ret < 0)
        {
            return 360 + ret;
        }
        return ret;
    }

    private void _assess()
    {
        if (this.Changed)
        {
            this._up = this.Up;
            this._forward = this.Forward;
            this._right = this.Right;
            this._plane = new Plane(this.Forward, Vector3.zero);
            this._assesed = true;
        }
    }

    private Vector3 _up;
    private Vector3 _forward;
    private Vector3 _right;
    private bool _assesed;
    private Plane _plane;
}
