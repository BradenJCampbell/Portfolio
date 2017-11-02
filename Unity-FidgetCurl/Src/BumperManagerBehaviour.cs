using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BCUnity;

public enum BumperType
{
    Cube,
    Sphere,
    Capsule
}

public class BumperSetup
{
    public BumperType Type;
    public SmartVector Start;
    public SmartVector End;
    public float width;
    public float height;
    public bool visible;
    public int Score;
    public Color Colour;

    public Bumper Deploy()
    {
        return this.Deploy(this.Start, this.End);
    }

    public Bumper Deploy(SmartVector Start, SmartVector End)
    {
        return this.Deploy(Start, End, this.Colour);
    }

    public Bumper Deploy(SmartVector Start, SmartVector End, Color Colour)
    {
        Bumper ret = GameEngine.Bumpers.Deploy(this.Type, Start, End, this.width, this.height, this.visible, this.Score);
        ret.Color = Colour;
        return ret;
    }
}

public class Bumper
{
    public float ScoreValue;

    public Bumper(Transform Prototype, SmartVector Start, SmartVector End, float width, float height)
    {
        if (Prototype == null)
        {
            throw new System.Exception("no prototype given");
        }
        this._manip = new BCUnity.TransformManipulator(BCUnity.TransformPool.Clone(Prototype));
        if (this.Transform == null)
        {
            throw new System.Exception("unable to clone");
        }
        this._manip.Active = true;
        this._manip.Place(Start, End, width, height);
        this._manip.Parent = Bumper._canvas;
    }

    public Color Color
    {
        get
        {
            return this._manip.Color;
        }
        set
        {
            this._manip.Color = value;
        }
    }

    public bool Visible
    {
        get
        {
            return this._manip.Visible;
        }
        set
        {
            this._manip.Visible = value;
        }
    }

    public Transform Transform
    {
        get
        {
            return this._manip.Transform;
        }
    }

    public int ID
    {
        get
        {
            return this._manip.ID;
        }
    }

    public void Destroy()
    {
        this._manip.Remove();
    }

    private TransformManipulator _manip;

    private static Transform _canvas
    {
        get
        {
            return GameEngine.Bumpers.transform;
        }
    }
}

public class BumperManagerBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameEngine.Debug.TraceLog.Update("BumperBehaviour.Start");
        this.get_prototype(BumperType.Cube);
    }

    public Bumper Cube(SmartVector Start, SmartVector End, float width = 1, float height = 1, bool visible = false, int score_value = 0)
    {
        return this.Deploy(BumperType.Cube, Start, End, width, height, visible, score_value);
    }

    public Bumper Sphere(SmartVector Start, SmartVector End, float width = 1, float height = 1, bool visible = false, int score_value = 0)
    {
        return this.Deploy(BumperType.Sphere, Start, End, width, height, visible, score_value);
    }

    public Bumper Capsule(SmartVector Start, SmartVector End, float width = 1, float height = 1, bool visible = false, int score_value = 0)
    {
        return this.Deploy(BumperType.Capsule, Start, End, width, height, visible, score_value);
    }

    public Bumper Deploy(BumperType Type, SmartVector Start, SmartVector End, float width = 1, float height = 1, bool visible = false, int score_value = 0)
    {
        GameEngine.Debug.TraceLog.Update("BumperBehaviour.Deploy");
        if (Type == BumperType.Capsule)
        {
            SmartVector old_start = Start;
            SmartVector diff = End - Start;
            Start = old_start + (float)0.25 * diff;
            End = old_start + (float)0.75 * diff;
        }
        Bumper new_bumper = new Bumper(this.get_prototype(Type), Start, End, width, height);
        new_bumper.Visible = visible;
        new_bumper.ScoreValue = score_value;
        this._instances.Add(new_bumper.ID, new_bumper);
        return new_bumper;
    }

    public Bumper Lookup(int ID)
    {
        if (this._instances.ContainsKey(ID))
        {
            return this._instances[ID];
        }
        return null;
    }

    public Bumper Lookup(Transform t)
    {
        return this.Lookup(t.GetInstanceID());
    }

    public void Clear()
    {
        foreach (int i in this.IDs)
        {
            this._instances[i].Destroy();
        }
        this._instances.Clear();
    }

    protected Transform get_prototype(BumperType type)
    {
        if (this._protos == null)
        {
            this._protos = new Dictionary<BumperType, Transform>();
            this._protos.Add(BumperType.Cube, this.transform.FindChild("Cube_Proto"));
            this._protos.Add(BumperType.Sphere, this.transform.FindChild("Sphere_Proto"));
            this._protos.Add(BumperType.Capsule, this.transform.FindChild("Capsule_Proto"));
            foreach (BumperType t in this._protos.Keys)
            {
                this._protos[t].gameObject.SetActive(false);
            }
        }
        return this._protos[type];
    }

    public int[] IDs
    {
        get
        {
            int[] ret = new int[this._instances.Keys.Count];
            this._instances.Keys.CopyTo(ret, 0);
            return ret;
        }
    }

    public BumperSetup Template
    {
        get
        {
            if (this._template == null)
            {
                this._template = new BumperSetup();
                this._template.width = 1;
                this._template.height = 1;
                this._template.visible = false;
            }
            return this._template;
        }
    }

    private Dictionary<int, Bumper> _instances
    {
        get
        {
            if (this.__instances == null)
            {
                this.__instances = new Dictionary<int, Bumper>();
            }
            return this.__instances;
        }
    }

    private Dictionary<BumperType, Transform> _protos;
    private Dictionary<int, Bumper> __instances;
    private BumperSetup _template;
}
