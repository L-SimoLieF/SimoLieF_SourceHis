using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A15TreeStop : MonoBehaviour
{
    GameObject Player;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        rb = Player.gameObject.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("qqqqq");
        if(other.name == "Player")
        {
            /*rb.constraints = RigidbodyConstraints.FreezePosition
                | RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ;*/
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            //Player.GetComponent<PlayerController>().enabled = false;
        }
    }
}
