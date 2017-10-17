using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class GameEngineBehaviour : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public class EngineDebug
    {
        private struct debug_item
        {
            public debug_item(Transform proto, Transform parent)
            {
                this.transform = UnityEngine.Object.Instantiate(proto);
                this.transform.gameObject.SetActive(true);
                this.transform.SetParent(parent);
                this.expires = 0;
            }

            public debug_item(Transform proto, Transform parent, SmartVector start, SmartVector end) : this(proto, parent)
            {
                GameEngine.GameWorld.Place(this.transform, start, end, 1, 1);
            }

            public void Remove()
            {
                UnityEngine.Object.Destroy(this.transform.gameObject);
            }

            public Color Color
            {
                set
                {
                    Renderer rend = this.transform.GetComponent<Renderer>();
                    Material mat = new Material(rend.material.shader);
                    mat.color = value;
                    rend.material = mat;
                }
                get
                {
                    return this.transform.GetComponent<Renderer>().material.color;
                }
            }

            public bool Expired
            {
                get
                {
                    return this.expires > 0 && UnityEngine.Time.fixedTime > this.expires;
                }
            }

            public Transform transform;
            public float expires;
        }

        public bool Enabled = false;

        public EngineDebug(GameEngineBehaviour instance, Transform line)
        {
            this._ins = instance;
            this._renders = new List<debug_item>();
            this._log = new TraceLog(Application.dataPath + "\\fidget_trace.txt");
            this._log.SpawnThread = false;
            this._log.Enabled = false;
            this._line = line;
            this._line.gameObject.SetActive(false);
        }

        public TraceLog TraceLog
        {
            get
            {
                return this._log;
            }
        }

        public void Line(SmartVector Start, SmartVector End)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform, Start + GameEngine.GameWorld.Forward, End + GameEngine.GameWorld.Forward);
            this._renders.Add(new_item);
        }

        public void Line(SmartVector Start, SmartVector End, Color col)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform, Start + GameEngine.GameWorld.Forward, End + GameEngine.GameWorld.Forward);
            new_item.Color = col;
            this._renders.Add(new_item);
        }

        public void Line(SmartVector Start, SmartVector End, float lifetime)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform, Start + GameEngine.GameWorld.Forward, End + GameEngine.GameWorld.Forward);
            new_item.expires = UnityEngine.Time.fixedTime + lifetime;
            this._renders.Add(new_item);
        }

        public void Line(SmartVector Start, SmartVector End, Color col, float lifetime)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform, Start + GameEngine.GameWorld.Forward, End + GameEngine.GameWorld.Forward);
            new_item.Color = col;
            new_item.expires = UnityEngine.Time.fixedTime + lifetime;
            this._renders.Add(new_item);
        }

        public void Point(SmartVector Point)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform);
            new_item.transform.position = (Vector3)(Point + GameEngine.GameWorld.Forward);
            this._renders.Add(new_item);
        }

        public void Point(SmartVector Point, Color col)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform);
            new_item.transform.position = (Vector3)(Point + GameEngine.GameWorld.Forward);
            new_item.Color = col;
            this._renders.Add(new_item);
        }

        public void Log(DebugMessage Message)
        {
            UnityEngine.Debug.Log(Message.Frame + " (" + Message.ElapsedTime + "):  " + Message.Message);
        }

        public void Log(object Message)
        {
            UnityEngine.Debug.Log(Time.frameCount + " (" + Time.fixedTime + "):   " + Message);
        }

        public void Update()
        {
            for (int i = 0; i < this._renders.Count;)
            {
                if (this._renders[i].Expired)
                {
                    UnityEngine.Object.Destroy(this._renders[i].transform.gameObject);
                    this._renders.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void Close()
        {
            if (this._log != null)
            {
                this._log.Close();
            }
        }

        private GameEngineBehaviour _ins;
        private TraceLog _log;
        private List<debug_item> _renders;
        private Transform _line;
    }

    public EngineDebug Debug
    {
        get
        {
            if (_debug == null)
            {
                _debug = new EngineDebug(this, this.transform.FindChild("DebugLine"));
            }
            return _debug;
        }
    }

    private static EngineDebug _debug;
}