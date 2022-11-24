

//見たら消して良いよ---下村
//メインカメラ周りは余り弄ってませんが、一番下にカメラのめり込み処理が追加されているのと、
//コントローラとの統合に際した条件の書き換えが行われています。

using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 2.0f;//回転スピード
    [SerializeField] private Transform player;//Player

    [SerializeField] private float distance = 5.0f;//Playerとカメラの距離
    [SerializeField] private Quaternion xRotation;//カメラのX軸
    [SerializeField] public Quaternion yRotation;//カメラのY軸

    [SerializeField] public float view;//視点移動

    RaycastHit wallHit;
    //QueryTriggerInteraction wallLayers = default;
    [SerializeField] LayerMask wallLayers;

    void Start()
    {
        //カメラの角度の初期化
        xRotation = Quaternion.Euler(15, 0, 0);//X軸
        yRotation = Quaternion.Euler(0, 0, 0);//Y軸
        transform.rotation = yRotation * xRotation;

        //カメラの位置の初期化
        transform.position = player.position - transform.rotation * Vector3.forward * distance;

        view = 0.0f;
    }

    void Update()
    {
        //コントローラ部、またキーボード用のコード自体は自分の担当じゃありません。
        //ただ、メインプロジェクトにくっつける部分は下村が行いました。

        //コントローラ部
        float rsh = Input.GetAxis("R_Stick_H");
       

        //Y軸の更新
        if (Input.GetKeyDown(KeyCode.N))
            view = -1.0f;
        if (Input.GetKeyDown(KeyCode.M))
            view = 1.0f;
        if (Input.GetKeyUp(KeyCode.N))
            view = 0.0f;
        if (Input.GetKeyUp(KeyCode.M))
            view = 0.0f;

        if (rsh == 1)
            view = 1.0f;
        else if (rsh == -1)
            view = -1.0f;
        else if (!Input.anyKey)
        { 
            view = 0.0f;
        }
        else if(Input.GetKey("joystick button 1")){

            view = 0.0f;

        }





            
        





        yRotation *= Quaternion.Euler(0, view * turnSpeed, 0);

        //transform.rotationの更新
        transform.rotation = yRotation * xRotation;


        transform.position = player.position - transform.rotation * Vector3.forward * distance;

        //メインカメラの壁めり込み対策処理。下村が担当しました。
        //Ray判定
        //Trueだった場合にRayの着弾点に移動する。
        if (Physics.Raycast(player.position, transform.position - player.position, out wallHit, Vector3.Distance(player.position, transform.position), wallLayers, QueryTriggerInteraction.UseGlobal))
        {
            //Debug.Log("eeee");
            transform.position = player.position - transform.rotation * Vector3.forward * distance;
            transform.position = wallHit.point;
        }
    }
}