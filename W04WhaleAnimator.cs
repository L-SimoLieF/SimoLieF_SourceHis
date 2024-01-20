using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//W04WhaleAnimator
//Whale = モササウルスの事をくじらって呼んでるだけ
//AnimatorControllerの操作をするスクリプト。

public class W04WhaleAnimator : MonoBehaviour
{
    //W01から参照
    W03X_AreaData[] AreaData;
    int nowNumber;
    int nextNumber;

    float Timer;

    //Animator
    [SerializeField] Animator Anim;
    bool moveAnim;
    bool atkAnim;

    public GameObject missileParant;//アタッチしろ

    public bool damageFlag;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        //移動中
        if(moveAnim == true)
        {
            Timer += Time.deltaTime;
            if(Timer > 3.0f)//時間は適当
            {
                moveAnim = false;
                AnimBoolReSet();
                Anim.SetBool("AtkBool", true);
                atkAnim = true;
                Timer = 0f;
                Anim.SetBool("AtkBool", true);
            }
        }
        //攻撃中
        if(atkAnim == true)
        {
            Timer += Time.deltaTime;
            if(Timer > 3.0f)//時間は適当
            {
                atkAnim = false;
                AnimBoolReSet();
                Timer = 0f;
            }
        }

        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Damage") == true)
            damageFlag = true;
        else
            damageFlag = false;
    }

    //W01のStart()でコール
    public void Constructer(W03X_AreaData[] a)
    {
        AreaData = a;
    }
    
    //W01のポジションチェンジ毎にコール
    public void SetNumbers(int now,int next)
    {
        nowNumber = now;
        nextNumber = next;
        AnimBoolSet();
    }

    //移動方向の算出
    //現在位置と次回位置の比較により、上下左右どちらに移動するのかをチェック。
    //Up,Down,Right,Leftの4要素の組み合わせで再生するモーションを決定
    void AnimBoolSet()
    {

        if(AreaData[nowNumber].Position.y < AreaData[nextNumber].Position.y)
        {
            Anim.SetBool("UpBool",true);
        }
        if (AreaData[nowNumber].Position.y > AreaData[nextNumber].Position.y)
        {
            Anim.SetBool("DownBool", true);
        }

        if (AreaData[nowNumber].Position.z < AreaData[nextNumber].Position.z)
        {
            Anim.SetBool("LeftBool", true);
        }
        if (AreaData[nowNumber].Position.z > AreaData[nextNumber].Position.z)
        {
            Anim.SetBool("RightBool", true);
        }

        moveAnim = true;
        //Anim.SetBool("MoveSet", true);

    }

    //リセット
    void AnimBoolReSet()
    {
        Anim.SetBool("UpBool", false);
        Anim.SetBool("DownBool", false);
        Anim.SetBool("LeftBool", false);
        Anim.SetBool("RightBool", false);
        Anim.SetBool("AtkBool", false);
    }

    //被弾モーション
    //12/06 被弾モーション最優先の為、各SetBoolのリセットも行う。
    public void DamageAnimSet()
    {
        Anim.SetTrigger("DamageTrigger");

        AnimBoolReSet();
        moveAnim = false;
        atkAnim = false;
        Timer = 0f;
        

        /*foreach(Transform child in missileParant.transform)
        {
            GameObject.Destroy(child.gameObject);
        }*/
    }

    //現在座標と次の座標を算出して、y,zの差で移動方向を算出する。
    //yがプラスでup,zがプラスでright.
    //0の時は変化しない。
    //bool型だったら出来る。Triggerだと出来ない。要相談
    //Triggerの場合は？
    //攻撃は移動の後にそのまま繋げれば良い。
    //各フラグの条件に応じてSetTriggerを呼び出す。
}

