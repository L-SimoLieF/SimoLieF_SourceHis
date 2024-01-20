using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W06SpeedController : MonoBehaviour
{
    W01WhaleMoving W01;

    // Start is called before the first frame update
    void Start()
    {
        W01 = transform.root.gameObject.GetComponent<W01WhaleMoving>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            W01.speed = 15.0f;
            W01.normalSet = true;
            this.gameObject.SetActive(false);
        }
    }
}
