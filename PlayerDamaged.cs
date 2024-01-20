using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamaged : MonoBehaviour
{
    public int HP = 1;//HP
    public bool invFlag;//多段ヒット防止
    public float timer = 0;
    public GameObject[] smokeObject = new GameObject[4];//煙パーティクル
    int smokecount = 0;

    //continue用
    public GameObject aaa;


    //ダメージエフェクト用 アタッチしろ
    public Material fadeMat;
    bool damageFlag;

    

    //墜落時に重力、減速させる為にXRrig。
    public GameObject XRrig;

    // Start is called before the first frame update
    void Start()

    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //一定時間の無敵処理
        if(invFlag == true)
        {
            timer += Time.deltaTime;

            if (timer < 0.35f)
            {
                fadeMat.color = new Color(50, 0, 0, timer * 2.0f);
                //damageFlag = true;
            }
            else
            {
                fadeMat.color = new Color(0, 0, 0, 0);
            }
        }
        if(timer > 3f)
        {
            invFlag = false;
            timer = 0;
          
        }

        //gameover
        //速度を0、重力を追加。
        if(HP <= 0)
        {
            //水野Se追加
            SeSystem.GameOver = true;
            XRrig.GetComponent<FollowLine>().speed = 0f;
            

            //床抜け防止
            if(this.transform.position.y < 20)
            {
                /*GetComponent<Rigidbody>().useGravity = false;
                GetComponent<UIGameOver>().a.useGravity = false;//a = mainCamera。カメラを墜落させないと視点が堕ちない為。
                GetComponent<UIGameOver>().flag = true;*/

            }
            else
            {
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<UIGameOver>().a.useGravity = true;//a = mainCamera。カメラを墜落させないと視点が堕ちない為。
                GetComponent<UIGameOver>().flag = true;
            }



            for (int i = 0; i < 4; i++)
            {
                smokeObject[i].SetActive(false);
                
            }
            smokecount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //普通に接触判定
        if (invFlag == false)
        {
            if (other.tag == "FishMissile")
            {
                //11/24 SE用の追加(プレイヤーの被弾)
                SeSystem.HitDamage = true;

                HP -= 1;
                invFlag = true;
                Debug.Log("Hit");
                if (smokecount < 4)
                {
                    smokeObject[smokecount].SetActive(true);
                    smokecount++;
                }
            }
        }
    }
}
