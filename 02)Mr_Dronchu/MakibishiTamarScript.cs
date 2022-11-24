//α版から何も変わってない。

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MakibishiTamarScript : MonoBehaviour
{
    Rigidbody rb;
    Vector3 FallPoint;//落下地点格納

    //追加しました。SimoLy
    [SerializeField] LayerMask layer;//誘引処理に使う変数。Inspecterからlayerを「Enemy」に指定してください。
    [SerializeField] private int AttractArea;

    //ここから↓

    public AudioClip makibishiRakka;//まきびしが地面に落ちた時のSE
    AudioSource audioSource;

    //ここ追加↑

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        //Debug.Log("Hit");

        FallPoint = houdaiScript.o.transform.position;//落下地点記憶
        Debug.Log(FallPoint);

        //誘引処理の追加。詳細は下。
        AttractEnemy();

        //ここから↓

        audioSource.PlayOneShot(makibishiRakka);//まきびしが地面に落ちた時のSEを追加しました。

        //ここ追加↑
    }


    //誘引処理。追加しました。@SimoLy
    void AttractEnemy()
    {
        //範囲内のコライダーを全取得する関数。()内は中心と半径と取得するレイヤーの指定。レイヤーは「Enemy」を指定。
        Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, AttractArea,layer);

        //念の為作ったColliderからGameObjectに変換する為だけの配列。取得したGameObject=敵を格納する。
        GameObject[] EnemyMember = new GameObject[hitColliders.Length];

        //格納した敵にアタッチされているスクリプトを格納する為の配列。
        AgentNav[] AN = new AgentNav[hitColliders.Length];

        for (int i = 0; i < EnemyMember.Length; i++)
        {
            //上から「変換処理」「AgentNavの取得」「AgentNav内関数の実行」。MakibishiAttractはFallPointに誘導する関数。
            EnemyMember[i] = hitColliders[i].gameObject;
            AN[i] = EnemyMember[i].GetComponent<AgentNav>();
            AN[i].MakibishiAttract(FallPoint);
        }

    }
}
