using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BCUnity
{
    public partial class PathTracker
    {

        public PathTracker()
        {
            this._path = new List<PathPosition>();
            this._movement = new List<PathMovement>();
        }

        public void Capture(SmartVector position)
        {
            PathPosition point = new PathPosition(position);
            if (this._path.Count > 0)
            {
                PathPosition last = this._path[0];
                if (last.Position.Distance(position) > 0)
                {
                    this._movement.Insert(0, new PathMovement(last, point));
                }
            }
            this._path.Insert(0, point);
        }

        public int PointCount
        {
            get
            {
                return this._path.Count;
            }
        }

        public void Clear()
        {
            this._movement.Clear();
            this._path.Clear();
        }

        public void CleanupBeforeTime(float seconds)
        {
            for (int i = 0; i < this._path.Count; i++)
            {
                if (this._path[i].TimeCaptured < seconds)
                {
                    this._path.RemoveAt(i);
                    i--;
                }
            }
        }

        public void CleanupBeforeFrame(int frame)
        {
            for (int i = 0; i < this._path.Count; i++)
            {
                if (this._path[i].FrameCaptured < frame)
                {
                    this._path.RemoveAt(i);
                    i--;
                }
            }
        }

        public bool Moved
        {
            get
            {
                return this._movement.Count > 0;
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

        public bool CompoundMovement(float DegreesOfFreedom, out PathMovement Result)
        {
            Result = this.CompoundMovement(DegreesOfFreedom);
            return (Result != null);
        }

        public bool CompoundMovement(SmartVector Direction, float DegreesOfFreedom, out PathMovement Result)
        {
            Result = this.CompoundMovement(Direction, DegreesOfFreedom);
            return (Result != null);
        }

        public PathMovement CompoundMovement(float DegreesOfFreedom)
        {
            if (this.LastMovement == null)
            {
                return null;
            }
            return this.CompoundMovement(this.LastMovement.Direction, DegreesOfFreedom);
        }

        public PathMovement CompoundMovement(SmartVector Direction, float DegreesOfFreedom)
        {
            PathMovement start = null;
            PathMovement end = null;
            for (int i = 0; i < this._movement.Count && Mathf.Abs(this._movement[i].Direction.Angle(Direction)) <= DegreesOfFreedom; i++)
            {
                GameEngine.Debug.TraceLog.Update("PathTracker.CompoundMovement[" + i + "]");
                start = this._movement[i];
                if (end == null)
                {
                    end = this._movement[i];
                }
            }
            if (end == null || start == null)
            {
                return null;
            }
            return new PathMovement(start.Start, end.End);
        }

        private List<PathPosition> _path;
        private List<PathMovement> _movement;
    }
}
