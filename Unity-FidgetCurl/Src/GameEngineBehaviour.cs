using BCUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum AngularDirection
{
    Clockwise = -1,
    CounterClockwise = 1
}

public partial class GameEngineBehaviour : MonoBehaviour {

    public bool InputCenteredInScreen = false;
    public GameEngineWorld World;
    public Canvas UICanvas;
    public bool UseTraceLog = false;
    public bool UseDebug = false;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        GameEngine.Debug.TraceLog.Enabled = this.UseTraceLog;
        GameEngine.Debug.Enabled = this.UseDebug;
        this._touch = new TouchListener(this);
        GameEngine.Bumpers.Template.Type = BumperType.Cube;
        GameEngine.Bumpers.Template.width = 1;
        GameEngine.Bumpers.Template.height = 5;
        GameEngine.Bumpers.Template.visible = true;
        GameEngine.Bumpers.Template.Deploy(this.World.Bounds.TopLeft, this.World.Bounds.TopRight);
        GameEngine.Bumpers.Template.Deploy(this.World.Bounds.TopLeft, this.World.Bounds.BottomLeft);
        GameEngine.Bumpers.Template.Deploy(this.World.Bounds.TopRight, this.World.Bounds.BottomRight);
        GameEngine.Bumpers.Template.Deploy(this.World.Bounds.BottomLeft, this.World.Bounds.BottomRight);
        GameEngine.Bumpers.Template.Type = BumperType.Capsule;
        GameEngine.Bumpers.Template.width = 10;
        GameEngine.Bumpers.Template.height = 1;
        GameEngine.Bumpers.Template.Score = 15;
        GameEngine.Bumpers.Template.visible = true;
        GameEngine.Bumpers.Template.Colour = Color.green;
        List<SmartVector> pts = new List<SmartVector>();
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.3, (float)0.8));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.2, (float)0.6));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.2, (float)0.4));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.3, (float)0.2));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.5, (float)0.1));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.7, (float)0.2));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.8, (float)0.4));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.8, (float)0.6));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.7, (float)0.8));
        pts.Add(GameEngine.GameWorld.RatioPosition((float)0.5, (float)0.9));
        for (int i = 0; i < pts.Count; i++)
        {
            GameEngine.Bumpers.Template.Deploy(pts[i], pts[(i + 1) % pts.Count]);
        }
        GameEngine.Background.Grid(Color.grey, 100);
        //this.transform.FindChild("Canvas").FindChild("PauseButton").GetComponent<Button>().onClick.AddListener(OnPauseClick);
    }

    // Update is called once per frame
    void FixedUpdate () {
        GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.FixedUpdate");
        this.Debug.Update();
        this.label(BCUnity.Format.Number(Time.fixedTime, 5, 2) + "\n(" + 1 / Time.deltaTime + "fps)");
        this.UICanvas.transform.FindChild("ScoreBox").GetComponent<Text>().text = Mathf.FloorToInt(GameEngine.Spinner.Score).ToString().PadLeft(6, '0');
        ///GameEngine.DebugLog("Update Start (path size: " + this._path.PointCount + ")");
        if (GameEngine.Spinner.Errors.Count > 0)
        {
            foreach (DebugMessage dm in GameEngine.Spinner.Errors)
            {
                GameEngine.Debug.Log(dm);
            }
            GameEngine.Spinner.Errors.Clear();
        }
        this._touch.Update();
        if (this.IsDragging && this._touch.Single.Moved)
        {
            //  send input to spinner
            SmartVector start = this._touch.Single.LastMovement.Start;
            SmartVector end = this._touch.Single.LastMovement.End;
            //  show spin
            //GameEngine.Debug.Line(start, end, Color.green, 2);
            //GameEngine.Debug.Point(start, Color.red, 2);
            //GameEngine.Debug.Point(end, Color.red, 2);
            //GameEngine.Debug.Log("Spin " + start + " => " + end);
            if (this.InputCenteredInScreen)
            {
                //  shift input so it is relative to spinner
                start += GameEngine.Spinner.WorldPosition;
                end += GameEngine.Spinner.WorldPosition;
            }
            if (GameEngine.Spinner.ApplyRotationalForce(start, end, this._touch.Single.LastMovement.Magnitude * 600000))
            {

            }
            else
            {
                //GameEngine.DebugLog("not spinning - rigidbody problems?");
            }
        }
        else if (this._touch.Single.InState(TouchState.End))
        {
            TouchMovement mov = this._touch.Single.CompoundMovement(15);
            GameEngine.Spinner.ApplyDirectionalForce(mov.Direction, mov.Magnitude * 75 * GameEngine.Spinner.SpinSpeed);
        }
    }

    public void OnPauseClick()
    {
        this.Pause = !this.Pause;
    }

    public void OnResetClick()
    {
        GameEngine.Spinner.Reset();
    }

    public bool Pause
    {
        get
        {
            return Time.timeScale <= 0;
        }
        set
        {
            switch (value)
            {
                case true:
                    this._old_time_scale = Time.timeScale;
                    Time.timeScale = 0;
                    break;
                case false:
                    Time.timeScale = this._old_time_scale;
                    break;
            }
        }
    }

    protected void label(string str)
    {
        this.UICanvas.transform.FindChild("Timer").GetComponent<Text>().text = str;
    }

    public bool IsDragFrame
    {
        get
        {
            GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.IsDragFrame");
            return this.IsDragging;
        }
    }

    public bool IsDragging
    {
        get
        {
            GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.IsDragging");
            return this._touch.Single.InState(TouchState.Start, TouchState.Touch);
        }
    }

    public bool IsThrown
    {
        get
        {
            return this._touch.Single.InState(TouchState.End);
        }
    }

    public void OnApplicationQuit()
    {
        this.Debug.Close();
    }

    protected static GameEngineBehaviour _instance
    {
        get
        {
            return GameObject.FindObjectOfType<GameEngineBehaviour>();
        }
    }

    private TouchListener _touch;
    private int _debug_frame;
    private float _old_time_scale;
}