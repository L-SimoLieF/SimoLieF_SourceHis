using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//------タワーを構成するブロックのステータスを管理する為のスクリプト(旧仕様)
//MirrorライブラリのNetworkIdentity,transformを使う事で、通信による同期を実現しました。
//形骸化したHPの概念や、スコア、破壊者の情報など、ゲーム的に重要な箇所になっていました。(過去)
//現在のROMでは使用していません。
public class C01CubeState : NetworkBehaviour
{
    //各定数の設定。それぞれブロックによって定められている。
    static int FLOOR_HP = 8192;
    static int WALL_HP = 1280;
    static int PILLER_HP = 1;
    static int FLOOR_SCORE = 9000;
    static int WALL_SCORE = 1500;
    static int PILLER_SCORE = 500;

    //SyncVar_HP。ダメージを共有しないと、各々のシーンで異なる結果になる為。
    [SyncVar] public int HitPoint;
    //Score。破壊時に得られるスコア。設定はStart()内。
    public int Score;
   

    //破壊者の情報を保持
    [SyncVar] public GameObject Player;

    //アイテム用のPrefabの取得
    public GameObject itemObj;
    GameObject item;

    //ゲーム全体を管理するスクリプト。ここではタワー破壊判定の為にアクセスする。
    M01GameManager M01;

    // Start is called before the first frame update
    void Start()
    {

        //ブロックステータスの初期設定。最初はHPを見て、スコアを設定していました。
        if (HitPoint == PILLER_HP)
            Score = PILLER_SCORE;
        else if (HitPoint == WALL_HP)
            Score = WALL_SCORE;
        else if (HitPoint == FLOOR_HP)
            Score = FLOOR_SCORE;
        else
            Score = Score;


        //仮置き
        //タワーの一番下が壊れないようにする処理。
        if (this.gameObject.transform.position.y < 5)
        {
            HitPoint = HitPoint + HitPoint;
        }

        //Prefabのアタッチを省略
        itemObj = GameObject.Find("Item");

        //M01のアタッチ
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();

    }

    // Update is called once per frame
    void Update()
    {

        //HPが1以下=破壊された時
        if (HitPoint < 1)
        {
            //Serverから、Rpcをコール。
            //Rpc=全クライアントに処理を行わせる機能。今回はブロックの破壊処理
            if (isServer)
                RpcBreakBlock(SpawnCheck(), this.gameObject.transform);

        }

    }

    //通信対応前の破壊用関数。
    //アイテムのSpawnと、スコア取得の関数をコールして、このオブジェクトを破壊する。
    void BreakBlock()
    {

        ItemSpawn(this.gameObject.transform);
        Destroy(this.gameObject);
        PointGainer(Player);

    }



    //アイテム生成用の関数。
    //前作、ともしびの精で使ったアイテム生成をそのまま流用、中身だけ変えて実装しました。
    void ItemSpawn(Transform pos)
    {
        //SplitOfLightから流用。

        //5割の確率で生成。
        int ans = Random.Range(0, 2);
        if (ans == 0)
        {
            item = Instantiate(itemObj, pos.position, Quaternion.identity);
            Debug.Log(item.transform.position);
            //アイテム出現処理。
            ans = Random.Range(0, 100);

            //種類が3つ、5割、2割、3割の確率。
            //ItemNumは、取得時にPlayer側が参照する為に必要。ここから増加するパラメータを決定する。
            if (ans < 50)
            {
                Debug.Log("PowerUp");
                item.GetComponent<C02ItemManager>().itemNum = 0;
            }
            else if (ans < 70)
            {
                Debug.Log("CountUp");
                item.GetComponent<C02ItemManager>().itemNum = 1;
            }
            else
            {
                Debug.Log("SpeedUp");
                item.GetComponent<C02ItemManager>().itemNum = 2;
            }

        }
    }

    //スコア加算用の関数。
    //渡したPlayerにScoreを加算する為のもの。
    //タワー破壊判定に関しては、M01に記載。
    void PointGainer(GameObject Player)
    {
     
        Player.GetComponent<P04ItemHolder>().Score += Score;

        //タワー破壊処理
        //条件は「Bomberかどうか」。使ってるスクリプトで判断
        if (Player.GetComponent<PlyerControlloer>().enabled == true)
        {
            M01.BreakCounter(Player);
        }
    }


    //接触判定
    //爆弾、床、別ブロックとの接触時にそれぞれ処理がある。
    private void OnCollisionEnter(Collision collision)
    {

        //field
        //床に落ちた場合即破壊。Destroyじゃないのはスコア加算の為。
        if (collision.gameObject.tag == "Field")
        {
            HitPoint -= 8192;
            //Destroy(this.gameObject);
        }

        //splinter---爆弾。威力が異なる為に分岐。
        if (collision.gameObject.tag == "splinter")
        {
            //通常爆弾
            if (isServer)
            {
                RpcDamage(collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 0);
            }

        }
        if (collision.gameObject.tag == "splinter2")
        {
            //粘着爆弾
            if (isServer)
            {
                RpcDamage(collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 1);
            }

        }

        //ブロック
        //吹っ飛ばされたブロックに当たって、床に落ちた場合の得点の行先がない為、当たったブロックの破壊者情報を伝達する。
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Piller")
        {
            if (collision.gameObject.GetComponent<C01CubeState>().Player != null)
            {
                Player = collision.gameObject.GetComponent<C01CubeState>().Player;
            }
        }
    }

    //Networkのお話
    //Cubeの情報を保存しないといけません。
    //各々が勝手に書き換えるデータを、各クライアントに共有する作業が必要です。
    //例えばHP,Playerの情報等です。
    //毎フレーム同期を行うのは沼To沼なので、変更があった時だけにしましょう。
    //変更を関数呼び出しで行い、その関数呼び出しをサーバーに通知すれば、全てが上手く行きます。


    //RpcDamage
    //ブロックへのダメージを各プレイヤーの環境へ同期する為のもの。
    //最後に触った人間にスコアが入るという都合上、Playerの情報伝達が必要。
    [ClientRpc]
    void RpcDamage(GameObject Bomber, float i)
    {
        P04ItemHolder P04;
        P04 = Bomber.GetComponent<P04ItemHolder>();

        //if (i == 0f)
        //    HitPoint -= (float)Mathf.Pow(2.0f, P04.itemPower / 10.0f);
        //else if (i == 1.0f)
        //    HitPoint -= (float)Mathf.Pow(2.0f, P04.itemPower / 10.0f - 1.0f);
        //HitPoint -= 2 * collision.gameObject.GetComponent<BombStatus>().xxx
        HitPoint -= 1;
        Player = Bomber;
    }

    //生成するかどうかの決定。
    //RpcBlockの引数に、アイテムの生成を行うか否かと、アイテムの種類が何かを保持させる。
    //0は生成無し。1-3でアイテムの区別。

    //RpcBreakBlock
    //ブロック破壊用の関数の完成版。NetworkServer.Destroyで通信に対応。
    //Scoreの加算はサーバーで行う。
    [ClientRpc]
    void RpcBreakBlock(int itemNum, Transform pos)
    {

        switch (itemNum)
        {
            case 1:
                item = Instantiate(itemObj, pos.position, Quaternion.identity);
                item.GetComponent<C02ItemManager>().itemNum = itemNum;
                break;
            case 2:
                item = Instantiate(itemObj, pos.position, Quaternion.identity);
                item.GetComponent<C02ItemManager>().itemNum = itemNum;
                break;
            case 3:
                item = Instantiate(itemObj, pos.position, Quaternion.identity);
                item.GetComponent<C02ItemManager>().itemNum = itemNum;
                break;
            default:
                break;
        }
        NetworkServer.Destroy(this.gameObject);
        if (isServer)
        {
            if (Player != null)
            {
                PointGainer(Player);
            }
        }
    }


    //SpawnCheck
    //アイテムがスポーンするか、するならなんの種類かを返す関数。
    //RpcBreakBlockの引数でコールし、同時に結果を返す。
    int SpawnCheck()
    {
        int ans = Random.Range(0, 2);
        if (ans == 0)
        {
            //アイテム出現処理。
            ans = Random.Range(0, 100);
            if (ans < 50)
            {
                //Debug.Log("PowerUp");
                return 1;
            }
            else if (ans < 70)
            {
                //Debug.Log("CountUp");
                return 2;
            }
            else
            {
                //Debug.Log("SpeedUp");
                return 3;
            }
        }
        else
            return 0;
    }


}

