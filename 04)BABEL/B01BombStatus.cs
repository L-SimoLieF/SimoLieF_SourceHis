using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------爆弾の情報を保持するスクリプト。
//----------bombOwnerの情報を使って、スコアなどの管理を行います。
//----------実際の情報伝達は、B02で行っています。


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
