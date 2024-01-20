using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W01WhaleMoving : MonoBehaviour
{
   
    public float speed = 10.0f;
    public float timer = 0.0f;

    public float hittimer = 0f;
    public float accTimer = 0f;

    public int distanceHP = 100;
    public int ferocity;
    public int beforeFerocity;
    public int gemGetCount;

    //攻撃周期。分かりやすさを重視しました。
    //public float[] atkCycle100m = new float[4] { 9.0f, 8.5f, 8.0f, 7.5f };
    //public float[] atkCycle75m = new float[5] { 0.0f, 9.0f, 8.4f, 7.8f, 7.2f };
    //public float[] atkCycle50m = new float[12] { 0.0f, 0.0f, 9.0f, 8.2f, 7.4f, 6.5f, 6.0f, 5.5f, 5.0f, 4.5f, 4.0f, 4.0f };
    //public float[] atkCycle25m = new float[12] { 0.0f, 0.0f, 0.0f, 8.0f, 7.5f, 7.0f, 6.5f, 6.0f, 5.5f, 5.0f, 4.5f, 4.0f };

    public float atkCycle;

    //攻撃用スクリプト Attack関数の呼び出しの為に必要
    W02WhaleAttack W02;


    //---------------------新攻撃システム用変数群

    //9分割された移動エリアの情報を格納する為に必要
    //W03Xは自作クラス。同名のスクリプトが存在。
    public W03X_AreaData[] testData = new W03X_AreaData[9];

    //クジラ(面倒だからそう呼称)の現在位置を保存する為に必要
    public W03X_AreaData NowArea;
    public int aaaaaa;

    //前回位置、算出された次回位置を格納する為の変数
    //前回位置は移動先から省く為に必要
    int lastPosNum;
    int nextPosNum;

    //CenterLineを軸に移動する為の変数群
    public GameObject centerLine;
    Vector3[] linePositions;
    int lineCount;

    public Vector3 lerpPosition;
    float lerpTimer;
    float lerpRange = 2.0f;

    //------------------------

    //(09/24)仕様変更後
    public int atkEnergy = 1; //雷槍のエネルギー量
    public int hitCount = 0; //被弾回数
    public int stopCount = 0;//被弾による減速処理用。(3回当たったら減速)

    //新攻撃間隔
    public float[] atkCycleNew = new float[12] { 12.0f, 11.5f, 11.0f, 10.5f, 10.0f, 9.5f, 9.0f, 8.5f, 8.0f, 7.5f, 7.0f, 7.0f };

    //減速時間
    public float stopTimer;
    //減速回数の制限
    public int distanceLimit;

    //宝石のgameObject
    public GameObject gemObject;
    [SerializeField] GameObject backet;
    int gemcount;

    //Animator用
    W04WhaleAnimator W04;

    //Tutorial
    W05WhaleTutorial W05;

    W07WallMG W07;

    //
    [SerializeField] GameObject damageEffect;

    int resetCount;

    public bool normalSet;
    public float shiftTimer;

    // Start is called before the first frame update
    void Start()
    {
        //必要な各要素の設定

        
        W02 = this.gameObject.GetComponent<W02WhaleAttack>();
        AtkCycleSet(ferocity);

        //CenterLine用
        centerLine = GameObject.Find("CenterLine");
        linePositions = new Vector3[centerLine.GetComponent<LineRenderer>().positionCount];
        centerLine.GetComponent<LineRenderer>().GetPositions(linePositions);
        lineCount = 0;
        //this.transform.position = new Vector3(this.transform.position.x, linePositions[0].y, linePositions[0].z);



        //縦40,横80
        //縦13,横26

        //9方向移動用の宣言
        //各要素が移動先の情報を保持(中心からの座標差、隣接マス、重要度、要素番号)
        testData[0] = new W03X_AreaData(new Vector3(0, 13, 26), new int[3] { 1, 3, 4 }, 1, 0);
        testData[1] = new W03X_AreaData(new Vector3(0, 13, 0), new int[5] { 0, 2, 3, 4, 5 }, 4, 1);
        testData[2] = new W03X_AreaData(new Vector3(0, 13, -26), new int[3] { 1, 4, 5 }, 1, 2);

        testData[3] = new W03X_AreaData(new Vector3(0, 0, 26), new int[5] { 0, 1, 4, 6, 7 }, 2, 3);
        testData[4] = new W03X_AreaData(new Vector3(0, 0, 0), new int[8] { 0, 1, 2, 3, 5, 6, 7, 8 }, 5, 4);
        testData[5] = new W03X_AreaData(new Vector3(0, 0, -26), new int[5] { 1, 2, 4, 7, 8 }, 2, 5);

        testData[6] = new W03X_AreaData(new Vector3(0, -13, 26), new int[3] { 3, 4, 7 }, 1, 6);
        testData[7] = new W03X_AreaData(new Vector3(0, -13, 0), new int[5] { 3, 4, 5, 6, 8 }, 4, 7);
        testData[8] = new W03X_AreaData(new Vector3(0, -13, -26), new int[3] { 4, 5, 7 }, 1, 8);

        /*W03X_AreaData[] testData = {new W03X_AreaData (new Vector3(0, 1, -1), new int[3] { 1, 3,4 }, 1, 0)
                                ,new W03X_AreaData(new Vector3(0, 1, 0), new int[5] { 0,2,3,4,5 }, 1, 1)
                                , new W03X_AreaData(new Vector3(0, 1, 1), new int[3] { 1,4,5 }, 1, 2)

                                , new W03X_AreaData(new Vector3(0, 0, -1), new int[5] { 0,1,4,6,7 }, 1, 3)
                                , new W03X_AreaData(new Vector3(0, 0, 0), new int[8] {0,1,2,3,5,6,7,8}, 1, 4)
                                , new W03X_AreaData(new Vector3(0, 0, 1), new int[5] { 1,2,4,7,8 }, 1, 5)

                                , new W03X_AreaData(new Vector3(0, -1, -1), new int[3] { 3,4,7 }, 1, 6)
                                , new W03X_AreaData(new Vector3(0, -1, 0), new int[5] { 3,4,5,6,8 }, 1, 7)
                                , new W03X_AreaData(new Vector3(0, -1, 1), new int[3] { 4,5,7 }, 1, 8)
                                };*/


        //初期位置の設定 中心
        NowArea = new W03X_AreaData(testData[4]);
        lastPosNum = 4;
        //NowArea.Position = new Vector3(0, 0, 0);


        //Animation
        W04 = this.gameObject.GetComponent<W04WhaleAnimator>();
        W04.Constructer(testData);

        W05 = this.gameObject.GetComponent<W05WhaleTutorial>();

        W07 = this.gameObject.transform.GetChild(4).gameObject.GetComponent<W07WallMG>();

        speed = 15.0f;


    }

    // Update is called once per frame
    void Update()
    {

        if (W05.startFlag == false)
            return;

        aaaaaa = NowArea.Number;

        ////////////////////////速度関係
        //減速、加速、通常の三種。各タイマーでそれぞれの速度を維持する時間を管理。
        //通常はspeed = 10.0f;

        /* if(accTimer > 0)
        {
            accTimer -= Time.deltaTime;
            speed = 22.5f;
        }

        else if(hittimer > 0) 
        {
            hittimer -= Time.deltaTime;
            speed = 7.5f;
           
            //attack = false;

            if(hittimer <= 0)
            {
                 distanceHP -= 25;
            }
        }
        else
        {
            timer += Time.deltaTime;
            //speed = 15.0f;
        }*/

        if (normalSet == true)
        {
            timer += Time.deltaTime;
            shiftTimer = 0f;
            hittimer = 0f;
        }
        else
            shiftTimer += Time.deltaTime; 



        //新仕様
        /*if (stopTimer > 0)
        {
            stopTimer -= Time.deltaTime;
            speed = 6.67f;
        }
        else
        {
            timer += Time.deltaTime;
            speed = 15.0f;
        }*/

        //進行方向がx方面に+の為、Vector3.Right。
        this.transform.position += Vector3.right * speed * Time.deltaTime;

        if (this.transform.position.x > linePositions[lineCount].x)
        {
            lineCount++;
            //this.transform.position = linePositions[lineCount];
        }

        /////////////////////////速度関係。


        //攻撃処理
        //詳細はW02WhaleAttackへ
        if (timer >= atkCycle)
        {


            //攻撃前の移動処理
            //前回位置の保存、次回位置の決定、次回位置の情報の格納
            //移動処理はCenterlineの都合で別スクリプトへ。
            lastPosNum = NowArea.Number;
            nextPosNum = NowArea.decideNextPos(lastPosNum, testData, NowArea);

            //Animation(移動→攻撃)
            W04.SetNumbers(lastPosNum, nextPosNum);

            //Debug.Log("now" + NowArea.Number + "next" + nextPosNum);
            NowArea = testData[nextPosNum];
            //this.transform.position = new Vector3(this.transform.position.x, linePositions[lineCount].y + NowArea.Position.y,
            //linePositions[lineCount].z + NowArea.Position.z);


            //攻撃処理
            //タイマーのリセット(線形補完と攻撃間隔)
            lerpTimer = 0;
            W02.Attack(ferocity);

            //11/24 SE用の追加(攻撃)
            //SeSystem.WhaleMissile=true; 

            timer = 0f;
        }

        //0m地点で15秒停滞した時の処理
        if (distanceHP == 0 && timer > 5f)
        {
            //距離のリセット。クジラの加速と、内部データ(distanceHP)の書き換え
            //ResetDistance(gemGetCount);
            
            ResetDistance(distanceLimit);
            timer = 0f;
        }

        //被弾時の減速処理
        //(仮置、雷槍の処理が不明な為)
        if (Input.GetKeyDown(KeyCode.M))
        {

            //被弾エフェクト
            GameObject ins = Instantiate(damageEffect, transform.position, Quaternion.identity);

            hitCount += atkEnergy;
            stopCount += atkEnergy;
            gemcount++;

            if (distanceHP > 0/*distanceLimit < 4*/ && stopCount > 9 && hittimer <= 0.0f)
            {
                stopTimer = 3f;
                stopCount = 0;
                distanceLimit += 1;

                hittimer = 3f;
                timer = 0f;
                //distanceHP -= 25;

                distanceHP -= 25;
                W07.wallActive(distanceHP, 7.5f);


                //Animation(被弾モーション)
                W04.DamageAnimSet();
            }

            if (gemcount > 2)
            {
                GemDrop();
                gemcount = 0;
            }

           

            //timer = timer + 3.0f;

        }

        //凶暴性の算出
        //処理負荷を軽減するなら、距離が縮まったタイミングだけでも良かったが、energyの取得がある為審議か
        //ferocity = (100 - distanceHP) / 25 + gemGetCount /* Player.energyCount*/;

        //0924仕様
        ferocity = hitCount / 36 /*+ Player.energyCount*/;

        //凶暴性の変化を検知したい。
        FerocityUpdate(ferocity);


        //モササウルスの移動処理
        //9方向への移動の際、線形補完によって移動する。
        //実際のtransformの書き換えはWhaleLineスクリプト
        //WhaleLineにて、決定づけられた位置にlerpPositionを足している。
        //足す値を時間に応じて増加させる事で、継続移動を実現した。
        if (lerpTimer < lerpRange)
            lerpTimer = lerpTimer + Time.deltaTime;
        else
            lerpTimer = lerpRange;
        lerpPosition = Vector3.Lerp(testData[lastPosNum].Position, NowArea.Position, lerpTimer / lerpRange);


    }

    Vector3 damagePos;

    void OnTriggerEnter(Collider other)
    {

        //減速処理 多分矢が飛んでくる？
        if (other.tag == "Arrow")
        {

            gemcount++;

            //11/24 SE用の追加  (被弾)
            SeSystem.WhaleDA1 = true;

            //hittimer = 3f;
            //timer = 0f;
            //distanceHP -= 25;

            //被弾エフェクト //0110,otherTransform→This.transformに変更
            //0117 発生位置の計算を追加
            damagePos = Vector3.Lerp(transform.position, other.transform.position, 0.2f);
            Instantiate(damageEffect, damagePos, Quaternion.identity);
            //ins.transform.position = other.transform.position;

            //0924仕様変更分
            //1101 エナジーは一律1らしいです。
            //エナジー消費量の取得
            //atkEnergy = other.gameObject.getComponent<?>.energy

            hitCount += atkEnergy;
            stopCount += atkEnergy;
            if (distanceHP > 0/*distanceLimit < 4*/ && stopCount > 9 && hittimer <= 0.0f)
            {
                stopTimer = 3f;
                stopCount = 0;
                distanceLimit += 1;

                hittimer = 3f;
                timer = 0f;

                distanceHP -= 25;

                if (speed < 20.0f)
                {
                    W07.wallActive(distanceHP, 7.5f);
                }


                //Animation
                W04.DamageAnimSet();

            }

            //timer = timer + 3.0f;


            if (gemcount > 2)
            {
                GemDrop();
                gemcount = 0;
            }



        }
    }

    void FerocityUpdate(int ferocity)
    {
        if (ferocity > 12)
        {
            ferocity = 12;
        }

        if (beforeFerocity != ferocity)
        {
            beforeFerocity = ferocity;
            AtkCycleSet(ferocity);
            //return true;
        }

        //return false;
    }

    /*void AtkCycleSet(int distance, int ferocity)
    {
        //switch 変数は距離
        //予め用意した配列と凶暴性を使って攻撃周期をセットする。
        switch (distance)
        {
            case 100:
                atkCycle = atkCycle100m[ferocity];
                break;
            case 75:
                atkCycle = atkCycle75m[ferocity];
                break;
            case 50:
                atkCycle = atkCycle50m[ferocity];
                break;
            case 25:
                atkCycle = atkCycle25m[ferocity];
                break;
            //0mの場合攻撃しない。
            //15秒で距離を取る為、攻撃周期を15秒以上にすることで「攻撃しない」を実装。距離を取った段階で再セットされる。
            default:
                atkCycle = 20.0f;
                break;
        
    }*/

    void AtkCycleSet(int ferocity)
    {
        atkCycle = atkCycleNew[ferocity];
    }

    void ResetDistance(int getCount)
    {
        if (resetCount > 2)
        {
            resetCount = 2;
        }
        distanceHP = 100 - 25 * resetCount;
        accTimer = 12 - (3.0f * resetCount);

        W07.wallActive(distanceHP, 30.0f);

        resetCount++;
    }

    //GemDrop
    //宝石落下処理
    void GemDrop()
    {

        GameObject ins = Instantiate(gemObject, transform.position, Quaternion.identity);
        ins.GetComponent<G02DropGem>().PredictionPosition(linePositions);
        ins.GetComponent<G02DropGem>().backet = backet;

    }

    public  void ResetParam(GameObject player)
    {
        ferocity = 0;
        lineCount = 0;
        hitCount = 0;
        distanceHP = 100;
        resetCount = 0;
        
        //Start();

        //this.transform.position = new Vector3(player.transform.position.x + 100, player.transform.position.y, player.transform.position.z);

        while (this.transform.position.x < linePositions[lineCount].x)
        {

            lineCount++;
            //this.transform.position = linePositions[lineCount];
        }

        GetComponent<WhaleLine>().ResetLinePoint();

        
    }
}
