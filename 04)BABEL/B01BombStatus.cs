using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------���e�̏���ێ�����X�N���v�g�B
//----------bombOwner�̏����g���āA�X�R�A�Ȃǂ̊Ǘ����s���܂��B
//----------���ۂ̏��`�B�́AB02�ōs���Ă��܂��B


public class B01BombStatus : MonoBehaviour
{
    public GameObject bombOwner;
    int power = 1;

    // Start is called before the first frame update
    void Start()
    {
        //bombOwner = transform.root.gameObject.GetComponent<B01BombStatus>().bombOwner;
    }

    // Update is called once per frame
    void Update()
    {
        bombOwner = transform.root.gameObject.GetComponent<B01BombStatus>().bombOwner;


    }
}
