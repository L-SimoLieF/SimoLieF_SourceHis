//何も変わってないし、誰にも弄らせてない。

using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEditor;//Gizmos用
using UnityEngine;

//アタッチ先は敵本体じゃなくて、子オブジェクトの視線用コライダーがついたEmptyObjectにしてください。

public class EnemyVision : MonoBehaviour
{
    [SerializeField] SphereCollider searchArea;
    [SerializeField] float searchAngle = 60f;
    [SerializeField] private LayerMask obstacleLayer;

    //追加しました。
    GameObject AtGage;
    EnemyScript ES;

    // Start is called before the first frame update
    void Start()
    {
        AtGage = GameObject.Find("AttentionText");
        ES = AtGage.GetComponent<EnemyScript>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        //変化の確認。マジしょーもないコード書いて罪悪感半端ない。
        //変化中、もしくは隠れ身中は視界判定を行わない。
        if (ES.HengeChecker == false)
        {

            //enemyStatus==false→ES.AttentionGageに変更。
            //ESは警戒ゲージ管理用スクリプト。表示と加算以外は何もしてないに等しい。
            //警戒度と警戒レベルを仮置き。1フレーム毎の加算の為、100単位で増加、100で区切って警戒レベルで分岐(はしてないけど。)
            //@SimoLy
            if (ES.AttentionGage / 100 < 4)
            {
                if (other.tag == "Player")
                {
                    Debug.DrawLine(transform.position + Vector3.up, other.transform.position+Vector3.up,Color.blue);
                    if (!Physics.Linecast(transform.position + Vector3.up,other.transform.position+Vector3.up, obstacleLayer))
                    {
                        //　主人公の方向
                        var playerDirection = other.transform.position - transform.position;
                        //　敵の前方からの主人公の方向
                        var angle = Vector3.Angle(transform.forward, playerDirection);
                        //　サーチする角度内だったら発見
                        if (angle <= searchAngle)
                        {
                            Debug.Log("主人公発見: " + angle);
                            ES.AttentionGage += 1;
                        }
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    //　サーチする角度表示
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -searchAngle, 0f) * transform.forward, searchAngle * 2f, searchArea.radius / 2);
    }
#endif

}
