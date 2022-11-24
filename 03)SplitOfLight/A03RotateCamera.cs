using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A03RotateCamera
//基本的なカメラワーク。
//Horizontalでカメラを回してるだけ。
public class A03RotateCamera : MonoBehaviour
{
    public GameObject targetObject;
    Vector3 initiateDistance;
    public float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        initiateDistance = transform.position - targetObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetObject.transform.position + initiateDistance;

        if (Input.GetAxis("Horizontal2") != 0)
        {
            transform.RotateAround(targetObject.transform.position, new Vector3(0, 1, 0), rotateSpeed * Input.GetAxis("Horizontal2"));
            initiateDistance = transform.position - targetObject.transform.position;
            transform.position = targetObject.transform.position + initiateDistance;
        }

    }
}
