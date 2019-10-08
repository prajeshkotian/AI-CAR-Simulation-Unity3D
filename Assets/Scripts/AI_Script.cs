using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_Script : MonoBehaviour
{
    public Transform pathGroup;
    public int sizeoflist;
    public float maxSteer = 30.0f;
    public float maxTorque = 140.0f;
    public float currentSpeed=0f;
    public float maxSpeed=140.0f;
    public float distancefrompath = 5.0f;
    public float decellerationSpeed = 8.0f;
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelRL;
    public WheelCollider WheelRR;
    public int currentPathObj;
    public Vector3 steerVector;
    public float dir; //test variable
    public float sensorLength = 5;
    public float frontSensorStartPoint = 5;
    public float frontSensorSideDist = 2;
    public float frontSensorsAngle = 30;
    public float sidewaySensorLength = 5;
    public float avoidSpeed = 10;

    private List<Transform> path; //we use a list so that it can have a dynamic size, meaning the size can change when we need it to

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 temp = rb.centerOfMass;
        temp.y = -0.8f;
        rb.centerOfMass = temp;
        GetPath();
    }

    void GetPath()
    {
        Transform[] path_objs = pathGroup.GetComponentsInChildren<Transform>();    //to get the path objects 1 by 1
        path = new List<Transform>();

        for (int i = 0; i < path_objs.Length; i++)
        {
            if (path_objs[i] != pathGroup)
            {
                path.Add(path_objs[i]);
            }
       
        }
        sizeoflist = path.Count;
    }

    void Update()
    {
        GetSteer();
        Move();
        Sensors();
    }

   public void GetSteer()
    {
        steerVector = transform.InverseTransformPoint(new Vector3(path[currentPathObj].position.x, transform.position.y, path[currentPathObj].position.z));
        float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
        //direction = steerVector.x / steerVector.magnitude;
        WheelFL.steerAngle = newSteer;
        WheelFR.steerAngle = newSteer;
        if (steerVector.magnitude <= distancefrompath)
        {
            currentPathObj++;
            if (currentPathObj >= sizeoflist)
                currentPathObj = 0;
        }

        
    }
    void Move()

    { 
       
        currentSpeed = 2 * (22 / 7) * WheelRL.radius * WheelRL.rpm * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);
        if (currentSpeed <= maxSpeed)
        {
            WheelRL.motorTorque = maxTorque;
            WheelRR.motorTorque = maxTorque;
            WheelRL.brakeTorque = 0;
            WheelRR.brakeTorque = 0;
        }
        else
        {
            WheelRL.motorTorque = 0;
            WheelRR.motorTorque = 0;
            WheelRL.brakeTorque =decellerationSpeed ;
            WheelRR.brakeTorque =decellerationSpeed;
        }
    }
    void ChangeSpeed()
    {
             
            dir = (steerVector.x / steerVector.magnitude) * 100;
            if (dir <= 5 && dir >= -5)
            {
                maxTorque = 140;
                maxSpeed = 140;
            }
            else
            {
                maxTorque = 70;
                maxSpeed = 80;
            }
    }
    void Sensors()
    {
        float flag = 0;
        float avoidSensitivity = 0;
        Vector3 pos;
        RaycastHit hit;
        Vector3 rightAngle = Quaternion.AngleAxis(frontSensorsAngle, transform.up) * transform.forward;
        Vector3 leftAngle = Quaternion.AngleAxis(-frontSensorsAngle, transform.up) * transform.forward;

        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;

        // Braking sensor
        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                WheelRL.brakeTorque = decellerationSpeed;
                WheelRR.brakeTorque = decellerationSpeed;
                Debug.DrawLine(pos, hit.point, Color.red);
            }
        }
        else {
            WheelRL.brakeTorque = 0;
            WheelRR.brakeTorque = 0;
        }

        //Front Straight Right Sensor  
        pos += transform.right * frontSensorSideDist;

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSensitivity -= 1f;
                Debug.Log("Avoiding");
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }
        else if (Physics.Raycast(pos, rightAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                avoidSensitivity -= 0.5f;
                flag++;
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }

        //Front Straight left Sensor  
        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;
        pos -= transform.right * frontSensorSideDist;

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSensitivity += 1f;
                Debug.Log("Avoiding");
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }
        else if (Physics.Raycast(pos, leftAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSensitivity += 0.5f;
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }

        //Right SideWay Sensor  
        if (Physics.Raycast(transform.position, transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSensitivity -= 0.5f;
                Debug.DrawLine(transform.position, hit.point, Color.white);
            }
        }

        //Left SideWay Sensor  
        if (Physics.Raycast(transform.position, -transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSensitivity += 0.5f;
                Debug.DrawLine(transform.position, hit.point, Color.white);
            }
        }

        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;

        //Front Mid Sensor  
        if (avoidSensitivity == 0)
        {

            if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
            {
                if (hit.transform.tag != "Terrain")
                {
                    if (hit.normal.x < 0)
                    {
                        avoidSensitivity = -1;
                    }
                    else {
                        avoidSensitivity = 1;
                    }
                    Debug.DrawLine(pos, hit.point, Color.white);

                }
            }
        }
        if (flag != 0)
        {
            AvoidSteer(avoidSensitivity);
        }
    }

    void AvoidSteer(float sensitivity)
    {
        WheelFL.steerAngle = avoidSpeed * sensitivity;
        WheelFR.steerAngle = avoidSpeed * sensitivity;
    }
}


