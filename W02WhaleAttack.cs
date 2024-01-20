using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ランダム算出から、凶暴性を引く。
//felocity = felocity - 攻撃関数
//3回のコール、IEnumeratorで行う必要がある
//3個の選出用関数。選出用関数の返り値をintにして、IEnumerator内でfelocity -= を行う。
//選出用関数内で発射関数をコール。W02もくじらに付随してるから問題ない。

//08/25 追記
//攻撃システムのアップデート。WhaleAttack2nd関数へ。
//11/01 追記
//攻撃システムのアップデート。WhaleAttack3rdへ。

public class W02WhaleAttack : MonoBehaviour
{
    public GameObject player;
    public GameObject gem;

    //弾幕の発射間隔((新システム)移動開始から発射までの時間)
    public float atkInterval = 0.5f;
    //何か知らんけど上手くいかなかったので使ってない。
    //WhaleAttack2ndの中、WaitforSecondsを変えてください。

    //CenterLine用
    public GameObject centerLine;
    Vector3[] linePositions;
    int lineCount;

    W01WhaleMoving W01;

    int randomDir;

    W04WhaleAnimator W04;

    // Start is called before the first frame update
    void Start()
    {
        //Player Objectのアタッチ
        player = GameObject.Find("Board");

        //攻撃偏差用のLinePositionのアタッチ
        //CenterLine用
        centerLine = GameObject.Find("CenterLine");
        linePositions = new Vector3[centerLine.GetComponent<LineRenderer>().positionCount];
        centerLine.GetComponent<LineRenderer>().GetPositions(linePositions);

        W01 = GetComponent<W01WhaleMoving>();

        W04 = GetComponent<W04WhaleAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack01(player);
        }
        if (Input.GetKeyDown(KeyCode.H))
            HorizontalShotBase(11);
    }


    //攻撃処理
    //本採用ver. W02のIEnumerator関数をコール。この関数自体はW01で呼び出している。
    //(別クラスからのコールに正常に反応しない可能性あり。)
    //新攻撃システム --- Whale Attack2nd.
    //新攻撃システム --- Whale Attack3rd.
    public void Attack(int ferocity)
    {
        //StartCoroutine("WhaleAttack", ferocity);
        //StartCoroutine("WhaleAttack2nd");
        StartCoroutine("WhaleAttack3rd",ferocity);
    }


    //攻撃処理
    //3度目の正直。0.5秒ずつに1回ずつ、計三回の弾幕発射を行う。
    //奇しくも1回目と似たような処理。
    IEnumerator WhaleAttack3rd(int ferocity)
    {
        //ポジションチェンジの待機時間。
        yield return new WaitForSeconds(3f);
        //Debug.Log(Vector3.Distance(player.transform.position, transform.position));

        SeSystem.WhaleMissile = true;

        //Barrage3rd(ferocity);

        randomDir = Random.Range(0, 2);

        //Barrage3rdをコール。
        //引数は凶暴性と回数。

        yield return new WaitForSeconds(atkInterval);
       
        Barrage3rd(ferocity,0);

        yield return new WaitForSeconds(atkInterval);
        
        Barrage3rd(ferocity,1);

        yield return new WaitForSeconds(1.2f);
       
        Barrage3rd(ferocity,2);

       

    }

    //Barrage3rd(Barrage = 弾幕 --3rd)
    //仕様書に記載。
    //最低弾数がammo,算出の為の式がadd
    void Barrage3rd(int ferocity,int count)
    {

        if (W04.damageFlag == true)
            return;

        int ammo = 1;
        int add = 1 * (ferocity / 6);
        ammo = ammo + add;

        //それぞれ一発は固定の為、別枠でコール。
        //Trueはホーミング、falseはランダム弾の意。
        //2発分を引く。
        Shot3rd(count,true);
        //Shot3rd(count,false);
        ammo -= 1;

        //弾幕の内訳決定
        //残弾分、繰り返すループ。
        for(int i = 0; i < ammo; i++)
        {
            int a = Random.Range(0, 5);
            //homing
            if(a < 2) 
            {
                Shot3rd(count, false);
            }
            //random
            else
            {
                Shot3rd(count, false);
            }


        }
    }

    public Transform missileParent;

    //3代目弾幕システム用生成関数。
    //弾を作って、弾のスクリプトに情報を渡してるだけ。
    void Shot3rd(int count,bool d)
    {
        //生成位置をランダムにする事で、弾幕っぽさの演出
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + Random.Range(-3, 3), transform.position.z + Random.Range(-5, 5));

        GameObject ins = Instantiate(gem,pos, Quaternion.identity, missileParent);

        ins.GetComponent<F02FishMove>().distanceHP = W01.distanceHP;
        //F02 引数は左からターゲット。弾幕の回数、ホーミングするか否か。
        ins.GetComponent<F02FishMove>().Constructer(player,count,d,linePositions,randomDir);
    }

    //8月更新 新攻撃システム
    //移動の時間を稼ぐ為にWaitforSeconds,W01側で制御しても良い。
    //自機狙いと、自機から一定範囲内をランダムに狙う直進弾の構成。
    IEnumerator WhaleAttack2nd()
    {

        //移動時間 lerpRangeと同じ数字
        yield return new WaitForSeconds(3f);

        //Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //追尾弾 渡してるのは発射位置
        Homing(transform.position);

        //直進弾 forの回数が弾の数
        //渡してるのは発射位置、y軸z軸に乱数を挟む事で弾幕っぽくした。
        for (int i = 0; i < 12; i++)
        {
            Straight(new Vector3(transform.position.x, player.transform.position.y + Random.Range(-20, 20), transform.position.z + Random.Range(-30, 30)));
        }
    }

    //------------------------------
    //WhaleAttack2ndで用いた関数群
    //弾丸の生成、生成後にF01.Constructer関数で初期設定を行っている。
    //NowArea.Numberはクジラの現在位置。軌道を曲げるのに必要。0を渡してるのは多態性の為。

    //発射位置を渡して追尾させる。
    void Homing(Vector3 pos)
    {
        GameObject ins = Instantiate(gem, pos, Quaternion.identity);
        ins.GetComponent<F01FishMove>().Constructer(player, this.gameObject.GetComponent<W01WhaleMoving>().NowArea.Number);
    }
    //発射位置を渡して、そこから直進
    void Straight(Vector3 pos)
    {
        GameObject ins = Instantiate(gem, pos, Quaternion.identity);
        ins.GetComponent<F01FishMove>().Constructer(player, this.gameObject.GetComponent<W01WhaleMoving>().NowArea.Number, 0);
    }

    //-------------------------------



  
    //-------------------------------- 以下、旧攻撃システム

    //攻撃処理の基礎部分
    //3弾幕、それぞれランダム。WaitforSecondsを使う為にIEnumerator
    IEnumerator WhaleAttack(int ferocity)
    {
        //3回攻撃
        //各Attack内で弾幕の決定、及び発射を行い、使った凶暴性の量を返す。
        //返ってきた凶暴性を、全体から引いて次の弾幕の決定に利用
        //(要相談) 3回打ち切った時点で0になるように使い切る。

        ferocity = ferocity - FirstAttack(ferocity);

        yield return new WaitForSeconds(atkInterval);

        ferocity = ferocity - SecondAttack(ferocity);

        yield return new WaitForSeconds(atkInterval);

        ferocity = ferocity - ThirdAttack(ferocity);


        //超過、もしくは使い切れなかった場合にメッセージを表示(デバッグ用)
        if (ferocity != 0)
        {
            Debug.LogError("ferocity is valied ---SimoLieF");
        }

        //Homing
        //新攻撃システムのテスト用 WhaleAttack2ndに移植済み
        //HomingBase();


    }


    

    //各回の攻撃用関数
    //(06/16) 弾幕プールが仮置きの為、仕組みだけ記載。
    //Caseの中にそれぞれに対応した弾幕関数を記述する。
    int FirstAttack(int ferocity)
    {
        //使うfelocityを決定して、そのSwitchの中で放つ弾幕を決定する→第一案
        //問題点:felocity別の選択数は変わらないが、同felocityの物が多ければ多いほど確率が減る。

        int rund = Random.Range(0, ferocity + 1);

        switch (rund)
        {
            case 0:
                Debug.Log("0 Attack");
                break;
            case 1:
                //水平上方向
                //HorizontalShot1Line(11, 3, 10);
                Debug.Log("1 Attack");
                break;
            case 2:
                //HorizontalShot2Line(19, 2, 5);
                Debug.Log("2 Attack");
                break;
            case 3:
                Debug.Log("3 Attack");
                break;
            case 4:
                //HorizontalShot4Line(19, 0,5);
                Debug.Log("4 Attack");
                break;
            case 5:
                Debug.Log("5 Attack");
                break;
            case 6:
                Debug.Log("6 Attack");
                break;
            case 7:
                Debug.Log("7 Attack");
                break;
            case 8:
                Debug.Log("8 Attack");
                break;
            case 9:
                Debug.Log("9 Attack");
                break;
            case 10:
                Debug.Log("10 Attack");
                break;

            //felocity >=11 の時
            default:
                Debug.Log("11 Attack");
                break;
        }

        //HorizontalShot4Line(19, 0, 5);
        //HomingBase();

        //返り値は使ったコスト(凶暴性)の数
        return rund;
    }

    int SecondAttack(int ferocity)
    {
        int rund = Random.Range(0, ferocity + 1);

        switch (rund)
        {
            case 0:
                Debug.Log("0 Attack");
                break;
            case 1:
                Debug.Log("1 Attack");
                break;
            case 2:
                Debug.Log("2 Attack");
                break;
            case 3:
                Debug.Log("3 Attack");
                break;
            case 4:
                Debug.Log("4 Attack");
                break;
            case 5:
                Debug.Log("5 Attack");
                break;
            case 6:
                Debug.Log("6 Attack");
                break;
            case 7:
                Debug.Log("7 Attack");
                break;
            case 8:
                Debug.Log("8 Attack");
                break;
            case 9:
                Debug.Log("9 Attack");
                break;
            case 10:
                Debug.Log("10 Attack");
                break;

            //felocity >=11 の時
            default:
                Debug.Log("11 Attack");
                break;
        }


        //HorizontalShot4Line(19, -6, 5);
        //HomingBase();

        return rund;
    }

    int ThirdAttack(int ferocity)
    {
        //凶暴性(felocity)の値を使い切らなければいけない以上、3回目の攻撃は与えられたfelocityのよってのみ決定付けられる。
        //その為、前二回で行っていたrandom処理は必要ない。


        switch (ferocity)
        {
            case 0:
                Debug.Log("0 Attack");
                break;
            case 1:
                Debug.Log("1 Attack");
                break;
            case 2:
                Debug.Log("2 Attack");
                break;
            case 3:
                Debug.Log("3 Attack");
                break;
            case 4:
                Debug.Log("4 Attack");
                break;
            case 5:
                Debug.Log("5 Attack");
                break;
            case 6:
                Debug.Log("6 Attack");
                break;
            case 7:
                Debug.Log("7 Attack");
                break;
            case 8:
                Debug.Log("8 Attack");
                break;
            case 9:
                Debug.Log("9 Attack");
                break;
            case 10:
                Debug.Log("10 Attack");
                break;

            //felocity >=11 の時
            default:
                Debug.Log("11 Attack");
                break;
        }

        //HorizontalShot5Line();
        //HomingBase();

        return ferocity;
    }


    ///////////////////弾幕用関数↓

    //水平中
    //本プロジェクトでの進行方向がx方面だった為、forの中のVector3の数値を弄りました。
    void HorizontalShotBase(int num)
    {
        Vector3 pos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);

        for (int x = num / 2; x > (num / 2) - num; x--)
        {
            GameObject ins = Instantiate(gem, pos + new Vector3(0, 0, x * 10), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }
    }

    //水平一列(個数、高さ、間が引数)
    void HorizontalShot1Line(int num, int height, int space)
    {
        Vector3 pos = new Vector3(transform.position.x, player.transform.position.y + height, transform.position.z);

        for (int x = num / 2; x > (num / 2) - num; x--)
        {
            GameObject ins = Instantiate(gem, pos + new Vector3(0, 0, x * space), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }
    }

    //水平二列
    void HorizontalShot2Line(int num, int height, int space)
    {
        HorizontalShot1Line(num, height, space);
        HorizontalShot1Line(num, height + height, space);
    }

    //水平三列
    //プール表に一つしかなかった為直接設定
    void HorizontalShot3Line()
    {
        HorizontalShot1Line(7, 2, 5);
        HorizontalShot1Line(11, 0, 5);
        HorizontalShot1Line(7, -2, 5);
    }

    //水平四列
    void HorizontalShot4Line(int num, int height, int space)
    {
        for (int i = 0; i < 4; i++)
        {
            HorizontalShot1Line(num, height, space);
            height = height + 2;
        }
    }

    //水平五列
    //プール表に一つしかなかった為
    void HorizontalShot5Line()
    {
        HorizontalShot1Line(7, 4, 5);
        HorizontalShot1Line(7, -4, 5);
        HorizontalShot1Line(11, 2, 5);
        HorizontalShot1Line(11, -2, 5);
        HorizontalShot1Line(19, 0, 5);
    }


    //Homingのテスト
    //追尾弾と、その周りに幾つかの直線弾。
    void HomingBase()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Homing(pos);
        for (int i = 0; i < 12; i++)
        {
            Straight(new Vector3(transform.position.x, player.transform.position.y + Random.Range(-20,20) ,transform.position.z + Random.Range(-30,30)));
        }
    }

  


    /////////////////////////弾幕用関数 ここまで。



    //草案シリーズ
    //弾幕案1 水平方向にばら撒く奴。
    public void HorizontalShotTest()
    {
        int a = 10;
        for (int x = a / 2; x > (a / 2) - a; x--)
        {
            GameObject ins = Instantiate(gem, transform.position + new Vector3(x * 10, 0, 0), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }

    }
    //HorizontalShotのオーバーライド。引数に個数を持つ。
    void HorizontalShotTest(int num)
    {
        int a = num;

        for (int x = a / 2; x > (a / 2) - a; x--)
        {
            GameObject ins = Instantiate(gem, transform.position + new Vector3(x * 2, 0, 0), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }
    }

    //攻撃 仮組
    void Attack01(GameObject player)
    {
        GameObject ins = Instantiate(gem, transform.position, Quaternion.identity);
        //ins.transform.LookAt(player.transform);
        //Vector3 dir = player.transform.position - ins.transform.position;
        //dir = new Vector3(dir.x, dir.y, dir.z + 50f);
        //ins.GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
        ins.GetComponent<F01FishMove>().target = player;
    }
}
