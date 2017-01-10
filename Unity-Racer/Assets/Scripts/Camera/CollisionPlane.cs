using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class CameraCollisionHandler
{
    [System.Serializable]
    public class CollisionPlane
    {
        public CollisionPlane(Vector3 WorldPosition, Vector3 WorldLookAtPoint, float FieldOfView, float PlaneDistance, float AspectRatio)
        {
            this._init(WorldPosition, WorldLookAtPoint, FieldOfView, PlaneDistance, AspectRatio, Vector3.up, Vector3.right);
        }

        public CollisionPlane(Vector3 WorldPosition, Vector3 WorldLookAtPoint, float FieldOfView, float PlaneDistance, float AspectRatio, Vector3 up)
        {
            this._init(WorldPosition, WorldLookAtPoint, FieldOfView, PlaneDistance, AspectRatio, up, Vector3.right);
        }

        public CollisionPlane(Vector3 WorldPosition, Vector3 WorldLookAtPoint, float FieldOfView, float PlaneDistance, float AspectRatio, Vector3 up, Vector3 right)
        {
            this._init(WorldPosition, WorldLookAtPoint, FieldOfView, PlaneDistance, AspectRatio, up, right);
        }

        public Vector3 Offset
        {
            get { return this._offset; }
        }

        public Vector3 Normal
        {
            get { return -1 * this.Offset.normalized; }
        }
        
        public Vector3 TopLeft
        {
            get { return this.Middle - this._x_rad + this._y_rad; }
        }

        public Vector3 TopMiddle
        {
            get { return this.Middle + this._y_rad;  }
        }

        public Vector3 TopRight
        {
            get { return this.Middle + this._x_rad + this._y_rad; }
        }

        public Vector3 MiddleLeft
        {
            get { return this.Middle - this._x_rad; }
        }

        public Vector3 Middle
        {
            get { return this._mid + this._offset; }
        }

        public Vector3 MiddleRight
        {
            get { return this.Middle + this._x_rad; }
        }

        public Vector3 BottomLeft
        {
            get { return this.Middle - this._x_rad - this._y_rad; }
        }

        public Vector3 BottomMiddle
        {
            get { return this.Middle - this._y_rad; }
        }

        public Vector3 BottomRight
        {
            get { return this.Middle + this._x_rad - this._y_rad; }
        }

        public Vector3[] Corners
        {
            get { return this._corners; }
        }

        public Helper.Line.Segment[] Edges
        {
            get { return this._edges; }
        }

        public Helper.Line.Segment[] LocalEdges
        {
            get { return this._local_edges; }
        }

        public float Diagonal
        {
            get { return Mathf.Sqrt(Mathf.Pow(this._x_rad.magnitude, 2) + Mathf.Pow(this._y_rad.magnitude, 2)); }
        }

        /// <summary>
        /// get the point on this planes bounds closest to the given point
        /// (note that this is on the bounds only; closest point on plane is NOT returned)
        /// </summary>
        public Vector3 ClosestPointOnBounds(Vector3 point)
        {
            Vector3 ret = this.Edges[0].ClosestPoint(point);
            for (int i = 1; i < this.Edges.Length; i++)
            {
                Vector3 curr = this.Edges[i].ClosestPoint(point);
                if (Vector3.Distance(ret, point) > Vector3.Distance(curr, point))
                {
                    ret = curr;
                }
            }
            return ret;
        }

        public Vector3 Localize(Vector3 world_point)
        {
            return world_point - this.Middle;
        }

        public Vector3 Project(Vector3 point)
        {
            return this.Middle + Vector3.ProjectOnPlane(point - this.Middle, this.Normal);
        }

        public Vector3 ProjectToBounds(Vector3 point)
        {
            //  project onto plane
            Vector3 projected = this.Project(point);
            //  compute point along full line (middle, projected) on bounds
            Helper.Line l = new Helper.Line(this.Middle, this.Middle + StaticHelper.BuildVector(projected - this.Middle, this.Diagonal));
            List<Vector3> hits = new List<Vector3>();
            Vector3 hit;
            for (int i = 0; i < this.Edges.Length; i++)
            {
                if (l.ClosestPoint(out hit, this.Edges[i]))
                {
                    hits.Add(hit);
                }
                if (Helper.Line.Intersect(out hit, this.Edges[i].p1, this.Edges[i].p2, this.Middle, projected))
                //if (((Helper.Line)this.Edges[i]).Intersect(out hit, l))
                {
                    //hits.Add(hit);
                }
            }
            //Debug.Log(hits.Count);
            Vector3 ret = hits[0];
            for (int i = 1; i < hits.Count; i++)
            {
                if (Vector3.Distance(ret, projected) > Vector3.Distance(hits[i], projected))
                {
                    ret = hits[i];
                }
            }
            return ret;
        }

        public RaycastHit[] Hits(Vector3 DesiredWorldPosition, LayerMask CollisionLayersMask)
        {
            List<RaycastHit> ret = new List<RaycastHit>();
            RaycastHit hit;
            for (int i = 0; i < this.Corners.Length; i++)
            { 
                if (StaticHelper.RaycastPoint(out hit, DesiredWorldPosition, this.Corners[i], CollisionLayersMask))
                {
                    ret.Add(hit);
                }
            }
            return ret.ToArray();
        }

        /// <summary>
        /// calculate a point within the plane section
        /// </summary>
        /// <param name="x">ratio from -1 (left) to +1 (right) along camera plane x axis</param>
        /// <param name="y">ratio from -1 (bottom) to +1 (top) along camera plane y axis</param>
        /// <returns></returns>
        public Vector3 Point(float x, float y)
        {
            return this.Middle + (x * this._x_rad) + (y * this._y_rad);
        }

        public void DebugRender()
        {
            Color col = Color.magenta;
            Debug.DrawLine(this.TopLeft, this.TopRight, col);
            Debug.DrawLine(this.TopRight, this.BottomRight, col);
            Debug.DrawLine(this.BottomRight, this.BottomLeft, col);
            Debug.DrawLine(this.BottomLeft, this.TopLeft, col);
        }

        private Vector3 _offset;
        private Vector3 _mid;
        private Vector3 _x_rad;
        private Vector3 _y_rad;
        private Vector3[] _corners;
        private Helper.Line.Segment[] _edges;
        private Helper.Line.Segment[] _local_edges;

        private void _init(Vector3 WorldPosition, Vector3 WorldLookAtPoint, float FieldOfView, float PlaneDistance, float AspectRatio, Vector3 up, Vector3 right)
        {
            this._offset = StaticHelper.BuildVector(WorldLookAtPoint - WorldPosition, PlaneDistance);
            // compute local coords
            float xRad = Mathf.Tan(FieldOfView / 2) * PlaneDistance;
            float yRad = xRad / AspectRatio;
            //  translate to global
            this._mid = WorldPosition + this.Offset;
            this._x_rad = right.normalized * xRad;
            this._y_rad = up.normalized * yRad;
            this._corners = null;
            this._edges = null;
            //  mark the corners
            List<Vector3> corn = new List<Vector3>();
            corn.Add(this.TopLeft);
            corn.Add(this.TopMiddle);
            corn.Add(this.TopRight);
            corn.Add(this.MiddleLeft);
            corn.Add(this.MiddleRight);
            corn.Add(this.BottomLeft);
            corn.Add(this.BottomMiddle);
            corn.Add(this.BottomRight);
            this._corners = corn.ToArray();
            //  mark the edges
            List<Helper.Line.Segment> edges = new List<Helper.Line.Segment>();
            edges.Add(new Helper.Line.Segment(this.TopLeft, this.TopRight));
            edges.Add(new Helper.Line.Segment(this.TopRight, this.BottomRight));
            edges.Add(new Helper.Line.Segment(this.BottomRight, this.BottomLeft));
            edges.Add(new Helper.Line.Segment(this.BottomLeft, this.TopLeft));
            this._edges = edges.ToArray();
            // mark the local edges
            List<Helper.Line.Segment> local_edges = new List<Helper.Line.Segment>();
            for (int i = 0; i < this.Edges.Length; i++)
            {
                local_edges.Add(new Helper.Line.Segment(this.Edges[i].p1 - this.Middle, this.Edges[i].p2 - this.Middle));
            }
            this._local_edges = local_edges.ToArray();
        }
    }
}
