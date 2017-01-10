using UnityEngine;
using System.Collections;

public class SunBehaviour : MonoBehaviour {
    public float DayLength;
    private float _anglepersecond;
	// Use this for initialization
	void Start () {
        this._anglepersecond = 180 / this.DayLength;
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(Vector3.up, this._anglepersecond * Time.deltaTime);
	}
}
