using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A07FlowerClimb : MonoBehaviour
{
    Collider stepCol;       //花上の足場
    GameObject wallObj;     //花上の壁。wall01-04まであるが、纏めた空オブジェクトで管理。
    GameObject Player;

    A10ClimbCheck A10CC;　//壁管理の為のスクリプト。「プレイヤーが足場に乗っているか」を管理する為だけに作った。

    // Start is called before the first frame update
    void Start()
    {

        //足場と壁の取得。inspecter上での指定はしないようにした。

        stepCol = gameObject.GetComponent<BoxCollider>();
        wallObj = transform.GetChild(0).gameObject;
        Player = GameObject.Find("Player");

        A10CC = GameObject.Find("FCCheck").GetComponent<A10ClimbCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        //花の上から降りる為の処理
        //条件はPlayerのisTrigger と 壁のActive(=プレイヤーが花の上に乗っているかどうか)

        /*if (Player.gameObject.GetComponent<BoxCollider>().isTrigger == false && Input.GetKeyDown("joystick button 1"))
        {
            if (A10CC.floatFlag == true)
            {
                //stepCol.isTrigger = !stepCol.isTrigger;
                Player.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                //wallObj.SetActive(false);
            }
        }*/

        //壁の生成処理。足場に乗っている時だけ、壁を作る。

        if (A10CC.floatFlag == false)
            wallObj.SetActive(false);
        else
            wallObj.SetActive(true);

        if (/*Player.gameObject.GetComponent<BoxCollider>().isTrigger == false && */Input.GetKeyDown("joystick button 1"))
        {
            //Debug.Log("aaaa");
            if (A10CC.floatFlag == true)
            {
                //水野サウンド用
                sound.JumpEndSE = true;
                if (Player.transform.position.y > transform.position.y)
                {
                    //stepCol.isTrigger = !stepCol.isTrigger;
                    Player.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                    //wallObj.SetActive(false);
                    Player.GetComponent<PlayerController>().enabled = false;
                }
                else
                {
                    A10CC.floatFlag = false;
                }
            }


        }
    }


    //足場に乗っている、という状況を取得する為だけにCollisionStayを使ってます。

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Player.GetComponent<PlayerController>().enabled = true;

            if (Player.gameObject.GetComponent<PlayerController>().ActionF == false)
            {
                A10CC.floatFlag = true;
                //wallObj.SetActive(true);
                Player.transform.GetChild(4).GetComponent<Animator>().SetBool("Jump", false);
            }


        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            A10CC.floatFlag = false;
            //wallObj.SetActive(true);
            //Player.transform.GetChild(4).GetComponent<Animator>().SetBool("Jump", false);
        }
    }

    void OnTriggerExit(Collider col)
    {

        //足場の透過処理
        //条件は「足場を過ぎ去った時」(ジャンプした時、また足場を下りた時)

        if (col.name == "Player")
        {
            Player.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            //Debug.Log(col.name);
            //stepCol.isTrigger = !stepCol.isTrigger;

            //wallObjはアクションキーの入力に関与します。
            //wallObj.SetActive(!wallObj.activeSelf);
            //A10CC.floatFlag = !A10CC.floatFlag;
            A10CC.floatFlag = false;

            if (Player.transform.position.y < transform.position.y)
            {

                Player.GetComponent<PlayerController>().enabled = true;
            }


        }
    }
}