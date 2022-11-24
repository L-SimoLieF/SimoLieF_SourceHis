using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


//M02EnemySearch
//名前の通り、索敵用のスクリプトです。
//全て、下村が担当しました。
public class M02EnemySearch : NetworkBehaviour
{
    [SerializeField] GameObject director;
    [SerializeField] GameObject directorPrehab;
    public GameObject target;
    public bool SetDir;
    int time;
    Vector3 dir;

    M01GameManager M01;
    public GameObject MyPlayer;
    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
       

        //自分が操作しているキャラクターを格納。
        //それ以外が爆弾を発射した際に、索敵機能をオンにする為の処理。
        if (MyPlayer == null)
        {
            for (int i = 0; i < M01.user.Count; i++)
            {
                if (M01.user[i].player.GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                    MyPlayer = M01.user[i].player;
                }
            }
        }
        else
        {
            //director = MyPlayer.transform.GetChild(24).gameObject;
            //target = 爆弾を発射したプレイヤーオブジェクト
            //自分が操作しているキャラクターじゃない場合、targetの方を向いたCornをInstatiateする。
            //Cornは時間経過で消滅。
            if (target != MyPlayer)
            {
                if (SetDir)
                {
                    if (time == 0)
                    {
                        //director.SetActive(true);
                        //MyPlayer.transform.GetChild(24).gameObject.SetActive(true);
                        direction = (target.transform.position - MyPlayer.transform.position).normalized;
                        Transform tsm = directorPrehab.transform;
                        tsm.position = MyPlayer.transform.position + direction;
                        tsm.LookAt(target.transform.position);
                        director = Instantiate(directorPrehab,tsm);
                        director.GetComponent<S01CornPosition>().Player = MyPlayer;
                        director.GetComponent<S01CornPosition>().direction = direction;
                        Destroy(director,3f);
                    }
                    //Quaternion rotation = Quaternion.LookRotation(target.transform.position, Vector3.up);
                    //director.transform.LookAt(target.transform.position);
                    //dir = (target.transform.position - this.gameObject.transform.position).normalized;
                    //director.transform.Rotate(dir);
                    //director.transform.rotation = rotation;
                    //director.transform.LookAt(target.transform);
                    //director.transform.position = MyPlayer.transform.position + direction;
                    time++;
                    if (time > 100)
                    {
                        SetDir = false;
                        //MyPlayer.transform.GetChild(24).gameObject.SetActive(false);
                        time = 0;
                    }

                }
            }
        }
    }
}

//索敵について
//他のプレイヤーが爆弾が発射した事を通達する必要がある。
//他のプレイヤーから、自身のプレイヤーオブジェクトへの情報の通達
//→これが面倒。M02という共通の受け皿を用意する事で対応。M02でCornのActiveを切り替える方針。
//M02に自分のLocalPlayerのオブジェクトを保持しておいて、発射毎にM02に発射者の情報を送信する。
//発射者がLocalのオブジェクトと異なった場合に、索敵機能を起動する。
