using UnityEngine;
using System.Collections.Generic;

public partial class TrackBehaviour : MonoBehaviour {
    public float InnerRadius;
    public float TrackWidth;
    public int PostCount;

    // Use this for initialization
    void Start()
    {
        this._render = new TrackRenderer(this);
        this._path = new TrackPath();
        this._path.Add(0, 0, 0);
        this._path.Add(0, 0, 30);
        this._path.Add(-30, 0, 30);
        this._path.Add(-30, 0, 60);
        this._path.Add(0, 0, 60);
        this._path.Add(30, 0, 60);
        this._path.Add(30, 0, 30);
        this._path.Add(30, 15, 15);
        this._path.Add(30, 0, 0);
        this._path.Add(30, 0, -30);
        this._path.Add(0, 0, -30);
        this._render.RenderPath(this._path);
        Destroy(this.Fence.gameObject);
        Destroy(this.Post.gameObject);
        Destroy(this.Road.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        this.DebugRender();
    }

    void DebugRender()
    {
        for (int i = 0; i < this._path.Count; i++)
        {
            Debug.DrawLine(this._path[i].Point, this._path[i + 1].Point, Color.magenta);
        }
        for (int i = 0; i < this._path.Road.Length; i++)
        {
            Debug.DrawLine(this._path.Road[i].start, this._path.Road[i].end, Color.yellow);
        }
        for (int i = 0; i < this._path.Count; i++)
        {
            this._path[i].DebugRender.Line(Color.red);
            this._path[i].DebugRender.Ends(Color.magenta);
            this._path[i].DebugRender.Pivot(Color.magenta);
            this._path[i].Left(5).DebugRender.Line(Color.red);
            this._path[i].Left(5).DebugRender.Ends(Color.cyan);
            this._path[i].Left(5).DebugRender.Pivot(Color.red);
            this._path[i].Right(5).DebugRender.Line(Color.red);
            this._path[i].Right(5).DebugRender.Ends(Color.cyan);
            this._path[i].Right(5).DebugRender.Pivot(Color.red);
        }
        for (int i = 0; i < this._path.Rails.Length; i++)
        {
            this._path.Rails[i].DebugRender(Color.yellow);
        }
    }

    public Transform Road
    {
        get { return this.transform.FindChild("Road");  }
    }

    public Transform Fence
    {
        get { return this.transform.FindChild("Fence");  }
    }

    public Transform Post
    {
        get { return this.transform.FindChild("Post");  }
    }

    private TrackPath _path;
    private TrackRenderer _render;
}

