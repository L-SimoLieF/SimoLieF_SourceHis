using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//移動経路を保存する為のスクリプト。
//A01の追従処理で参照
public class A02PositionUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3[] position = new Vector3[100];
    int setCounter = 0;

    public float waitTime = 0.1f;   // 記録の間隔　sec
    private float timer = 0.0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > waitTime)
        {
            timer = 0;

            position[setCounter] = transform.position;
            setCounter++;


            if (position.Length <= setCounter)
            {
                setCounter = 0;
            }
        }
    }

    public Vector3 GetCurrentPositon(out int nextIndex)
    {
        nextIndex = setCounter;

        return transform.position;
    }

    public Vector3 GetPositon(int index, out int nextIndex)
    {
        Vector3 retPostion;

        //Debug.Log(index);

        retPostion = position[index];

        // 追随する仲間が追い付いた場合
        if (setCounter == index)
        {
            nextIndex = setCounter;

            // position[index]の情報は更新されていないので１個前のデータを渡す
            if (0 == nextIndex)
            {
                retPostion = position[position.Length - 1];
            }
            else
            {
                retPostion = position[index - 1];
            }
        }
        else
        {
            nextIndex = index + 1;

            if (position.Length <= nextIndex)
            {
                nextIndex = 0;
            }
        }

        return retPostion;
    }

    //現在位置でPosition内をリセットしてから、次の目標を渡す
    //号令処理等、過去の軌跡を追わせたくない場合に使用する。
    public Vector3 ResetPosition(out int nextIndex)
    {
        for (int i = 0; i < position.Length; i++)
        {
            position[i] = transform.position;
        }
        setCounter = 0;
        nextIndex = 0;
        return transform.position;

    }

    //GetLatePositionは、引数として渡された数字の数だけ、更新位置から後ろの位置を参照する関数。
    //「プレイヤーが移動しているかどうか」の判定に使用した。
    public Vector3 GetLatePosition(int lates)
    {
        int i = setCounter - lates;
        if (i < 0)
        {
            i = position.Length + i;
        }
        return position[i];
    }
}
