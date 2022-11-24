//ミニマップの奴。送られてきた奴そのまま使いました。
//どうでもいいけど、これキーボードの時だけ視点と一緒にミニマップ回りますね。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 2.0f;//回転スピード
    [SerializeField] private Transform player;//Player

    [SerializeField] private float distance = 100.0f;//Playerとカメラの距離
    [SerializeField] private Quaternion xRotation;//カメラのX軸
    [SerializeField] public Quaternion yRotation;//カメラのY軸

    [SerializeField] public float view;//視点移動
    void Start()
    {
        //カメラの角度の初期化
        xRotation = Quaternion.Euler(90, 0, 0);//X軸
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

        /*//コントローラ部
        float rsh = Input.GetAxis("R_Stick_H");

        if (rsh == 1)
            view = 1.0f;
        else if (rsh == -1)
            view = -1.0f;
        else
            view = 0.0f;*/

        //Y軸の更新
        if (Input.GetKeyDown(KeyCode.N))
            view = -1.0f;
        if (Input.GetKeyDown(KeyCode.M))
            view = 1.0f;
        if (Input.GetKeyUp(KeyCode.N))
            view = 0.0f;
        if (Input.GetKeyUp(KeyCode.M))
            view = 0.0f;
        yRotation *= Quaternion.Euler(0, view * turnSpeed, 0);

        //transform.rotationの更新
        //transform.rotation = yRotation * xRotation;


        transform.position = player.position - transform.rotation * Vector3.forward * distance;
    }
}
