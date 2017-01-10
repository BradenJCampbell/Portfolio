using UnityEngine;
using System.Collections.Generic;

public partial class TrackBehaviour : MonoBehaviour
{
    public class TrackPathPoint
    {
        public TrackPathPoint(Vector3 prev, Vector3 curr, Vector3 next)
        {
            this._update(prev, curr, next);
            this._db_rend = new DebugRenderer(this);
        }

        public Vector3 Point
        {
            get { return this._curr; }
        }

        public Vector3 Prev
        {
            get { return this._prev;  }
        }

        public Vector3 Next
        {
            get { return this._next; }
        }

        public Vector3 Start
        {
            get { return this._curr + (StaticHelper.ToOrigin(this._prev, this._curr) / 2); }
        }

        public Vector3 End
        {
            get { return this._curr + (StaticHelper.ToOrigin(this._next, this._curr) / 2); }
        }

        public bool IsCurved
        {
            get { return this._has_pivot; }
        }

        public Vector3 Pivot
        {
            get { return this._pivot; }
        }

        public Vector3 Level
        {
            get { return this._point_level; }
        }

        public TrackPathPoint Left(float distance)
        {
            if (distance <= 0)
            {
                return null;
            }
            return this._parallel(-1 * distance);
        }

        public TrackPathPoint Right(float distance)
        {
            if (distance <= 0)
            {
                return null;
            }
            return this._parallel(distance);
        }

        public DebugRenderer DebugRender
        {
            get { return this._db_rend; }
        }

        private TrackPathPoint _parallel(float distance)
        {
            // calculate points on the new segment
            Vector3 prev = this._prev + (distance * this._start_level);
            Vector3 next = this._next + (distance * this._end_level);
            //  find intersection on new segment
            Vector3 curr = this._curr + (distance * this._point_level);
            Vector3 try_curr;
            if (this.IsCurved)
            {
                if (StaticHelper.Intersect(out try_curr, new Ray(prev, this._curr - this._prev), new Ray(next, this._curr - this._next)))
                {
                    curr = try_curr;
                }
            }
            //  calculate a pseudo-halfway point on new segment
            Vector3 start_half = this.Start + (distance * this._start_level);
            Vector3 end_half = this.End + (distance * this._end_level);
            //  calculate the previous/next points in the new segment
            prev = curr + (2 * (start_half - curr));
            next = curr + (2 * (end_half - curr));
            return new TrackPathPoint(prev, curr, next);
        }

        private void _update(Vector3 prev, Vector3 curr, Vector3 next)
        {
            this._curr = curr;
            this._prev = prev;
            this._next = next;
            this._has_pivot = TrackCalc.PivotPoint(out this._pivot, this.Start, this.Point, this.End);
            // compute start level
            this._start_normal = TrackCalc.Normal(this._prev, this._curr);
            this._start_level = TrackCalc.Level(this._prev, this._curr);
            //  compute end level
            this._end_normal = TrackCalc.Normal(this._curr, this._next);
            this._end_level = TrackCalc.Level(this._curr, this._next);
            //  compute point level
            this._point_level = (this._start_level + this._end_level).normalized;
        }

        protected Vector3 _curr;
        protected Vector3 _prev;
        protected Vector3 _next;
        protected Vector3 _pivot;
        protected Vector3 _start_normal;
        protected Vector3 _start_level;
        protected Vector3 _end_normal;
        protected Vector3 _end_level;
        protected Vector3 _point_level;
        protected bool _has_pivot;
        private DebugRenderer _db_rend;

        public class DebugRenderer
        {
            public DebugRenderer (TrackPathPoint targ)
            {
                this._point = targ;
            }
            public void Line(Color Colour)
            {
                Debug.DrawLine(this._point.Start, this._point.Point, Colour);
                Debug.DrawLine(this._point.End, this._point.Point, Colour);

            }
            public void Ends(Color Colour)
            {
                StaticHelper.DebugDrawStar(this._point.Start, (float)0.5, Colour);
                StaticHelper.DebugDrawStar(this._point.End, (float)0.5, Colour);
            }
            public void Pivot(Color Colour)
            {
                if (this._point.IsCurved)
                {
                    StaticHelper.DebugDrawStar(this._point.Pivot, (float)0.5, Colour);
                }
            }
            private TrackPathPoint _point;
        }
    }
}
