
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//ゲームオーバー時の暗転、及び再開用スクリプト
public class UIGameOver : MonoBehaviour
{
    //墜落時の暗転用

    public GameObject gameOverUI; //game overのCanvas
    public Image fadeImage;//暗転用のImage
    public Text goText; //game over text.
    public GameObject conButton; //リトライ用のボタン。

    public bool flag;
    float lerpTime;//暗転用
    float lerpRange = 4f;//暗転用

    Vector3 endPosition; //墜落時の座標 再開地点の決定に使用。
    Vector3 restartPos;//再開地点

    public bool conflag;//コンテニュー処理用のフラグ。
    float conLerp = 3f; //再開時の明転

    public GameObject whaleObject; //クジラ。再開時に位置を設定し直すのに必要。

    public GameObject camObject; //MainCameraのオブジェクト。墜落時にカメラも落とす為に必要。
    Vector3 cameraOffset = new Vector3(0, 1.36144257f, 0); // 多分カメラの座標のズレ。
    public Rigidbody a; //カメラのRigidbody 墜落するのに使用。

    public GameObject XRrig; //前進速度を0にする為に必要。

    Vector3 boardOffset = new Vector3(0, 1.36143994f, 0);

    //public GameObject uiColider;//UIColider Continue処理用 アタッチしろ

    public W05WhaleTutorial W05;

    public Material fademat;

    float alphaPlus;

    public C01GOMamager C01; //アタッチしろ

    public EnergyCharge energyCharge;//アタッチ

    ///public Material fadeMat;

  


    // Start is called before the first frame update
    void Start()
    {
        //fadeImage = gameOverUI.GetComponent<Image>();
        W05 = whaleObject.GetComponent<W05WhaleTutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        //ゲームオーバー。 PlayerDamagedのOnTriggerでflagをセット。
        if (flag == true)
        {
            //UI Canvasがfalseの場合(死んで最初のフレーム)
            //UIオブジェクトを表示。終了時の座標を保存。
            if (gameOverUI.activeSelf == false)
            {
                gameOverUI.SetActive(true);
                gameOverUI.transform.GetChild(0).gameObject.SetActive(true);
                endPosition = this.transform.position;

            }

            //暗転用のTimerの加算。
            //暗転処理
            if (lerpTime < lerpRange)
            {
                //落ちはじめshot用
                energyCharge.IsEnd = true;
                lerpTime = lerpTime + Time.deltaTime;
                alphaPlus += 0.35f * Time.deltaTime;

                fademat.color = new Color(0, 0, 0, alphaPlus);

                if(this.transform.position.y < 20)
                {
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<UIGameOver>().a.useGravity = false;//a = mainCamera。カメラを墜落させないと視点が堕ちない為。

                    a.useGravity = false;


                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    a.velocity = Vector3.zero;
                }


            }
            //決められた時間を経過した場合(暗転終了)
            //テキストとボタンを表示。
            else
            {
                W05.startFlag = false;
                lerpTime = lerpRange;
                goText.enabled = true;
                conButton.SetActive(true);

                //alphaPlus += 50.0f * Time.deltaTime;

                //暗転の終了に合わせて落下を止める
                GetComponent<Rigidbody>().useGravity = false;
                a.useGravity = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                a.velocity = Vector3.zero;

                //
                //uiColider.SetActive(true);

                GetComponent<PlayerDamaged>().HP = 5;

                //flag = false;

                //ゲームオーバールームに転送する為の処理。以降はC01。
                if (C01.fadeOutFlag == false)
                {
                    //FSShaderScript.SetBlendMode(fademat, FSShaderScript.Mode.Cutout);
                    C01.fadeOutFlag = true;

                }
            }

            //フェードアウト
            fadeImage.color = new Color(0, 0, 0, lerpTime / 3);
            if (C01.fadeOutFlag == false)
            {
                
            }
        }

        //コンティニュー処理
        if (conflag == true)
        {

            //明転 0になったら明転完了
            if (lerpTime > 0)
                lerpTime = lerpTime - Time.deltaTime;
            //ゲームスタート
            //Canvasをオフ、速度を元通り、くじらの位置を再設定。
            else
            {
                lerpTime = 0;
                gameOverUI.SetActive(false);
                gameOverUI.transform.GetChild(0).gameObject.SetActive(false);

                XRrig.GetComponent<FollowLine>().speed = 15f;
                XRrig.GetComponent<FollowLine>().ResetLineOrder();

                conflag = false;
                //flag = false;
               


                //uiColider
                //uiColider.SetActive(false);
                W05.startFlag = true;

                //shot回復用
                energyCharge.IsEnd = false;
            }
            //フェードイン
            fadeImage.color = new Color(0, 0, 0, lerpTime);
        }
    }


    //Buttonにアタッチ。
    //開始位置、明転用のタイマー、重力の解除など。
    public void Continue()
    {
        //mizunoSE
        SeSystem.GameOver = false;
        //開始位置の指定
        //終了時のプレイヤーのx座標を参照。
        if (endPosition.x < 1200)
            restartPos = new Vector3(0, 100, 50);
        else if (endPosition.x < 2400)
            restartPos = new Vector3(1200, 100, 50);
        else
            restartPos = new Vector3(2400, 100, 50);

        //明転用の時間
        lerpTime = 3f;
        //処理を始める為のフラグ。
        conflag = true;

        //テキスト、ボタンの削除。
        goText.enabled = false;
        conButton.SetActive(false);

        //HPの再設定、重力の解除。慣性の削除の為にvelociyも0に。
        GetComponent<PlayerDamaged>().HP = 5;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //カメラの方も同様。
        a.useGravity = false;
        a.velocity = Vector3.zero;

        //暗転処理の解除
        flag = false;


        //くじらの位置
        whaleObject.transform.position = new Vector3(restartPos.x + 100, 100, 50);
        whaleObject.GetComponent<W01WhaleMoving>().ResetParam(W05.player);

        //再開位置。向きの修正。
        XRrig.transform.position = restartPos;
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        this.gameObject.transform.position = restartPos + cameraOffset;

        //カメラの位置も同様。
        camObject.transform.position = this.gameObject.transform.position + cameraOffset;
        camObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        //GameOverRoom周りのフラグのリセット。
        C01.ResetParam();

       
    }



    
}
