using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A08WallThrough : MonoBehaviour
{
    public bool throughFlag = false;
    [SerializeField] LayerMask layer;
    [SerializeField] private int searchArea;

    A10ClimbCheck A10CC;

    // Start is called before the first frame update
    void Start()
    {
        A10CC = GameObject.Find("FCCheck").GetComponent<A10ClimbCheck>();
    }

    // Update is called once per frame
    void Update()
    {

        //Physics.overlapshare
        //Playerの座標から前方方向に対して足場が存在するか否か。
        //か、壁に対して、2つ以上の足場が接しているか。
        //接していた場合、throughFlagをtrueにする。
        //flagがfalseの場合、透過処理を行わない。

        if (Physics.OverlapSphere(this.gameObject.transform.position - new Vector3(0, 1, 0), searchArea, layer).Length > 1)
        {
            throughFlag = true;
        }

        //Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, searchArea, layer);

    }



    //足場の壁の透過処理
    //壁に触れた際、throughFlagの有無で透過処理を行うか決定する。

    //透過処理
    //Collisionで判定チェック、Triggerをonにして、Triggerを抜け次第Triggerを戻す事で透過処理を実現してます。
    //この時、消すのは接した壁の当たり判定のみです。
    //理論上、足場に乗っていなければ壁は存在しない(はず)なので、MushJumpでも乗れる(はず)。
    private void OnCollisionEnter(Collision col)
    {



        if (throughFlag)
        {
            if (col.gameObject.name == "Player")
            {
                this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                /*col.gameObject.GetComponent<Rigidbody>().useGravity = false;
                col.collider.isTrigger = true;*/

            }
        }

        if (col.gameObject.tag == "flower")
        {
            throughFlag = true;
        }
    }


    private void OnTriggerExit(Collider col)
    {
        if (col.name == "Player")
        {
            this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            /*col.isTrigger = false;
            col.gameObject.GetComponent<Rigidbody>().useGravity = true;*/
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            //A10CC.floatFlag = true;
        }
    }
}

//壁透過処理の分岐条件について
//横に位置する花が再生済みか否かの検知が必要。
//Colliderの有無、もしくはWallのActiveを検知。

