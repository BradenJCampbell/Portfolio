using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCUnity
{
    struct DragTracker
    {
        public float DegreesOfFreedom;

        public DragTracker(float DegreesOfFreedom)
        {
            this.DegreesOfFreedom = DegreesOfFreedom;
            this._comp_dir = SmartVector.Zero;
            this._last_start = SmartVector.Zero;
            this._last_end = SmartVector.Zero;
            this._has_last_pos = false;
            this._has_last_dir = false;
        }

        public void Clear()
        {
            this._comp_dir = SmartVector.Zero;
            this._last_start = SmartVector.Zero;
            this._last_end = SmartVector.Zero;
            this._has_last_pos = false;
            this._has_last_dir = false;
        }

        public void Capture(SmartVector position)
        {
            this._last_start = this._last_end;
            this._last_end = position;
            if (this.HasLastPosition)
            {
                if (this.HasLastDirection && this._comp_dir.Angle(this.LastDirection) <= this.DegreesOfFreedom)
                {
                    this._comp_dir += this.LastDirection;
                }
                else
                {
                    this._comp_dir = this.LastDirection;
                }
                this._has_last_dir = true;
            }
            this._has_last_pos = true;
        }

        public bool Moved
        {
            get
            {
                return this.LastDirection.Magnitude > 0;
            }
        }

        public bool HasLastPosition
        {
            get
            {
                return this._has_last_pos;
            }
        }

        public bool HasLastDirection
        {
            get
            {
                return this._has_last_dir;
            }
        }

        public SmartVector LastStart
        {
            get
            {
                return this._last_start;
            }
        }

        public SmartVector LastEnd
        {
            get
            {
                return this._last_end;
            }
        }

        public SmartVector LastDirection
        {
            get
            {
                return this._last_end - this._last_start;
            }
        }

        public SmartVector CompositeDirection
        {
            get
            {
                return this._comp_dir;
            }
        }

        private SmartVector _comp_dir;
        private SmartVector _last_start;
        private SmartVector _last_end;
        private bool _has_last_pos;
        private bool _has_last_dir;
    }
}