using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02DropGem : MonoBehaviour
{
    float timer;
    Vector3 dir;

    bool getFlag;
    GameObject player;

    float lerpTimer;

    public GameObject backet;
    public GameObject gem2;

    Vector3 randomDir;

    Vector3 bezieStart;
    Vector3 bezieCenter;
    Vector3 bezieEnd;
    float bezieT;

    bool crossFlag;
    GameObject target;

    float xBox;
    float dropTimer;

    Vector3[] linePos;

    // Start is called before the first frame update
    void Start()
    {
        //dir = new Vector3(0f, 0f, Random.Range(-2, 2));

        //dir = player.transform.position - this.transform.position;

        
    }

    // Update is called once per frame
    void Update()
    {
        //timer += Time.deltaTime;

        if ((bezieT / xBox) <= 1.5f)
        {
            BezieController();
        }

        /*if (timer < 30.0f)
        {
            //transform.Rotate(new Vector3(8f, 15f, 10f));
            //transform.position += dir * Time.deltaTime;

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), 360.0f * Time.deltaTime);

            //transform.position =  * 4f * Time.deltaTime;

            //lerpTimer += Time.deltaTime * 0.01f;
            //transform.position = Vector3.Lerp(this.transform.position, dir, lerpTimer/5);
           



            if (/*Vector3.Distance(this.transform.position, dir) > 0.5f*//*(bezieT/xBox) < 1.0f){

                //BezieController();

                /*transform.LookAt(dir);
                this.transform.position += transform.forward * 5f * Time.deltaTime;

            }



            

           
        }*/

        if (timer > 50f)
            Destroy(this.gameObject);
      
        if(getFlag == true)
        {
            GameObject ins = Instantiate(gem2, backet.transform.position, Quaternion.identity);
            ins.transform.parent = backet.transform;
            Destroy(this.gameObject);

            //GameObject ins = Instantiate(this.gameObject,backet.transform.position,Quaternion.identity);
            //ins.transform.localScale = ins.transform.localScale / 2;
            //ins.transform.parent = backet.transform;
            //ins.GetComponent<G02DropGem>().enabled = false;
            //this.transform.position = backet.transform.position;/*new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 2)*/;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SeSystem.reer = true;
            SeSystem.GemGet = false;
            SeSystem.GemGet = true;
            //Destroy(this.gameObject);
            Debug.Log("get");
            getFlag = true;
            player = other.gameObject;
            //スコア用
            JewelScore.jewelCount += 1;
        }
    }

    //Prediciton = 予測
    public void PredictionPosition(Vector3[] linePosition)
    {
        target = GameObject.Find("Board");
        //linePos = new Vector3[linePosition.Length];
        //linePos = linePosition;
        //Vector3 n = Vector3.Lerp(linePosition[count], linePosition[count + 1], linePosition[count + 1].x / this.transform.position.x);
        //Vector3 q = new Vector3(transform.position.x, 0, 0);
        //Vector3 e = new Vector3(target.transform.position.x, 0, 0);

        float x /*= Vector3.Distance(q, e)*/;
        /*x = Mathf.Abs(x);
        x = x / (15 * Time.deltaTime);//直撃までに掛かる時間*/


        Vector3 t1 = new Vector3(target.transform.position.x, 0, 0);
        Vector3 t2 = new Vector3(transform.position.x, 0, 0);


        if (Vector3.Distance(t1,t2) > 50.0f)
        {
            x = 6.0f;
        }
        else
            x = 2.0f;
        xBox = x;


        float r = target.transform.position.x + x * 15 * Time.deltaTime;//直撃時のplayerのx座標

        r = r + 60.0f;

        //centerLine上の位置を検索。
        //[count].x < r < [count + 1]になるようなcountを算出。
        int count = GetLinePosCount(r,linePosition);
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

        int k = GetLinePosCount(target.transform.position.x,linePosition);
        //現在地の割合を算出
        //全体の差に対し、現在どの程度の位置にいるのかを差の比較で算出
        float g1 = linePosition[k + 1].x - linePosition[k].x;
        float g2 = linePosition[k + 1].x - target.transform.position.x;
        Vector3 f = Vector3.Lerp(linePosition[k + 1], linePosition[k], g2 / g1);//lerp,座標の算出 CenterLine上のplayerの位置

        //Debug.Log(f);

        Vector3 offset = target.transform.position - f;//playerの操作によって、centerlineから現座標がどれだけズレているか

        n = n + offset;//ズレ分を目標位置に足す。

        //transform.LookAt(n);

        dir = n;

        //randomDir = RandomShoot(dir);
        BezieSet(linePosition);

    }


    int GetLinePosCount(float xPos,Vector3[] linePosition)
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

    Vector3 RandomShoot(Vector3 v)
    {
        float a = Random.Range(0, 3);
        float b = Random.Range(-3, 3);
        float c = Random.Range(-5, 5);

        return new Vector3(v.x + a, v.y + b, v.z + c);
    }




    //bezieMiddleの中 = 曲線の大きさ
    //y,z = 着弾点の範囲

    void BezieSet(Vector3[] linePosition)
    {

        //ベジェの中間点指定。同名別関数で行っている為コメントアウト。
        Vector3 bezieMiddle = new Vector3(Random.Range(0,10), Random.Range(-3,3),Random.Range(-3,3));

        //ベジェ曲線用の座標指定。

        //開始地点
        bezieStart = transform.position;




        float y = Random.Range(-2, 2);
        float z = Random.Range(-5, 5);



        //CenterLineの移動を考慮した偏差補正
        //aaa = aaa + ReviseBezieEnd();
        Vector3 aaa = dir;






        Vector3 t1 = new Vector3(target.transform.position.x, 0, 0);
        Vector3 t2 = new Vector3(transform.position.x, 0, 0);

        if (Vector3.Distance(t1, t2) > 50)
        {
            aaa = dir;
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;
            crossFlag = false;


            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);

            //transform.LookAt(Vector3.Lerp(a, b, 0.5f));
        }
        else
        {
            //+fishSpeed = fishSpeed * 2;
            aaa =dir;//dir;
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f);
            crossFlag = true;
            //transform.LookAt(bezieEnd);

        }









        //aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);


        //bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);

        //中間地点 Lerpによって、始点と終点を結んだ線の5割地点を算出、それに大きく外れた中間点を加算。
        //結果曲がる様になる。詳しくはベジェ曲線の仕組みを調べろ。
        //bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;

        //進行度のリセット。
        bezieT = 0;

    }

    //bezieT = 宝石の速度

    void BezieController()
    {

        if (crossFlag == false)
        {
            bezieT += 1.0f * Time.deltaTime;
            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            this.transform.position = Vector3.Lerp(a, b, bezieT/xBox);
        }

        if(crossFlag == true)
        {
            bezieT += 1.0f * Time.deltaTime;
            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            this.transform.position = Vector3.Lerp(a, b, bezieT / xBox);


        }


    }
}
