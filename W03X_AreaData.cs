using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//W03X_AreaData classed by SimoLieF
//自作クラス。9分割された攻撃エリアの情報を保持する為のクラス。
//W01で使用。現在位置と前回位置から次の位置を決定する関数がある。

public class W03X_AreaData
{

    Vector3 position;       //CenterLineからの公差
    int[] adjoinNumber;     //隣接してるマス
    int weight;             //マスの重さ
    int number;             //マスの番号


    /////////////////////////////プロパティ群
    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public int[] AdjoinNumber
    {
        get { return adjoinNumber; }
        set { adjoinNumber = value; }
    }

    public int Weight
    {
        get { return weight; }
        set { weight = value; }
    }
    public int Number
    {
        get { return number; }
        set { number = value; }
    }
    //////////////////////////////プロパティ
    

    //////////////////////////////コンストラクタ
    public W03X_AreaData(Vector3 pos, int[] array, int w, int n)
    {
        position = pos;
        adjoinNumber = new int[array.Length];
        array.CopyTo(adjoinNumber, 0);
        weight = w;
        number = n;
    }
    public W03X_AreaData(int num)
    {
        position = new Vector3(0, 0, 0);
        adjoinNumber = new int[3];
        adjoinNumber[0] = 0;
        adjoinNumber[1] = 0;
        adjoinNumber[2] = 0;
        weight = 0;
        number = 0;
    }

    public W03X_AreaData(W03X_AreaData x)
    {
        position = x.Position;
        adjoinNumber = new int[x.AdjoinNumber.Length];
        x.AdjoinNumber.CopyTo(adjoinNumber, 0);
        
        weight = x.Weight;
        number = x.Number;
    }
    //////////////////////////////////
    


    //移動位置を決定する関数
    //(decide = 決定 - nextpos)
    //現在位置と隣接マスの重さにより、指向性を持たせる。
    //乱数による決定。Adjoin[0]から足していったCOUNT結果を種として、ランダムに決定。
    //決定した値に対してAdjoin[0]から減算する事で偏りを実現した。
    public int decideNextPos(int lastpos, W03X_AreaData[] allArea, W03X_AreaData nowArea)
    {
        int count = 0;

        foreach (int Num in adjoinNumber)
        {
            if (lastpos != Num)
            {
                count += Mathf.Abs(allArea[Num].Weight - weight);
            }
        }

        int rund = Random.Range(0, count);

        //int num = 0;
        foreach (int Num in adjoinNumber)
        {
            if (lastpos != Num)
            {
                rund = rund - Mathf.Abs(allArea[Num].Weight - weight);

                if (rund <= 0)
                {
                    return Num;
                }
            }
        }

        return adjoinNumber[adjoinNumber.Length - 1];
    }

}
