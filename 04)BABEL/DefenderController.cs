using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

//Defender用の操作スクリプト
//自分の担当は、各爆弾関数群の通信適応と、所々にあるアニメーション用の変数管理です。
//Teleport関係に関しては、自分ではありません。
public class DefenderController : NetworkBehaviour
{
    //爆弾関連変数
    private float BombReloadTime = 0f, AdhensionReloadTime = 0f, ClusterReloadTime = 0f, WallReloadTime = 10.00f, speed = 20000.0f;
    //バリア関連変数
    private float BarrierReloadTime = 45.0f, BarrierTime = 10.0f;

    public GameObject mainCamera;
    public GameObject BombObj;
    public GameObject AdhensionObj;
    public GameObject ClusterObj;
    public GameObject BombWall;

    bool AdThrow, BrThrow, ClThrow, BaOpen, WaThrow, first = false, StanTeleport = false, camSet = false, tuPos=false;

    //カメラ関係変数ーーーーーーーーーーーーーーーーーーーーーーーーー

    //方向転換のスピード
    private float angleSpeed = 10;
    private float rightSpeed;

    Transform playerLook;
    private Vector3 offset;
    private Vector3 setPosition;
    float firstPosY, creiterionY;

    //半径
    private float r = 5;
    private float CameraLimitY = 2f;
    //ラジアン
    private float deg = 0;
    float horizontal;
    float vertical;
    float forwardCamera = 0;

    //回転速度
    float stepSpeed = 120f;
    //--------------------------------------------------------------------
    public static float change;
    float BRtime, ADtime, CLtime, Batime, Watime, keepSpeed, changeTime = 0f, i;
    float a;

    GameObject obj;
    Rigidbody rb;
    PlayerStan playerstan;
    C02ItemManager ItemManager;
    Timer timerSet;
    P04ItemHolder P04;

    //Animator
    Animator anim;
    NetworkAnimator Nanim;
    JetPack jet;

    GameObject Dcolor;
    GameObject ModelPrefab;
    SkinnedMeshRenderer SMR;
    public Material Defender_M;
    public Material[] matArray = new Material[2];

    public GameObject JetObj1;
    public GameObject JetObj2;
    public GameObject JetObj3;
    public GameObject JetObj4;

    //索敵用
    M02EnemySearch M02;
    // Start is called before the first frame update
    void Start()
    {
        this.ItemManager = FindObjectOfType<C02ItemManager>();
        //GameScreenUi.PlayerMode = 1;
        //爆弾の時間制限を保管するために代入
        BRtime = BombReloadTime;
        ADtime = AdhensionReloadTime;
        CLtime = ClusterReloadTime;
        Batime = BarrierReloadTime;
        Watime = WallReloadTime;
        WallReloadTime = 0f;

        //爆弾処理用初期化
        AdThrow = false;
        BrThrow = false;
        ClThrow = false;
        BaOpen = false;

        if (isLocalPlayer)
        {

            mainCamera = GameObject.Find("Main Camera");

            //視点移動用処理
            
            creiterionY = this.transform.position.y;

            playerLook = mainCamera.transform.GetChild(1).gameObject.transform;

            //float lookZ= this.transform.position.z,
            //      lookY= this.transform.position.y,
            //      lookX= this.transform.position.x-(mainCamera.transform.position.x - this.transform.position.x);
            //playerLook.transform.position = new Vector3(lookX, lookY, lookZ);
            timerSet = this.gameObject.GetComponent<Timer>();
            P04 = this.gameObject.GetComponent<P04ItemHolder>();

            //anim = this.gameObject.GetComponent<Animator>();

        }
        rb = GetComponent<Rigidbody>();

        //Animator
        anim = this.gameObject.GetComponent<Animator>();
        Nanim = this.gameObject.GetComponent<NetworkAnimator>();
        Nanim.animator = anim;
        jet = this.gameObject.GetComponent<JetPack>();
        //jet.AnimMode = 5*/;
        jet.anim = this.gameObject.GetComponent<Animator>();

        Dcolor = this.gameObject.transform.GetChild(4).gameObject;
        ModelPrefab = Dcolor.transform.GetChild(0).gameObject;
        SMR = ModelPrefab.transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>();
        matArray[0] = Defender_M;
        matArray[1] = Defender_M;
        SMR.materials = matArray;
        //SMR = this.gameObject.transform.GetChild(5).
        //anim.runtimeAnimatorController = (RuntimeAnimatorController)

        //JetObj1.SetActive(false);
        //JetObj2.SetActive(true);



        //スピード保管
        keepSpeed = speed;

        //PlayerStanScript取得
        playerstan = this.gameObject.GetComponent<PlayerStan>();

        change = 1;
        RoundSet.StopPlayer = true;

        //索敵用
        M02 = GameObject.Find("GameMG").GetComponent<M02EnemySearch>();
    }

    // Update is called once per frame
    void Update()
    {
        //ジェットパック音
        if (!RoundSet.StopPlayer)
            this.transform.GetChild(6).gameObject.SetActive(true);
        else
            this.transform.GetChild(6).gameObject.SetActive(false);
        if (!RoundSet.firstPos && SceneManager.GetActiveScene().name == "SquareTower")
            NetworkFirstPos();

        if (isLocalPlayer &&( !RoundSet.StopPlayer || SceneManager.GetActiveScene().name != "SquareTower")&& !escEndScript.stopClick)
        {
            //座標セット
            if (timerSet.posSet)
                NetworkTeleport();
            //カメラ視点処理
            CameraMove();
            //キャラ移動処理
            PlayerMove();
            if ((RoundSet.DefenderPhase == true || !first || SceneManager.GetActiveScene().name != "SquareTower"))
            {

                //スタン制限のある処理-------------------------------------------------------------------
                if (!playerstan.StanOn)
                {
                    Quaternion CamRot = mainCamera.transform.localRotation;
                    this.transform.GetChild(1).gameObject.transform.rotation = CamRot;

                    if (Input.GetMouseButtonDown(0) && change != 3 && change != 4 && P04.bombPossession > 0)
                    {
                        this.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    if (Input.GetMouseButtonUp(0))
                        this.transform.GetChild(1).gameObject.SetActive(false);
                    //爆弾右クリックで変更
                    if (Input.GetMouseButtonDown(1))
                    {
                        change++;
                        if (change > 3)
                            change = 1;

                    }
                    //爆弾所持数が1以上&&設置制限を越えていない場合使用可能
                    if (P04.bombPossession > 0 || AdThrow || ClThrow)
                    {
                        //粘着爆弾投下
                        if ((Input.GetMouseButtonUp(0) && change == 1 && !RoundSet.StopBomb) || AdThrow)
                        {
                            //Adhension();
                            NetworkAdhension();
                            //anim.SetTrigger("Shoot");
                            Nanim.SetTrigger("Shoot");
                        }
                        //クラスター爆弾投下
                        if ((Input.GetMouseButtonUp(0) && change == 2) || ClThrow)
                        {
                            //ClusterBomb();
                            NetworkClusterBomb();
                            Nanim.SetTrigger("Shoot");
                        }
                    }
                    //壁展開
                    if (change == 3&&!RoundSet.StopWall)
                        NetworkWallBomb();
                    else
                        this.transform.GetChild(3).gameObject.SetActive(false);
                    WallReloadTime -= Time.deltaTime;


                }
                else if (!StanTeleport && Input.GetKeyDown(KeyCode.R))
                {
                    //バリアー展開
                    Barrier();
                    NetworkStanTeleport();

                }
                //バリアー展開
                if (BaOpen)
                    Barrier();
                if (BaOpen)
                    BarrierReloadTime -= Time.deltaTime;
                first = true;
            }


        }
        if (SceneManager.GetActiveScene().name != "SquareTower" && !tuPos)
        {
            this.transform.position = new Vector3(7, 19, 130);
            tuPos = true;
        }

    }
    void CameraMove()
    {
        if (!camSet)
        {
            offset = mainCamera.transform.position - this.transform.position;
            mainCamera.transform.position += offset;
            setPosition.y = this.transform.position.y;
            firstPosY = this.transform.position.y;
            camSet = true;
        }


        horizontal = Input.GetAxis("Mouse X") * 3f;
        vertical = -1 * Input.GetAxis("Mouse Y");

        if (horizontal != 0)
        {
            deg -= horizontal;
        }

        Vector3 playerPos = this.transform.position;

        offset.y = mainCamera.transform.position.y - this.transform.position.y;

        float UplimitY = playerPos.y + CameraLimitY;
        float DownlimitY = playerPos.y - CameraLimitY;
        float setCameraPos = playerPos.y + offset.y + vertical;

        //上下視点移動制限(セットするカメラ座標が上下制限値内の場合更新)
        if (UplimitY + 2 > setCameraPos && DownlimitY < setCameraPos)
        {
            setPosition.y = setCameraPos;
            //カメラの上下移動から前に出す補正,上下範囲と現在地から自然数の値を算出
            forwardCamera = (float)Mathf.Abs((UplimitY - DownlimitY) / 2 - (setCameraPos - DownlimitY)) / 3f;

        }

        setPosition.x = playerPos.x + r * Mathf.Cos(Mathf.Deg2Rad * deg);
        setPosition.z = playerPos.z + r * Mathf.Sin(Mathf.Deg2Rad * deg);

        //z軸の補正処理

        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
        {
            if (playerPos.y < mainCamera.transform.position.y)
                setPosition += transform.forward * forwardCamera * 2.0f;
            else
                setPosition += transform.forward * forwardCamera;
        }
        //Player上下移動に対応
        float addY = firstPosY - playerPos.y;
        if (firstPosY != addY)
        {
            setPosition.y -= addY;
            firstPosY = playerPos.y;
        }

        //カメラ座標の更新
        mainCamera.transform.position = setPosition;

        //視点対象の移動補正
        GameObject LookObj;
        LookObj = this.transform.GetChild(0).gameObject;
        LookObj.transform.position = this.transform.position;
        LookObj.transform.position += transform.forward * 2.0f;

        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
        {
            //下を見ている時
            if (playerPos.y < mainCamera.transform.position.y)
                LookObj.transform.position -= new Vector3(0f, forwardCamera, 0f);
            //上を見ている時
            if (playerPos.y > mainCamera.transform.position.y)
                LookObj.transform.position += new Vector3(0f, forwardCamera, 0f);
        }
        mainCamera.transform.LookAt(LookObj.transform);

    }
    void PlayerLook()
    {
        //GameObject rot;
        //rot.transform.rotation = new Vector3(0f, 0f, 0f);
        //playerLook=rot;
        //float y = mainCamera.transform.rotation.y;
        //playerLook.transform.rotation =  Quaternion.Euler(0,y, 0);

        float step = stepSpeed * Time.deltaTime;

        Transform sampleTransform = this.transform;



        transform.rotation = Quaternion.RotateTowards(transform.rotation, playerLook.rotation, step);
        Vector3 worldAngle = sampleTransform.eulerAngles;
        worldAngle.x = 0f;
        worldAngle.z = 0f;
        sampleTransform.eulerAngles = worldAngle;

    }
    void PlayerMove()
    {
        //無敵時間中スピード20％Up
        if (playerstan.invincibility)
            speed = keepSpeed * (1.0f + 0.1f * P04.itemSpeed) * 1.2f;
        else
            speed = keepSpeed * (1.0f + 0.1f * P04.itemSpeed);

        //WSキー、↑↓キーで移動する
        float z = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;

        //Debug.Log(z);

        if (z == 0)
        {
            this.transform.GetChild(11).gameObject.SetActive(false);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            anim.SetBool("front", false);
            anim.SetBool("back", false);
        }
        //前進の後退
        //後退は前進の3分の1のスピードになる
        if (z > 0)
        {
            this.transform.GetChild(11).gameObject.SetActive(true);
            rb.AddForce(transform.forward * z, ForceMode.Force);  // 力を加える
            //transform.position += transform.forward * z;
            anim.SetBool("Stay", false);
            anim.SetBool("back", false);
            anim.SetBool("front", true);
        }
        else if(z < 0)
        {
            this.transform.GetChild(11).gameObject.SetActive(true);
            rb.AddForce(transform.forward * z /*/ 3*/, ForceMode.Force);  // 力を加える
            anim.SetBool("Stay", false);
            anim.SetBool("back", true);
            anim.SetBool("front", false);
        }

        //ADキー、←→キーで方向を替える
        float x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * angleSpeed;



        if (x != 0)
        {
            anim.SetBool("Stay", false);
            if (z == 0)
                rightSpeed = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed * 12.0f;
            else
                rightSpeed = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed / 2;
            this.transform.GetChild(11).gameObject.SetActive(true);

            //Animator
            if (rightSpeed > 0)
            {
                anim.SetBool("left", false);
                anim.SetBool("right", true);
            }
            else if (rightSpeed < 0)
            {
                anim.SetBool("left", true);
                anim.SetBool("right", false);
            }

            rb.AddForce(transform.right * rightSpeed, ForceMode.Force);
            //transform.Rotate(Vector3.up * x);
        }

        else
        {

            anim.SetBool("left", false);
            anim.SetBool("right", false);
            PlayerLook();
            if (z == 0)
            {
                this.transform.GetChild(11).gameObject.SetActive(false);
                if ((!Input.GetKey(KeyCode.Space)) && (!Input.GetKey(KeyCode.LeftShift)))
                {
                    anim.SetBool("Stay", true);
                }
            }
         
        }
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, mainCamera.transform.GetChild(0).rotation, 0.1f);
    }
    void Barrier()
    {

        //初期セットアップ
        if (!BaOpen)
        {
            BarrierReloadTime = Batime - 0.5f * P04.itemCount;
            //this.transform.GetChild(2).gameObject.SetActive(true);
            //発射SE
            this.transform.GetChild(8).gameObject.SetActive(false);
            this.transform.GetChild(8).gameObject.SetActive(true);
            CmdBarrier(this.gameObject, true);
            BaOpen = true;
        }

        if (BarrierReloadTime < (Batime - 0.5f * P04.itemCount) - BarrierTime)
        {
            //バリア発動終了
            //this.transform.GetChild(2).gameObject.SetActive(false);
            CmdBarrier(this.gameObject, false);
        }
        if (BarrierReloadTime < 0 - BarrierTime)
        {
            //バリアリロード時間終了
            BaOpen = false;
            BarrierReloadTime = Batime;
        }

    }

    //-------------担当箇所
    void NetworkBomb()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //爆弾発射処理(連続投下出来ないように時間で制御)
        if (BombReloadTime >= BRtime)
        {
            //Debug.Log("playerPos" + this.transform.position);
            BrThrow = true;

            //発射SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);

            //爆弾発射。プレイヤーのオブジェクト情報と、向いている向きをサーバーに送信。
            CmdBomb(this.gameObject, mainCamera.transform.forward);

            P04.bombCount++;
            BombReloadTime -= Time.deltaTime;



        }
        else if (BombReloadTime < BRtime && BombReloadTime >= 0)
        {
            //爆弾投下開始
            BombReloadTime -= Time.deltaTime;
        }
        else if (BombReloadTime < 0)
        {//爆弾の時間制限が過ぎたらリセット
            BrThrow = false;
            BombReloadTime = BRtime;
        }
    }
    void NetworkAdhension()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //粘着爆弾発射処理(連続投下出来ないように時間で制御)
        if (AdhensionReloadTime >= ADtime)
        {
            AdThrow = true;
            //発射SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);
            CmdAdhension(this.gameObject, mainCamera.transform.forward, mainCamera.transform.rotation);

            P04.bombCount++;
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < ADtime && AdhensionReloadTime >= 0)
        {
            //爆弾投下開始
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < 0)
        {//爆弾の時間制限が過ぎたらリセット
            AdThrow = false;
            AdhensionReloadTime = ADtime;
        }


    }
    void NetworkClusterBomb()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //爆弾発射処理(連続投下出来ないように時間で制御)
        if (ClusterReloadTime >= CLtime)
        {
            Debug.Log("playerPos" + this.transform.position);
            ClThrow = true;
            //発射SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);
            CmdClusterBomb(this.gameObject, mainCamera.transform.forward, mainCamera.transform.eulerAngles);

            P04.bombCount++;
            ClusterReloadTime -= Time.deltaTime;


        }
        else if (ClusterReloadTime < CLtime && ClusterReloadTime >= 0)
        {
            //爆弾投下開始
            ClusterReloadTime -= Time.deltaTime;
        }
        else if (ClusterReloadTime < 0)
        {//爆弾の時間制限が過ぎたらリセット
            ClThrow = false;
            ClusterReloadTime = CLtime;
        }
    }
    void NetworkWallBomb()
    {
        //Lineの削除
        this.transform.GetChild(1).gameObject.SetActive(false);
        //壁Objのフォルダ
        GameObject WallFolder = this.transform.GetChild(3).gameObject;
        GameObject denoteWall = WallFolder.transform.GetChild(0).gameObject;
        WallFolder.SetActive(true);
        //左Keyで置ける場所がある場合(uselessWall.csで判定)実行開始
        if (Input.GetMouseButtonDown(0) && uselessWall.useless == false && WallReloadTime <= 0)
        {
            //発射SE
            this.transform.GetChild(8).gameObject.SetActive(false);
            this.transform.GetChild(8).gameObject.SetActive(true);

            //ProvisionalWall.csで判定した位置に壁を設置
            //Instantiate(BombWall, denoteWall.transform.position, denoteWall.transform.rotation);
            CmdWallBomb(denoteWall.transform.position, denoteWall.transform.rotation, gameObject.GetComponent<P04ItemHolder>().itemPower,this.gameObject);

            Nanim.SetTrigger("Wall");

            //リロード時間中は黄色にマテリアル変更
            uselessWall.Reload = true;
            //リロード時間を代入
            WallReloadTime = Watime - (0.3f * P04.itemCount);
        }


        WallReloadTime -= Time.deltaTime;
        if (WallReloadTime <= 0)
            uselessWall.Reload = false;
    }
    //-------------担当箇所

    void NetworkStanTeleport()
    {
        CmdStanTeleport(this.gameObject);
    }

    //void NetWorkLine()
    //{
    //    CmdLine(mainCamera.transform.rotation, this.transform.GetChild(1).gameObject);
    //}
    //[Command(requiresAuthority = false)]
    //void CmdLine(Quaternion Camera, GameObject Line)
    //{
    //    RpcLine(Camera, Line);
    //}
    //[ClientRpc]
    //void RpcLine(Quaternion Camera, GameObject Line)
    //{
    //    Line.transform.rotation = Camera;
    //}


    //CmdBomb
    //[Command]属性を持つ。詳しくは説明しないが、サーバー側で一回のみ実行する関数。
    //------------担当箇所
    [Command]
    void CmdBomb(GameObject Shooter, Vector3 ShooterCamera)
    {
        //位置の調整
        Vector3 BombPos;
        BombPos = Shooter.transform.position + ShooterCamera * 1.5f;
        BombPos.y += 1.5f;

        //爆弾生成
        GameObject bomb = Instantiate(BombObj, BombPos, Shooter.transform.rotation);
        bomb.GetComponent<B01BombStatus>().bombOwner = Shooter.gameObject;

        //生成した事を各プレイヤーへ送信
        //送信後に、「サーバー側で」発射時の(同一の)加速を行う為にConstructer関数をCall。
        NetworkServer.Spawn(bomb);
        RpcBombInit(bomb, Shooter);
        //bomb.GetComponent<BombThrow>().Constructer(Shooter);
    }

    [ClientRpc]
    void RpcBombInit(GameObject bomb, GameObject Shooter)
    {
        bomb.GetComponent<BombThrow>().Constructer(Shooter);
    }

    //CmdAdhension
    //Adhension版。
    [Command]
    void CmdAdhension(GameObject Shooter, Vector3 ShooterCamera, Quaternion CameraRot)
    {

        Vector3 AdhensionPos;
        AdhensionPos = Shooter.transform.position + ShooterCamera * 1.5f;
        AdhensionPos.y += 1.5f;

        GameObject Adhension = Instantiate(AdhensionObj, AdhensionPos, CameraRot);
        Adhension.GetComponent<B01BombStatus>().bombOwner = Shooter.gameObject;

        NetworkServer.Spawn(Adhension);
        RpcAdhensionInit(Adhension, Shooter);
        //Adhension.GetComponent<adhesionThrow>().Constructer(Shooter);
    }

    [ClientRpc]
    void RpcAdhensionInit(GameObject Adhension, GameObject Shooter)
    {
        Adhension.GetComponent<adhesionThrow>().Constructer(Shooter);
        Adhension.GetComponent<B02OwnerInheritance>().SendOwnerChild(Adhension, Shooter);

        //索敵用
        /*M02.target = Shooter;
        M02.SetDir = true;*/
    }

    [Command]
    void CmdClusterBomb(GameObject Shooter, Vector3 ShooterCamera, Vector3 CameraAngles)
    {
        Vector3 ClusterPos;
        ClusterPos = Shooter.transform.position + ShooterCamera * 1.5f;
        ClusterPos.y += 1.5f;

        GameObject Cluster = Instantiate(ClusterObj, ClusterPos, Quaternion.Euler(CameraAngles.x, CameraAngles.y, 90));
        Cluster.GetComponent<B01BombStatus>().bombOwner = Shooter.gameObject;

        NetworkServer.Spawn(Cluster);
        RpcClusterInit(Cluster, Shooter);


    }

    [ClientRpc]
    void RpcClusterInit(GameObject Cluster, GameObject Shooter)
    {
        Cluster.GetComponent<ClusterThrow>().Constructer(Shooter);
        Cluster.GetComponent<B02OwnerInheritance>().SendOwnerChild(Cluster, Shooter);

        //索敵用
        /*M02.target = Shooter;
        M02.SetDir = true;*/
    }

    [Command]
    void CmdWallBomb(Vector3 WallPos, Quaternion WallRot, int power,GameObject Shooter)
    {


        GameObject Wall = Instantiate(BombWall, WallPos, WallRot);
        NetworkServer.Spawn(Wall);
        RpcWallInit(Wall, Shooter);

        //表示エフェクト
        /*a += 40f * Time.deltaTime;
        if (a >= 4.0f)
            a = 4.0f;*/
        a = 2f;
        Wall.transform.localScale = new Vector3(a * (1 + 0.04f * power), (a * 2f) * (1 + 0.04f * power), 0.3f);


        //return Wall;
    }

    [ClientRpc]
    void RpcWallInit(GameObject Wall, GameObject Shooter)
    {
        Wall.GetComponent<C03WallStatus>().Constructer(Shooter);

        //索敵用
        /*M02.target = Shooter;
        M02.SetDir = true;*/
    }

    [Command]
    void CmdBarrier(GameObject Player, bool state)
    {
        RpcBarrier(Player, state);
    }
    [ClientRpc]
    void RpcBarrier(GameObject Player, bool state)
    {
        Player.transform.GetChild(2).gameObject.SetActive(state);
    }

    //-----------担当箇所
    [Command]
    void CmdStanTeleport(GameObject Player)
    {

        RpcStanTeleport(Player);
        StanTeleport = true;
    }
    [ClientRpc]
    void RpcStanTeleport(GameObject Player)
    {
        Player.transform.position = new Vector3(-45, 45, -45);
    }
    void NetworkTeleport()
    {
        CmdTeleport(this.gameObject);
        timerSet.posSet = false;
    }
    [Command]
    void CmdTeleport(GameObject Player)
    {

        RpcTeleport(Player);
    }
    [ClientRpc]
    void RpcTeleport(GameObject Player)
    {
        Player.transform.position = new Vector3(0, 32, 120);
    }
    void NetworkFirstPos()
    {
        UIInit();
        this.transform.position = new Vector3(-52, 18, -52);
        CmdFirstPos(this.gameObject);
        //RoundSet.firstPos = true;
    }
    [Command]
    void CmdFirstPos(GameObject Player)
    {

        Player.transform.position = new Vector3(-52, 18, -52);
        RpcFirstPos(Player);
    }
    [ClientRpc]
    void RpcFirstPos(GameObject Player)
    {
        Player.transform.position = new Vector3(-52, 18, -52);
    }


    public override void OnStartLocalPlayer()
    {
        //PlayerManager.PlayerObj[PlayerManager.num] = this.gameObject;
        //Debug.Log(PlayerManager.PlayerObj[PlayerManager.num].name);
        //PlayerManager.num++;

        base.OnStartLocalPlayer();
        UIInit();
    }
    public void UIInit()
    {

        GameObject Ui1 = GameObject.Find("equipUi");
        GameObject Ui2 = GameObject.Find("ItemUi");
        GameObject Ui3 = GameObject.Find("ScoreUi");
        GameObject Ui4 = GameObject.Find("PowerUi");
        GameScreenUi GSU1 = Ui1.GetComponent<GameScreenUi>();
        GameScreenUi GSU2 = Ui2.GetComponent<GameScreenUi>();
        GameScreenUi GSU3 = Ui3.GetComponent<GameScreenUi>();
        GameScreenUi GSU4 = Ui4.GetComponent<GameScreenUi>();
        GSU1.Player = this.gameObject;
        GSU2.Player = this.gameObject;
        GSU3.Player = this.gameObject;
        GSU4.Player = this.gameObject;
        GSU1.P04 = this.gameObject.GetComponent<P04ItemHolder>();
        GSU2.P04 = this.gameObject.GetComponent<P04ItemHolder>();
        GSU3.P04 = this.gameObject.GetComponent<P04ItemHolder>();
        GSU4.P04 = this.gameObject.GetComponent<P04ItemHolder>();
        GSU1.PlayerMode = 1;
        GSU2.PlayerMode = 1;
        GSU3.PlayerMode = 1;
        GSU4.PlayerMode = 1;

        //anim = default;
        //this.gameObject.transform.GetChild()

    }



    //非通信関数群
    //これらを通信に適応させる処理を主に組みました。ROMでは未使用です。
    void Adhension()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //粘着爆弾発射処理(連続投下出来ないように時間で制御)
        if (AdhensionReloadTime >= ADtime)
        {
            AdThrow = true;
            Vector3 AdhensionPos;
            AdhensionPos = this.transform.position + transform.forward * 1.5f;
            AdhensionPos.y += 1.5f;
            //爆弾生成
            Instantiate(AdhensionObj, AdhensionPos, mainCamera.transform.rotation);
            P04.bombCount++;
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < ADtime && AdhensionReloadTime >= 0)
        {
            //爆弾投下開始
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < 0)
        {//爆弾の時間制限が過ぎたらリセット
            AdThrow = false;
            AdhensionReloadTime = ADtime;
        }

    }
    void ClusterBomb()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //爆弾発射処理(連続投下出来ないように時間で制御)
        if (ClusterReloadTime >= CLtime)
        {
            Debug.Log("playerPos" + this.transform.position);
            ClThrow = true;
            Vector3 ClusterPos;
            ClusterPos = this.transform.position + mainCamera.transform.forward * 1.5f;
            ClusterPos.y += 1.5f;
            //爆弾生成

            Instantiate(ClusterObj, ClusterPos, Quaternion.Euler(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, 90));
            P04.bombCount++;
            ClusterReloadTime -= Time.deltaTime;

            //0823追記 by SimoLieF  爆弾のユーザー情報の送信
            ClusterObj.GetComponent<B01BombStatus>().bombOwner = this.gameObject;

        }
        else if (ClusterReloadTime < CLtime && ClusterReloadTime >= 0)
        {
            //爆弾投下開始
            ClusterReloadTime -= Time.deltaTime;
        }
        else if (ClusterReloadTime < 0)
        {//爆弾の時間制限が過ぎたらリセット
            ClThrow = false;
            ClusterReloadTime = CLtime;
        }
    }
    void WallBomb()
    {
        //Lineの削除
        this.transform.GetChild(1).gameObject.SetActive(false);
        //壁Objのフォルダ
        GameObject WallFolder = this.transform.GetChild(3).gameObject;
        GameObject denoteWall = WallFolder.transform.GetChild(0).gameObject;

        WallFolder.SetActive(true);
        //左Keyで置ける場所がある場合(uselessWall.csで判定)実行開始
        if (Input.GetMouseButtonDown(0) && uselessWall.useless == false && WallReloadTime <= 0)
        {
            //ProvisionalWall.csで判定した位置に壁を設置
            obj = Instantiate(BombWall, denoteWall.transform.position, denoteWall.transform.rotation);

            i = 0;

            //リロード時間中は黄色にマテリアル変更
            uselessWall.Reload = true;

            //リロード時間を代入
            WallReloadTime = Watime - (0.35f * P04.itemCount);

        }
        //表示エフェクト
        i += 40f * Time.deltaTime;
        if (i >= 4.0f)
            i = 4.0f;
        obj.transform.localScale = new Vector3(i * (1 + 0.1f * P04.itemPower), (i * 2f) * (1 + 0.1f * P04.itemPower), 0.3f);
        //リロード時間
        if (WallReloadTime <= 0)
            uselessWall.Reload = false;
    }

}
