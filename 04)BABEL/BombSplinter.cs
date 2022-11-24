using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------スフィアボムの投射スクリプト。
//---------基本的には粘着爆弾と変わりません。
//---------スタン判定の部分を自分が担当しました。


public class BombSplinter : MonoBehaviour
{
    //爆風数値関連ーーーーーーーーーーーーーーーーーー

    //爆風のスピード数値
    private float forwardPower = 30.0f;
    //爆発範囲
    private float bombRange = 5.0f;
    //スタン範囲
    private float stanRange = 5.0f;

    //ーーーーーーーーーーーーーーーーーーーーー

    Vector3 startPos;

    Rigidbody rg;
    Vector3 playerPos;

    PlayerStan playerstan;

    GameObject bombOwner;
    LayerMask layer = 1 << 7;

    LayerMask itemLayer = 1 << 18;

    private void Start()
    {
        //爆発開始座標を取得
        startPos = this.transform.position;

        bombOwner = this.gameObject.GetComponent<B01BombStatus>().bombOwner;

       

        //スタン範囲処理
        //PowerUpアイテムで威力増加

        /*if (this.gameObject.tag == "splinter")
            //通常爆弾範囲
            bombRange = 0.2f * C02ItemManager.itemPower + 2;
        if (this.gameObject.tag == "splinter2")
            //粘着爆弾範囲
            bombRange = 0.12f * C02ItemManager.itemPower + 2;
        if (this.gameObject.tag == "splinterCluster")
            //クラスター爆弾範囲
            bombRange = 0.112f * C02ItemManager.itemPower + 0.7f;
        stanRange = bombRange;*/

        //PlayerStun処理
        //playerstan = FindObjectOfType<PlayerStan>();
        ////役職を検索してから座標を取得
        //if (GameScreenUi.PlayerMode == 0)
        //    playerPos = GameObject.Find("Player Attacker").transform.position;
        //if (GameScreenUi.PlayerMode == 1)
        //    playerPos = GameObject.Find("Player Defender").transform.position;
        //float dis = Vector3.Distance(startPos, playerPos);
        //if (stanRange > dis)
        //    playerstan.StanOn = true;

       //---------------担当箇所

        Collider[] HitCollider = Physics.OverlapSphere(startPos, 3.0f, layer);

        if (HitCollider.Length > 0)
        {
            for (int i = 0; i < HitCollider.Length; i++)
            {
                GameObject StanPlayer = HitCollider[i].gameObject;
                int LostScore = 0;
                StanPlayer.GetComponent<PlayerStan>().StanOn = true;

                if (StanPlayer.GetComponent<PlyerControlloer>().enabled == true)
                {
                    LostScore = (int)(StanPlayer.GetComponent<P04ItemHolder>().Score * 0.9f);
                    //StanPlayer.GetComponent<P04ItemHolder>().Score -= LostScore;
                }

                /*if (bombOwner.GetComponent<DefenderController>().enabled == true)
                {
                    bombOwner.GetComponent<P04ItemHolder>().Score += LostScore / 2;
                }*/


            }
        }


        //爆風によるアイテム取得処理。5.0fが取得範囲の半径(Radius)
        HitCollider = Physics.OverlapSphere(startPos, 5.0f, itemLayer);
        for (int i = 0; i < HitCollider.Length; i++)
        {
            HitCollider[i].gameObject.GetComponent<C02ItemManager>().Itemget(bombOwner);

            //HitCollider[i].gameObject.transform.position = bombOwner.transform.position;

        }


        //-----------------担当箇所

        //爆弾の破片移動処理
        rg = GetComponent<Rigidbody>();
        forwardPower = 5.0f * bombOwner.GetComponent<P04ItemHolder>().itemPower + forwardPower * 1.5f;
        rg.AddForce(rg.mass * transform.forward * forwardPower, ForceMode.Impulse);



    }
    void Update()
    {
        //爆弾の破片はスタート座標から指定の距離で削除
        float dis = Vector3.Distance(startPos, this.transform.position);

        if (bombRange < dis)
            Destroy(this.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
            Destroy(this.gameObject);
    }
}
