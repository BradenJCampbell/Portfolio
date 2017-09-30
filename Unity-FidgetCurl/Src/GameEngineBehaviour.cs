using BCUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameEngineBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    public int FrameFlex = 0;
    public bool InputCenteredInScreen = false;
    public GameEngineWorld World;
    public Camera GameCamera;
    public CurlSpinnerBehaviour CurlSpinner;

    // Use this for initialization
    void Start()
    {
        if (GameEngineBehaviour._instance == null)
        {
            GameEngineBehaviour._instance = this;
            GameEngine.TraceLog.Enabled = true;
            this.World.GameCamera = this.GameCamera;
            this._line = this.transform.FindChild("Line");
            this._line.gameObject.SetActive(false);
            this._dragging = false;
            this._path = new PathTracker();
            SmartVector tl = this.World.ScreenRatioToWorldPosition(-1, -1);
            SmartVector tr = this.World.ScreenRatioToWorldPosition(1, -1);
            SmartVector bl = this.World.ScreenRatioToWorldPosition(-1, 1);
            SmartVector br = this.World.ScreenRatioToWorldPosition(1, 1);
            this.DeployBumper(tl, tr, 1, 2000);
            this.DeployBumper(tl, bl, 1, 2000);
            this.DeployBumper(tr, br, 1, 2000);
            this.DeployBumper(bl, br, 1, 2000);
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        GameEngine.TraceLog.Update("GameEngineBehaviour.FixedUpdate");
        this.label(Time.fixedTime.ToString() + "(" + this.CurlSpinner.Mass + ")");
        ///GameEngine.DebugLog("Update Start (path size: " + this._path.PointCount + ")");
        if (this.CurlSpinner.Errors.Count > 0)
        {
            foreach (DebugMessage dm in this.CurlSpinner.Errors)
            {
                GameEngine.DebugLog(dm);
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

    public bool DeployBumper(SmartVector Start, SmartVector End, float width = 1, float height = 1)
    {
        GameEngine.TraceLog.Update("GameEngineBehaviour.DeployBumper");
        BumperBehaviour bump = GameObject.FindObjectOfType<BumperBehaviour>();
        if (bump == null)
        {
            return false;
        }
        bump.Deploy(Start, End, width);
        return true;
    }

    public bool IsDragFrame
    {
        get
        {
            GameEngine.TraceLog.Update("GameEngineBehaviour.IsDragFrame");
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
            GameEngine.TraceLog.Update("GameEngineBehaviour.IsDragging");
            return this._dragging;
        }
    }

    public void OnDrag(PointerEventData pev)
    {
        GameEngine.TraceLog.Update("GameEngineBehaviour.OnDrag");
        if (!this.IsDragging)
        {
            GameEngine.TraceLog.Update("GameEngineBehaviour.OnDrag.ClearPath");
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
        GameEngine.TraceLog.Update("GameEngineBehaviour.OnPointerDown");
        this.OnDrag(pev);
    }

    public void OnPointerUp(PointerEventData pev)
    {
        GameEngine.TraceLog.Update("GameEngineBehaviour.OnPointerUp");
        PathTracker.PathMovement mov = this._path.CompoundMovement(15);
        if (mov != null)
        {
            this.CurlSpinner.ApplyDirectionalForce(mov.Direction, mov.Magnitude * 500);
        }
        this._path.Clear();
        this.IsDragging = false;
    }

    private Transform _make_debug_line
    {
        get
        {
            Transform clone = UnityEngine.Object.Instantiate(this._line);
            clone.gameObject.SetActive(true);
            clone.SetParent(this.transform);
            return clone;
        }
    }

    private void _transform_color(Transform t, Color col)
    {
        Renderer rend = t.GetComponent<Renderer>();
        Material mat = new Material(rend.material.shader);
        mat.color = col;
        rend.material = mat;
    }

    public void DebugLine(SmartVector Start, SmartVector End)
    {
        Transform line = this._make_debug_line;
        Start -= this.World.Forward;
        End -= this.World.Forward;
        this.World.Place(line, Start, End, 1, 1);
    }

    public void DebugLine(SmartVector Start, SmartVector End, Color col)
    {
        Transform line = this._make_debug_line;
        Start -= this.World.Forward;
        End -= this.World.Forward;
        this.World.Place(line, Start, End, 1, 1);
        this._transform_color(line, col);
    }

    public void DebugPoint(SmartVector Point)
    {
        Transform line = this._make_debug_line;
        Point -= this.World.Forward;
        line.position = (Vector3)Point.World;
    }

    public void DebugPoint(SmartVector Point, Color col)
    {
        Transform line = this._make_debug_line;
        Point -= this.World.Forward;
        line.position = (Vector3)Point.World;
        this._transform_color(line, col);
    }

    public void OnApplicationQuit()
    {
        if (this._trace_log != null)
        {
            this._trace_log.Close();
        }
    }

    public TraceLog TraceLog
    {
        get
        {
            if (this._trace_log == null)
            {
                this._trace_log = new TraceLog(Application.dataPath + "\\fidget_trace.txt");
                GameEngine.DebugLog("TraceLog to " + Application.dataPath + "\\fidget_trace.txt");
                this._trace_log.SpawnThread = false;
                this._trace_log.Enabled = false;
            }
            return this._trace_log;
        }

    }

    private Transform _line;
    private bool _dragging;
    private SmartVector _last_drag;
    private PathTracker _path;
    private int _debug_frame;
    private TraceLog _trace_log;
    protected static GameEngineBehaviour _instance;
}

public class GameEngine : GameEngineBehaviour
{
    public static int RecursionMaxLevel = 0;

    public static GameEngineWorld GameWorld
    {
        get
        {
            GameEngine.TraceLog.Update("GameEngineWorld.GameWorld");
            if (GameEngine._instance == null)
            {
                return null;
            }
            return GameEngine._instance.World;
        }
    }

    public static new void DebugLine(SmartVector Start, SmartVector End)
    {
        _instance.DebugLine(Start, End);
    }

    public static new void DebugLine(SmartVector Start, SmartVector End, Color col)
    {
        _instance.DebugLine(Start, End, col);
    }

    public static new void DebugPoint(SmartVector Point)
    {
        _instance.DebugPoint(Point);
    }

    public static new void DebugPoint(SmartVector Point, Color col)
    {
        _instance.DebugPoint(Point, col);
    }

    public static SmartVector ScreenRatioToWorldPosition(float x, float y)
    {
        if (GameEngine._instance == null)
        {
            return SmartVector.Zero;
        }
        return GameEngine._instance.World.ScreenRatioToWorldPosition(x, y);
    }

    public static bool Place(Transform Target, SmartVector Start, SmartVector End, float width, float height)
    {
        GameEngine.TraceLog.Update("GameEngineWorld.Place");
        return GameEngine.GameWorld.Place(Target, Start, End, width, height);
    }

    public static void DebugLog(DebugMessage Message)
    {
        Debug.Log(Message.Frame + " (" + Message.ElapsedTime + "):  " + Message.Message);
    }

    public static void DebugLog(object Message)
    {
        Debug.Log(Time.frameCount + " (" + Time.fixedTime + "):   " + Message);
    }

    public static new TraceLog TraceLog
    {
        get
        {
            return _instance.TraceLog;
        }
    }
}