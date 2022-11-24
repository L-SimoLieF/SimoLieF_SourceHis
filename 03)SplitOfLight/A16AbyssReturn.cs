using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A16AbyssReturn : MonoBehaviour
{
    GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.transform.position.y < -20)
        {
            Player.transform.position = new Vector3(Player.transform.position.x, 30, Player.transform.position.z);
            Player.GetComponent<BoxCollider>().isTrigger = false;
        }
    }
}
