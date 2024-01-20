using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//W05WhaleTutorial
//WhaleMoving script for After Tutorial.

public class W05WhaleTutorial : MonoBehaviour
{
    Vector3 startPos = new Vector3(100, 90, 60); //�ŏ��Ɍ������W

    public GameObject player; //player �A�^�b�`����

    float lerpTimer; //lerpTimer

    public bool startFlag = false; //�Q�[�����n�܂��Ă��邩�ǂ����BW01���Ŏg�p�B

    public TutorialManager TM; //RockDestroy(���A�𔲂������ǂ���)�̔���̂��߂Ɏg�p�B

    bool aaa = false;

    public bool tutorial; //�f�o�b�O�p�B false = Tutorial�����s���Ȃ��B

    float lerpTimer2;

    float lerpTimer0;

    float lerpTimer00;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(-5, 180, 30);
        if(tutorial == false)
            this.transform.position = new Vector3(player.transform.position.x + 100, player.transform.position.y, player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

        //�������s��Ȃ��ꍇ
        if (tutorial == false)
            return;
            //startFlag = true;


        if (startFlag == true)
            return;

        if (TM.rockDestroyed == false)
        {
            this.transform.position = new Vector3(-5, 150, 30);
            transform.LookAt(startPos);
            return;
        }

        /*if (Vector3.Distance(this.transform.position, startPos) > 2.0f && aaa == false)
        {
            transform.LookAt(startPos);
            this.transform.position += transform.forward * 20f * Time.deltaTime;
        }*/

        //���A�o�����Ƃ̑ҋ@����   
        if(lerpTimer00 < 3)
        {
            lerpTimer00 += Time.deltaTime;
            return;
        }

        //����Ⴂ����
        if(lerpTimer0 < 4)
        {
            lerpTimer0 += Time.deltaTime;
            this.transform.position += transform.forward * 30f * Time.deltaTime;
        }

        //����Ⴂ��ACenterLine�ɖ߂�����
        else
        {
            aaa = true;
            lerpTimer += Time.deltaTime;


            if (lerpTimer / 5 < 1)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(player.transform.position.x + 500, player.transform.position.y, player.transform.position.z)), 20.0f * Time.deltaTime);
                transform.position = Vector3.Lerp(this.transform.position, new Vector3(player.transform.position.x + 100, 100, 50)/*new Vector3(100, 120, 30)*/, lerpTimer / 5);
            }

            //�Q�[���J�n
            if (lerpTimer / 5 > 1 && lerpTimer2 < 3)
            {
                lerpTimer2 += Time.deltaTime; 
                /*lerpTimer2 +=  0.5f * Time.deltaTime;
                //startFlag = true;
                //this.transform.position = new Vector3(player.transform.position.x + 100, player.transform.position.y, player.transform.position.z);
                transform.position = Vector3.Lerp(this.transform.position, new Vector3(player.transform.position.x + 100, this.transform.position.y,this.transform.position.z), lerpTimer2 / 3);*/
                transform.position = Vector3.Lerp(this.transform.position, new Vector3(player.transform.position.x + 100, 100, 50), lerpTimer2 / 3);
                startFlag = true;

            }

            //�K�v�Ȃ��S�~
            if(lerpTimer2 / 3 > 1)
            {
                //lerpTimer = 0f;
                startFlag = true;
            }
        }

        
    }
}
