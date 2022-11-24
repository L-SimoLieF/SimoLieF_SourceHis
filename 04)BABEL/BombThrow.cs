using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//-------スフィアボムの投出用スクリプト。
//-------通信対応用に、初期化関数たるConstructer関数を作りました。

public class BombThrow : NetworkBehaviour
{
    //爆弾関連数値ーーーーーーーーーーーー

    //forwardPower前方向数値
    public float forwardPower = 20.0f,
    //upPower上方向数値
  　              upPower = 0.0f,
    //爆弾静止後の爆発待機時間
                  bombTimer = 3.0f,
    //全体威力調整
                  power = 100f,
    //爆発後削除時間
                  destroyTime = 1.0f;
    //ーーーーーーーーーーーーーーーーーーーー

    //爆弾エフェクト取得
    [SerializeField] GameObject BombEffect;

    //爆弾数値保管用
    float Fpower, Upower;

    //プレイヤー取得用
    [SyncVar] GameObject playerObj;
    Vector3 keepPlayer;
    Rigidbody rg;
    float set = 0;
    C02ItemManager ItemManager;
    void Start()
    {
        //通信対応により、都合が良いのでConstructer関数を宣言。
        //Start関数で行っていた事を別関数で行う事にしました。
        //爆弾生成時にConstructer関数をPlayerControllerから呼び出してます。

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

        //Debug.Log(forwardPower + ")(" + upPower);

        //float posY = this.transform.position.y;
        //float playerY = playerObj.transform.position.y;
        //Debug.Log("objPos" + posY + "player" + playerY);


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
        if (bombTimer <= -1.0f * destroyTime) {
            //playerObj.GetComponent<P04ItemHolder>().bombCount--;
           NetworkServer.Destroy(this.gameObject);
        }
        


    }

    //-------担当箇所

    //爆弾の初期設定。
    //加速もここで行う。
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
    }


    //チュートリアル用に用意した専用のもの。
    //ItemHolderを持ってないオブジェクトが爆弾を発射する際に使用しました。
    public void Constructer(GameObject Player,int a)
    {
        rg = this.GetComponent<Rigidbody>();
        playerObj = Player;
        keepPlayer = this.transform.position;

        /*Fpower = forwardPower;
        Upower = upPower;

        //爆弾投下数値環境合わせるための処理
        forwardPower = Fpower * power * Time.deltaTime;
        upPower = Upower * power * Time.deltaTime;

        rg.AddForce(rg.mass * transform.forward * forwardPower, ForceMode.Impulse);
        rg.AddForce(rg.mass * transform.up * upPower, ForceMode.Impulse);*/
    }

    //---------担当箇所
}
