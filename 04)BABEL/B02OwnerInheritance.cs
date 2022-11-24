using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//---------OwnerInheritanse
//---------発射者の情報を、各破片に伝達する為のスクリプト
//必要な理由は、各破片によるスコア判定などの際に、各破片にも発射者の情報を伝達する必要があるから。
//ここのアルゴリズムは、C10でのブロック管理でも使用しました。

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
    //親オブジェクトから子オブジェクトに保有者の情報を送信する為の関数。
    //爆弾の外装にPlayerの情報を渡した後の、各破片に伝達する役割を担う。
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
