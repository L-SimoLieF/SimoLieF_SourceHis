using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//C10TowerAdmin
//今回のメインスクリプトの一つで、タワー全体の管理を行う。
//C01による方法では、NetworkServer.Spawnが使えない為、新たに組みなおした。
//TowerPrefabの頂点にアタッチされ、Towerを構成する全てのブロックの情報を持つ。
//ブロックの情報は、cubeData自作クラスの配列により取得、管理される。
//通信による位置の同期はcubeData配列(=CubeArray)にあるPositionなどを送信する事で行われる。

public class C10TowerAdmin : NetworkBehaviour
{
    //アイテム生成用
    public GameObject itemPrefab;
    GameObject item;

    //ゲームマネージャー。
    //終了条件などを管理する。
    M01GameManager M01;
    
    //ラウンド終了条件用。タワーの構成ブロックの8割と、現在破壊されてる個数を管理する。
    //breakendは条件を満たした際に、M01に通達する為のもの。
    int breakLimit;
    int breakCount;
    public bool breakend;

    //ブロックのの初期設定用
    //HP管理などもここで行う為。
    static int FLOOR_HP = 8192;
    static int WALL_HP = 1280;
    static int PILLER_HP = 1;
    static int FLOOR_SCORE = 9000;
    static int WALL_SCORE = 1500;
    static int PILLER_SCORE = 500;

    //破壊時のエフェクト
    public GameObject breakEffect;

    //担当外
    public int blockcount = 6;
    public GameObject Blocks;
    GameObject keepblocks;


    //構成ブロックをC10で管理する為の、ブロックの情報を格納する為の自作クラス。
    //ブロックのオブジェクトの情報や、同期用の座標格納、スコア計算の為の破壊者の情報などがメンバとして存在。
    public class CubeData
    {
        //cubeObj。ブロック本体
        public GameObject cubeObj;
        //HPとScore,破壊者
        [SyncVar] public int HitPoint;
        [SyncVar] public int Score;
        [SyncVar] public GameObject lastPlayer;

        //配列番号。C11からここにアクセスする為に使用する
        [SyncVar] public int ArrayID = 0;

        //位置の同期、送信用
        [SyncVar] public Vector3 pos;
        [SyncVar] public Quaternion rot;
        [SyncVar] public Transform trans;

        //CubeDataクラスのコンストラクタ
        public CubeData()
        {
            cubeObj = default;
            HitPoint = 0;
            Score = 0;
            lastPlayer = default;
            ArrayID = 0;
        }

        //コンストラクタその2。オブジェクトの情報と、配列番号を渡した際にそれを登録する。
        //オブジェクトのタグによって、HPとスコアを自動で格納する。
        public CubeData(GameObject Object, int Array)
        {
            cubeObj = Object;
            ArrayID = Array;
            if (Object.tag == "Floor")
            {
                HitPoint = FLOOR_HP;
                Score = FLOOR_SCORE;
            }
            if (Object.tag == "Wall")
            {
                HitPoint = WALL_HP;
                Score = WALL_SCORE;
            }
            if (Object.tag == "Piller")
            {
                HitPoint = PILLER_HP;
                Score = PILLER_SCORE;
            }

            //pos = cubeObj.transform.position;
        }

        //ダメージ関数。C11のCollisionEnterから、この関数を呼んでダメージを処理する。
        //ここに集めているのは、サーバー---クライアント間で共有するため。
        public void Damage(GameObject player, int power)
        {
            lastPlayer = player;
            HitPoint -= power;

        }

        //ダメージ関数。こちらは床に落ちた際に呼ばれる。渡すPlayer情報が無い時にエラーを回避する為のもの。
        public void Damage(int power)
        {
            HitPoint -= power;
        }

        //ブロックの接触時に接触したブロックからプレイヤー情報を取得する。
        public void SendLastPlayer(GameObject other)
        {
            lastPlayer = other;
        }




    }

    //位置の同期用に作った。Transform型がSyncListに対応してなかった(?）
    //positionとRotationを同時に設定する為に必要。
    public struct CubeTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    //タワーの構成ブロックを取得する為に必要。
    //『子』の入れ子構造で全ブロックを取得する。
    Transform children;
    
    //List。次々追加する為、タワーによって構成ブロック数が異なる為に採用。
    //元々は、破壊されたブロックの要素を消去するつもりだったが、非効率だった為見送った。
    //CubeArray,CubeTransを紐づけて、位置の同期を実現した。
    public List<CubeData> CubeArray = new List<CubeData>();
    public SyncList<CubeTransform> CubeTrans = new SyncList<CubeTransform>();




    //Start。初期設定。breakLimitはラウンドの終了条件用
    // Start is called before the first frame update
    void Start()
    {
        //GetAllChild(this.gameObject.transform);
        //M01のアタッチ
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
        if (isServer)
        {
            M01.C10 = this.gameObject.GetComponent<C10TowerAdmin>();
        }
        if (isClient)
        {
            //SetArrayID();
        }

        //GetAllChild
        //タワーの構成ブロックを取得、CubeArrayに格納する。
        GetAllChild(this.gameObject.transform);

        breakLimit = (int)(CubeArray.Count * 0.8);

        //DontDestroyOnLoad(this.gameObject);

    }


    //Awake
    //Scene遷移時の消滅を免れようとした結果。必要なかったのでコメントアウト。
    /*private void Awake()
    {
        //GetAllChild(this.gameObject.transform);
        //M01のアタッチ
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
        if (isServer)
        {
            M01.C10 = this.gameObject.GetComponent<C10TowerAdmin>();
        }
        if (isClient)
        {
            //SetArrayID();
        }
        GetAllChild(this.gameObject.transform);

        breakLimit = (int)(CubeArray.Count * 0.8);
    }*/



    // Update is called once per frame
    void Update()
    {
        //プラクティスモードでブロックを生成する処理
        if(blockcount <= 0 && this.gameObject.tag == "spawner")
        {
            keepblocks = Instantiate(Blocks, this.transform.position, this.transform.rotation);
            NetworkServer.Spawn(keepblocks);
            Destroy(this.gameObject);
        }


        if (isServer)
        {
            //NetworkTransformの代替。
            //毎フレーム、全ブロックの位置をサーバーからクライアントに送信する。
            //クライアントは、CubeTransの情報を基に、自分達のCubeの位置を更新する。
            for (int i = 0; i < CubeArray.Count; i++)
            {
                //CubeObj == null = 既に破壊されたブロック。破壊されたブロックの座標更新は行わない。
                if (CubeArray[i].cubeObj != null)
                {
                    //CubeArray[i].pos = CubeArray[i].cubeObj.transform.position;
                    //CubeArray[i].trans = CubeArray[i].cubeObj.transform;
                    //CubeArray[i].rot = CubeArray[i].cubeObj.transform.rotation;
                    CubeTransform a;
                    a.Position = CubeArray[i].cubeObj.transform.position;
                    a.Rotation = CubeArray[i].cubeObj.transform.rotation;
                    CubeTrans[i] = a;

                }
            }
        }

        //Server以外 = 情報の受け取り側。
        //各Cubeの座標を更新する。
        else
        {

            for (int i = 0; i < CubeArray.Count; i++)
            {
                if (CubeArray[i].cubeObj != null)
                {

                    //Vector3 pos = Vector3.Lerp(transform.position, m_ReceivedPosition, m_LerpRate * Time.deltaTime);
                    //Quaternion rot = Quaternion.Slerp(transform.rotation, m_ReceivedRotation, m_LerpRate * Time.deltaTime);

                    //CubeArray[i].cubeObj.transform.position = Vector3.Lerp(CubeArray[i].cubeObj.transform.position,CubeTrans[i].Position, 4f * Time.deltaTime) ;
                    //CubeArray[i].cubeObj.transform.rotation = CubeTrans[i].Rotation;

                    Vector3 pos = Vector3.Lerp(CubeArray[i].cubeObj.transform.position, CubeTrans[i].Position, 10f * Time.deltaTime);
                    Quaternion rot = Quaternion.Slerp(CubeArray[i].cubeObj.transform.rotation, CubeTrans[i].Rotation, 10f * Time.deltaTime);

                    //実際の更新。同時に更新するとなぜかうまく行った。
                    CubeArray[i].cubeObj.transform.SetPositionAndRotation(pos, rot);
                }
            }

        }
    }

    //タワーを構成しているブロックを全て配列に格納する為の変数。
    void GetAllChild(Transform parent)
    {
        //引数のオブジェクトの子を取得。
        //これを繰り返す事で最下層まで全て取得する。
        children = parent.GetComponentInChildren<Transform>();

        foreach (Transform child in children)
        {
            //RigidBodyの存在でブロックを検知
            if (child.gameObject.GetComponent<Rigidbody>())
            {
                //CubeData型で宣言。オブジェクトと、CubeArrayのカウント(=このオブジェクトで何個目か)を渡す
                CubeData b = new CubeData(child.transform.gameObject, CubeArray.Count);

                //miniTowerの得点除外処理
                if(this.gameObject.tag == "miniTower")
                {
                    b.Score = 0;
                }

                //CubeArrayに追加。
                CubeArray.Add(b);
                //ブロックのオブジェクトにアクセス用のIDを保存
                child.gameObject.GetComponent<C11CubeState>().arrayID = b.ArrayID;
                //Serverのみ、クライアントへの通達用に座標を格納。
                if (isServer)
                {
                    CubeTransform c = new CubeTransform();
                    c.Position = b.cubeObj.transform.position;
                    c.Rotation = b.cubeObj.transform.rotation;
                    CubeTrans.Add(c);
                }
            }
            //Rigidbodyが無いオブジェクト=オブジェクトの下にブロックが存在する。
            //=再度GetAllChildをコール
            else
                GetAllChild(child);

        }
        //Debug.Log(CubeArray.Count);


    }


    //C11と、C10に配列を繋ぐ関数。
    //idはこれを呼んだブロックと、配列のブロックを繋げる為のもの。
    //プロパティ的な物。C10の配列が外部から参照出来ない為、このような手法を取った。
    public void ActDamage(int id, GameObject player, int power)
    {
        if (isServer)
        {
            CubeArray[id].Damage(player, power);

            if (CubeArray[id].HitPoint < 1)
            {
                //破壊処理
                BreakBlock(SpawnCheck(), id);
                //RpcCubeDestroy(id);

            }
        }
    }
    //Playerが居ない時用(自然落下等)
    public void ActDamage(int id, int power)
    {
        if (isServer)
        {
            CubeArray[id].Damage(power);

            if (CubeArray[id].HitPoint < 1)
            {
                //破壊処理
                BreakBlock(SpawnCheck(), id);
                //RpcCubeDestroy(id);

            }
        }
    }

    //ブロックの破壊処理
    void BreakBlock(int itemNum, int id)
    {
        //破壊エフェクト。各プレイヤーで出す為にRpc
        RpcBreakEffect(id);

        //SwitchはitemNumの結果がユーザーによって異なった為、アイテムが複数生まれる不具合が起こり、不採用。
        switch (itemNum)
        {
            case 1:
                //item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
                //item.GetComponent<C02ItemManager>().itemNum = itemNum;
                //NetworkServer.Spawn(item);
                break;
            case 2:
                //item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
                //item.GetComponent<C02ItemManager>().itemNum = itemNum;
                //NetworkServer.Spawn(item);
                break;
            case 3:
                //item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
                //item.GetComponent<C02ItemManager>().itemNum = itemNum;
                //NetworkServer.Spawn(item);
                break;
            default:
                break;
        }

        //0じゃない時=アイテムが生成されている
        //一か所に纏めて重複Spawnを防止。
        if(itemNum != 0)
        {
            item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
            item.GetComponent<C02ItemManager>().itemNum = itemNum;
            //生成を各プレイヤーの世界に通達するためのSpawn関数。
            NetworkServer.Spawn(item);
        }

        //ブロックの破壊。通達の為にRpc
        RpcCubeDestroy(id);

        //得点計算はサーバーで行い、結果をクライアントに送信する。
        if (isServer)
        {
            //lastPlayerは得点を獲得するプレイヤー。Score!=0は、得点計算を一回きりにする為のもの。
            if (CubeArray[id].lastPlayer != null && CubeArray[id].Score != 0)
            {
                PointGainer(CubeArray[id].lastPlayer, CubeArray[id].Score);
                CubeArray[id].Score = 0;
            }
            //居ないときは、NoneScoreという変数に消える筈だったスコアを格納する。
            else if (CubeArray[id].lastPlayer == null)
            {
                PointGainer(CubeArray[id].Score);
            }

            //終了条件の一つ、ブロックの破壊割合用の処理。
            if (breakCount > breakLimit)
            {
                breakend = true;
            }
            else
                breakend = false;
        }
    }

    //得点計算用の関数。
    void PointGainer(GameObject Player, int Score)
    {
        //Playerの得点を加算する処理。
        //恐らく各playerが保持している情報だと思うので、そこを直接参照して加算する。
        //(Aex.ゲーム管理クラスが存在した場合でも、「誰が壊したか」の情報は必要であるため、引数にplayerを持つ)
        //ボマーのみ得点加算
        if (Player.GetComponent<DefenderController>().enabled == false)
        {
            Player.GetComponent<P04ItemHolder>().Score += Score;
        }
        //タワー破壊処理

        if (Player.GetComponent<PlyerControlloer>().enabled == true)
        {

            M01.BreakCounter(Player);
        }

        //破壊割合用
        breakCount++;
    }

    //行き場のないスコアの加算
    void PointGainer(int Score)
    {
        M01.NoneScoreCount(Score);
        breakCount++;
    }

    //C01か何かと同一。
    int SpawnCheck()
    {
        int ans = Random.Range(0, 8);
        if (ans == 0)
        {
            //アイテム出現処理。
            ans = Random.Range(0, 100);
            if (ans < 60)
            {
                //Debug.Log("PowerUp");
                return 1;
            }
            else if (ans < 75)
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

    //以下、位置同期で使用していた処理の没案
    public Vector3 PositionUpdate(int id)
    {
        //クライアントへの位置の送信
        if (isClient)
        {
            return CubeArray[id].pos;
        }

        return CubeArray[id].cubeObj.transform.position;
    }
    public Vector3 GetCubePosition(int id)
    {
        return CubeArray[id].pos;
    }
    public Quaternion GetCubeRotation(int id)
    {
        return CubeArray[id].rot;
    }

    public void SetArrayID()
    {
        for (int i = 0; i < CubeArray.Count; i++)
        {
            CubeArray[i].cubeObj.GetComponent<C11CubeState>().arrayID = CubeArray[i].ArrayID;
        }
    }

    //ClientCheck
    //C11でNetworkBehavierが使えない為、クライアントかどうかを判断する方法が無いと思い込んでいた。
    //C10を参照する事で、サーバー/クライアントのどちらであるかを検知している。
    public bool ClientCheck()
    {
        return isServer;
    }

    public void GetCubeTransform(int id)
    {
        if (isServer)
            RpcGetCubeTransform(id, CubeArray[id].pos, CubeArray[id].rot);

        //return CubeArray[id].cubeObj.transform;
    }
    [ClientRpc]
    public void RpcGetCubeTransform(int id, Vector3 pos, Quaternion rot)
    {
        CubeArray[id].cubeObj.transform.position = pos;
        CubeArray[id].cubeObj.transform.rotation = rot;
    }


    //ブロックの破壊処理。
    [ClientRpc]
    public void RpcCubeDestroy(int id)
    {
        //Instantiate(breakEffect, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
        blockcount -= 1;
        Destroy(CubeArray[id].cubeObj);
        //Effect
        //Destroy(x, 5f);

    }

    //破壊エフェクトの生成
    [ClientRpc]
    public void RpcBreakEffect(int id)
    {
        Instantiate(breakEffect, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
    }

    public void SendPlayerData(int reciverid, int Senderid)
    {
        if (isServer)
        {
            if (CubeArray[Senderid].lastPlayer != null)
            {
                CubeArray[reciverid].SendLastPlayer(CubeArray[Senderid].lastPlayer);
            }
        }
    }

    public GameObject GetLastPlayer(int id)
    {
        return CubeArray[id].lastPlayer;
    }

    //Scoreを取得する為のプロパティ
    public int GetCubeScore(int id)
    {
        return CubeArray[id].Score;
    }

    //ラウンド終了時、残ブロックの合計点がスコアとして加算されるDefender用の関数。
    //合計点を算出して返す。M01で使用
    public int GetDefScore()
    {
        int AllScore = 0;

        for (int i = 0; i < CubeArray.Count; i++)
        {
            //Scoreが0じゃない時 = 破壊されていないブロック
            if (CubeArray[i].Score != 0)
            {
                AllScore += CubeArray[i].Score;
                if (CubeArray[i].Score == PILLER_SCORE)
                {

                }
                if (CubeArray[i].Score == WALL_SCORE)
                {

                }
                if (CubeArray[i].Score == FLOOR_SCORE)
                {

                }
            }
        }

        return AllScore;
    }
}
