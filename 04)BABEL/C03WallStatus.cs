using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Defender����������Wall�p�̃X�N���v�g�B
//�ǂɓ��������ۂ̃X�R�A���Z�ƁADefender�̃N���X�^�[���e�ŉ󂳂�Ȃ��悤�ɂ��鏈���Ȃǂ��L�ځB

public class C03WallStatus : NetworkBehaviour
{
    int Hitpoint = 1;
    [SyncVar] GameObject bombOwner;
    bool collisionFlag = false;

    //public GameObject spawner;
    //objspawner Objsp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Hitpoint < 1)
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        //�u���b�N��ǂŎx�����ۂ̉��_�B
        //collisionFlag�͕ǈ�ɕt���A���_�����ɂ��邽�߂̂��́B
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Piller")
        {
            if (collisionFlag == false)
            {
                bombOwner.GetComponent<P04ItemHolder>().Score += (collision.gameObject.GetComponent<C11CubeState>().GetScore()) / 2;
                collisionFlag = true;
            }
        }
        if(collision.gameObject.tag == "clusterSplinter")
        {
            //Defender�̏��O���� Bomber�̃N���X�^�[�ł̂ݔj�󂳂��B
            if (collision.gameObject.GetComponent<B01BombStatus>().bombOwner.GetComponent<DefenderController>().enabled == false)
            {
                //spawner.GetComponent<objspawner>().objcount -= 1;
                NetworkServer.Destroy(this.gameObject);
            }
        }
        if(collision.gameObject.tag == "splinter")
        {
            //spawner.GetComponent<objspawner>().objcount -= 1;
            NetworkServer.Destroy(this.gameObject);
        }
        if(collision.gameObject.tag == "splinter2")
        {
            //spawner.GetComponent<objspawner>().objcount -= 1;
            NetworkServer.Destroy(this.gameObject);
        }

    }

    public void Constructer(GameObject player)
    {
        bombOwner = player;
    }
}
