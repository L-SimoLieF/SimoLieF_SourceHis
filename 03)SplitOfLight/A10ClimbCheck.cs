using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A10ClimbCheck : MonoBehaviour
{
    public bool floatFlag = false;
    GameObject Player;
    float playerPosY;

   
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        playerPosY = Player.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
