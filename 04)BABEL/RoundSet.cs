using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NobleConnect.Examples.Mirror;

//バトル中の進行を管理するスクリプト。
//Set==5,Set==6の二つは、主に自分が担当しました。(全てではありません)
//ラウンド終了時の得点計算と表示、バトル終了時のタイトルへの帰還を行っています。

public class RoundSet : NetworkBehaviour
{
    //Network共有変数
    [SyncVar] public float RoundTimer=4.9f;
    [SyncVar] public int set = 0;
    [SyncVar] public int roundSet = 1;

    //Ui管理用
    [SerializeField] GameObject CanvasObj;
    CanvasGroup EquipCanvas;
    CanvasGroup ItemCanvas;
    CanvasGroup ScoreCanvas;
    CanvasGroup PowerCanvas;
    bool FadeButton = true;
    float FadeAlpha=1f;

    //UIText用
    [SerializeField] Text ADtext;
    [SerializeField] Text DroundText;
    [SerializeField] Text AroundText;
    [SerializeField] Text CountText;
    [SerializeField] Text PlayerpointText1, PlayerpointText2, PlayerpointText3, PlayerpointText4,
        resultText1,resultText2, resultText3, resultText4,roundText;

    //SimoLieF
    [SerializeField] Text[] PlayerpointText = new Text[4];
    [SerializeField] Text[] ResultPlayerText = new Text[4];
    [SerializeField] Text[] ResultScoreText = new Text[4];
    
    //Player制限用
    public static bool DefenderPhase=false;
    public static bool AttackerPhase = false;
    public static bool StopPlayer = true;
    public static bool StopWall = false;
    public static bool StopBomb = false;
    public static bool firstPos = false;

    //タワーオブジェクトの取得
    public GameObject PyramidTower;
    public GameObject CircleTower;
    public GameObject SqurareTower;
    public GameObject WiPyramidTower;
    public GameObject WiCircleTower;
    public GameObject WiSqurareTower;
    
    GameObject KeepPyramid;
    GameObject KeepCircle;
    GameObject KeepSquare;
    GameObject WiKeepPyramid;
    GameObject WiKeepCircle;
    GameObject WiKeepSquare;
    

    bool spawnLimit,WispawnLimit;
    float bomber1, bomber2, defender1, defender2;

    //スコア計算用
    [SyncVar] public float TimerStop;
    M01GameManager M01;
    public P04ItemHolder P04;
    bool resultCheck = false;

    //リセット用
    ExampleMirrorNetworkHUD EMN_HUD;

    int RoundNum = 0;

    void Start()
    {
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
        EMN_HUD = GameObject.Find("MirrorMG").GetComponent<ExampleMirrorNetworkHUD>();

    }

    public void Update()
    {
        /*if (SceneManager.GetActiveScene().name == "SquareTower")
        {
            AttackerPhase = true;
            DefenderPhase = true;
            return;
        }*/


        if (BattleStartCamera.GameStart) {
            firstPos = true;
            //GameUiの表示
            if (FadeButton)
                UiFadeIn();
            else
                UiFadeOut();

            if (isServer&&!spawnLimit) {
                //GameUi表示On

                wallDestroy.Destroy = false;
                TowerManager.destroyOn = false;

                spawnLimit = true;
            }
            //ステージのスポーン
            if (isServer && roundSet == 1 &&!WispawnLimit)
            {
                TowerManager.destroyOn = false;
                WiKeepSquare = Instantiate(WiSqurareTower, new Vector3(0, 0.5f, 0), Quaternion.identity);
                NetworkServer.Spawn(WiKeepSquare);
                WispawnLimit = true;
            }
            else if (isServer && roundSet == 2&&!WispawnLimit)
            {
                TowerManager.destroyOn = false;
                WiKeepPyramid = Instantiate(WiPyramidTower, new Vector3(0, 4, 0), Quaternion.identity);
                NetworkServer.Spawn(WiKeepPyramid);
                WispawnLimit = true;
            }
            else if (isServer && roundSet == 3&&!WispawnLimit)
            {
                TowerManager.destroyOn = false;
                WiKeepCircle = Instantiate(WiCircleTower, new Vector3(0, 0.5f, 0), Quaternion.identity);
                NetworkServer.Spawn(WiKeepCircle);
                WispawnLimit = true;
            }
            Timer.timeStart = false;
            Timer.setStart = false;

            if (set == 0)
            {//CountDown
                this.transform.GetChild(6).gameObject.SetActive(false);
                if (RoundTimer < 3.9f) { 
                    GameObject obj1;
                    obj1 = this.gameObject.transform.GetChild(3).gameObject;
                    obj1.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    obj1.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    obj1.gameObject.transform.GetChild(3).gameObject.SetActive(false);
                    obj1.gameObject.transform.GetChild(4).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(3).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(2).gameObject.SetActive(true);
               

                    if (RoundTimer > 0)
                    {
                        CountText.text = "" + (int)RoundTimer;
                        Timer.timeStart = true;
                    }
                    else
                    {
                        if (isServer)
                            Timer.setStart = true;
                    }
                }
                else
                {
                    Timer.timeStart = true;
                }
            }

            if (set == 1) {
                this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                StopPlayer = false;
                //AttackerPhase
                ADtext.text = "Break     Phase";
                //プレイヤーの制限処理
                if (roundSet == 1) {
                    //BGM再生
                    this.transform.GetChild(5).gameObject.SetActive(true);
                    StopWall = true;
                    AttackerPhase = true;
                    DefenderPhase = true;
                }
                if (roundSet >= 2)
                {
                    //BGM再生
                    this.transform.GetChild(5).gameObject.SetActive(true);
                    //クラスターだけは撃てる
                    StopBomb = false;
                    AttackerPhase = true;
                    //ディフェンダーは壁は置けない
                    StopWall = true;
                    DefenderPhase = true;
                }

                
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
               
                if (RoundTimer < 87f)
                {
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.SetActive(false);
                }

                if (RoundTimer > 45f)
                {
                    float minutes = (int)RoundTimer / 60;
                    if ((int)RoundTimer % 60 >= 10)
                        DroundText.text = "0" + minutes + ":" + (int)RoundTimer % 60;
                    else
                        DroundText.text = "0" + minutes + ":0" + (int)RoundTimer % 60;
                    Timer.timeStart = true;
                    
                }
                else
                {
                    if (isServer)
                    {
                        
                        Timer.setStart=true;
                    }

                    DroundText.text = "0";
                    
                    
                }
            }
            if(set==2)
            {
                
                //プレイヤーの制限処理
                StopWall = false;
                StopBomb = true;
                if (roundSet == 1)
                {
                    AttackerPhase = true;
                    DefenderPhase = true;
                }
                if (roundSet == 2)
                {
                    AttackerPhase = true;
                    DefenderPhase = true;
                }
                if (RoundTimer < 42f)
                {
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.SetActive(true);
                }
                //DefenderPhase
                ADtext.text = "Stand By Phase";

                if (RoundTimer > 3)
                {
                    if ((int)RoundTimer >= 10)
                        DroundText.text = "00:" + (int)RoundTimer;
                    else
                        DroundText.text = "00:0" + (int)RoundTimer;
                    Timer.timeStart = true;

                }
                else
                {
                    
                    if (isServer) {

                        TowerManager.destroyOn = true;
                        //ステージのスポーン
                        if (roundSet == 1)
                        {
                            KeepSquare = Instantiate(SqurareTower, new Vector3(0, 0.5f, 0), Quaternion.identity);
                            NetworkServer.Spawn(KeepSquare);
                        }
                        else if (roundSet == 2)
                        {
                            KeepPyramid = Instantiate(PyramidTower, new Vector3(0, 4, 0), Quaternion.identity);
                            NetworkServer.Spawn(KeepPyramid); 
                        }
                        else if (roundSet == 3)
                        {
                            KeepCircle = Instantiate(CircleTower, new Vector3(0, 0.5f, 0), Quaternion.identity);
                            NetworkServer.Spawn(KeepCircle);
                        }
                        Timer.setStart = true;
                    }
                    

                }
            }


            if (set == 3)
            {//CountDown

                DefenderPhase = false;
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                this.gameObject.transform.GetChild(2).gameObject.SetActive(true);

                if (RoundTimer > 0)
                {
                    CountText.text = "" + (int)RoundTimer;
                    Timer.timeStart = true;
                }
                else
                {
                    if(isServer)
                        Timer.setStart = true;
                }
            }
            if (set == 4)
            {//BattlePhase
                this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                //プレイヤーの制限処理
                DefenderPhase = true;
                AttackerPhase = true;
                StopBomb = false;

                if (RoundTimer > 0)
                {
                    int minutes;
                    minutes = (int)RoundTimer / 60;
                    if ((int)RoundTimer % 60 >= 10)
                        AroundText.text = "0" + minutes + ":" + (int)RoundTimer % 60;
                    else
                        AroundText.text = "0" + minutes + ":0" + (int)RoundTimer % 60;

                    Timer.timeStart = true;
                    
                }
                else if (RoundTimer <= 0 && RoundTimer > -3.0f) 
                {
                    this.transform.GetChild(5).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                    StopPlayer = true;
                    DefenderPhase = false;
                    AttackerPhase = false;
                    Timer.timeStart = true;

                    //M01.breakend = false;
                }
                else
                {
                    if (isServer)
                        Timer.setStart = true;
                }

                //得点計算用 及び終了条件
                if (RoundTimer > 0 && M01.breakend == true)
                {
                    TimerStop = RoundTimer;
                    RoundTimer = 0;
                }
                else if (RoundTimer > 0 && M01.end == true)
                {
                    TimerStop = RoundTimer;
                    RoundTimer = 0;
                }
                resultCheck = false;
                
            }


            //---------------------------担当箇所
            if (set == 5)
            {

                //result
                if (resultCheck == false)
                {
                    if (isServer)
                    {
                        /*//player1得点位置---------------------------------------------
                        if (M01.user[0].side == true)
                            bomber1 = 0;
                        else
                            defender1 = 0;
                        //player2得点位置
                        if (M01.user[1].side == true && bomber1 == 5)
                            bomber1 = 1;
                        else if (M01.user[1].side == false && defender1 == 5) 
                            defender1 = 1;
                        if (M01.user[1].side == true && bomber1 != 5)
                            bomber2 = 1;
                        else if (M01.user[1].side == false && defender1 != 5)
                            defender2 = 1;
                        //player3得点位置
                        if (M01.user[2].side == true && bomber1 == 5)
                            bomber1 = 2;
                        else if (M01.user[2].side == false && defender1 == 5)
                            defender1 = 2;
                        if (M01.user[2].side == true && bomber2 != 5)
                            bomber2 = 2;
                        else if (M01.user[2].side == false && defender1 != 5)
                            defender2 = 2;
                        //player4得点位置
                        if (M01.user[2].side == true)
                            bomber2 = 3;
                        else if (M01.user[2].side == false)
                            defender2 = 3;
                        //--------------------------------------------------------------*/


                       Debug.Log("m01" + M01.timeScore);
                        int player1Score = M01.user[0].player.GetComponent<P04ItemHolder>().Score;
                        M01.BonusSet((int)TimerStop);
                        resultCheck = true;
                        RpcTextUpdate(M01.timeScore,M01.remainScore,M01.bonusScore/*,P04.Score,player1Score*/);
                    }
                }

                this.transform.GetChild(6).gameObject.SetActive(true);
                //Ui関連の表示
               
                GameObject obj1;
                obj1 = this.gameObject.transform.GetChild(3).gameObject;
                this.gameObject.transform.GetChild(3).gameObject.SetActive(true);
                this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                //プレイヤーの制限処理
                DefenderPhase = false;
                AttackerPhase = false;
                //タワーの削除
               
                roundText.text = "Round" + roundSet;

               
              

                if (RoundTimer > 16f)
                {
                    if (isServer)
                    {
                        TowerManager.destroyOn = true;
                        wallDestroy.Destroy = true;
                    }
                    Timer.timeStart = true;
                    obj1.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    //最初にスコアを代入してる為、ここの処理は削除しました。
                    //resultText1.text = "00000";


                    //各プレイヤーへのスコアの加算(時間)
                    for (int i = 0; i < M01.user.Count; i++)
                    {
                        
                        Debug.Log(M01.user.Count);
                        //Defenderのみ
                        if (M01.user[i].side == false)
                        {
                            M01.user[i].player.GetComponent<P04ItemHolder>().Score += M01.timeScore;
                        }

                    }
                    M01.timeScore = 0;

                    M01.towerBreakcount = 0;

                }
                else if (RoundTimer > 13f)
                {
                    Timer.timeStart = true;
                    obj1.gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    //resultText2.text = "00000";


                    //プレイヤーへのスコアの加算(残存ブロック数)
                    for (int i = 0; i < M01.user.Count; i++)
                    {
                        Debug.Log("ddd");
                        //Defenderのみ
                        if (M01.user[i].side == false)
                        {
                            M01.user[i].player.GetComponent<P04ItemHolder>().Score += M01.remainScore;
                        }

                    }
                    M01.remainScore = 0;
                }
                else if (RoundTimer > 9f)
                {
                    Timer.timeStart = true;
                    obj1.gameObject.transform.GetChild(3).gameObject.SetActive(true);
                    //resultText3.text = "00000";


                    //終了条件によるスコアの加算
                    for (int i = 0; i < M01.user.Count; i++)
                    {
                        //Debug.Log("dddd");
                        if (M01.bonusNum == 3)
                        {
                            //Defenderのみ 終了条件3の時
                            if (M01.user[i].side == false)
                                M01.user[i].player.GetComponent<P04ItemHolder>().Score += M01.bonusScore;
                        }
                        else
                        {
                            //Bomber,終了条件1,2の時。
                            if (M01.user[i].side == true)
                                M01.user[i].player.GetComponent<P04ItemHolder>().Score += M01.bonusScore;

                        }
                    }
                    M01.bonusScore = 0;


                }
                else if (RoundTimer <= 6f && RoundTimer > 0f) 
                {
                    Timer.timeStart = true;
                    if (M01.JudgeScore() == 0)
                        resultText4.text = "Bomberチームがリードしています";
                    else if(M01.JudgeScore() == 1)
                        resultText4.text = "Defenderチームがリードしています";
                    else
                        resultText4.text = "どちらも同じ点数です";
                    obj1.gameObject.transform.GetChild(4).gameObject.SetActive(true);
                }
                else
                {
                    DefenderPhase = false;
                    AttackerPhase = false;
                   

                    if (isServer)
                    {
                        WispawnLimit = false; wallDestroy.Destroy = false;
                        RoundNum++;
                        Timer.setStart = true;
                       
                    }


                }
            }
            //Set:6がゲームの終わり、つまりここからタイトルに戻す。
            if (set == 6)
            {
                //endResultの表示
                this.gameObject.transform.GetChild(4).gameObject.SetActive(true);
                GameObject enduiobj = this.gameObject.transform.GetChild(4).gameObject;
                GameObject uiobj = enduiobj.transform.GetChild(0).gameObject;




                string Winner,Loser;
                bool Bomber=false, Defender=false;
                if (M01.JudgeScore() == 0)
                {
                    uiobj.transform.GetChild(0).gameObject.SetActive(true);
                    uiobj.transform.GetChild(1).gameObject.SetActive(false);
                    Winner = "Bomber";
                    Loser = "Defender";


                    for (int i = 0; i < M01.user.Count; i++)
                    {
                        if (M01.user[i].side == true)
                        {
                            if (Bomber == false)
                            {
                                ResultScoreText[0].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                                Bomber = true;
                            }
                            if (Bomber == true)
                            {
                                ResultScoreText[1].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                            }
                        }
                        else
                        {
                            if (Defender == false)
                            {
                                ResultScoreText[2].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                                Defender = true;
                            }
                            if (Defender == true)
                            {
                                ResultScoreText[3].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                            }
                        }
                    }
                

                    resultText4.text = "Bomberチームがリードしています";

                }
                else
                {

                    uiobj.transform.GetChild(0).gameObject.SetActive(false);
                    uiobj.transform.GetChild(1).gameObject.SetActive(true);
                    Winner = "Defender";
                    Loser = "Bomber";

                    for (int i = 0; i < M01.user.Count; i++)
                    {
                        if (M01.user[i].side == false)
                        {
                            if (Defender == false)
                            {
                                ResultScoreText[0].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                                Defender = true;
                            }
                            if (Defender == true)
                            {
                                ResultScoreText[1].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                            }
                        }
                        else
                        {
                            if (Bomber == false)
                            {
                                ResultScoreText[2].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                                Bomber = true;
                            }
                            if (Bomber == true)
                            {
                               ResultScoreText[3].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                            }
                        }
                    }


                    resultText4.text = "Defenderチームがリードしています";
                }

                ResultPlayerText[0].text = Winner + "1";
                ResultPlayerText[1].text = Winner + "2";
                ResultPlayerText[2].text = Loser + "1";
                ResultPlayerText[3].text = Loser + "2";

             
                    

               
               
                DefenderPhase = false;
                AttackerPhase = false;
                if (Input.GetKeyDown(KeyCode.M))
                {
                    //EMN_HUD.
                    //P05_TeamChange.Characterset = false;
                    //BattleStartCamera.GameStart = false;

                    if (isServer)
                    {
                        //RpcGameReset();
                        EMN_HUD.networkManager.StopClient();
                        EMN_HUD.networkManager.StopHost();

                        SceneManager.LoadScene("TitleScene");
                    }

                    set = -1;
                    
                }
                
            }
        }

        if (!NetworkClient.isConnected)
        {
            SceneManager.LoadScene("TitleScene");
        }

        //------------------------担当箇所

    }
    void UiFadeIn()
    {
        //UiCanvasGroupを取得
        EquipCanvas = CanvasObj.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        ItemCanvas = CanvasObj.transform.GetChild(1).gameObject.GetComponent<CanvasGroup>();
        ScoreCanvas = CanvasObj.transform.GetChild(2).gameObject.GetComponent<CanvasGroup>();
        PowerCanvas = CanvasObj.transform.GetChild(3).gameObject.GetComponent<CanvasGroup>();

        //Alpha値の調整
        FadeAlpha += Time.deltaTime;
        if (FadeAlpha > 1.0f)
            FadeAlpha = 1.0f;

        EquipCanvas.alpha = FadeAlpha;
        ItemCanvas.alpha = FadeAlpha;
        ScoreCanvas.alpha = FadeAlpha;
        PowerCanvas.alpha = FadeAlpha;
    }
    void UiFadeOut()
    {
        //UiCanvasGroupを取得
        EquipCanvas = CanvasObj.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        ItemCanvas = CanvasObj.transform.GetChild(1).gameObject.GetComponent<CanvasGroup>();
        ScoreCanvas = CanvasObj.transform.GetChild(2).gameObject.GetComponent<CanvasGroup>();
        PowerCanvas = CanvasObj.transform.GetChild(3).gameObject.GetComponent<CanvasGroup>();

        //Alpha値の調整
        FadeAlpha -= Time.deltaTime;
        if (FadeAlpha < 0f)
            FadeAlpha = 0f;

        EquipCanvas.alpha = FadeAlpha;
        ItemCanvas.alpha = FadeAlpha;
        ScoreCanvas.alpha = FadeAlpha;
        PowerCanvas.alpha = FadeAlpha;

    }


    [Command(requiresAuthority = false)]
    void CmdTimerSet(bool set)
    {
        set = true;
        RpcTimerSet(set);
    }

    [ClientRpc]
    void RpcTimerSet(bool set)
    {
        

    }


    //----------------------担当箇所
    [ClientRpc]
    void RpcTextUpdate(int tScore,int rScore,int bScore/*,int score,int m1*/)
    {
        bool bomber= false, defender = false;

        resultText1.text = tScore.ToString();
        resultText2.text = rScore.ToString();
        resultText3.text = bScore.ToString();

        //if (bomber1 == 0) 
            //PlayerpointText1.text = score.ToString();
        //if (bomber1 == 1)
        //    PlayerpointText1.text = m1.ToString();
        //if (bomber1 == 2)
        //    PlayerpointText1.text = m1.ToString();
        //if (bomber2 == 1)
        //    PlayerpointText2.text = m1.ToString();
        //if (bomber2 == 2)
        //    PlayerpointText2.text = m1.ToString();
        //if (bomber2 == 3)
        //    PlayerpointText2.text = m1.ToString();

        //if (defender1 == 0)
        //    PlayerpointText1.text = m1.ToString();
        //if (defender1 == 1)
        //    PlayerpointText1.text = m1.ToString();
        //if (defender1 == 2)
        //    PlayerpointText1.text = m1.ToString();
        //if (defender2 == 1)
        //    PlayerpointText2.text = m1.ToString();
        //if (defender2 == 2)
        //    PlayerpointText2.text = m1.ToString();
        //if (defender2 == 3)
        //    PlayerpointText2.text = m1.ToString();

        /*PlayerpointText1.text = M01.user[0].player.GetComponent<P04ItemHolder>().Score.ToString();
        PlayerpointText2.text = M01.user[1].player.GetComponent<P04ItemHolder>().Score.ToString();
        PlayerpointText3.text = M01.user[2].player.GetComponent<P04ItemHolder>().Score.ToString();
        PlayerpointText4.text = M01.user[3].player.GetComponent<P04ItemHolder>().Score.ToString();*/

        for(int i = 0; i < M01.user.Count; i++)
        {
            if(M01.user[i].side == true)
            {
                if(bomber == false)
                {
                    PlayerpointText[0].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                    bomber = true;
                }
                if(bomber == true)
                {
                    PlayerpointText[1].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                }
            }
            else
            {
                if(defender == false)
                {
                    PlayerpointText[2].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                    defender = true; 
                }
                if(defender == true)
                {
                    PlayerpointText[3].text = M01.user[i].player.GetComponent<P04ItemHolder>().Score.ToString();
                }
            }
          
        }
        resultCheck = true;
    }

    public void GameEnd()
    {
        Debug.Log("ssssss");
            //RpcGameReset();
            EMN_HUD.networkManager.StopClient();
            EMN_HUD.networkManager.StopHost();

            SceneManager.LoadScene("TitleScene");
    }

    [ClientRpc]
    void RpcGameReset()
    {
        P05_TeamChange.Characterset = false;
        BattleStartCamera.GameStart = false;


        EMN_HUD.networkManager.StopClient();
        if (isServer)
            EMN_HUD.networkManager.StopHost();

        SceneManager.LoadScene("TitleScene");
    }

    //-------------------------担当箇所
}
