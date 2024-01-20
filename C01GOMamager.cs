using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C01GOMamager : MonoBehaviour { 

    public bool fadeOutFlag;

    float timer;

    public GameObject player;//アタッチしろ
    public GameObject XRRig;//アタッチしろ
    public Material fadeMate;

    public bool teleportFlag;
    public float alphaminus;

    public GameObject missileParant;//アタッチしろ

    Vector3 warpPosition = new Vector3(-500, -503, -500);
    // Start is called before the first frame update
    void Start()
    {
        FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Fade);
        fadeMate.color = new Color(0, 0, 0, 0);
        alphaminus = 255;
    }

    // Update is called once per frame
    void Update()
    {

        //UIGameOverからフラグをセット
        //暗転、及び落下の終了からスタート。
        if(fadeOutFlag == true)
        {

            //明転処理 Ending用のミサイル消す処理を勝手に拝借した。
            timer += Time.deltaTime;
            missileParant.SetActive(false);

            //暗転用の予備待機時間
            //テレポートの瞬間を映さない為の処理
            if(timer < 2.0f)
            {
                //alphaminus = 255;
                //テレポート
                player.transform.position = warpPosition;
            }

            //明転
            if(timer > 2.0f)
            {
                //XRRigが付いてこないと移動制限で動けなくなる
                if (teleportFlag == false)
                {
                    //alphaminusは0-1の範囲。
                    alphaminus = 1;
                    XRRig.transform.position = warpPosition;
                    player.transform.position = XRRig.transform.localPosition;//カメラを親の0,0,0に合わせる
                    FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Fade);
                }
                teleportFlag = true;
                
            }
            //テレポート終了後
            //明転
            if(teleportFlag == true)
            {
                //0以下で0に固定
                if (alphaminus < 0)
                {
                    alphaminus = 0;
                    fadeMate.color = new Color(0, 0, 0, 0);

                }
                if (alphaminus > 0)
                {
                    alphaminus -= Time.deltaTime * 0.2f;
                    fadeMate.color = new Color(0, 0, 0, alphaminus);
                    //fadeMate.color = new Color(0, 0, 0, 0);
                }
                if(alphaminus == 0)
                {
                    //FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Cutout);
                    fadeMate.color = new Color(0, 0, 0, alphaminus);
                }

                //fadeMate.color = new Color(0, 0, 0, 0);
                
                
            }
        }
    }

    public void ResetParam()
    {
        timer = 0f;
        fadeOutFlag = false;
        alphaminus = 255;
        teleportFlag = false;
        FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Fade);
        missileParant.SetActive(true);
    }
}
