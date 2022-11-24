using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//------------粘着爆弾の投出用のスクリプト。
//担当したのは、投射時に発射した人物の情報を伝達する部分です。
//Constructer関数を、各PlayerスクリプトでCallする形で使用しています。

public class adhesionThrow : NetworkBehaviour
{
    //爆弾エフェクト取得
    [SerializeField] GameObject BombEffect;

    //爆弾関連数値ーーーーーーーーーーーー

    //forwardPower前方向数値
    public float forwardPower = 5.0f,
    //upPower上方向数値
  　              upPower = 0.0f,
    //爆弾静止後の爆発待機時間
                  bombTimer = 3.0f,
    //全体威力調整
                  power = 100f,
    //爆発後削除時間
                  destroyTime = 1.0f;
    
    //ーーーーーーーーーーーーーーーーーーーー

    //爆弾数値保管用
    float Fpower, Upower;

    //プレイヤー取得用
    [SyncVar] GameObject playerObj;
    Vector3 keepPlayer;
    Rigidbody rg;
    float set = 0;
    float resetTime=3.0f;
    bool hit = false;

    float hittimer = 0.1f;
    
    C02ItemManager ItemManager;

    bool count=true;
    bool flag = false;

    void Start()
    {

        

        /*rg = this.GetComponent<Rigidbody>();
        playerObj = GameObject.Find("Player");
        keepPlayer = this.transform.position;

        Fpower = forwardPower;
        Upower = upPower;

        //爆弾投下数値環境合わせるための処理
        forwardPower = Fpower * power * Time.deltaTime;
        upPower = Upower * power * Time.deltaTime;

        rg.AddForce(transform.forward * forwardPower, ForceMode.Impulse);
        rg.AddForce(transform.up * upPower, ForceMode.Impulse);*/
    }

    void Update()
    {
        

        if(hit)
        {
            hittimer -= Time.deltaTime;
            rg.velocity = Vector3.zero;

            if(hittimer < 0)
            {
                rg.isKinematic = true;
            }
        }

        bombTimer -= Time.deltaTime;

        //rg.velocity = Vector3.zero;
        //rg.angularVelocity = Vector3.zero;
        if (bombTimer <= 0 && set == 0)
        {
            //発射SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);
            Instantiate(BombEffect, this.transform.position, this.transform.rotation);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(4).gameObject.SetActive(true);
            transform.GetChild(5).gameObject.SetActive(true);
            transform.GetChild(6).gameObject.SetActive(true);
            set++;
        }
        if (set == 1)
        {
            playerObj.GetComponent<P04ItemHolder>().bombCount--;
            set++;
        }
        if (bombTimer <= -1.0f * destroyTime)
        {
            //playerObj.GetComponent<P04ItemHolder>().bombCount--;
            NetworkServer.Destroy(this.gameObject);
        }

        //if (resetTime < 0 && set == 0)
        //{
        //    Instantiate(BombEffect, this.transform.position, this.transform.rotation);
        //    transform.GetChild(0).gameObject.SetActive(false);
        //    transform.GetChild(1).gameObject.SetActive(true);
        //    transform.GetChild(2).gameObject.SetActive(true);
        //    transform.GetChild(3).gameObject.SetActive(true);
        //    transform.GetChild(4).gameObject.SetActive(true);
        //    transform.GetChild(5).gameObject.SetActive(true);
        //    transform.GetChild(6).gameObject.SetActive(true);
        //    set++;
        //    flag = true;

        //    //playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //    //count = false;

        //    //NetworkServer.Destroy(this.gameObject);

        //}

        //if (hit)
        //{
        //    //this.transform.position += 0.05f * transform.forward;
        //    bombTimer -= Time.deltaTime;
        //    rg.velocity= Vector3.zero;
        //    if(bombTimer<2.9f)
        //        rg.isKinematic = true;
        //    if (bombTimer <= 0&&set==0)
        //    {
        //        Instantiate(BombEffect, this.transform.position, this.transform.rotation);
        //        transform.GetChild(0).gameObject.SetActive(false);
        //        transform.GetChild(1).gameObject.SetActive(true);
        //        transform.GetChild(2).gameObject.SetActive(true);
        //        transform.GetChild(3).gameObject.SetActive(true);
        //        transform.GetChild(4).gameObject.SetActive(true);
        //        transform.GetChild(5).gameObject.SetActive(true);
        //        transform.GetChild(6).gameObject.SetActive(true);
        //        set++;
        //        flag = true;
               
        //    }

        //    if(flag && count)
        //    {
        //        playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //        count = false;

        //    }

        //if (resetTime <= -1.0f * destroyTime)
        //{
        //    //playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //    NetworkServer.Destroy(this.gameObject);
        //}

        //if (bombTimer <= -1.0f * destroyTime && playerObj.GetComponent<P04ItemHolder>().bombCount == 0)
        //{
        //    //playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //    NetworkServer.Destroy(this.gameObject);
        //}

        //}


        /*if(count == true)
        {
            playerObj.GetComponent<P04ItemHolder>().bombCount--;
            count = false;
        }*/


    }
    private void OnCollisionEnter(Collision collision)
    {
        //プレイヤーに当たって止まるのを防ぐため
        if (collision.gameObject.tag != "Player"&& collision.gameObject.tag != "Barrier" && hit == false)
            hit = true;
    }


    //---------担当箇所
    public void Constructer(GameObject Player)
    {
        rg = this.GetComponent<Rigidbody>();
        playerObj = Player;
        keepPlayer = this.transform.position;
       
        Fpower = forwardPower;
        Upower = upPower;

        //爆弾投下数値環境合わせるための処理
        forwardPower = Fpower * power * Time.deltaTime;
        upPower = Upower * power * Time.deltaTime;

        rg.AddForce(rg.mass * transform.forward * forwardPower, ForceMode.Impulse);
        rg.AddForce(rg.mass * transform.up * upPower, ForceMode.Impulse);
        resetTime = 8.0f;
    }
    //---------担当箇所
}
