using UnityEngine;
using System.Collections.Generic;

public partial class TrackBehaviour
{
    public class TrackPathLine
    {
        private Vector3 _start;
        private Vector3 _end;
        private Vector3 _midpoint;

        public TrackPathLine(Vector3 start, Vector3 end)
        {
            this._start = start;
            this._end = end;
            this._midpoint = start + ((end - start) / 2);
        }

        public Vector3 start
        {
            get { return this._start; }
        }

        public Vector3 end
        {
            get { return this._end; }
        }

        public Vector3 midpoint
        {
            get { return this._midpoint; }
        }

        public Vector3 direction
        {
            get { return this.end - this.start; }
        }

        public float magnitude
        {
            get { return this.direction.magnitude; }
        }

        public bool IsParallel(TrackPathLine other)
        {
            return (this.direction.normalized == other.direction.normalized);
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            return this.start + (this.direction * Vector3.Dot(this.direction, point - this.start));
        }

        public bool Contains(Vector3 point)
        {
            return Mathf.Abs(this.magnitude - Vector3.Distance(this.start, point) - Vector3.Distance(this.end, point)) <= 0.0001;
        }

        public float Distance(Vector3 point)
        {
            Vector3 closest = this.ClosestPoint(point);
            if (this.Contains(closest))
            {
                return Vector3.Cross(this.end - this.start, point - this.start).magnitude;
            }
            return Mathf.Min(Vector3.Distance(this.end, point), Vector3.Distance(this.start, point));
        }

        public void DebugRender(Color Colour)
        {
            Debug.DrawLine(this.start, this.end, Colour);
        }
    }
}
