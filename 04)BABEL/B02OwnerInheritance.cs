using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------OwnerInheritanse
//---------���ˎ҂̏����A�e�j�Ђɓ`�B����ׂ̃X�N���v�g
//�K�v�ȗ��R�́A�e�j�Ђɂ��X�R�A����Ȃǂ̍ۂɁA�e�j�Ђɂ����ˎ҂̏���`�B����K�v�����邩��B
//�����̃A���S���Y���́AC10�ł̃u���b�N�Ǘ��ł��g�p���܂����B

public class B02OwnerInheritance : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    //SendOwnerChild
    //�e�I�u�W�F�N�g����q�I�u�W�F�N�g�ɕۗL�҂̏��𑗐M����ׂ̊֐��B
    //���e�̊O����Player�̏���n������́A�e�j�Ђɓ`�B���������S���B
    public void SendOwnerChild(GameObject Bomb, GameObject Owner)
    {

        B01BombStatus B01;
        Transform children = Bomb.GetComponentInChildren<Transform>();

        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform child in children)
        {
            if (child.GetComponent<B01BombStatus>())
            {

                B01 = child.GetComponent<B01BombStatus>();
                B01.bombOwner = Owner;
                SendOwnerChild(child.gameObject, Owner);

            }
            else
            {
                SendOwnerChild(child.gameObject, Owner);
            }
        }
    }
}
