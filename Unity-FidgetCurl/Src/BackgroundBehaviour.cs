using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BCUnity;

public class BackgroundBehaviour : MonoBehaviour {

    public bool ColourChange = false;
    public float ColourCycle = (float)1;
    public float ColourDelta = (float)0.1;

	// Use this for initialization
	void Start () {
        if (this.ColourChange)
        {
            this._timer = new BCUnity.Timer();
            this._timer.Start();
            this.Colour = new Color(Random.value, Random.value, Random.value);
            this._r_delta = 0;
            this._g_delta = 0;
            this._b_delta = 0;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (this.ColourChange)
        {
            if (this._timer.ElapsedTime >= this.ColourCycle)
            {
                this._r_delta = this.ColourCycle * Random.Range(this.ColourDelta / -2, this.ColourDelta / 2);
                this._g_delta = this.ColourCycle * Random.Range(this.ColourDelta / -2, this.ColourDelta / 2);
                this._b_delta = this.ColourCycle * Random.Range(this.ColourDelta / -2, this.ColourDelta / 2);
                this._timer.Reset();
                this._timer.Start();
            }
            this.Colour = new Color(
                Mathf.Clamp(this.Colour.r + Time.fixedDeltaTime * this._r_delta, 0, 1),
                Mathf.Clamp(this.Colour.g + Time.fixedDeltaTime * this._g_delta, 0, 1),
                Mathf.Clamp(this.Colour.b + Time.fixedDeltaTime * this._b_delta, 0, 1)
            );
        }
    }

    public void Resize(float x, float y)
    {
        this.transform.localScale = new Vector3(x, y, 1);
    }

    public Color Colour
    {
        set
        {
            this.transform.GetComponent<Renderer>().material.color = value;
        }
        get
        {
            return this.transform.GetComponent<Renderer>().material.color;
        }
    }

    public void Grid(Color colour, float size = 10)
    {
        if (size <= 0)
        {
            size = 10;
        }
        for (float x = GameEngine.GameWorld.Bounds.Left; x <= GameEngine.GameWorld.Bounds.Right; x += size)
        {
            GameEngine.Debug.Line(SmartVector.CreateWorldPoint(x, GameEngine.GameWorld.Bounds.Bottom, 0), SmartVector.CreateWorldPoint(x, GameEngine.GameWorld.Bounds.Top, 0), colour);
        }
        for (float y = GameEngine.GameWorld.Bounds.Bottom; y <= GameEngine.GameWorld.Bounds.Top; y += size)
        {
            GameEngine.Debug.Line(SmartVector.CreateWorldPoint(GameEngine.GameWorld.Bounds.Left, y, 0), SmartVector.CreateWorldPoint(GameEngine.GameWorld.Bounds.Right, y, 0), colour);
        }
    }
    private Timer _timer;
    private float _r_delta;
    private float _g_delta;
    private float _b_delta;
}
