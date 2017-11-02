using BCUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CameraBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //  center camera
        SmartVector spin_pos = GameObject.FindObjectOfType<CurlSpinnerBehaviour>().WorldPosition;
        this.LookAt(spin_pos);
        /*
        SmartVector edge_pos;
        if (this.Bounds.Project(spin_pos, out edge_pos))
        {
            SmartVector target = BCUnity.Helper.Math.Scale(this.ScreenRatioToWorldPosition(0, 0), edge_pos, (float)0.2);
            //GameEngine.Debug.Point(target, Color.green);
            //this.LookAt(target);
        }
        */
    }

    public bool ValidScreenPoint(float x, float y)
    {
        return this.Bounds.Screen.Contains(x, y);
    }

    public bool ScreenToWorldPosition(float x, float y, out SmartVector output)
    {
        if (this.ValidScreenPoint(x, y))
        {
            Ray pos_ray = this.ScreenPointToRay(x, y);
            return GameEngine.GameWorld.Raycast(pos_ray, out output);
        }
        output = SmartVector.Zero;
        return false;
    }

    public SmartVector ScreenRatioToWorldPosition(float x, float y)
    {
        return SmartVector.CreateWorldPoint(this.Bounds.World.minX + this.Bounds.World.Width * x, this.Bounds.World.minY + this.Bounds.World.Height * y, 0);
    }

    public bool LookAt(SmartVector WorldPosition)
    {
        try
        {
            this.transform.position = new Vector3(
                Mathf.Clamp(WorldPosition.x, 
                    GameEngine.GameWorld.Bounds.Left + (this.Bounds.World.Width / 2), 
                    GameEngine.GameWorld.Bounds.Right - (this.Bounds.World.Width / 2)),
                Mathf.Clamp(WorldPosition.y, 
                    GameEngine.GameWorld.Bounds.Bottom + (this.Bounds.World.Height / 2), 
                    GameEngine.GameWorld.Bounds.Top - (this.Bounds.World.Height / 2)),
                this.transform.position.z
            );
            return this.ContainsPoint(WorldPosition);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public Ray ScreenPointToRay(float x, float y)
    {
        return this.GetComponent<Camera>().ScreenPointToRay(new Vector3(x, y, 0));
    }

    public bool ContainsPoint(SmartVector Point)
    {
        return Point.World.x >= this.Bounds.Left && Point.World.x <= this.Bounds.Right && 
            Point.World.y >= this.Bounds.Bottom && Point.World.y <= this.Bounds.Top;
    }

    private Camera _camera
    {
        get
        {
            return this.GetComponent<Camera>();
        }
    }
}
