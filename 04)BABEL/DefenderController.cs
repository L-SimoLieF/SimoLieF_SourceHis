using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

//Defender�p�̑���X�N���v�g
//�����̒S���́A�e���e�֐��Q�̒ʐM�K���ƁA���X�ɂ���A�j���[�V�����p�̕ϐ��Ǘ��ł��B
//Teleport�֌W�Ɋւ��ẮA�����ł͂���܂���B
public class DefenderController : NetworkBehaviour
{
    //���e�֘A�ϐ�
    private float BombReloadTime = 0f, AdhensionReloadTime = 0f, ClusterReloadTime = 0f, WallReloadTime = 10.00f, speed = 20000.0f;
    //�o���A�֘A�ϐ�
    private float BarrierReloadTime = 45.0f, BarrierTime = 10.0f;

    public GameObject mainCamera;
    public GameObject BombObj;
    public GameObject AdhensionObj;
    public GameObject ClusterObj;
    public GameObject BombWall;

    bool AdThrow, BrThrow, ClThrow, BaOpen, WaThrow, first = false, StanTeleport = false, camSet = false, tuPos=false;

    //�J�����֌W�ϐ��[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[

    //�����]���̃X�s�[�h
    private float angleSpeed = 10;
    private float rightSpeed;

    Transform playerLook;
    private Vector3 offset;
    private Vector3 setPosition;
    float firstPosY, creiterionY;

    //���a
    private float r = 5;
    private float CameraLimitY = 2f;
    //���W�A��
    private float deg = 0;
    float horizontal;
    float vertical;
    float forwardCamera = 0;

    //��]���x
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

    //���G�p
    M02EnemySearch M02;
    // Start is called before the first frame update
    void Start()
    {
        this.ItemManager = FindObjectOfType<C02ItemManager>();
        //GameScreenUi.PlayerMode = 1;
        //���e�̎��Ԑ�����ۊǂ��邽�߂ɑ��
        BRtime = BombReloadTime;
        ADtime = AdhensionReloadTime;
        CLtime = ClusterReloadTime;
        Batime = BarrierReloadTime;
        Watime = WallReloadTime;
        WallReloadTime = 0f;

        //���e�����p������
        AdThrow = false;
        BrThrow = false;
        ClThrow = false;
        BaOpen = false;

        if (isLocalPlayer)
        {

            mainCamera = GameObject.Find("Main Camera");

            //���_�ړ��p����
            
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



        //�X�s�[�h�ۊ�
        keepSpeed = speed;

        //PlayerStanScript�擾
        playerstan = this.gameObject.GetComponent<PlayerStan>();

        change = 1;
        RoundSet.StopPlayer = true;

        //���G�p
        M02 = GameObject.Find("GameMG").GetComponent<M02EnemySearch>();
    }

    // Update is called once per frame
    void Update()
    {
        //�W�F�b�g�p�b�N��
        if (!RoundSet.StopPlayer)
            this.transform.GetChild(6).gameObject.SetActive(true);
        else
            this.transform.GetChild(6).gameObject.SetActive(false);
        if (!RoundSet.firstPos && SceneManager.GetActiveScene().name == "SquareTower")
            NetworkFirstPos();

        if (isLocalPlayer &&( !RoundSet.StopPlayer || SceneManager.GetActiveScene().name != "SquareTower")&& !escEndScript.stopClick)
        {
            //���W�Z�b�g
            if (timerSet.posSet)
                NetworkTeleport();
            //�J�������_����
            CameraMove();
            //�L�����ړ�����
            PlayerMove();
            if ((RoundSet.DefenderPhase == true || !first || SceneManager.GetActiveScene().name != "SquareTower"))
            {

                //�X�^�������̂��鏈��-------------------------------------------------------------------
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
                    //���e�E�N���b�N�ŕύX
                    if (Input.GetMouseButtonDown(1))
                    {
                        change++;
                        if (change > 3)
                            change = 1;

                    }
                    //���e��������1�ȏ�&&�ݒu�������z���Ă��Ȃ��ꍇ�g�p�\
                    if (P04.bombPossession > 0 || AdThrow || ClThrow)
                    {
                        //�S�����e����
                        if ((Input.GetMouseButtonUp(0) && change == 1 && !RoundSet.StopBomb) || AdThrow)
                        {
                            //Adhension();
                            NetworkAdhension();
                            //anim.SetTrigger("Shoot");
                            Nanim.SetTrigger("Shoot");
                        }
                        //�N���X�^�[���e����
                        if ((Input.GetMouseButtonUp(0) && change == 2) || ClThrow)
                        {
                            //ClusterBomb();
                            NetworkClusterBomb();
                            Nanim.SetTrigger("Shoot");
                        }
                    }
                    //�ǓW�J
                    if (change == 3&&!RoundSet.StopWall)
                        NetworkWallBomb();
                    else
                        this.transform.GetChild(3).gameObject.SetActive(false);
                    WallReloadTime -= Time.deltaTime;


                }
                else if (!StanTeleport && Input.GetKeyDown(KeyCode.R))
                {
                    //�o���A�[�W�J
                    Barrier();
                    NetworkStanTeleport();

                }
                //�o���A�[�W�J
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

        //�㉺���_�ړ�����(�Z�b�g����J�������W���㉺�����l���̏ꍇ�X�V)
        if (UplimitY + 2 > setCameraPos && DownlimitY < setCameraPos)
        {
            setPosition.y = setCameraPos;
            //�J�����̏㉺�ړ�����O�ɏo���␳,�㉺�͈͂ƌ��ݒn���玩�R���̒l���Z�o
            forwardCamera = (float)Mathf.Abs((UplimitY - DownlimitY) / 2 - (setCameraPos - DownlimitY)) / 3f;

        }

        setPosition.x = playerPos.x + r * Mathf.Cos(Mathf.Deg2Rad * deg);
        setPosition.z = playerPos.z + r * Mathf.Sin(Mathf.Deg2Rad * deg);

        //z���̕␳����

        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
        {
            if (playerPos.y < mainCamera.transform.position.y)
                setPosition += transform.forward * forwardCamera * 2.0f;
            else
                setPosition += transform.forward * forwardCamera;
        }
        //Player�㉺�ړ��ɑΉ�
        float addY = firstPosY - playerPos.y;
        if (firstPosY != addY)
        {
            setPosition.y -= addY;
            firstPosY = playerPos.y;
        }

        //�J�������W�̍X�V
        mainCamera.transform.position = setPosition;

        //���_�Ώۂ̈ړ��␳
        GameObject LookObj;
        LookObj = this.transform.GetChild(0).gameObject;
        LookObj.transform.position = this.transform.position;
        LookObj.transform.position += transform.forward * 2.0f;

        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
        {
            //�������Ă��鎞
            if (playerPos.y < mainCamera.transform.position.y)
                LookObj.transform.position -= new Vector3(0f, forwardCamera, 0f);
            //������Ă��鎞
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
        //���G���Ԓ��X�s�[�h20��Up
        if (playerstan.invincibility)
            speed = keepSpeed * (1.0f + 0.1f * P04.itemSpeed) * 1.2f;
        else
            speed = keepSpeed * (1.0f + 0.1f * P04.itemSpeed);

        //WS�L�[�A�����L�[�ňړ�����
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
        //�O�i�̌��
        //��ނ͑O�i��3����1�̃X�s�[�h�ɂȂ�
        if (z > 0)
        {
            this.transform.GetChild(11).gameObject.SetActive(true);
            rb.AddForce(transform.forward * z, ForceMode.Force);  // �͂�������
            //transform.position += transform.forward * z;
            anim.SetBool("Stay", false);
            anim.SetBool("back", false);
            anim.SetBool("front", true);
        }
        else if(z < 0)
        {
            this.transform.GetChild(11).gameObject.SetActive(true);
            rb.AddForce(transform.forward * z /*/ 3*/, ForceMode.Force);  // �͂�������
            anim.SetBool("Stay", false);
            anim.SetBool("back", true);
            anim.SetBool("front", false);
        }

        //AD�L�[�A�����L�[�ŕ�����ւ���
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

        //�����Z�b�g�A�b�v
        if (!BaOpen)
        {
            BarrierReloadTime = Batime - 0.5f * P04.itemCount;
            //this.transform.GetChild(2).gameObject.SetActive(true);
            //����SE
            this.transform.GetChild(8).gameObject.SetActive(false);
            this.transform.GetChild(8).gameObject.SetActive(true);
            CmdBarrier(this.gameObject, true);
            BaOpen = true;
        }

        if (BarrierReloadTime < (Batime - 0.5f * P04.itemCount) - BarrierTime)
        {
            //�o���A�����I��
            //this.transform.GetChild(2).gameObject.SetActive(false);
            CmdBarrier(this.gameObject, false);
        }
        if (BarrierReloadTime < 0 - BarrierTime)
        {
            //�o���A�����[�h���ԏI��
            BaOpen = false;
            BarrierReloadTime = Batime;
        }

    }

    //-------------�S���ӏ�
    void NetworkBomb()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //���e���ˏ���(�A�������o���Ȃ��悤�Ɏ��ԂŐ���)
        if (BombReloadTime >= BRtime)
        {
            //Debug.Log("playerPos" + this.transform.position);
            BrThrow = true;

            //����SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);

            //���e���ˁB�v���C���[�̃I�u�W�F�N�g���ƁA�����Ă���������T�[�o�[�ɑ��M�B
            CmdBomb(this.gameObject, mainCamera.transform.forward);

            P04.bombCount++;
            BombReloadTime -= Time.deltaTime;



        }
        else if (BombReloadTime < BRtime && BombReloadTime >= 0)
        {
            //���e�����J�n
            BombReloadTime -= Time.deltaTime;
        }
        else if (BombReloadTime < 0)
        {//���e�̎��Ԑ������߂����烊�Z�b�g
            BrThrow = false;
            BombReloadTime = BRtime;
        }
    }
    void NetworkAdhension()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //�S�����e���ˏ���(�A�������o���Ȃ��悤�Ɏ��ԂŐ���)
        if (AdhensionReloadTime >= ADtime)
        {
            AdThrow = true;
            //����SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);
            CmdAdhension(this.gameObject, mainCamera.transform.forward, mainCamera.transform.rotation);

            P04.bombCount++;
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < ADtime && AdhensionReloadTime >= 0)
        {
            //���e�����J�n
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < 0)
        {//���e�̎��Ԑ������߂����烊�Z�b�g
            AdThrow = false;
            AdhensionReloadTime = ADtime;
        }


    }
    void NetworkClusterBomb()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //���e���ˏ���(�A�������o���Ȃ��悤�Ɏ��ԂŐ���)
        if (ClusterReloadTime >= CLtime)
        {
            Debug.Log("playerPos" + this.transform.position);
            ClThrow = true;
            //����SE
            this.transform.GetChild(7).gameObject.SetActive(false);
            this.transform.GetChild(7).gameObject.SetActive(true);
            CmdClusterBomb(this.gameObject, mainCamera.transform.forward, mainCamera.transform.eulerAngles);

            P04.bombCount++;
            ClusterReloadTime -= Time.deltaTime;


        }
        else if (ClusterReloadTime < CLtime && ClusterReloadTime >= 0)
        {
            //���e�����J�n
            ClusterReloadTime -= Time.deltaTime;
        }
        else if (ClusterReloadTime < 0)
        {//���e�̎��Ԑ������߂����烊�Z�b�g
            ClThrow = false;
            ClusterReloadTime = CLtime;
        }
    }
    void NetworkWallBomb()
    {
        //Line�̍폜
        this.transform.GetChild(1).gameObject.SetActive(false);
        //��Obj�̃t�H���_
        GameObject WallFolder = this.transform.GetChild(3).gameObject;
        GameObject denoteWall = WallFolder.transform.GetChild(0).gameObject;
        WallFolder.SetActive(true);
        //��Key�Œu����ꏊ������ꍇ(uselessWall.cs�Ŕ���)���s�J�n
        if (Input.GetMouseButtonDown(0) && uselessWall.useless == false && WallReloadTime <= 0)
        {
            //����SE
            this.transform.GetChild(8).gameObject.SetActive(false);
            this.transform.GetChild(8).gameObject.SetActive(true);

            //ProvisionalWall.cs�Ŕ��肵���ʒu�ɕǂ�ݒu
            //Instantiate(BombWall, denoteWall.transform.position, denoteWall.transform.rotation);
            CmdWallBomb(denoteWall.transform.position, denoteWall.transform.rotation, gameObject.GetComponent<P04ItemHolder>().itemPower,this.gameObject);

            Nanim.SetTrigger("Wall");

            //�����[�h���Ԓ��͉��F�Ƀ}�e���A���ύX
            uselessWall.Reload = true;
            //�����[�h���Ԃ���
            WallReloadTime = Watime - (0.3f * P04.itemCount);
        }


        WallReloadTime -= Time.deltaTime;
        if (WallReloadTime <= 0)
            uselessWall.Reload = false;
    }
    //-------------�S���ӏ�

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
    //[Command]���������B�ڂ����͐������Ȃ����A�T�[�o�[���ň��̂ݎ��s����֐��B
    //------------�S���ӏ�
    [Command]
    void CmdBomb(GameObject Shooter, Vector3 ShooterCamera)
    {
        //�ʒu�̒���
        Vector3 BombPos;
        BombPos = Shooter.transform.position + ShooterCamera * 1.5f;
        BombPos.y += 1.5f;

        //���e����
        GameObject bomb = Instantiate(BombObj, BombPos, Shooter.transform.rotation);
        bomb.GetComponent<B01BombStatus>().bombOwner = Shooter.gameObject;

        //�������������e�v���C���[�֑��M
        //���M��ɁA�u�T�[�o�[���Łv���ˎ���(�����)�������s���ׂ�Constructer�֐���Call�B
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
    //Adhension�ŁB
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

        //���G�p
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

        //���G�p
        /*M02.target = Shooter;
        M02.SetDir = true;*/
    }

    [Command]
    void CmdWallBomb(Vector3 WallPos, Quaternion WallRot, int power,GameObject Shooter)
    {


        GameObject Wall = Instantiate(BombWall, WallPos, WallRot);
        NetworkServer.Spawn(Wall);
        RpcWallInit(Wall, Shooter);

        //�\���G�t�F�N�g
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

        //���G�p
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

    //-----------�S���ӏ�
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



    //��ʐM�֐��Q
    //������ʐM�ɓK�������鏈������ɑg�݂܂����BROM�ł͖��g�p�ł��B
    void Adhension()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //�S�����e���ˏ���(�A�������o���Ȃ��悤�Ɏ��ԂŐ���)
        if (AdhensionReloadTime >= ADtime)
        {
            AdThrow = true;
            Vector3 AdhensionPos;
            AdhensionPos = this.transform.position + transform.forward * 1.5f;
            AdhensionPos.y += 1.5f;
            //���e����
            Instantiate(AdhensionObj, AdhensionPos, mainCamera.transform.rotation);
            P04.bombCount++;
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < ADtime && AdhensionReloadTime >= 0)
        {
            //���e�����J�n
            AdhensionReloadTime -= Time.deltaTime;
        }
        else if (AdhensionReloadTime < 0)
        {//���e�̎��Ԑ������߂����烊�Z�b�g
            AdThrow = false;
            AdhensionReloadTime = ADtime;
        }

    }
    void ClusterBomb()
    {
        this.transform.GetChild(1).gameObject.SetActive(false);
        //���e���ˏ���(�A�������o���Ȃ��悤�Ɏ��ԂŐ���)
        if (ClusterReloadTime >= CLtime)
        {
            Debug.Log("playerPos" + this.transform.position);
            ClThrow = true;
            Vector3 ClusterPos;
            ClusterPos = this.transform.position + mainCamera.transform.forward * 1.5f;
            ClusterPos.y += 1.5f;
            //���e����

            Instantiate(ClusterObj, ClusterPos, Quaternion.Euler(mainCamera.transform.eulerAngles.x, mainCamera.transform.eulerAngles.y, 90));
            P04.bombCount++;
            ClusterReloadTime -= Time.deltaTime;

            //0823�ǋL by SimoLieF  ���e�̃��[�U�[���̑��M
            ClusterObj.GetComponent<B01BombStatus>().bombOwner = this.gameObject;

        }
        else if (ClusterReloadTime < CLtime && ClusterReloadTime >= 0)
        {
            //���e�����J�n
            ClusterReloadTime -= Time.deltaTime;
        }
        else if (ClusterReloadTime < 0)
        {//���e�̎��Ԑ������߂����烊�Z�b�g
            ClThrow = false;
            ClusterReloadTime = CLtime;
        }
    }
    void WallBomb()
    {
        //Line�̍폜
        this.transform.GetChild(1).gameObject.SetActive(false);
        //��Obj�̃t�H���_
        GameObject WallFolder = this.transform.GetChild(3).gameObject;
        GameObject denoteWall = WallFolder.transform.GetChild(0).gameObject;

        WallFolder.SetActive(true);
        //��Key�Œu����ꏊ������ꍇ(uselessWall.cs�Ŕ���)���s�J�n
        if (Input.GetMouseButtonDown(0) && uselessWall.useless == false && WallReloadTime <= 0)
        {
            //ProvisionalWall.cs�Ŕ��肵���ʒu�ɕǂ�ݒu
            obj = Instantiate(BombWall, denoteWall.transform.position, denoteWall.transform.rotation);

            i = 0;

            //�����[�h���Ԓ��͉��F�Ƀ}�e���A���ύX
            uselessWall.Reload = true;

            //�����[�h���Ԃ���
            WallReloadTime = Watime - (0.35f * P04.itemCount);

        }
        //�\���G�t�F�N�g
        i += 40f * Time.deltaTime;
        if (i >= 4.0f)
            i = 4.0f;
        obj.transform.localScale = new Vector3(i * (1 + 0.1f * P04.itemPower), (i * 2f) * (1 + 0.1f * P04.itemPower), 0.3f);
        //�����[�h����
        if (WallReloadTime <= 0)
            uselessWall.Reload = false;
    }

}
