using UnityEngine;
using System.Collections;

public class RobotArm : MonoBehaviour {

    

    private Rigidbody arm;

    public SteamVR_TrackedObject head;
    public WandController controller;
    public GameObject robot;
    
    private Vector3 posTarget;
    private Vector3 posDelta;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;
    private float rotationFactor = 400;
    private float velocityFactor = 2000f;

    private float distanceFactor = 1;
    

    // Use this for initialization
    void Start () {
        controller.attachArm(this);
        arm = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (controller != null)
        {
            posTarget = controller.transform.position + (distanceFactor * (controller.transform.position - head.transform.position));

            posDelta = posTarget - arm.transform.position;
            arm.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = controller.transform.rotation * Quaternion.Inverse(arm.transform.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }

            arm.angularVelocity = (Time.deltaTime * angle * axis) * rotationFactor;
        }
    }

    
}
