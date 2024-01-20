using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F01FishMove : MonoBehaviour
{

    float speed = 5f;
    public GameObject target;
    Vector3 dir;


    //�x�W�F�Ȑ��p�ϐ� (�x�W�F�Ȑ� = �g���ƒe���Ȃ���Z�p)
    //�J�n�ʒu�A���ԁA�I�_
    Vector3 bezieStart;
    Vector3 bezieCenter;
    Vector3 bezieEnd;

    //�Ȑ���`�����߂̒��ԓ_�B
    //�N�W���̈ʒu�ɂ���ĕς��
    Vector3 bezieMiddle;

    //�e�̋O���Ǘ��p�ϐ��Q
    bool curveFlag;    //Player�Ƃ̋������������̓x�W�F�Ȑ��ŋȂ���
    float bezieT;      //�x�W�F�Ȑ��̐i�s�x
    bool homingFlag;   //�������k�܂����ہAPlayer��ǂ����ۂ�
    bool straightFlag; //���ǂ�Ȃ��ꍇ
    bool rotateFlag = true; //�N�W�������S�ʒu�̏ꍇ�A�e�̉�]���K�v�Ȃ��ׂ����e���ׂɕK�v�B

    // Start is called before the first frame update
    void Start()
    {
        //Constructer�֐������
    }

    // Update is called once per frame
    void Update()
    {

        //�x�W�F�ɂ��p�ȏ���
        if(curveFlag == true)
        {

            //�����x�W�F�Ȑ����g�����z
            bezieT += 0.3f * Time.deltaTime;
            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            this.transform.position = Vector3.Lerp(a, b, bezieT);


            //�I�������B�x�W�F�̐i�s�x���������APlayer�Ƃ̈ʒu�̕�����肭�������B
            if(Vector3.Distance(target.transform.position,this.transform.position) < 30/*bezieT > 0.8f*/)
            {
                curveFlag = false;
                
                //�ǔ��e����Ȃ��ꍇ
                //�ڕW�ʒu�̐ݒ�ƒe�̉�]����
                if(straightFlag == true)
                {
                    //���S�ʒu����Ȃ��ꍇ
                    //Player�̕����Ɍ����A�ڕW�������_���ɂ��炷����
                    if (rotateFlag == true)
                    {
                        //Random.Range�̓Y���p�Bx+25�͓����܂ł̑O�i��
                        Vector3 aaa = new Vector3(target.transform.position.x + 25, target.transform.position.y + Random.Range(-20, 20), target.transform.position.z + Random.Range(-20, 20));
                        aaa = aaa - this.transform.position;
                        transform.rotation = Quaternion.LookRotation(aaa);
                    }
                    //���S�ʒu�̏ꍇ
                    //-x���ʂɐ^��������΂��B
                    else
                    {
                        //target = this.gameObject;
                        transform.rotation = Quaternion.LookRotation( -Vector3.right);

                    }
                         


                }
                //�ǔ��e�̏ꍇ
                else
                 homingFlag = true;
                
            }
        }

        //�Ȑ���A�ǔ�����ꍇ
        else if(homingFlag == true)
        {
            dir = target.transform.position - this.transform.position;
            dir = new Vector3(-5f, dir.y, dir.z);
            this.transform.position += dir * Time.deltaTime;
        }

        //�ǔ����Ȃ��ꍇ �O�Ɍ������Ē��i(��]�����͘p�ȏ����̒�)
        else if(straightFlag == true)
        {
            this.transform.position += this.gameObject.transform.forward * 10f * Time.deltaTime ;    
        }

    }

    //�������p�֐�
    //W02�ŃR�[��
    //�N�W���̈ʒu�ɉ����ăx�W�F�̒��ԓ_���i�[
    //�x�W�F��3�_���`�A�e�t���O���Z�b�g���Ă���B
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


    //�t���O�Z�b�g�ƁA���ԓ_�̐ݒ肪�ʏ���
    //���i�e�p�̊֐�
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
