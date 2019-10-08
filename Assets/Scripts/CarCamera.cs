using UnityEngine;
using System.Collections;

public class CarCamera : MonoBehaviour
{
    public Transform car;
    public float Height = 1.6f;
    public float distance = 4.0f;
    public float RotationDamping = 2.0f;
    public float HeightDamping = 3.0f;
    public float ZoomRatio = 0.5f;
    Vector3 RotationVector;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
       float WantedAngle = car.eulerAngles.y;
        float WantedHeight = car.position.y+Height;
        float MyAngle = transform.eulerAngles.y;
        float MyHeight = transform.position.y;
        MyAngle = Mathf.LerpAngle(MyAngle, WantedAngle, RotationDamping * Time.deltaTime);
        MyHeight = Mathf.Lerp(MyHeight, WantedHeight, HeightDamping * Time.deltaTime);
        Quaternion Rotation = Quaternion.Euler(0, MyAngle,0);
        transform.position = car.transform.position;
        transform.position -= Rotation * Vector3.forward * distance;
        transform.position =new Vector3(transform.position.x, MyHeight, transform.position.z);
        transform.LookAt(car.transform);




	
	}
}
