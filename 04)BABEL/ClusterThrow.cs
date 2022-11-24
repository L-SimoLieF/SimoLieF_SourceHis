using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//----�N���X�^�[���e�̓��˃X�N���v�g�B
//���̔��e�ƕς�炸�A�ʐM���̏�������S�����܂����B

public class ClusterThrow : NetworkBehaviour
{

    //���e�G�t�F�N�g�擾
    [SerializeField] GameObject BombEffect;

    //���e�֘A���l�[�[�[�[�[�[�[�[�[�[�[�[

    //forwardPower�O�������l
    public float forwardPower = 5.0f,
    //upPower��������l
  �@              //upPower = 0.0f,
    //���ڂ̔����ҋ@����
                  firstBombTimer = 0.5f,
    //���ڂ̔����ҋ@����
                  secondBombTimer = 1.0f,
    //�S�̈З͒���
                  power = 100f,
    //������폜����
                  destroyTime = 1.0f;
    //�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[

    //���e�G�t�F�N�g�擾
    //[SerializeField] GameObject ClusterEffect;

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
        /*rg = this.GetComponent<Rigidbody>();
        playerObj = GameObject.Find("Player");
        keepPlayer = this.transform.position;

        Fpower = forwardPower;
        //Upower = upPower;

        //���e�������l�����킹�邽�߂̏���
        forwardPower = Fpower * power * Time.deltaTime;
        //upPower = Upower * power * Time.deltaTime;

        rg.AddForce(transform.forward * forwardPower, ForceMode.Impulse);
       // rg.AddForce(transform.up * upPower, ForceMode.Impulse);*/
    }

    void Update()
    {
        if (set == 0)
            firstBombTimer -= Time.deltaTime;
        else if (set >= 1)
            secondBombTimer -= Time.deltaTime;

        if (firstBombTimer <= 0 && set == 0)
        {


            //���e�������l�����킹�邽�߂̏���
            Fpower = forwardPower * 20 * power * Time.deltaTime;
                       
            //����SE
            transform.GetChild(7).gameObject.SetActive(false);
            transform.GetChild(7).gameObject.SetActive(true);
            //Instantiate(ClusterEffect, this.transform.position, this.transform.rotation);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
            transform.GetChild(4).gameObject.SetActive(true);
            transform.GetChild(5).gameObject.SetActive(true);
            transform.GetChild(6).gameObject.SetActive(true);
            Rigidbody rg1 = transform.GetChild(1).gameObject.GetComponent<Rigidbody>();
            Rigidbody rg2 = transform.GetChild(2).gameObject.GetComponent<Rigidbody>();
            Rigidbody rg3 = transform.GetChild(3).gameObject.GetComponent<Rigidbody>();
            Rigidbody rg4 = transform.GetChild(4).gameObject.GetComponent<Rigidbody>();
            Rigidbody rg5 = transform.GetChild(5).gameObject.GetComponent<Rigidbody>();
            Rigidbody rg6 = transform.GetChild(6).gameObject.GetComponent<Rigidbody>();
            rg1.AddForce(rg1.mass * rg1.transform.forward * Fpower, ForceMode.Impulse);
            rg2.AddForce(rg2.mass * rg2.transform.forward * Fpower, ForceMode.Impulse);
            rg3.AddForce(rg3.mass * rg3.transform.forward * Fpower, ForceMode.Impulse);
            rg4.AddForce(rg4.mass * rg4.transform.forward * Fpower, ForceMode.Impulse);
            rg5.AddForce(rg5.mass * rg5.transform.forward * Fpower, ForceMode.Impulse);
            rg6.AddForce(rg6.mass * rg6.transform.forward * Fpower, ForceMode.Impulse);

            set++;
        }

        if (secondBombTimer <= 0 && set == 1) 
        {
            //�g�U�������e�̔�������
            GameObject bombObj1 = transform.GetChild(1).gameObject;
            GameObject bombObj2 = transform.GetChild(2).gameObject;
            GameObject bombObj3 = transform.GetChild(3).gameObject;
            GameObject bombObj4 = transform.GetChild(4).gameObject;
            GameObject bombObj5 = transform.GetChild(5).gameObject;
            GameObject bombObj6 = transform.GetChild(6).gameObject;
            //����SE
            bombObj1.transform.GetChild(7).gameObject.SetActive(true);
            bombObj2.transform.GetChild(7).gameObject.SetActive(true);
            bombObj3.transform.GetChild(7).gameObject.SetActive(true);
            bombObj4.transform.GetChild(7).gameObject.SetActive(true);
            bombObj5.transform.GetChild(7).gameObject.SetActive(true);
            bombObj6.transform.GetChild(7).gameObject.SetActive(true);
            Instantiate(BombEffect, bombObj1.transform.position, bombObj1.transform.rotation);
            Instantiate(BombEffect, bombObj2.transform.position, bombObj2.transform.rotation);
            Instantiate(BombEffect, bombObj3.transform.position, bombObj3.transform.rotation);
            Instantiate(BombEffect, bombObj4.transform.position, bombObj4.transform.rotation);
            Instantiate(BombEffect, bombObj5.transform.position, bombObj5.transform.rotation);
            Instantiate(BombEffect, bombObj6.transform.position, bombObj6.transform.rotation);



            //�g�U�������e�̂��ꂼ��̊O�����폜
            bombObj1.transform.GetChild(0).gameObject.SetActive(false);
            bombObj2.transform.GetChild(0).gameObject.SetActive(false);
            bombObj3.transform.GetChild(0).gameObject.SetActive(false);
            bombObj4.transform.GetChild(0).gameObject.SetActive(false);
            bombObj5.transform.GetChild(0).gameObject.SetActive(false);
            bombObj6.transform.GetChild(0).gameObject.SetActive(false);
            //�g�U�������e�̓����j�Ђ��΂�����
            for (int i = 1; i <= 6; i++) { 
                bombObj1.transform.GetChild(i).gameObject.SetActive(true);
                bombObj2.transform.GetChild(i).gameObject.SetActive(true);
                bombObj3.transform.GetChild(i).gameObject.SetActive(true);
                bombObj4.transform.GetChild(i).gameObject.SetActive(true);
                bombObj5.transform.GetChild(i).gameObject.SetActive(true);
                bombObj6.transform.GetChild(i).gameObject.SetActive(true);
                //Debug.Log(i);
            }
            set++;

        }
        if (set == 2)
        {
            playerObj.GetComponent<P04ItemHolder>().bombCount--;
            set++;
        }
        if (secondBombTimer <= -1.0f * destroyTime) {
            //playerObj.GetComponent<P04ItemHolder>().bombCount--;
            NetworkServer.Destroy(this.gameObject);
        }
        


    }

    //---------�S���ӏ�
    public void Constructer(GameObject Player)
    {
        rg = this.GetComponent<Rigidbody>();
        playerObj = Player;
        keepPlayer = this.transform.position;

        Fpower = forwardPower* 22.0f * power * Time.deltaTime;
        //Upower = upPower;

        //���e�������l�����킹�邽�߂̏���
        //upPower = Upower * power * Time.deltaTime;

        rg.AddForce(rg.mass * transform.forward * Fpower, ForceMode.Impulse);
        // rg.AddForce(transform.up * upPower, ForceMode.Impulse);

    }
    //----------�S���ӏ�

}
