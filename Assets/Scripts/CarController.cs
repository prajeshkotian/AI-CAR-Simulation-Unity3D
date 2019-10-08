using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {

    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelBL;
    public WheelCollider WheelBR;
    public float MaxTorque = 100f;
    public float currentSpeed = 0f;
	// Use this for initialization
	void Start () {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 temp = rb.centerOfMass;
        temp.y = -1.8f;
        rb.centerOfMass = temp;

	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        WheelBR.motorTorque = MaxTorque * Input.GetAxis("Vertical")*30*Time.deltaTime;
        WheelBL.motorTorque = MaxTorque * Input.GetAxis("Vertical")*30;
        WheelFR.steerAngle = 30 * Input.GetAxis("Horizontal");
        WheelFL.steerAngle = 30 * Input.GetAxis("Horizontal");
        currentSpeed = 2 * (22 / 7) * WheelBL.radius * WheelBL.rpm * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);
    }
}
