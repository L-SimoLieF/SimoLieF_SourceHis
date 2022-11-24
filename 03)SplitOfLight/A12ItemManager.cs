using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A12ItemManager : MonoBehaviour
{
    public bool getF = false;//アイテムを取得中かどうかの判定
    public float limitTime;        //アイテム効果の残り時間
    [SerializeField] float effectTime;  //アイテムの効果時間
    public float UpSpeed;               //SpeedUpの倍率。
    int counter = 0;                    //60f=1secのカウント用。

    [SerializeField] GameObject speedUPIcon;    //UI表示

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame


    //1フレームで加算
    //60フレームで1秒。limitTimeが減っていく。
    //0以下になった場合、getFをfalseにする
    //→プレイヤー、仲間ともにgetFを見ている為、ここの状態で加減速を行う。
    void Update()
    {
        counter++;

        if (getF == true)
        {
            if (counter > 59)
            {
                counter = 0;
                limitTime--;
            }
        }


        if (limitTime < 0)
        {
            getF = false;
            speedUPIcon.SetActive(false); // UI表示 OFF
        }

    }

    //getFのtrueと、効果時間の加算。
    public void ItemGet()
    {
        getF = true;
        limitTime += effectTime;

        speedUPIcon.SetActive(true); // UI表示 ON
    }
}
