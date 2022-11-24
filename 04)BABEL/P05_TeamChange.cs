using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using NobleConnect.Examples.Mirror;
using UnityEngine.SceneManagement;

//マッチング画面の状態遷移を行っている関数。
//殆ど手を加えてませんが、CmdListAddという、M01にuser情報を渡す関数の制作を自分が行いました。

public class P05_TeamChange : NetworkBehaviour
{
    //ネットワーク共有用
    [SyncVar] public GameObject BottonScript;
    [SyncVar] public GameObject RoundScript;
    [SyncVar] public int setPosNum;
    [SyncVar] public int MuchNum = 1;
    [SyncVar] public string playerName1 = "player1";
    [SyncVar] public string playerName2 = "player2";
    [SyncVar] public string playerName3 = "player3";
    [SyncVar] public string playerName4 = "player4";
    [SyncVar] public int mode1 = 2;
    [SyncVar] public int mode2 = 2;
    [SyncVar] public int mode3 = 2;
    [SyncVar] public int mode4 = 2;
    [SyncVar] public bool startButton = false;
    GameObject CanvasObj;

    //public GUISkin font;
    //マッチング管理用
    public int CharacrerMode;
    CharacterBotton CB;
    //名前用
    string nameString = "";
    //FadeOut用
    bool FadeOut = false;
    bool strSet = false;
    float keepTime = 1.0f;
    public Texture buttonTexture;
    public static bool Characterset = false, limit = false;

    //12/03 PlayerListの制作
    M01GameManager M01;
    public int PlayerID;

    //12/19
    ExampleMirrorNetworkHUD EMN_HUD;

    //マッチング人数
    int playerNum = 2;

    void Start()
    {
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
        EMN_HUD = GameObject.Find("MirrorMG").GetComponent<ExampleMirrorNetworkHUD>();
        
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "SquareTower")
        {
            if (!limit)
            {


                if (isLocalPlayer)
                {

                    CmdGameObj();
                    BottonScript = GameObject.Find("ButtonScript");
                    
                    CB = BottonScript.GetComponent<CharacterBotton>();
                    CharacrerMode = BottonScript.GetComponent<CharacterBotton>().PlayerMode;

                    
                }

                CmdMuchNum();
                Debug.Log("mode1" + mode1 + "mode2" + mode2);



                //if (PlayerID == 0)
                //{
                //    mode1 = 2;
                //    mode2 = 2;
                //    mode3 = 2;
                //    mode4 = 2;
                //}
                //if (PlayerID == 1&& isServer)
                //{
                //    CharacrerMode = BottonScript.GetComponent<CharacterBotton>().PlayerMode;
                //    CmdMode(mode1,CharacrerMode);
                //}
                //if (PlayerID == 2 && !isServer)
                //{
                //    CharacrerMode = BottonScript.GetComponent<CharacterBotton>().PlayerMode;
                //    CmdMode(mode2, CharacrerMode);
                //}
                //if (PlayerID == 3 && isLocalPlayer)
                //{
                //    CharacrerMode = BottonScript.GetComponent<CharacterBotton>().PlayerMode;
                //    CmdMode(mode3, CharacrerMode);
                //}
                //if (PlayerID == 4 && isLocalPlayer)
                //{
                //    CharacrerMode = BottonScript.GetComponent<CharacterBotton>().PlayerMode;
                //    CmdMode(mode4, CharacrerMode);
                //}
                //CanvasObj = GameObject.Find("CImage3");
                //IconColor(CanvasObj, mode1, mode2, mode3, mode4);
                //初期選択用
                if (CharacrerMode == 0 && BottonScript.GetComponent<CharacterBotton>().setNum == MuchNum) 
                {
                    CharacrerMode = 0;
                    CmdTeamColor(this.gameObject, true, 4);
                    this.gameObject.GetComponent<PlyerControlloer>().enabled = true;
                    this.gameObject.GetComponent<DefenderController>().enabled = false;
                    this.gameObject.GetComponent<PlyerControlloer>().UIInit();

                    //担当しました
                    CmdListAdd(this.gameObject, true);
                    
                }
                if (CharacrerMode == 1 && BottonScript.GetComponent<CharacterBotton>().setNum == MuchNum)
                {
                    CharacrerMode = 1;
                    CmdTeamColor(this.gameObject, true, 5);
                    this.gameObject.GetComponent<PlyerControlloer>().enabled = false;
                    this.gameObject.GetComponent<DefenderController>().enabled = true;
                    this.gameObject.GetComponent<DefenderController>().UIInit();

                    //担当しました
                    CmdListAdd(this.gameObject, false);
                }

                if (CB.setNum == MuchNum)
                {
                    //IP消し
                    EMN_HUD.flag = true;



                    RoundSet.firstPos = false;
                    //if (CharacrerMode == 0)
                    //    this.transform.position = new Vector3(52, 18, 52);
                    //else
                    //    this.transform.position = new Vector3(-52, 18, -52);
                    FadeOut = true;
                    limit = true;
                }

                //PlayerName変更
                if (PlayerID != 0)
                {
                    //CanvasObj.transform.GetChild(6).gameObject.transform.GetChild(13).gameObject.GetComponent<Text>().text = "" + playerName1;
                    //CanvasObj.transform.GetChild(6).gameObject.transform.GetChild(14).gameObject.GetComponent<Text>().text = "" + playerName2;
                    //CanvasObj.transform.GetChild(6).gameObject.transform.GetChild(15).gameObject.GetComponent<Text>().text = "" + playerName3;
                    //CanvasObj.transform.GetChild(6).gameObject.transform.GetChild(16).gameObject.GetComponent<Text>().text = "" + playerName4;
                }
               
            }
            if (FadeOut)
            {
                if (isLocalPlayer)
                {
                    //CmdGameObj();
                    //BottonScript = GameObject.Find("ButtonScript");
                    //CB = BottonScript.GetComponent<CharacterBotton>();
                }
                
                CanvasObj = GameObject.Find("Canvas");
                CanvasObj.transform.GetChild(6).gameObject.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
                if (CanvasObj.transform.GetChild(6).gameObject.GetComponent<CanvasGroup>().alpha <= 0)
                {
                    //マッチ画面FadeIn完了
                    //FadeOut開始処理
                    
                    keepTime -= Time.deltaTime;
                    if (keepTime < 0)
                    {//間隔を置いたら開始
                        Characterset = true;
                        CanvasObj.transform.GetChild(5).gameObject.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
                        if (CanvasObj.transform.GetChild(5).gameObject.GetComponent<CanvasGroup>().alpha <= 0)
                        {
                            
                            BottonScript.transform.parent.gameObject.SetActive(false);
                            FadeOut = false;
                        }
                    }



                }
            }

        }
        else
        {
            if (SceneManager.GetActiveScene().name == "TutorialAttacker")
            {
                CmdTeamColor(this.gameObject, true, 4);
                //CmdTeamColor(this.gameObject, false,5);
                this.gameObject.GetComponent<PlyerControlloer>().enabled = true;
                this.gameObject.GetComponent<DefenderController>().enabled = false;
                this.gameObject.GetComponent<PlyerControlloer>().UIInit();
            }
            if (SceneManager.GetActiveScene().name == "TutorialDefender")
            {
                CmdTeamColor(this.gameObject, true, 5);
                //CmdTeamColor(this.gameObject, true, 5);
                this.gameObject.GetComponent<PlyerControlloer>().enabled = false;
                this.gameObject.GetComponent<DefenderController>().enabled = true;
                this.gameObject.GetComponent<DefenderController>().UIInit();
            }



        }
      

    }
    public void IconColor(GameObject cav,int mode1, int mode2, int mode3, int mode4)
    {
       

        //player1
        if (mode1 == 0)
            cav.gameObject.transform.GetChild(10).gameObject.GetComponent<Text>().text = "Bomber";
        if (mode1 == 1)
            cav.gameObject.transform.GetChild(10).gameObject.GetComponent<Text>().text = "Defender";
        //player2
        if (mode2 == 0)
            cav.gameObject.transform.GetChild(11).gameObject.GetComponent<Text>().text = "Bomber";
        if (mode2 == 1)
            cav.gameObject.transform.GetChild(11).gameObject.GetComponent<Text>().text = "Defender";
        //player3
        if (mode3 == 0)
            cav.gameObject.transform.GetChild(12).gameObject.GetComponent<Text>().text = "Bomber";
        if (mode3 == 1)
            cav.gameObject.transform.GetChild(12).gameObject.GetComponent<Text>().text = "Defender";
        //player4
        if (mode4 == 0)
            cav.gameObject.transform.GetChild(13).gameObject.GetComponent<Text>().text = "Bomber";
        if (mode4 == 1)
            cav.gameObject.transform.GetChild(13).gameObject.GetComponent<Text>().text = "Defender";
    }
 

    [Command]
    void startbutton()
    {
        startButton = true;
        Rpcstartbutton(startButton);
    }
    [ClientRpc]
    void Rpcstartbutton(bool start)
    {
        startButton = true;
    }

    [Command]
    void CmdMuchNum()
    {
        MuchNum = NetworkServer.connections.Count;
        if (isServer)
        {
            
            if (PlayerID == 0)
            {
                PlayerID = MuchNum;
                //nameStri = playerName1;
            }
            //else if (playerName1 == "")
            //    playerName1 = "" + nameStri;
        }
        
        //if (PlayerID == 2 && !isServer)
        //    playerName2 = "" + nameStri;
        //if (PlayerID == 3 && !isServer)
        //    playerName3 = "" + nameStri;
        //if (PlayerID == 4 && !isServer)
        //    playerName4 = "" + nameStri;
        RpcMuchNum(MuchNum);
    }
    [ClientRpc]
    void RpcMuchNum(int Num)
    {
        if (PlayerID == 0)
        {
            PlayerID = Num;
        }


        //ボタン位置をIDによって移動




        //if (PlayerID == 2 && !isServer)
        //    playerName2 = "" + nameS;
        //if (PlayerID == 3 && !isServer)
        //    playerName3 = "" + nameS;
        //if (PlayerID == 4 && !isServer)
        //    playerName4 = "" + nameS;
    }


    [Command]
    void CmdGameObj()
    {
        CanvasObj = GameObject.Find("Canvas");
        BottonScript = GameObject.Find("ButtonScript");
        RoundScript = GameObject.Find("RoundSetUi");
        
        //RpcGameObj(BottonScript, RoundScript,CanvasObj);
    }
    [ClientRpc]
    void RpcGameObj(GameObject obj1, GameObject obj2,GameObject obj3)
    {
       
    }
    [Command]
    void CmdMode(int mode,int charamode)
    {
        mode = charamode;
        Rpcmode(mode,charamode);
    }
    [ClientRpc]
    void Rpcmode(int mode,int charamode)
    {
        mode = charamode;
    }

    [Command]
    void CmdTeamColor(GameObject Player, bool state, int num)
    {
        RpcTeamColor(Player, state, num);
    }
    [ClientRpc]
    void RpcTeamColor(GameObject Player, bool state, int num)
    {
        Player.transform.GetChild(4).gameObject.SetActive(true);
        if (num == 4)
        {
            Player.GetComponent<PlyerControlloer>().JetObj1.SetActive(true);
            Player.GetComponent<PlyerControlloer>().JetObj2.SetActive(true);
            Player.GetComponent<PlyerControlloer>().JetObj3.SetActive(false);
            Player.GetComponent<PlyerControlloer>().JetObj4.SetActive(false);
        }
        if (num == 5)
        {
            GameObject Dcolor;
            GameObject ModelPrefab;
            SkinnedMeshRenderer SMR;
            Material[] matArray = new Material[2];

            Dcolor = Player.transform.GetChild(4).gameObject;
            ModelPrefab = Dcolor.transform.GetChild(0).gameObject;
            SMR = ModelPrefab.transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>();
            matArray[0] = Player.GetComponent<DefenderController>().Defender_M;
            matArray[1] = Player.GetComponent<DefenderController>().Defender_M;
            SMR.materials = matArray;

            Player.GetComponent<DefenderController>().JetObj1.SetActive(false);
            Player.GetComponent<DefenderController>().JetObj2.SetActive(false);
            Player.GetComponent<DefenderController>().JetObj3.SetActive(true);
            Player.GetComponent<DefenderController>().JetObj4.SetActive(true);
        }
    }


    //担当しました
    [Command]
    void CmdListAdd(GameObject Player, bool Team)
    {
        int a = M01.AddPlayerList(Player, Team);
        //Player.GetComponent<P05_TeamChange>().PlayerID = a;
    }

    private void OnGUI()
    {
        if (isServer) { 
            /*if (CB.setNum == MuchNum && !startButton)
            {
                if (GUI.Button(new Rect(140, 881, 480, 130), buttonTexture))
                {
                    startbutton();
                }
            }*/
        }


        //GUI.skin.textField.fontSize = 55;
        //if (ExampleMirrorNetworkHUD.buttonSet && PlayerID == 1)
        //    nameString = GUI.TextField(new Rect(155, 397, 355, 58), nameString);
        //if (ExampleMirrorNetworkHUD.buttonSet && PlayerID == 2)
        //{
        //    nameString = GUI.TextField(new Rect(155, 577, 355, 58), nameString);
        //}
        //if (ExampleMirrorNetworkHUD.buttonSet && PlayerID == 3)
        //    nameString = GUI.TextField(new Rect(155, 757, 355, 58), nameString);
        //if (ExampleMirrorNetworkHUD.buttonSet && PlayerID == 4)
        //    nameString = GUI.TextField(new Rect(155, 937, 355, 58), nameString);


    }

}


// //IconColor(CanvasObj);
//                //CanvasObj.transform.GetChild(6).gameObject.transform.GetChild(8).gameObject.GetComponent<Text>().text = "PlayerName：" + nameString;
//                //CanvasObj.transform.GetChild(6).gameObject.transform.GetChild(11).gameObject.SetActive(false);
//                //CanvasObj.transform.GetChild(6).gameObject.transform.GetChild(12).gameObject.SetActive(true);
//        

    ////if (!strSet)
    //        {
    //            nameString = "player2";
    //            strSet = true;

    //            //OnGUI();
    //        }
               
