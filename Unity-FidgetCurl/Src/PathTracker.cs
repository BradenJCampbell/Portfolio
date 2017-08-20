using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class PathTracker
{

    public PathTracker()
    {
        this._path = new List<PathPosition>();
        this._movement = new List<PathMovement>();
    }

    public void Capture(Vector3 position)
    {
        PathPosition point = new PathPosition(position);
        if (this._path.Count > 0)
        {
            PathPosition last = this._path[0];
            if (Vector3.Distance(last.Position, position) > 0)
            {
                this._movement.Insert(0, new PathMovement(last, point));
            }
        }
        this._path.Insert(0, point);
    }

    public void CleanupBeforeTime(float seconds)
    {
        foreach (PathPosition p in this._path)
        {
            if (p.TimeCaptured < seconds)
            {
                this._path.Remove(p);
            }
        }
    }

    public void CleanupBeforeFrame(int frame)
    {
        foreach (PathPosition p in this._path)
        {
            if (p.FrameCaptured < frame)
            {
                this._path.Remove(p);
            }
        }
    }

    public int LastMovementFrame
    {
        get
        {
            if (this.LastMovement == null)
            {
                return -1;
            }
            return this.LastMovement.FrameCaptured;
        }
    }

    public PathMovement LastMovement
    {
        get
        {
            if (this._movement.Count > 0)
            {
                return this._movement[0];
            }
            return null;
        }
    }

    private List<PathPosition> _path;
    private List<PathMovement> _movement;
}
