using BCUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class GameEngineBehaviour : MonoBehaviour
{
    public class EngineDebug
    {
        private struct debug_item
        {
            public debug_item(Transform proto, Transform parent)
            {
                this.transform = BCUnity.TransformPool.Clone(proto);
                this.transform.gameObject.SetActive(true);
                this.transform.SetParent(parent);
                this.transform.localScale = new Vector3(1, 1, 1);
                this.transform.rotation = Quaternion.identity;
                this.expires = float.PositiveInfinity;
            }

            public debug_item(Transform proto, Transform parent, SmartVector start, SmartVector end) : this(proto, parent)
            {
                GameEngine.GameWorld.Place(this.transform, start, end, 1, 1);
            }

            public void Remove()
            {
                BCUnity.TransformPool.Destroy(this.transform);
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
            this._renders = new Dictionary<float, List<debug_item>>();
            this._unused = new Stack<debug_item>();
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
            this._add(new_item);
        }

        public void Line(SmartVector Start, SmartVector End, Color col)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform, Start + GameEngine.GameWorld.Forward, End + GameEngine.GameWorld.Forward);
            new_item.Color = col;
            this._add(new_item);
        }

        public void Line(SmartVector Start, SmartVector End, float lifetime)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform, Start + GameEngine.GameWorld.Forward, End + GameEngine.GameWorld.Forward);
            new_item.expires = UnityEngine.Time.fixedTime + lifetime;
            this._add(new_item);
        }

        public void Line(SmartVector Start, SmartVector End, Color col, float lifetime)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform, Start + GameEngine.GameWorld.Forward, End + GameEngine.GameWorld.Forward);
            new_item.Color = col;
            new_item.expires = UnityEngine.Time.fixedTime + lifetime;
            this._add(new_item);
        }

        public void Point(SmartVector Point)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform);
            new_item.transform.position = (Vector3)(Point + GameEngine.GameWorld.Forward);
            this._add(new_item);
        }

        public void Point(SmartVector Point, Color col)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform);
            new_item.transform.position = (Vector3)(Point + GameEngine.GameWorld.Forward);
            new_item.Color = col;
            this._add(new_item);
        }

        public void Point(SmartVector Point, Color col, float lifetime)
        {
            debug_item new_item = new debug_item(this._line, this._ins.transform);
            new_item.transform.position = (Vector3)(Point + GameEngine.GameWorld.Forward);
            new_item.Color = col;
            new_item.expires = UnityEngine.Time.fixedTime + lifetime;
            this._add(new_item);
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
            float[] keys = new float[this._renders.Keys.Count];
            this._renders.Keys.CopyTo(keys, 0);
            foreach (float t in keys)
            {
                if (t <= Time.fixedTime)
                {
                    for (int i = 0; i < this._renders[t].Count; i++)
                    {
                        BCUnity.TransformPool.Destroy(this._renders[t][i].transform);
                    }
                    this._renders[t].Clear();
                    this._renders.Remove(t);
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

        private void _add(debug_item new_item)
        {
            if (!this._renders.ContainsKey(new_item.expires))
            {
                this._renders.Add(new_item.expires, new List<debug_item>());
            }
            this._renders[new_item.expires].Add(new_item);
        }

        private GameEngineBehaviour _ins;
        private TraceLog _log;
        private Dictionary<float, List<debug_item>> _renders;
        private Stack<debug_item> _unused;
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