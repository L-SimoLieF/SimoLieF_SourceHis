using NobleConnect.Mirror;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

//アセット NobleConnectの通信用スクリプト
//追加箇所に自分が手を加えました。
//終了からタイトルまでの画面遷移を行う為の変数の追加等が主な変更点です。

namespace NobleConnect.Examples.Mirror
{
    // A GUI for use with NobleNetworkManager
    public class ExampleMirrorNetworkHUD : MonoBehaviour
    {
        // The NetworkManager controlled by the HUD
        public NobleNetworkManager networkManager;
        [SerializeField] GameObject image1;
        [SerializeField] GameObject image2;
        [SerializeField] GameObject image3;
        // The relay ip and port from the GUI text box
        string hostIP = "";
        string hostPort = "";




        // Used to determine which GUI to display
        bool isHost, isClient;

        // Get a reference to the NetworkManager

        //Added by SimoLieF
        public bool flag = false;
        public Texture buttonTexture;
        public static bool buttonSet = false;
        public bool PlayerSpawn = false;

        GameObject StartsetUi;

        public void Start()
        {
            // Cast from Unity's NetworkManager to a NobleNetworkManager.
            networkManager = (NobleNetworkManager)NetworkManager.singleton;
        }

        public void Update()
        {
            //---------------追加箇所
            if (SceneManager.GetActiveScene().name == "TutorialAttacker")
            {
                networkManager.StartHost();
                //Debug.Log(NetworkServer.connections[NetworkClient.connection.connectionId]);
                Debug.Log(NetworkClient.isConnected);

                if (PlayerSpawn == false)
                {
                    isHost = true;
                    isClient = false;

                    PlayerSpawn = true
                    networkManager.OnServerAddPlayer(NetworkServer.connections[NetworkClient.connection.connectionId]);

                }

                if (escEndScript.endTu)
                {
                    Debug.Log("dd");
                    isHost = false;
                    networkManager.StopHost();
                    PlayerSpawn = false;
                    escEndScript.endTu = false;

                    P05_TeamChange.Characterset = false;
                    BattleStartCamera.GameStart = false;

                }

            }

            if (SceneManager.GetActiveScene().name == "TutorialDefender")
            {
                networkManager.StartHost();
                //Debug.Log(NetworkServer.connections[NetworkClient.connection.connectionId]);
                Debug.Log(NetworkClient.isConnected);

                if (PlayerSpawn == false)
                {
                    isHost = true;
                    isClient = false;

                    PlayerSpawn = true;




                    networkManager.OnServerAddPlayer(NetworkServer.connections[NetworkClient.connection.connectionId]);

                }

                if (escEndScript.endTu)
                {
                    Debug.Log("dd");
                    isHost = false;
                    networkManager.StopHost();
                    PlayerSpawn = false;
                    escEndScript.endTu = false;

                    P05_TeamChange.Characterset = false;
                    BattleStartCamera.GameStart = false;

                }

            }



            if (SceneManager.GetActiveScene().name == "TitleScene")
            {
                isHost = false;
                isClient = false;
                //P05_TeamChange.limit = true;
            }

            if(SceneManager.GetActiveScene().name == "SquareTower")
            {
                Debug.Log(NetworkClient.isConnected);
            }

            //-----------------追加箇所
        }

        // Draw the GUI
        private void OnGUI()
        {
            if (!isHost && !isClient && SceneManager.GetActiveScene().name == "SquareTower")
            {
                // Host button
                Debug.Log("aaaaaa");

                P05_TeamChange.Characterset = false;
                BattleStartCamera.GameStart = false;

                if (GUI.Button(new Rect(485, 398, 972, 205), buttonTexture))
                {
                    isHost = true;
                    isClient = false;

                    //--------------追加
                    Debug.Log("qqqq");
                    networkManager.StartHost();
                    P05_TeamChange.limit = false;
                    flag = false;

                    StartsetUi = GameObject.Find("StartSetUi");
                    image1 = StartsetUi.transform.GetChild(10).gameObject;
                    image2 = StartsetUi.transform.GetChild(11).gameObject;
                    image3 = StartsetUi.transform.GetChild(12).gameObject;

                    //--------------追加
                }

                // Client button
                if (GUI.Button(new Rect(485, 718, 972, 205), buttonTexture))
                {
                    networkManager.InitClient();
                    isHost = false;
                    isClient = true;


                    //---------------追加
                    P05_TeamChange.limit = false;
                    flag = false;

                    StartsetUi = GameObject.Find("StartSetUi");
                    image1 = StartsetUi.transform.GetChild(10).gameObject;
                    image2 = StartsetUi.transform.GetChild(11).gameObject;
                    image3 = StartsetUi.transform.GetChild(12).gameObject;
                    //---------------追加
                }
            }
            else if (SceneManager.GetActiveScene().name != "SquareTower")
            {
               
                   /* isHost = true;
                    isClient = false;

                    networkManager.StartHost();*/
                    
            }
            else
            {
                // Host or client GUI
                if (isHost) GUIHost();
                else if (isClient) GUIClient();
            }
        }

        // Draw the host GUI
        void GUIHost()
        {
            // Display host addresss
            if (networkManager.HostEndPoint != null)
            {
                if (flag == false)
                {
                    image3.SetActive(true);
                    image1.SetActive(false);
                    buttonSet = true;
                    GUI.skin.label.fontSize = 80;
                    GUI.Label(new Rect(10, 230, 150, 142), "");
                    GUI.TextField(new Rect(190, 20, 520, 322), networkManager.HostEndPoint.Address.ToString(), "Label");
                    GUI.Label(new Rect(10, 37, 150, 22), "");
                    GUI.TextField(new Rect(190, 100, 500, 100), networkManager.HostEndPoint.Port.ToString(), "Label");
                }
            }

            // Disconnect Button
            /*if (GUI.Button(new Rect(10, 81, 110, 30), "Disconnect"))
            {
                networkManager.StopHost();
                isHost = false;
            }*/

            if (!NobleServer.active) isHost = false;
        }

        // Draw the client GUI
        void GUIClient()
        {
            if (!networkManager.isNetworkActive)
            {
                image2.SetActive(true);
                image1.SetActive(false);
                //image3.SetActive(false);
                // Text boxes for entering host's address
                GUI.skin.textField.fontSize = 70;
                GUI.Label(new Rect(10, 10, 150, 22), "Host IP:");
                hostIP = GUI.TextField(new Rect(380, 380, 600, 102), hostIP);
                GUI.Label(new Rect(10, 37, 150, 22), "Host Port:");
                hostPort = GUI.TextField(new Rect(380, 630, 600, 102), hostPort);

                // Connect button
                if (GUI.Button(new Rect(1405, 881, 480, 130), "Connect"))
                {
                    image3.SetActive(true);
                    image2.SetActive(false);
                    buttonSet = true;
                    networkManager.networkAddress = hostIP;
                    networkManager.networkPort = ushort.Parse(hostPort);
                    networkManager.StartClient();
                }

                // Back button
                if (GUI.Button(new Rect(10, 81, 95, 30), "Back"))
                {
                    isClient = false;
                    image1.SetActive(true);
                    image2.SetActive(false);
                    networkManager.StopClient();
                }
            }
            else if (networkManager.client != null)
            {
                // Disconnect button
                if (flag == false)
                {
                    GUI.Label(new Rect(10, 10, 150, 22), "Connection type: " + networkManager.client.latestConnectionType);
                    if (GUI.Button(new Rect(10, 50, 110, 30), "Disconnect"))
                    {
                        if (networkManager.client.isConnected)
                        {
                            // If we are already connected it is best to quit gracefully by sending
                            // a disconnect message to the host.
                            networkManager.client.Disconnect();
                        }
                        else
                        {
                            // If the connection is still in progress StopClient will cancel it
                            networkManager.StopClient();
                        }
                        isClient = false;
                    }
                }
            }
        }
    }
}