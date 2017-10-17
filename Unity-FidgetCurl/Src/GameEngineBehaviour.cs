using BCUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class GameEngineBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    public int FrameFlex = 0;
    public bool InputCenteredInScreen = false;
    public GameEngineWorld World;
    public Camera GameCamera;
    public CurlSpinnerBehaviour CurlSpinner;
    public bool UseTraceLog = false;
    public bool UseDebug = false;

    // Use this for initialization
    void Start()
    {
        GameEngine.Debug.TraceLog.Enabled = this.UseTraceLog;
        GameEngine.Debug.Enabled = this.UseDebug;
        this.World.GameCamera = this.GameCamera;
        this._dragging = false;
        this._path = new PathTracker();
        this.Debug.Enabled = true;
        GameEngine.Bumpers.Template.Type = BumperType.Cube;
        GameEngine.Bumpers.Template.width = 1;
        GameEngine.Bumpers.Template.height = 2000;
        GameEngine.Bumpers.Deploy(BumperType.Cube, this.World.Bounds.TopLeft, this.World.Bounds.TopRight, 1, 2000);
        GameEngine.Bumpers.Deploy(BumperType.Cube, this.World.Bounds.TopLeft, this.World.Bounds.BottomLeft, 1, 2000);
        GameEngine.Bumpers.Deploy(BumperType.Cube, this.World.Bounds.TopRight, this.World.Bounds.BottomRight, 1, 2000);
        GameEngine.Bumpers.Deploy(BumperType.Cube, this.World.Bounds.BottomLeft, this.World.Bounds.BottomRight, 1, 2000);
        SmartVector cap_start = this.World.ScreenRatioToWorldPosition((float)-0.5, (float)0.5);
        SmartVector cap_end = this.World.ScreenRatioToWorldPosition((float)-0.5, (float)-0.5);
        GameEngine.Bumpers.Capsule(cap_start, cap_end, 1, 1, true, 2);
    }

    // Update is called once per frame
    void FixedUpdate () {
        GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.FixedUpdate");
        this.Debug.Update();
        this.label(this.CurlSpinner.Score + "   " + Time.fixedTime.ToString() + "(" + this.CurlSpinner.Mass + ")");
        ///GameEngine.DebugLog("Update Start (path size: " + this._path.PointCount + ")");
        if (this.CurlSpinner.Errors.Count > 0)
        {
            foreach (DebugMessage dm in this.CurlSpinner.Errors)
            {
                GameEngine.Debug.Log(dm);
            }
            this.CurlSpinner.Errors.Clear();
        }
        if (this.IsDragging)
        {
            this._path.Capture(this._last_drag);
            if (this._path.Moved)
            {
                //  send input to spinner
                SmartVector start = this._path.LastMovement.Start.Position;
                SmartVector end = this._path.LastMovement.End.Position;
                if (this.InputCenteredInScreen)
                {
                    //  shift input so it is relative to spinner
                    start += this.CurlSpinner.WorldPosition;
                    end += this.CurlSpinner.WorldPosition;
                }
                if (this.CurlSpinner.ApplyRotationalForce(start, end, this._path.LastMovement.Magnitude * 150000))
                {

                }
                else
                {
                    //GameEngine.DebugLog("not spinning - rigidbody problems?");
                }
            }
            this._path.CleanupBeforeTime(Time.fixedTime - 2);  //  only keep the last 2 seconds
        }
        //GameEngine.DebugLog("Update End (path size: " + this._path.PointCount + ")");
    }

    protected void label(string str)
    {
        this.transform.FindChild("Canvas").FindChild("Timer").GetComponent<Text>().text = str;
    }

    public bool IsDragFrame
    {
        get
        {
            GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.IsDragFrame");
            return this.IsDragging && Time.frameCount - this._path.LastMovementFrame <= this.FrameFlex;
        }
    }

    public bool IsDragging
    {
        protected set
        {
            this._dragging = value;
        }
        get
        {
            GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.IsDragging");
            return this._dragging;
        }
    }

    public void OnDrag(PointerEventData pev)
    {
        GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.OnDrag");
        if (!this.IsDragging)
        {
            GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.OnDrag.ClearPath");
            this._path.Clear();
        }
        //  the position in world space of the touch
        SmartVector worldTouch;
        if (this.World.ScreenToWorldPosition(pev.position.x, pev.position.y, out worldTouch))
        {
            this._last_drag = worldTouch;
            this.IsDragging = true;
        }
    }

    public void OnPointerDown(PointerEventData pev)
    {
        GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.OnPointerDown");
        this.OnDrag(pev);
    }

    public void OnPointerUp(PointerEventData pev)
    {
        GameEngine.Debug.TraceLog.Update("GameEngineBehaviour.OnPointerUp");
        PathTracker.PathMovement mov = this._path.CompoundMovement(15);
        if (mov != null)
        {
            this.CurlSpinner.ApplyDirectionalForce(mov.Direction, mov.Magnitude * 500);
        }
        this._path.Clear();
        this.IsDragging = false;
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

    private bool _dragging;
    private SmartVector _last_drag;
    private PathTracker _path;
    private int _debug_frame;
}