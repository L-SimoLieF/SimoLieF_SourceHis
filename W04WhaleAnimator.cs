using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//W04WhaleAnimator
//Whale = ���T�T�E���X�̎�����������ČĂ�ł邾��
//AnimatorController�̑��������X�N���v�g�B

public class W04WhaleAnimator : MonoBehaviour
{
    //W01����Q��
    W03X_AreaData[] AreaData;
    int nowNumber;
    int nextNumber;

    float Timer;

    //Animator
    [SerializeField] Animator Anim;
    bool moveAnim;
    bool atkAnim;

    public GameObject missileParant;//�A�^�b�`����

    public bool damageFlag;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        //�ړ���
        if(moveAnim == true)
        {
            Timer += Time.deltaTime;
            if(Timer > 3.0f)//���Ԃ͓K��
            {
                moveAnim = false;
                AnimBoolReSet();
                Anim.SetBool("AtkBool", true);
                atkAnim = true;
                Timer = 0f;
                Anim.SetBool("AtkBool", true);
            }
        }
        //�U����
        if(atkAnim == true)
        {
            Timer += Time.deltaTime;
            if(Timer > 3.0f)//���Ԃ͓K��
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

    //W01��Start()�ŃR�[��
    public void Constructer(W03X_AreaData[] a)
    {
        AreaData = a;
    }
    
    //W01�̃|�W�V�����`�F���W���ɃR�[��
    public void SetNumbers(int now,int next)
    {
        nowNumber = now;
        nextNumber = next;
        AnimBoolSet();
    }

    //�ړ������̎Z�o
    //���݈ʒu�Ǝ���ʒu�̔�r�ɂ��A�㉺���E�ǂ���Ɉړ�����̂����`�F�b�N�B
    //Up,Down,Right,Left��4�v�f�̑g�ݍ��킹�ōĐ����郂�[�V����������
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

    //���Z�b�g
    void AnimBoolReSet()
    {
        Anim.SetBool("UpBool", false);
        Anim.SetBool("DownBool", false);
        Anim.SetBool("LeftBool", false);
        Anim.SetBool("RightBool", false);
        Anim.SetBool("AtkBool", false);
    }

    //��e���[�V����
    //12/06 ��e���[�V�����ŗD��ׁ̈A�eSetBool�̃��Z�b�g���s���B
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

    //���ݍ��W�Ǝ��̍��W���Z�o���āAy,z�̍��ňړ��������Z�o����B
    //y���v���X��up,z���v���X��right.
    //0�̎��͕ω����Ȃ��B
    //bool�^��������o����BTrigger���Əo���Ȃ��B�v���k
    //Trigger�̏ꍇ�́H
    //�U���͈ړ��̌�ɂ��̂܂܌q����Ηǂ��B
    //�e�t���O�̏����ɉ�����SetTrigger���Ăяo���B
}

