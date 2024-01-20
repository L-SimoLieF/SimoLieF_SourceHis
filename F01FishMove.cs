using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F01FishMove : MonoBehaviour
{

    float speed = 5f;
    public GameObject target;
    Vector3 dir;


    //ベジェ曲線用変数 (ベジェ曲線 = 使うと弾が曲がる技術)
    //開始位置、中間、終点
    Vector3 bezieStart;
    Vector3 bezieCenter;
    Vector3 bezieEnd;

    //曲線を描くための中間点。
    //クジラの位置によって変わる
    Vector3 bezieMiddle;

    //弾の軌道管理用変数群
    bool curveFlag;    //Playerとの距離が遠い内はベジェ曲線で曲げる
    float bezieT;      //ベジェ曲線の進行度
    bool homingFlag;   //距離が縮まった際、Playerを追うか否か
    bool straightFlag; //↑追わない場合
    bool rotateFlag = true; //クジラが中心位置の場合、弾の回転が必要ない為それを弾く為に必要。

    // Start is called before the first frame update
    void Start()
    {
        //Constructer関数が代替
    }

    // Update is called once per frame
    void Update()
    {

        //ベジェによる湾曲処理
        if(curveFlag == true)
        {

            //ただベジェ曲線を使った奴
            bezieT += 0.3f * Time.deltaTime;
            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            this.transform.position = Vector3.Lerp(a, b, bezieT);


            //終了条件。ベジェの進行度だったが、Playerとの位置の方が上手くいった。
            if(Vector3.Distance(target.transform.position,this.transform.position) < 30/*bezieT > 0.8f*/)
            {
                curveFlag = false;
                
                //追尾弾じゃない場合
                //目標位置の設定と弾の回転処理
                if(straightFlag == true)
                {
                    //中心位置じゃない場合
                    //Playerの方向に向けつつ、目標をランダムにずらす処理
                    if (rotateFlag == true)
                    {
                        //Random.Rangeはズレ用。x+25は到着までの前進分
                        Vector3 aaa = new Vector3(target.transform.position.x + 25, target.transform.position.y + Random.Range(-20, 20), target.transform.position.z + Random.Range(-20, 20));
                        aaa = aaa - this.transform.position;
                        transform.rotation = Quaternion.LookRotation(aaa);
                    }
                    //中心位置の場合
                    //-x方面に真っすぐ飛ばす。
                    else
                    {
                        //target = this.gameObject;
                        transform.rotation = Quaternion.LookRotation( -Vector3.right);

                    }
                         


                }
                //追尾弾の場合
                else
                 homingFlag = true;
                
            }
        }

        //曲線後、追尾する場合
        else if(homingFlag == true)
        {
            dir = target.transform.position - this.transform.position;
            dir = new Vector3(-5f, dir.y, dir.z);
            this.transform.position += dir * Time.deltaTime;
        }

        //追尾しない場合 前に向かって直進(回転処理は湾曲処理の中)
        else if(straightFlag == true)
        {
            this.transform.position += this.gameObject.transform.forward * 10f * Time.deltaTime ;    
        }

    }

    //初期化用関数
    //W02でコール
    //クジラの位置に応じてベジェの中間点を格納
    //ベジェの3点を定義、各フラグをセットしている。
   public void Constructer(GameObject player,int posNumber)
    {
        target = player;
        switch (posNumber) 
        {
            case 0:
                bezieMiddle = new Vector3(0, 30, 30);
                break;
            case 1:
                bezieMiddle = new Vector3(0, 30, 0);
                break;
            case 2:
                bezieMiddle = new Vector3(0, 30, -30);
                break;
            case 3:
                bezieMiddle = new Vector3(0, 0, 30);
                break;
            case 4:
                bezieMiddle = new Vector3(0, 0, 0);
                break;
            case 5:
                bezieMiddle = new Vector3(0, 0, -30);
                break;
            case 6:
                bezieMiddle = new Vector3(0, -30, 30);
                break;
            case 7:
                bezieMiddle = new Vector3(0, -30, 0);
                break;
            case 8:
                bezieMiddle = new Vector3(0, -30, -30);
                break;
            default:
                bezieMiddle = new Vector3(0, 0, 0);
                break;
        }

        bezieStart = transform.position;
        bezieEnd = Vector3.Lerp(transform.position, target.transform.position, 0.5f);
        bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;

        curveFlag = true;
        bezieT = 0;

        straightFlag = false; 
    }


    //フラグセットと、中間点の設定が別処理
    //直進弾用の関数
    public void Constructer(GameObject player,int posNumber,int mode)
    {
        target = player;
        switch (posNumber)
        {
            case 0:
                bezieMiddle = new Vector3(0, 30, 30);
                break;
            case 1:
                bezieMiddle = new Vector3(0, 30, 0);
                break;
            case 2:
                bezieMiddle = new Vector3(0, 30, -30);
                break;
            case 3:
                bezieMiddle = new Vector3(0, 0, 30);
                break;
            case 4:
                bezieMiddle = new Vector3(0, 0, 0);
                rotateFlag = false;
                break;
            case 5:
                bezieMiddle = new Vector3(0, 0, -30);
                break;
            case 6:
                bezieMiddle = new Vector3(0, -30, 30);
                break;
            case 7:
                bezieMiddle = new Vector3(0, -30, 0);
                break;
            case 8:
                bezieMiddle = new Vector3(0, -30, -30);
                break;
            default:
                bezieMiddle = new Vector3(0, 0, 0);
                break;
        }



                                                                                         
        bezieStart = transform.position;
        Vector3 eeee = new Vector3(target.transform.position.x + 40, target.transform.position.y, target.transform.position.z);
        bezieEnd = Vector3.Lerp(transform.position, eeee, 1f);
        bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;

        curveFlag = true;
        bezieT = 0;

        straightFlag = true;
    }
}
