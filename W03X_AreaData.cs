using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//W03X_AreaData classed by SimoLieF
//����N���X�B9�������ꂽ�U���G���A�̏���ێ�����ׂ̃N���X�B
//W01�Ŏg�p�B���݈ʒu�ƑO��ʒu���玟�̈ʒu�����肷��֐�������B

public class W03X_AreaData
{

    Vector3 position;       //CenterLine����̌���
    int[] adjoinNumber;     //�אڂ��Ă�}�X
    int weight;             //�}�X�̏d��
    int number;             //�}�X�̔ԍ�


    /////////////////////////////�v���p�e�B�Q
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
    //////////////////////////////�v���p�e�B
    

    //////////////////////////////�R���X�g���N�^
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
    


    //�ړ��ʒu�����肷��֐�
    //(decide = ���� - nextpos)
    //���݈ʒu�Ɨאڃ}�X�̏d���ɂ��A�w��������������B
    //�����ɂ�錈��BAdjoin[0]���瑫���Ă�����COUNT���ʂ���Ƃ��āA�����_���Ɍ���B
    //���肵���l�ɑ΂���Adjoin[0]���猸�Z���鎖�ŕ΂�����������B
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
