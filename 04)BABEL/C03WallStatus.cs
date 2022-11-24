using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Defenderが生成するWall用のスクリプト。
//壁に当たった際のスコア加算と、Defenderのクラスター爆弾で壊されないようにする処理などを記載。

public class C03WallStatus : NetworkBehaviour
{
    int Hitpoint = 1;
    [SyncVar] GameObject bombOwner;
    bool collisionFlag = false;

    //public GameObject spawner;
    //objspawner Objsp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Hitpoint < 1)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        //ブロックを壁で支えた際の加点。
        //collisionFlagは壁一つに付き、加点を一回にするためのもの。
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Piller")
        {
            if (collisionFlag == false)
            {
                bombOwner.GetComponent<P04ItemHolder>().Score += (collision.gameObject.GetComponent<C11CubeState>().GetScore()) / 2;
                collisionFlag = true;
            }
        }
        if(collision.gameObject.tag == "clusterSplinter")
        {
            //Defenderの除外処理 Bomberのクラスターでのみ破壊される。
            if (collision.gameObject.GetComponent<B01BombStatus>().bombOwner.GetComponent<DefenderController>().enabled == false)
            {
                //spawner.GetComponent<objspawner>().objcount -= 1;
                NetworkServer.Destroy(this.gameObject);
            }
        }
        if(collision.gameObject.tag == "splinter")
        {
            //spawner.GetComponent<objspawner>().objcount -= 1;
            NetworkServer.Destroy(this.gameObject);
        }
        if(collision.gameObject.tag == "splinter2")
        {
            //spawner.GetComponent<objspawner>().objcount -= 1;
            NetworkServer.Destroy(this.gameObject);
        }

    }

    public void Constructer(GameObject player)
    {
        bombOwner = player;
    }
}
