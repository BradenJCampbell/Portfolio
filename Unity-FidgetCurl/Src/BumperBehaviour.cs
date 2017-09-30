using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BCUnity;

public class BumperBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameEngine.TraceLog.Update("BumperBehaviour.Start");
        this._prototype.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        GameEngine.TraceLog.Update("BumperBehaviour.Update");
        //this._prototype.gameObject.SetActive(false);
    }

    public Transform Deploy(SmartVector Start, SmartVector End, float width = 1, float height = 1)
    {
        GameEngine.TraceLog.Update("BumperBehaviour.Deploy");
        Transform newBumper = Object.Instantiate(this._prototype);
        newBumper.gameObject.SetActive(true);
        GameEngine.Place(newBumper, Start, End, width, height);
        newBumper.parent = this.transform;
        return newBumper;
    }

    protected Transform _prototype
    {
        get
        {
            GameEngine.TraceLog.Update("BumperBehaviour._prototype");
            if (this._proto == null)
            {
                this._proto = this.transform.FindChild("Prototype");
            }
            return this._proto;
        }
    }

    private Transform _proto;
}
