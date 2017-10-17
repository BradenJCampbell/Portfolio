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

public struct BumperSetup
{
    public BumperType Type;
    public SmartVector Start;
    public SmartVector End;
    public float width;
    public float height;
    public bool visible;
    public int ScoreMultiplier;

    public Bumper Deploy()
    {
        return GameEngine.Bumpers.Deploy(this.Type, this.Start, this.End, this.width, this.height, this.visible, this.ScoreMultiplier);
    }
}

public class Bumper
{
    public int ScoreValue;

    public Bumper(Transform Prototype, SmartVector Start, SmartVector End, float width, float height)
    {
        this._transform = Object.Instantiate(Prototype);
        this._transform.gameObject.SetActive(true);
        GameEngine.Place(this._transform, Start, End, width, height);
        this._transform.parent = GameObject.FindObjectOfType<BumperManagerBehaviour>().transform;
    }

    public bool Visible
    {
        get
        {
            return this._transform.GetComponent<Renderer>().enabled;
        }
        set
        {
            this._transform.GetComponent<Renderer>().enabled = value;
        }
    }

    public Transform Transform
    {
        get
        {
            return this._transform;
        }
    }

    public int ID
    {
        get
        {
            return this._transform.GetInstanceID();
        }
    }

    private Transform _transform;
}

public class BumperManagerBehaviour : MonoBehaviour {

    public BumperSetup Template;


	// Use this for initialization
	void Start () {
        GameEngine.Debug.TraceLog.Update("BumperBehaviour.Start");
        this.Template = new BumperSetup();
        this.Template.width = 1;
        this.Template.height = 1;
        this.Template.visible = false;
        this.get_prototype(BumperType.Cube);
        this._instances = new Dictionary<int, Bumper>();
    }

    // Update is called once per frame
    void Update () {
        GameEngine.Debug.TraceLog.Update("BumperBehaviour.Update");
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

    private Dictionary<BumperType, Transform> _protos;
    private Dictionary<int, Bumper> _instances;
}
