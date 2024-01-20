using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamaged : MonoBehaviour
{
    public int HP = 1;//HP
    public bool invFlag;//���i�q�b�g�h�~
    public float timer = 0;
    public GameObject[] smokeObject = new GameObject[4];//���p�[�e�B�N��
    int smokecount = 0;

    //continue�p
    public GameObject aaa;


    //�_���[�W�G�t�F�N�g�p �A�^�b�`����
    public Material fadeMat;
    bool damageFlag;

    

    //�ė����ɏd�́A����������ׂ�XRrig�B
    public GameObject XRrig;

    // Start is called before the first frame update
    void Start()

    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //��莞�Ԃ̖��G����
        if(invFlag == true)
        {
            timer += Time.deltaTime;

            if (timer < 0.35f)
            {
                fadeMat.color = new Color(50, 0, 0, timer * 2.0f);
                //damageFlag = true;
            }
            else
            {
                fadeMat.color = new Color(0, 0, 0, 0);
            }
        }
        if(timer > 3f)
        {
            invFlag = false;
            timer = 0;
          
        }

        //gameover
        //���x��0�A�d�͂�ǉ��B
        if(HP <= 0)
        {
            //����Se�ǉ�
            SeSystem.GameOver = true;
            XRrig.GetComponent<FollowLine>().speed = 0f;
            

            //�������h�~
            if(this.transform.position.y < 20)
            {
                /*GetComponent<Rigidbody>().useGravity = false;
                GetComponent<UIGameOver>().a.useGravity = false;//a = mainCamera�B�J������ė������Ȃ��Ǝ��_�����Ȃ��ׁB
                GetComponent<UIGameOver>().flag = true;*/

            }
            else
            {
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<UIGameOver>().a.useGravity = true;//a = mainCamera�B�J������ė������Ȃ��Ǝ��_�����Ȃ��ׁB
                GetComponent<UIGameOver>().flag = true;
            }



            for (int i = 0; i < 4; i++)
            {
                smokeObject[i].SetActive(false);
                
            }
            smokecount = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //���ʂɐڐG����
        if (invFlag == false)
        {
            if (other.tag == "FishMissile")
            {
                //11/24 SE�p�̒ǉ�(�v���C���[�̔�e)
                SeSystem.HitDamage = true;

                HP -= 1;
                invFlag = true;
                Debug.Log("Hit");
                if (smokecount < 4)
                {
                    smokeObject[smokecount].SetActive(true);
                    smokecount++;
                }
            }
        }
    }
}
