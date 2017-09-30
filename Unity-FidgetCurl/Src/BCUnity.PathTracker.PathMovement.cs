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
        public class PathMovement
        {
            public PathMovement(PathPosition Start, PathPosition End)
            {
                this._start = Start;
                this._end = End;
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

            public SmartVector Direction
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
                    return (this.End.Position - this.Start.Position).Magnitude;
                }
            }

            private PathPosition _start;
            private PathPosition _end;
        }
    }
}