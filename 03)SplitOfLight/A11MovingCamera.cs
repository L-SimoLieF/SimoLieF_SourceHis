using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A11MovingCamera : MonoBehaviour
{
    public GameObject targetObject;
    //Vector3 initiateDistance;
    public float rotateSpeed;
    float gear = 11.0f;

    Vector3 initPosition;
    GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");

        initPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            transform.position = initPosition;
            //transform.LookAt(Player.transform.position);
        }
        else
        {
            Vector3 cameraFoward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 moveFoward = cameraFoward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
            //rb.velocity = moveFoward * speed + new Vector3(0,rb.velocity.y, 0);
            //rb.velocity = moveFoward * speed + Vector3.forward;

            if (moveFoward != Vector3.zero)
            {
                //transform.rotation = Quaternion.LookRotation(moveFoward);
            }
            transform.position += moveFoward * (float)Time.deltaTime * gear;


        }

        if (Input.GetAxis("Horizontal2") != 0)
        {
            transform.RotateAround(targetObject.transform.position, new Vector3(0, 1, 0), rotateSpeed * Input.GetAxis("Horizontal2"));
            //initiateDistance = transform.position - targetObject.transform.position;
            //transform.position = targetObject.transform.position + initiateDistance;
        }

    }

    public void GetInitPosition(Vector3 pos)
    {
        initPosition = pos;
    }
}
