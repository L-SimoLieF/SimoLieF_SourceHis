using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//新仕様 魚ミサイル

public class F02FishMove : MonoBehaviour
{
    GameObject target;//player


    //ベジェ曲線用変数 (ベジェ曲線 = 使うと弾が曲がる技術)
    //開始位置、中間、終点
    Vector3 bezieStart;
    Vector3 bezieCenter;
    Vector3 bezieEnd;

    //曲線を描くための中間点。
    //新仕様ではとりあえず左上方向と右下方向にのみ存在。
    Vector3 bezieMiddle;

    //弾の軌道管理用変数群
    float bezieT;      //ベジェ曲線の進行度


    //ベジェ曲線の変化値。
    //(弾丸の速度を指定する為、ベジェによる座標移動を行っていない)
    Vector3 beziePos;

    int ferocity = 0;//凶暴性
    public float  fishSpeed;//速度
    bool reTarget;//接近時の補完
    bool homFlag;//追尾するかどうか。W02から受け取った値を使用。
    int a;//ベジェの進行を遅らせる為の物。試行錯誤の結果。今のところ使ってない。

    float timer;

    //補正用
    Vector3[] linePosition;
    int lineCount;

    //距離に応じて補正を変更するためのフラグ
    bool crossFlag;

    //距離に応じて到達時間を調整するために必要
    //W01から引っ張ってくる
    public float distanceHP;
    int distanceCount;

    //x(到達時間)のスコープが狭い為、BezieNewで使えない。
    //使う為のPublic変数。
    public float xBox;

    //2発目以降、垂直/平行方向に弾をバラけさせる為の変数。
    int dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //ベジェの進行管理。詳細はBezieControllerへ。
        //現状、カーブが終わった段階で補正処理に移行。
        //補正処理はターゲットの再設定。また追尾弾/ランダム弾でターゲット座標を変えている。

        //BezieControllerNew
        //小細工を弄したが結局全部使ってない。単純にBezieNewを呼んでるだけ。
        
        if (/*Vector3.Distance(target.transform.position, this.transform.position) > 30*/(bezieT/xBox) < 1.0f)
        {
            
            timer += Time.deltaTime;
            //進行の遅延処理。弾速との兼ね合いで必要かもしれない。
            //今は使ってない。
            if (timer > 2f)
            {
                timer = 0;
                //BezieController();
                //Constructer();

            }
            //BezieController();
            BezieControllerNew();
           
        }
        else
        {

            BezieControllerNew();
            //ターゲットの再設定
            if(reTarget == false)
            {
                //
                //Debug.Log("aaaaaaaaaa");
                //ResetTarget(homFlag);
                //Constructer();
            }
           
            if (homFlag == true)
            {
                //beziePos = new Vector3(target.transform.position.x + 10, target.transform.position.y, target.transform.position.z);
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position + new Vector3(5,0,0) - this.transform.position), 30.0f * Time.deltaTime);
                //transform.LookAt(target.transform.position);

                //BezieController();
                //BezieControllerNew();
                /*Vector3 s = target.transform.position - this.transform.position;
                Quaternion q = Quaternion.LookRotation(s);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.3f);*/
            }
            else
            {
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(beziePos - this.transform.position), 20.0f * Time.deltaTime);
                //BezieController();
            }
        }

        //弾の前進処理。ただ速度で前に進ませてるだけ。
        //ベジェ曲線上の座標に対しLookatをしている為、曲線を描いて飛んでいる。
        //transform.position += transform.forward * fishSpeed * Time.deltaTime;

        //transform.position += transform.forward * fishSpeed;
     }




    //わぁ、同じ名前の関数だぁ。
    //多態性を使ってます。追加されている変数は回数と追尾するか否か。
    //それぞれW02で指定。本体はこの関数の最後でコール。
    public void Constructer(GameObject player, int a, bool b,Vector3[] array,int randomDir)
    {
        //追尾するかどうか。
        homFlag = b;

        //Lineposition
        linePosition = array;

        dir = randomDir;

        //1回目は直進、2回目は左上方向、3回目は右下方向に曲がる。
        if (a == 0)
        {
            bezieMiddle = new Vector3(0, 0, 0);
        }
        else if (a == 1)
        {
            bezieMiddle = new Vector3(0, 20, 20);
        }
        else if (a == 2)
        {
            bezieMiddle = new Vector3(0, -20, -20);
        }

        if(homFlag == false)
        {
            bezieMiddle = new Vector3(0, Random.Range(-10, 10), Random.Range(-10, 10));
        }

        //本体部分のコール。
        Constructer(player);
    }


    //初期化関数。コールされるのは同スクリプトConstructer関数内。
    //W02で呼んでいるのは同名の別の関数。詳しくは別の方で記載。
    //基本処理はここ。
    void Constructer(GameObject player)
    {

        //W02から渡されたplayerをtargetとして指定。
        target = player;

        //速度計算。式は仕様書から。
        fishSpeed = 15 + (ferocity / 2);
        if (fishSpeed > 23)
            fishSpeed = 23;

        //ベジェの中間点指定。同名別関数で行っている為コメントアウト。
        //bezieMiddle = new Vector3(0, 30,0);

        //ベジェ曲線用の座標指定。

        //開始地点
        bezieStart = transform.position;

        //終了地点
        //速度に合わせた偏差算出
        //上手くいかないのでとりあえず使ってない。
        //float x = 100f/*Vector3.Distance(this.transform.position, target.transform.position)*/;
        //x = x / fishSpeed * 15;


        //見せ球用 trueは本命
        float y, z;
        if (homFlag == true)
        {
            //終了地点もズラした方が弾幕っぽい説浮上したので、それ用に組む。
            y = Random.Range(-1, 1);
            z = Random.Range(-1, 1);
        }
        //見せ弾。y,z方面にランダムな値を取り、目標座標のズレを表現する。
        //弾幕毎で固定。W02で方向を決定している。
        else
        {
            if (dir == 0)
            {
                y = Random.Range(-20, 20);
                z = 0;
            }
            else
            {
                y = 0;
                z = Random.Range(-20, 20);
            }
        }




        //偏差込みの終了地点。xが加算されるのはplayerの前進を加味しての物。
        Vector3 aaa /*= new Vector3(target.transform.position.x + 100, target.transform.position.y + y, target.transform.position.z + z)*/;

        //CenterLineの移動を考慮した偏差補正
        //aaa = aaa + ReviseBezieEnd();



        //aaa = new Vector3(aaa.x + 7, aaa.y + y, aaa.z + z);




        //中間地点 Lerpによって、始点と終点を結んだ線の5割地点を算出、それに大きく外れた中間点を加算。
        //結果曲がる様になる。詳しくはベジェ曲線の仕組みを調べろ。


        Vector3 t1 = new Vector3(target.transform.position.x, 0, 0);
        Vector3 t2 = new Vector3(transform.position.x, 0, 0);

        if (Vector3.Distance(t1, t2) > 50)
        {
            aaa = PredictionPosition();
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;
            crossFlag = false;


            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);

            transform.LookAt(Vector3.Lerp(a, b, 0.5f));
        }
        else
        {
            //+fishSpeed = fishSpeed * 2;
            aaa = PredictionPosition();
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + new Vector3(10,0,0);
            crossFlag = true;
            transform.LookAt(bezieEnd);
           
        }

        //Debug.Log(crossFlag + "/" + (t1 - t2).magnitude);

        //進行度のリセット。
        bezieT = 0;
    }



    //public void Constructer()
    //{
    //    //開始地点
    //    bezieStart = transform.position;

    //    //終了地点
    //    //速度に合わせた偏差算出
    //    //
    //    Vector3 q = new Vector3(transform.position.x, 0, 0);
    //    Vector3 e = new Vector3(target.transform.position.x, 0, 0);

    //    float x = Vector3.Distance(q, e);
    //    x = x / fishSpeed * 15;

    //    //終了地点もズラした方が弾幕っぽい説浮上したので、それ用に組む。
    //    float y = Random.Range(-3, 3);
    //    float z = Random.Range(-3, 3);

    //    //偏差込みの終了地点。xが加算されるのはplayerの前進を加味しての物。
    //    Vector3 aaa = new Vector3(target.transform.position.x + 10, target.transform.position.y + y, target.transform.position.z + z);

    //    //CenterLineの移動を考慮した偏差補正
    //    aaa = aaa + ReviseBezieEnd();
    //    //aaa = aaaaa();

    //    bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);

    //    //中間地点 Lerpによって、始点と終点を結んだ線の5割地点を算出、それに大きく外れた中間点を加算。
    //    //結果曲がる様になる。詳しくはベジェ曲線の仕組みを調べろ。
    //    bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;

    //}


    //ベジェ曲線の計算関数
    //シンプルな処理。
    void BezieController()
    {
        bezieT += 0.2f * Time.deltaTime;
        Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
        Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
        beziePos = Vector3.Lerp(a, b, bezieT);


        //速度反映用の処理
        //ベジェ曲線による座標書き換えではなく、移動する座標に向けて直進させる処理
        //弾丸の速度変化を実現する為に必要だった。(厳密には座標の直接書き換えでも出来ると思う。考えるのが面倒だった。)
        //transform.LookAt(beziePos);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(beziePos- this.transform.position),35.0f * Time.deltaTime);
        //Vector3 c = beziePos - this.transform.position;
        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(c), 1f * Time.deltaTime);


        if (crossFlag == false)
        {
            Vector3 s = beziePos - this.transform.position;
            Quaternion q = Quaternion.LookRotation(s);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.2f * Time.deltaTime);
            Debug.Log("遠いよ!");
        }
        else if (crossFlag == true)
        {
            Vector3 s = beziePos - this.transform.position;
            Quaternion q = Quaternion.LookRotation(s);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.015f);
            Debug.Log("近いよ!");
        }

    }

    //ターゲットの再設定
    //Constructer関数と同じだが、紆余曲折あって最後に一回Bezieを呼んでいる。
    //transform.Lookatをしたかった。BeziePosの計算が関数内の為、呼んだ方が良いと判断。
    //現状は関係ない。
    void ResetTarget(Vector3 targetPos)
    {
        //Vector3 aaa = new Vector3(target.transform.position.x + 5, target.transform.position.y, target.transform.position.z);
        bezieStart = transform.position;
        bezieEnd = Vector3.Lerp(transform.position, targetPos, 1.0f);
        bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f);
        bezieT = 0;
        reTarget = true;
        BezieController();
    }

    //Constructer関数と同様、同名別関数。
    //ホーミングか、ランダムかで終了地点の座標が違う。
    //それを変える為に用意した。
    //最後にResetTargetをコール。
    void ResetTarget(bool a)
    {
        Vector3 aaa;
        //ホーミングする場合
        if (a == true)
        {
            float d = 100f;
            d = d / fishSpeed * 15;

            aaa = new Vector3(target.transform.position.x + d, target.transform.position.y, target.transform.position.z);
        }
        //しない場合。playerの付近からランダムに設定。
        else
            aaa = new Vector3(target.transform.position.x + 10, target.transform.position.y + Random.Range(-5, 5), target.transform.position.z + Random.Range(-5, 5));
        //算出した座標をResetTargetに渡している。
        ResetTarget(aaa);

    }

    //CenterLineの情報
    //x座標から現在の区間を取得、現座標から次の座標までの傾きを取得、補正に使用。
    //傾きに対して速度を掛ければ直撃する？
    //補正後の座標にRandom.rangeでランダム性を追加？

    //X座標に対して傾きは一定。再設定時の座標から現在位置における傾きを算出する必要がある。
    //各弾で算出するのはスマートじゃない。が、各弾で算出するのが一番速いという話もある。
    //Findは遅いらしい。オブジェクト参照だけW02で渡す方式か。
    //何を渡す？配列？オブジェクトデータ？
    //W02のLinePositionに対する参照を用意する方が良いのでは？
    //LinePositionの参照も生成時に渡す方式で行きましょう。

    void SetLinepos()
    {
        //positionの参照
        //LinePos.xがpos.xを超えるまで。
        //超えた瞬間のcountを傾き計算に使用。
        //countとcount + 1。

        int count = 0;

        while (true)
        {
            if (this.transform.position.x > linePosition[count].x)
            {
                count++;
            }
            else
            {
                lineCount = count;
                break;
            }
        }

    }


    //Revise = 補正
    Vector3 ReviseBezieEnd()
    {
        SetLinepos();

        Vector3 a = new Vector3(0, linePosition[lineCount].y, linePosition[lineCount].z);
        Vector3 b = new Vector3(0, linePosition[lineCount + 1].y, linePosition[lineCount + 1].z);

        Vector3 c = b - a;
        c = c.normalized;

        //c = 傾き
        //これに着弾までの時間で進む距離を掛ける。
        //着弾までの時間 = xの差 / 速度。

        //速度に合わせた偏差算出
        //
        Vector3 q = new Vector3(transform.position.x, 0, 0);
        Vector3 e = new Vector3(target.transform.position.x, 0, 0);

        float x = Vector3.Distance(q, e);
        x = Mathf.Abs(x);
        x = x / fishSpeed * 15;

        //x = 着弾までの時間
        //x * fishSpeed * time.deltatime

        //c = c * (x * fishSpeed * Time.deltaTime);


        return c;
    }

    //Prediction = 予測
    //プレイヤーの未来位置を算出する関数。
    Vector3 PredictionPosition()
    {
      

        //Vector3 n = Vector3.Lerp(linePosition[count], linePosition[count + 1], linePosition[count + 1].x / this.transform.position.x);

        Vector3 q = new Vector3(transform.position.x, transform.position.y,transform.position.z);
        Vector3 e = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);

        float x = Vector3.Distance(q, e);
        x = Mathf.Abs(x);

        //固定値を使います。x = 掛かる時間
        x = 4f/*x / ((fishSpeed + 15) * Time.deltaTime)*/;//直撃までに掛かる時間
        x = x - ( (100.0f - distanceHP) / 25.0f) *0.25f;

        //Debug.Log(x);
        //Debug.Log(distanceHP);

        xBox = x;


       


        float r = target.transform.position.x + x * 15 /** Time.deltaTime*/;//直撃時のplayerのx座標
        //Debug.Log(r);

        //centerLine上の位置を検索。
        //[count].x < r < [count + 1]になるようなcountを算出。
        int count = GetLinePosCount(r);
        //現在地の割合を算出
        //全体の差に対し、現在どの程度の位置にいるのかを差の比較で算出
        float u = linePosition[count + 1].x - linePosition[count].x;
        float y = linePosition[count + 1].x - r;
        Vector3 n = Vector3.Lerp(linePosition[count + 1],linePosition[count], y/u);//lerp,座標の算出 CenterLine上の目標位置

        //nに対してplayerのx座標から導くCenterLine上からのズレを加算すれば補正が完了する。

        //player.xを基にCenterLine.x
        //centerLineからplayerがどれだけ離れているかの算出 = SA
        // n に SAを足せば操作無しでの着弾位置が算出できるって事？？？？？


        //////////////playerの座標編///////////////

        int k = GetLinePosCount(target.transform.position.x);
        //現在地の割合を算出
        //全体の差に対し、現在どの程度の位置にいるのかを差の比較で算出
        float g1 = linePosition[k + 1].x - linePosition[k].x;
        float g2= linePosition[k + 1].x - target.transform.position.x;
        Vector3 f = Vector3.Lerp(linePosition[k + 1], linePosition[k], g2 / g1);//lerp,座標の算出 CenterLine上のplayerの位置

        //Debug.Log(f);

        Vector3 offset = target.transform.position - f;//playerの操作によって、centerlineから現座標がどれだけズレているか

        n = n + offset;//ズレ分を目標位置に足す。



        //算出方法の変更

        Vector3 l = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 h = n;

        float j = Vector3.Distance(l, h);

        float missileSpeed = j / x;

        fishSpeed = missileSpeed;


        return n;

    }

    //近い時のやつ
    //時間指定だと無理だから単純に速度で飛ばすためのやつ
    Vector3 PredictionPosition(float speed)
    {


        //Vector3 n = Vector3.Lerp(linePosition[count], linePosition[count + 1], linePosition[count + 1].x / this.transform.position.x);

        Vector3 q = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        Vector3 e = new Vector3(target.transform.position.x,target.transform.position.y,target.transform.position.z);

        float x = Vector3.Distance(q, e);
        x = Mathf.Abs(x);

        //固定値を使いません。x =
        x = x / (15 * Time.deltaTime);//直撃までに掛かる時間(
        //x = x - ((100.0f - distanceHP) / 25.0f) * 0.25f;

        //Debug.Log(x);
        //Debug.Log(distanceHP);

        //xBox = x;





        float r = target.transform.position.x + x * 15 * Time.deltaTime;//直撃時のplayerのx座標
        //Debug.Log(r);

        //centerLine上の位置を検索。
        //[count].x < r < [count + 1]になるようなcountを算出。
        int count = GetLinePosCount(r);
        //現在地の割合を算出
        //全体の差に対し、現在どの程度の位置にいるのかを差の比較で算出
        float u = linePosition[count + 1].x - linePosition[count].x;
        float y = linePosition[count + 1].x - r;
        Vector3 n = Vector3.Lerp(linePosition[count + 1], linePosition[count], y / u);//lerp,座標の算出 CenterLine上の目標位置

        //nに対してplayerのx座標から導くCenterLine上からのズレを加算すれば補正が完了する。

        //player.xを基にCenterLine.x
        //centerLineからplayerがどれだけ離れているかの算出 = SA
        // n に SAを足せば操作無しでの着弾位置が算出できるって事？？？？？


        //////////////playerの座標編///////////////

        int k = GetLinePosCount(target.transform.position.x);
        //現在地の割合を算出
        //全体の差に対し、現在どの程度の位置にいるのかを差の比較で算出
        float g1 = linePosition[k + 1].x - linePosition[k].x;
        float g2 = linePosition[k + 1].x - target.transform.position.x;
        Vector3 f = Vector3.Lerp(linePosition[k + 1], linePosition[k], g2 / g1);//lerp,座標の算出 CenterLine上のplayerの位置

        //Debug.Log(f);

        Vector3 offset = target.transform.position - f;//playerの操作によって、centerlineから現座標がどれだけズレているか

        n = n + offset;//ズレ分を目標位置に足す。



        return n;

    }



    //CenterLineに置ける位置を取得する。
    //与えられたx座標に対し、要素番号a < x < 要素番号a+1となるaを返す。
    int GetLinePosCount(float xPos)
    {
        int count = 0;
        for (int i = 0; i < linePosition.Length; i++)
        {
            if (xPos > linePosition[i].x)
            {
                count = i;
            }
            else
                break;

        }

        return count;

    }


    //ベジェ曲線の進行管理用関数。
    //新仕様。到達時間が決定するようになったため、座標移動をベジェで行えるようになった。

    void BezieControllerNew()
    {
        Vector3 a; 
        Vector3 b;

        if (crossFlag == false)
        {
            bezieT += 1.0f * Time.deltaTime;
            a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            transform.position = Vector3.Lerp(a, b, bezieT / xBox);


            if (bezieT < 3.0f)
            {
                //Vector3 s = target.transform.position - this.transform.position;
                Vector3 s = Vector3.Lerp(a, b, bezieT + Time.deltaTime / xBox)/*target.transform.position*/ - this.transform.position;
                Quaternion q = Quaternion.LookRotation(s);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.05f);
                Debug.Log("遠いよ!");
            }
            else
            {
                Vector3 s = target.transform.position - this.transform.position;
                //Vector3 s = Vector3.Lerp(a, b, bezieT + Time.deltaTime / xBox)/*target.transform.position*/ - this.transform.position;

                Quaternion q = Quaternion.LookRotation(s);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.05f);
                Debug.Log("近いよ!");
            }



        }
        else
        {
            //bezieT = 3.1f;
            bezieT += 1.0f * Time.deltaTime;
            a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            transform.position = Vector3.Lerp(a, b, bezieT/2);

            //transform.position += transform.forward * 15.0f * Time.deltaTime;

            Vector3 s = target.transform.position - this.transform.position;
            //Vector3 s = Vector3.Lerp(a, b, bezieT + Time.deltaTime / xBox)/*target.transform.position - this.transform.position;*/

            Quaternion q = Quaternion.LookRotation(s);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.05f);
        }


        //速度反映用の処理
        //ベジェ曲線による座標書き換えではなく、移動する座標に向けて直進させる処理
        //弾丸の速度変化を実現する為に必要だった。(厳密には座標の直接書き換えでも出来ると思う。考えるのが面倒だった。)
        //transform.LookAt(beziePos);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(beziePos- this.transform.position),35.0f * Time.deltaTime);
        //Vector3 c = beziePos - this.transform.position;
        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(c), 1f * Time.deltaTime);

        

    }

}
