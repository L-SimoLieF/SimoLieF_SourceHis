using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------�X�t�B�A�{���̓��˃X�N���v�g�B
//---------��{�I�ɂ͔S�����e�ƕς��܂���B
//---------�X�^������̕������������S�����܂����B


public class BombSplinter : MonoBehaviour
{
    //�������l�֘A�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[

    //�����̃X�s�[�h���l
    private float forwardPower = 30.0f;
    //�����͈�
    private float bombRange = 5.0f;
    //�X�^���͈�
    private float stanRange = 5.0f;

    //�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[

    Vector3 startPos;

    Rigidbody rg;
    Vector3 playerPos;

    PlayerStan playerstan;

    GameObject bombOwner;
    LayerMask layer = 1 << 7;

    LayerMask itemLayer = 1 << 18;

    private void Start()
    {
        //�����J�n���W���擾
        startPos = this.transform.position;

        bombOwner = this.gameObject.GetComponent<B01BombStatus>().bombOwner;

       

        //�X�^���͈͏���
        //PowerUp�A�C�e���ňЗ͑���

        /*if (this.gameObject.tag == "splinter")
            //�ʏ픚�e�͈�
            bombRange = 0.2f * C02ItemManager.itemPower + 2;
        if (this.gameObject.tag == "splinter2")
            //�S�����e�͈�
            bombRange = 0.12f * C02ItemManager.itemPower + 2;
        if (this.gameObject.tag == "splinterCluster")
            //�N���X�^�[���e�͈�
            bombRange = 0.112f * C02ItemManager.itemPower + 0.7f;
        stanRange = bombRange;*/

        //PlayerStun����
        //playerstan = FindObjectOfType<PlayerStan>();
        ////��E���������Ă�����W���擾
        //if (GameScreenUi.PlayerMode == 0)
        //    playerPos = GameObject.Find("Player Attacker").transform.position;
        //if (GameScreenUi.PlayerMode == 1)
        //    playerPos = GameObject.Find("Player Defender").transform.position;
        //float dis = Vector3.Distance(startPos, playerPos);
        //if (stanRange > dis)
        //    playerstan.StanOn = true;

       //---------------�S���ӏ�

        Collider[] HitCollider = Physics.OverlapSphere(startPos, 3.0f, layer);

        if (HitCollider.Length > 0)
        {
            for (int i = 0; i < HitCollider.Length; i++)
            {
                GameObject StanPlayer = HitCollider[i].gameObject;
                int LostScore = 0;
                StanPlayer.GetComponent<PlayerStan>().StanOn = true;

                if (StanPlayer.GetComponent<PlyerControlloer>().enabled == true)
                {
                    LostScore = (int)(StanPlayer.GetComponent<P04ItemHolder>().Score * 0.9f);
                    //StanPlayer.GetComponent<P04ItemHolder>().Score -= LostScore;
                }

                /*if (bombOwner.GetComponent<DefenderController>().enabled == true)
                {
                    bombOwner.GetComponent<P04ItemHolder>().Score += LostScore / 2;
                }*/


            }
        }


        //�����ɂ��A�C�e���擾�����B5.0f���擾�͈͂̔��a(Radius)
        HitCollider = Physics.OverlapSphere(startPos, 5.0f, itemLayer);
        for (int i = 0; i < HitCollider.Length; i++)
        {
            HitCollider[i].gameObject.GetComponent<C02ItemManager>().Itemget(bombOwner);

            //HitCollider[i].gameObject.transform.position = bombOwner.transform.position;

        }


        //-----------------�S���ӏ�

        //���e�̔j�Јړ�����
        rg = GetComponent<Rigidbody>();
        forwardPower = 5.0f * bombOwner.GetComponent<P04ItemHolder>().itemPower + forwardPower * 1.5f;
        rg.AddForce(rg.mass * transform.forward * forwardPower, ForceMode.Impulse);



    }
    void Update()
    {
        //���e�̔j�Ђ̓X�^�[�g���W����w��̋����ō폜
        float dis = Vector3.Distance(startPos, this.transform.position);

        if (bombRange < dis)
            Destroy(this.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
            Destroy(this.gameObject);
    }
}
