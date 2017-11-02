using BCUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class GameEngineBehaviour
{
    public enum TouchState
    {
        None,
        Start,
        Touch,
        End
    }

    public struct TouchMovement
    {
        public static explicit operator TouchMovement(PathTracker.PathMovement move)
        {
            return new TouchMovement(move);
        }

        private TouchMovement(PathTracker.PathMovement move)
        {
            this._base = move;
        }

        public bool Exists
        {
            get
            {
                return this._base != null;
            }
        }

        public SmartVector Start
        {
            get
            {
                if (this.Exists)
                {
                    return this._base.Start.Position;
                }
                return SmartVector.Empty;
            }
        }

        public SmartVector End
        {
            get
            {
                if (this.Exists)
                {
                    return this._base.End.Position;
                }
                return SmartVector.Empty;
            }
        }

        public float Magnitude
        {
            get
            {
                if (this.Exists)
                {
                    return this._base.Magnitude;
                }
                return 0;
            }
        }

        public SmartVector Direction
        {
            get
            {
                if (this.Exists)
                {
                    return this._base.Direction;
                }
                return SmartVector.Empty;
            }
        }

        private PathTracker.PathMovement _base;
    }

    public partial class TouchListener
    {
        public TouchListener(GameEngineBehaviour Parent)
        {
            this._parent = Parent;
            this._single = new TouchListener.SingleTouchListener(this._parent);
        }

        public void Update()
        {
            this._single.Update();
        }

        public SingleTouchListener Single
        {
            get
            {
                return this._single;
            }
        }

        private GameEngineBehaviour _parent;
        private SingleTouchListener _single;
    }
}
