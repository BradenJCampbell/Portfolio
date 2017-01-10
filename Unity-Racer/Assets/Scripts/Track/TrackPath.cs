using UnityEngine;
using System.Collections.Generic;

public partial class TrackBehaviour
{
    public class TrackPath
    {
        public TrackPath()
        {
            this._pts = new List<Vector3>();
            this._meta_pts = new List<TrackPathPoint>();
            this._sections = new List<TrackPathLine>();
            this._rail_length = 5;
            this._road_width = 10;
        }

        public float RailLength
        {
            set { this._rail_length = value;  this._update(); }
            get { return this._rail_length; }
        }

        public float RoadWidth
        {
            set { this._road_width = value;  this._update(); }
            get { return this._road_width;  }
        }

        public TrackPathLine[] Road
        {
            get { return this._road;  }
        }

        public TrackPathLine[] Rails
        {
            get { return this._rails; }
        }

        public bool Add(float x, float y, float z)
        {
            return this.Add(new Vector3(x, y, z));
        }

        public bool Add(Vector3 point)
        {
            this._pts.Add(point);
            this._update();
            return true;
        }

        public int Count
        {
            get { return this._meta_pts.Count; }
        }

        public TrackPathPoint this[int i]
        {
            get { return this._meta_pts[i % this.Count]; }
        }

        private List<Vector3> _pts;
        private List<TrackPathLine> _sections;
        private List<TrackPathPoint> _meta_pts;
        private TrackPathLine[] _road;
        private TrackPathLine[] _rails;
        private float _rail_length;
        private float _road_width;

        private Vector3 _point(int index)
        {
            if (index < 0)
            {
                index += (Mathf.CeilToInt(Mathf.Abs((float)index / (float)this._pts.Count))) * this._pts.Count;
            }
            return this._pts[index % this._pts.Count];
        }

        private void _update()
        {
            List<TrackPathLine> road = new List<TrackPathLine>();
            List<TrackPathLine> rails = new List<TrackPathLine>();
            this._sections.Clear();
            this._meta_pts.Clear();
            for (int i = 0; i < this._pts.Count; i++)
            {
                this._sections.Add(new TrackPathLine(this._point(i), this._point(i + 1)));
                this._meta_pts.Add(new TrackPathPoint(this._point(i - 1), this._point(i), this._point(i + 1)));
            }
            for (int i = 0; i < this.Count; i++)
            {
                TrackPathPoint left = this[i].Left(this.RoadWidth / 2);
                TrackPathPoint right = this[i].Right(this.RoadWidth / 2);
                TrackCalc.Road(ref road, this[i].Start, this[i].Point, this[i].End, this.RoadWidth);
                TrackCalc.Rail(ref rails, left.Start, left.Point, left.End, this.RailLength);
                TrackCalc.Rail(ref rails, right.Start, right.Point, right.End, this.RailLength);
            }
            this._road = road.ToArray();
            this._rails = rails.ToArray();
        }
    }
}
