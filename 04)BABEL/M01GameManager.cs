using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//ゲームの得点周りを管理する為のスクリプト。
//RoundSetとは違う意味での、ゲーム進行の中枢。
//終了条件や、ボーナスポイントの計算などを行っています。

public class M01GameManager : NetworkBehaviour
{
    //ラウンド終了条件(破壊判定が行われる上限回数)
    const int TOWER_LIMIT = 4;
    //ブロックの破壊個数(破壊判定と見なされる個数)
    const int BREAK_LIMIT = 25;
    //破壊判定を継続する時間(現在は最初の1個～20個破壊されるまでの時間)
    const float BREAK_TIME = 7.0f;
    //タワーの破壊判定によるボーナス付与の閾値
    const int CLUSH_BONUS = 5;

    [SyncVar] public int towerBreakcount = 0;
    [SyncVar] int blockBreakcount = 0;
    [SyncVar] public bool end;

    public GameObject localPlayer;
    //public SyncList<GameObject> user = new SyncList<GameObject>();
    [SyncVar] int resultScore_B;
    [SyncVar] int resultScore_D;

    [SyncVar] public int NoneScore = 0;

    float timer = 0f;

    public GameObject towerPrefab;
    GameObject tower;


    //スコア保存用変数群
    [SyncVar] public int timeScore;
    [SyncVar] public int remainScore;
    [SyncVar] public int bonusScore;
    [SyncVar] public int bonusNum;


    [SyncVar] public C10TowerAdmin C10;

    public struct PlayerData
    {
        public GameObject player;
        //Ture = Bomber False = Defender
        public bool side;
        public string name;
    }
    public SyncList<PlayerData> user = new SyncList<PlayerData>();

    public bool breakend;

    //singleton用。使えなかった
    static bool existsInstance = false;

    // Start is called before the first frame update
    void Start()
    {

        //DontDestroyOnLoad(this.gameObject);

    }

    // 初期化
    //Awake関数。シーン遷移において、SingletonとDontDestroyOnLoadを使う事で常に持たせようとしたが、
    //UNET-Mirrorによるバグ、仕様により、Awakeが正常なタイミングで呼ばれず、期待通りの動作をしない為に未使用となった。
    /*void Awake()
    {
        // インスタンスが存在するなら破棄する
        if (existsInstance)
        {
            Destroy(gameObject);
            return;
        }

        // 存在しない場合
        // 自身が唯一のインスタンスとなる
        existsInstance = true;
        DontDestroyOnLoad(gameObject);
    }*/


    //タワー破壊判定の加算や、ブロック破壊割合での終了を常に検知している。
    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            if (blockBreakcount > 0)
            {
                timer += Time.deltaTime;
            }
            if (timer > BREAK_TIME)
            {
                blockBreakcount = 0;
                timer = 0f;
                Debug.Log("Count end");
            }
        }

        //Debug.Log(user[0].player.GetComponent<P04ItemHolder>().Score);

        if (C10.breakend == true)
        {
            breakend = true;
        }
        else
            breakend = false;


    }


    //タワー破壊判定のスコア加算処理。
    //終了条件の確認も行っている。
    public void BreakCounter(GameObject Player)
    {
        Debug.Log("Count start");
        if (end == false)
        {
            blockBreakcount += 1;
            if (blockBreakcount > BREAK_LIMIT)
            {
                Player.GetComponent<P04ItemHolder>().Score += 150000;
                blockBreakcount = 0;
                towerBreakcount++;
                Debug.Log("Tower Break!");
            }
            if (towerBreakcount > TOWER_LIMIT)
            {
                end = true;
            }
        }
    }


    //ラウンド終了時の得点計算用
    //未使用
    [ClientRpc]
    public void RpcSetPlayerList(GameObject Player)
    {
        if (isServer)
        {
            //user.Add(Player);
        }
        Debug.Log(user);
    }

    //得点計算用関数(仮組)
    //陣営別の得点計算。勝敗や有利不利の判定に使っている。
    public void ResultScoreCount()
    {
        resultScore_B = 0;
        resultScore_D = 0;

        for (int i = 0; i < user.Count; i++)
        {
            if (user[i].side == true)
            {
                resultScore_B += user[i].player.GetComponent<P04ItemHolder>().Score;
            }
            if (user[i].side == false)
            {
                resultScore_D += user[i].player.GetComponent<P04ItemHolder>().Score;
            }

        }

        Debug.Log("Score_B:" + resultScore_B);
        Debug.Log("Score_D:" + resultScore_D);
    }

    //行き場のないスコアの加算
    //本来は、終了時にBomberの点数に加算する予定だった(破壊はされている為)
    public void NoneScoreCount(int score)
    {
        NoneScore += score;
    }

    //プレイヤーが接続してきた際に、そのオブジェクトを格納する関数
    //得点計算や表示など、ここで作った配列からプレイヤー情報を取得している。
    //特に、RoundSet.csで、ここのuser配列が頻出する。
    public int AddPlayerList(GameObject player, bool team)
    {
        int id = 9;
        if (isServer)
        {
            PlayerData p = new PlayerData();
            p.player = player;
            p.side = team;
            user.Add(p);
            id = user.Count - 1;
        }

        return id;
    }

    //ラウンド終了時の残ブロック数による得点の計算を行う為の関数。
    //C10の関数をコールし、結果を返す。
    public int DefScoreCount()
    {
        int BlockScore = 0;
        BlockScore = C10.GetDefScore();
        return BlockScore;
    }


    //ラウンド終了時のボーナススコアの計算を行う。
    //時間、ブロック数、終了条件の3つのボーナスがある。
    public void BonusSet(int endtime)
    {
        int time = endtime;
        //timeScore = 0;

        //時間によるボーナス付与
        if (time >= 0)
            timeScore = (180 - time) * 500;
        else
            timeScore = 0;

        //残存ブロック数によるボーナス付与
        remainScore = 0;
        remainScore = DefScoreCount();



        //タワー破壊判定の回数によるボーナス付与
        if (towerBreakcount >= CLUSH_BONUS)
        {
            bonusScore = 500000;
            bonusNum = 1;
            end = false;
        }
        //ブロックの破壊割合によるボーナス付与
        else if (C10.breakend == true)
        {
            bonusScore = 100000;
            bonusNum = 2;
        }
        //タワー防衛によるボーナス付与
        else
        {
            bonusScore = 300000;
            bonusNum = 3;
        }

    }

    //有利不利、勝敗を確認する関数。
    public int JudgeScore()
    {
        ResultScoreCount();
        if (resultScore_B > resultScore_D)

            return 0;

        else if (resultScore_B < resultScore_D)

            return 1;

        else
            return 2;
    }
}
