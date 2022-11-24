using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//-------�X�t�B�A�{���̓��o�p�X�N���v�g�B
//-------�ʐM�Ή��p�ɁA�������֐�����Constructer�֐������܂����B

public class BombThrow : NetworkBehaviour
{
    //���e�֘A���l�[�[�[�[�[�[�[�[�[�[�[�[

    //forwardPower�O�������l
    public float forwardPower = 20.0f,
    //upPower��������l
  �@              upPower = 0.0f,
    //���e�Î~��̔����ҋ@����
                  bombTimer = 3.0f,
    //�S�̈З͒���
                  power = 100f,
    //������폜����
                  destroyTime = 1.0f;
    //�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[

    //���e�G�t�F�N�g�擾
    [SerializeField] GameObject BombEffect;

    //���e���l�ۊǗp
    float Fpower, Upower;

    //�v���C���[�擾�p
    [SyncVar] GameObject playerObj;
    Vector3 keepPlayer;
    Rigidbody rg;
    float set = 0;
    C02ItemManager ItemManager;
    void Start()
    {
        //�ʐM�Ή��ɂ��A�s�����ǂ��̂�Constructer�֐���錾�B
        //Start�֐��ōs���Ă�������ʊ֐��ōs�����ɂ��܂����B
        //���e��������Constructer�֐���PlayerController����Ăяo���Ă܂��B

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

        //Debug.Log(forwardPower + ")(" + upPower);

        //float posY = this.transform.position.y;
        //float playerY = playerObj.transform.position.y;
        //Debug.Log("objPos" + posY + "player" + playerY);


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
        if (bombTimer <= -1.0f * destroyTime) {
            //playerObj.GetComponent<P04ItemHolder>().bombCount--;
           NetworkServer.Destroy(this.gameObject);
        }
        


    }

    //-------�S���ӏ�

    //���e�̏����ݒ�B
    //�����������ōs���B
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
    }


    //�`���[�g���A���p�ɗp�ӂ�����p�̂��́B
    //ItemHolder�������ĂȂ��I�u�W�F�N�g�����e�𔭎˂���ۂɎg�p���܂����B
    public void Constructer(GameObject Player,int a)
    {
        rg = this.GetComponent<Rigidbody>();
        playerObj = Player;
        keepPlayer = this.transform.position;

        /*Fpower = forwardPower;
        Upower = upPower;

        //���e�������l�����킹�邽�߂̏���
        forwardPower = Fpower * power * Time.deltaTime;
        upPower = Upower * power * Time.deltaTime;

        rg.AddForce(rg.mass * transform.forward * forwardPower, ForceMode.Impulse);
        rg.AddForce(rg.mass * transform.up * upPower, ForceMode.Impulse);*/
    }

    //---------�S���ӏ�
}
