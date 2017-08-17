using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurlSpinDirection
{
    Clockwise = 1,
    CounterClockwise = -1,
    Decelerate = 0
}

public class CurlSpinnerBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this._last_rotation = 0;
        this._spin_speed = 0;
        this._spin_accel = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (this._spin_dir == CurlSpinDirection.Decelerate)
        {
            this._spin_speed = Mathf.Sign(this._spin_speed) * Mathf.Clamp(Mathf.Abs(this._spin_speed) - Time.deltaTime * this._spin_accel, 0, 900);
        }
        else
        {
            this._spin_speed = Mathf.Clamp(this._spin_speed + (float)this._spin_dir * Time.deltaTime * this._spin_accel, -900, 900);
        }

        this.transform.Rotate(-Vector3.forward, Time.deltaTime * this._spin_speed);
    }

    public void Spin(CurlSpinDirection direction)
    {
        this._spin_dir = direction;
        this._spin_accel = 360;
    }

    private float _last_rotation;
    private float _spin_speed;
    private float _spin_accel;
    private CurlSpinDirection _spin_dir;
}
