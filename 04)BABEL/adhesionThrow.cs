using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//------------�S�����e�̓��o�p�̃X�N���v�g�B
//�S�������̂́A���ˎ��ɔ��˂����l���̏���`�B���镔���ł��B
//Constructer�֐����A�ePlayer�X�N���v�g��Call����`�Ŏg�p���Ă��܂��B

public class adhesionThrow : NetworkBehaviour
{
    //���e�G�t�F�N�g�擾
    [SerializeField] GameObject BombEffect;

    //���e�֘A���l�[�[�[�[�[�[�[�[�[�[�[�[

    //forwardPower�O�������l
    public float forwardPower = 5.0f,
    //upPower��������l
  �@              upPower = 0.0f,
    //���e�Î~��̔����ҋ@����
                  bombTimer = 3.0f,
    //�S�̈З͒���
                  power = 100f,
    //������폜����
                  destroyTime = 1.0f;
    
    //�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[

    //���e���l�ۊǗp
    float Fpower, Upower;

    //�v���C���[�擾�p
    [SyncVar] GameObject playerObj;
    Vector3 keepPlayer;
    Rigidbody rg;
    float set = 0;
    float resetTime=3.0f;
    bool hit = false;

    float hittimer = 0.1f;
    
    C02ItemManager ItemManager;

    bool count=true;
    bool flag = false;

    void Start()
    {

        

        /*rg = this.GetComponent<Rigidbody>();
        playerObj = GameObject.Find("Player");
        keepPlayer = this.transform.position;

        Fpower = forwardPower;
        Upower = upPower;

        //���e�������l�����킹�邽�߂̏���
        forwardPower = Fpower * power * Time.deltaTime;
        upPower = Upower * power * Time.deltaTime;

        rg.AddForce(transform.forward * forwardPower, ForceMode.Impulse);
        rg.AddForce(transform.up * upPower, ForceMode.Impulse);*/
    }

    void Update()
    {
        

        if(hit)
        {
            hittimer -= Time.deltaTime;
            rg.velocity = Vector3.zero;

            if(hittimer < 0)
            {
                rg.isKinematic = true;
            }
        }

        bombTimer -= Time.deltaTime;

        //rg.velocity = Vector3.zero;
        //rg.angularVelocity = Vector3.zero;
        if (bombTimer <= 0 && set == 0)
        {
            //����SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);
            Instantiate(BombEffect, this.transform.position, this.transform.rotation);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(4).gameObject.SetActive(true);
            transform.GetChild(5).gameObject.SetActive(true);
            transform.GetChild(6).gameObject.SetActive(true);
            set++;
        }
        if (set == 1)
        {
            playerObj.GetComponent<P04ItemHolder>().bombCount--;
            set++;
        }
        if (bombTimer <= -1.0f * destroyTime)
        {
            //playerObj.GetComponent<P04ItemHolder>().bombCount--;
            NetworkServer.Destroy(this.gameObject);
        }

        //if (resetTime < 0 && set == 0)
        //{
        //    Instantiate(BombEffect, this.transform.position, this.transform.rotation);
        //    transform.GetChild(0).gameObject.SetActive(false);
        //    transform.GetChild(1).gameObject.SetActive(true);
        //    transform.GetChild(2).gameObject.SetActive(true);
        //    transform.GetChild(3).gameObject.SetActive(true);
        //    transform.GetChild(4).gameObject.SetActive(true);
        //    transform.GetChild(5).gameObject.SetActive(true);
        //    transform.GetChild(6).gameObject.SetActive(true);
        //    set++;
        //    flag = true;

        //    //playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //    //count = false;

        //    //NetworkServer.Destroy(this.gameObject);

        //}

        //if (hit)
        //{
        //    //this.transform.position += 0.05f * transform.forward;
        //    bombTimer -= Time.deltaTime;
        //    rg.velocity= Vector3.zero;
        //    if(bombTimer<2.9f)
        //        rg.isKinematic = true;
        //    if (bombTimer <= 0&&set==0)
        //    {
        //        Instantiate(BombEffect, this.transform.position, this.transform.rotation);
        //        transform.GetChild(0).gameObject.SetActive(false);
        //        transform.GetChild(1).gameObject.SetActive(true);
        //        transform.GetChild(2).gameObject.SetActive(true);
        //        transform.GetChild(3).gameObject.SetActive(true);
        //        transform.GetChild(4).gameObject.SetActive(true);
        //        transform.GetChild(5).gameObject.SetActive(true);
        //        transform.GetChild(6).gameObject.SetActive(true);
        //        set++;
        //        flag = true;
               
        //    }

        //    if(flag && count)
        //    {
        //        playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //        count = false;

        //    }

        //if (resetTime <= -1.0f * destroyTime)
        //{
        //    //playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //    NetworkServer.Destroy(this.gameObject);
        //}

        //if (bombTimer <= -1.0f * destroyTime && playerObj.GetComponent<P04ItemHolder>().bombCount == 0)
        //{
        //    //playerObj.GetComponent<P04ItemHolder>().bombCount--;
        //    NetworkServer.Destroy(this.gameObject);
        //}

        //}


        /*if(count == true)
        {
            playerObj.GetComponent<P04ItemHolder>().bombCount--;
            count = false;
        }*/


    }
    private void OnCollisionEnter(Collision collision)
    {
        //�v���C���[�ɓ������Ď~�܂�̂�h������
        if (collision.gameObject.tag != "Player"&& collision.gameObject.tag != "Barrier" && hit == false)
            hit = true;
    }


    //---------�S���ӏ�
    public void Constructer(GameObject Player)
    {
        rg = this.GetComponent<Rigidbody>();
        playerObj = Player;
        keepPlayer = this.transform.position;
       
        Fpower = forwardPower;
        Upower = upPower;

        //���e�������l�����킹�邽�߂̏���
        forwardPower = Fpower * power * Time.deltaTime;
        upPower = Upower * power * Time.deltaTime;

        rg.AddForce(rg.mass * transform.forward * forwardPower, ForceMode.Impulse);
        rg.AddForce(rg.mass * transform.up * upPower, ForceMode.Impulse);
        resetTime = 8.0f;
    }
    //---------�S���ӏ�
}
