﻿using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BCUnity
{
    public partial class PathTracker
    {
        public class PathPosition
        {
            public PathPosition(SmartVector position)
            {
                this._position = position;
                this._time = Time.fixedTime;
                this._frame = Time.frameCount;
            }

            public SmartVector Position
            {
                get
                {
                    return this._position;
                }
            }

            public int FrameCaptured
            {
                get
                {
                    return this._frame;
                }
            }

            public float TimeCaptured
            {
                get
                {
                    return this._time;
                }
            }

            private SmartVector _position;
            private int _frame;
            private float _time;
        }
    }
}