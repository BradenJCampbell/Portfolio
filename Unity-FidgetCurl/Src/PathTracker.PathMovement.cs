using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class PathTracker
{
    public class PathMovement
    {
        public PathMovement(PathPosition Start, PathPosition End)
        {
            this._start = Start;
            this._end = End;
            this._ray = new Ray(Start.Position, End.Position - Start.Position);
        }

        public int FrameCaptured
        {
            get
            {
                return this.End.FrameCaptured;
            }
        }

        public float TimeCaptured
        {
            get
            {
                return this._end.TimeCaptured;
            }
        }

        public PathPosition Start
        {
            get
            {
                return this._start;
            }
        }

        public PathPosition End
        {
            get
            {
                return this._end;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return this.End.Position - this.Start.Position;
            }
        }

        public float Magnitude
        {
            get
            {
                return (this.End.Position - this.Start.Position).magnitude;
            }
        }
        public static explicit operator Ray(PathMovement pm)
        {
            return pm._ray;
        }

        private PathPosition _start;
        private PathPosition _end;
        private Ray _ray;
    }
}