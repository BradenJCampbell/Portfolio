using UnityEngine;
using System.Collections.Generic;

public class CarBehaviour : MonoBehaviour
{
    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have

    public void Start()
    {
        this.GetComponent<Rigidbody>().centerOfMass = this.transform.localScale.z * Vector3.forward / 2;
    }
    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.ApplySteering(steering);
            axleInfo.ApplyTorque(motor);
            //axleInfo.ApplyStabilizedTorque(this.GetComponent<Rigidbody>(), motor);
        }

        
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
    public void ApplyTorque(float torque)
    {
        if (this.motor)
        {
            this.leftWheel.motorTorque = torque;
            this.rightWheel.motorTorque = torque;
        }
    }
    public void ApplyStabilizedTorque(Rigidbody car_body, float torque, float AntiRoll = (float)5000)
    {
        this.ApplyTorque(torque);
        if (this.motor)
        {
            //  taken from http://forum.unity3d.com/threads/how-to-make-a-physically-real-stable-car-with-wheelcolliders.50643/
            float travelL = 1;
            float travelR = 1;

            float leftAntiRollForce = (this.RightTravel - this.LeftTravel) * AntiRoll;
            float rightAntiRollForce = (this.LeftTravel - this.RightTravel) * AntiRoll;
            if (this.LeftGrounded)
            {
                car_body.AddForceAtPosition(this.leftWheel.transform.up * leftAntiRollForce, this.leftWheel.transform.localPosition);
            }
            if (this.RightGrounded)
            {
                car_body.AddForceAtPosition(this.rightWheel.transform.up * rightAntiRollForce, this.rightWheel.transform.localPosition);
            }
        }
    }
    public bool LeftGrounded
    {
        get
        {
            WheelHit hit;
            return this.leftWheel.GetGroundHit(out hit);
        }
    }
    public float LeftTravel
    {
        get
        {
            WheelHit hit;
            if (this.leftWheel.GetGroundHit(out hit))
            {
                return (-this.leftWheel.transform.InverseTransformPoint(hit.point).y - this.leftWheel.radius) / this.leftWheel.suspensionDistance;
            }
            return 1;
        }
    }
    public bool RightGrounded
    {
        get
        {
            WheelHit hit;
            return this.rightWheel.GetGroundHit(out hit);
        }
    }
    public float RightTravel
    {
        get
        {
            WheelHit hit;
            if (this.rightWheel.GetGroundHit(out hit))
            {
                return (-this.rightWheel.transform.InverseTransformPoint(hit.point).y - this.rightWheel.radius) / this.rightWheel.suspensionDistance;
            }
            return 1;
        }
    }
    public void ApplySteering(float steering)
    {
        if (this.steering)
        {
            this.leftWheel.transform.Rotate(Vector3.up, steering - this.leftWheel.steerAngle);
            this.rightWheel.transform.Rotate(Vector3.up, steering - this.rightWheel.steerAngle);
            this.leftWheel.steerAngle = steering;
            this.rightWheel.steerAngle = steering;
        }
    }
}