using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//C10TowerAdmin
//����̃��C���X�N���v�g�̈�ŁA�^���[�S�̂̊Ǘ����s���B
//C01�ɂ����@�ł́ANetworkServer.Spawn���g���Ȃ��ׁA�V���ɑg�݂Ȃ������B
//TowerPrefab�̒��_�ɃA�^�b�`����ATower���\������S�Ẵu���b�N�̏������B
//�u���b�N�̏��́AcubeData����N���X�̔z��ɂ��擾�A�Ǘ������B
//�ʐM�ɂ��ʒu�̓�����cubeData�z��(=CubeArray)�ɂ���Position�Ȃǂ𑗐M���鎖�ōs����B

public class C10TowerAdmin : NetworkBehaviour
{
    //�A�C�e�������p
    public GameObject itemPrefab;
    GameObject item;

    //�Q�[���}�l�[�W���[�B
    //�I�������Ȃǂ��Ǘ�����B
    M01GameManager M01;
    
    //���E���h�I�������p�B�^���[�̍\���u���b�N��8���ƁA���ݔj�󂳂�Ă�����Ǘ�����B
    //breakend�͏����𖞂������ۂɁAM01�ɒʒB����ׂ̂��́B
    int breakLimit;
    int breakCount;
    public bool breakend;

    //�u���b�N�̂̏����ݒ�p
    //HP�Ǘ��Ȃǂ������ōs���ׁB
    static int FLOOR_HP = 8192;
    static int WALL_HP = 1280;
    static int PILLER_HP = 1;
    static int FLOOR_SCORE = 9000;
    static int WALL_SCORE = 1500;
    static int PILLER_SCORE = 500;

    //�j�󎞂̃G�t�F�N�g
    public GameObject breakEffect;

    //�S���O
    public int blockcount = 6;
    public GameObject Blocks;
    GameObject keepblocks;


    //�\���u���b�N��C10�ŊǗ�����ׂ́A�u���b�N�̏����i�[����ׂ̎���N���X�B
    //�u���b�N�̃I�u�W�F�N�g�̏���A�����p�̍��W�i�[�A�X�R�A�v�Z�ׂ̈̔j��҂̏��Ȃǂ������o�Ƃ��đ��݁B
    public class CubeData
    {
        //cubeObj�B�u���b�N�{��
        public GameObject cubeObj;
        //HP��Score,�j���
        [SyncVar] public int HitPoint;
        [SyncVar] public int Score;
        [SyncVar] public GameObject lastPlayer;

        //�z��ԍ��BC11���炱���ɃA�N�Z�X����ׂɎg�p����
        [SyncVar] public int ArrayID = 0;

        //�ʒu�̓����A���M�p
        [SyncVar] public Vector3 pos;
        [SyncVar] public Quaternion rot;
        [SyncVar] public Transform trans;

        //CubeData�N���X�̃R���X�g���N�^
        public CubeData()
        {
            cubeObj = default;
            HitPoint = 0;
            Score = 0;
            lastPlayer = default;
            ArrayID = 0;
        }

        //�R���X�g���N�^����2�B�I�u�W�F�N�g�̏��ƁA�z��ԍ���n�����ۂɂ����o�^����B
        //�I�u�W�F�N�g�̃^�O�ɂ���āAHP�ƃX�R�A�������Ŋi�[����B
        public CubeData(GameObject Object, int Array)
        {
            cubeObj = Object;
            ArrayID = Array;
            if (Object.tag == "Floor")
            {
                HitPoint = FLOOR_HP;
                Score = FLOOR_SCORE;
            }
            if (Object.tag == "Wall")
            {
                HitPoint = WALL_HP;
                Score = WALL_SCORE;
            }
            if (Object.tag == "Piller")
            {
                HitPoint = PILLER_HP;
                Score = PILLER_SCORE;
            }

            //pos = cubeObj.transform.position;
        }

        //�_���[�W�֐��BC11��CollisionEnter����A���̊֐����Ă�Ń_���[�W����������B
        //�����ɏW�߂Ă���̂́A�T�[�o�[---�N���C�A���g�Ԃŋ��L���邽�߁B
        public void Damage(GameObject player, int power)
        {
            lastPlayer = player;
            HitPoint -= power;

        }

        //�_���[�W�֐��B������͏��ɗ������ۂɌĂ΂��B�n��Player��񂪖������ɃG���[���������ׂ̂��́B
        public void Damage(int power)
        {
            HitPoint -= power;
        }

        //�u���b�N�̐ڐG���ɐڐG�����u���b�N����v���C���[�����擾����B
        public void SendLastPlayer(GameObject other)
        {
            lastPlayer = other;
        }




    }

    //�ʒu�̓����p�ɍ�����BTransform�^��SyncList�ɑΉ����ĂȂ�����(?�j
    //position��Rotation�𓯎��ɐݒ肷��ׂɕK�v�B
    public struct CubeTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    //�^���[�̍\���u���b�N���擾����ׂɕK�v�B
    //�w�q�x�̓���q�\���őS�u���b�N���擾����B
    Transform children;
    
    //List�B���X�ǉ�����ׁA�^���[�ɂ���č\���u���b�N�����قȂ�ׂɍ̗p�B
    //���X�́A�j�󂳂ꂽ�u���b�N�̗v�f������������肾�������A������������׌��������B
    //CubeArray,CubeTrans��R�Â��āA�ʒu�̓��������������B
    public List<CubeData> CubeArray = new List<CubeData>();
    public SyncList<CubeTransform> CubeTrans = new SyncList<CubeTransform>();




    //Start�B�����ݒ�BbreakLimit�̓��E���h�̏I�������p
    // Start is called before the first frame update
    void Start()
    {
        //GetAllChild(this.gameObject.transform);
        //M01�̃A�^�b�`
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
        if (isServer)
        {
            M01.C10 = this.gameObject.GetComponent<C10TowerAdmin>();
        }
        if (isClient)
        {
            //SetArrayID();
        }

        //GetAllChild
        //�^���[�̍\���u���b�N���擾�ACubeArray�Ɋi�[����B
        GetAllChild(this.gameObject.transform);

        breakLimit = (int)(CubeArray.Count * 0.8);

        //DontDestroyOnLoad(this.gameObject);

    }


    //Awake
    //Scene�J�ڎ��̏��ł�Ƃ�悤�Ƃ������ʁB�K�v�Ȃ������̂ŃR�����g�A�E�g�B
    /*private void Awake()
    {
        //GetAllChild(this.gameObject.transform);
        //M01�̃A�^�b�`
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
        if (isServer)
        {
            M01.C10 = this.gameObject.GetComponent<C10TowerAdmin>();
        }
        if (isClient)
        {
            //SetArrayID();
        }
        GetAllChild(this.gameObject.transform);

        breakLimit = (int)(CubeArray.Count * 0.8);
    }*/



    // Update is called once per frame
    void Update()
    {
        //�v���N�e�B�X���[�h�Ńu���b�N�𐶐����鏈��
        if(blockcount <= 0 && this.gameObject.tag == "spawner")
        {
            keepblocks = Instantiate(Blocks, this.transform.position, this.transform.rotation);
            NetworkServer.Spawn(keepblocks);
            Destroy(this.gameObject);
        }


        if (isServer)
        {
            //NetworkTransform�̑�ցB
            //���t���[���A�S�u���b�N�̈ʒu���T�[�o�[����N���C�A���g�ɑ��M����B
            //�N���C�A���g�́ACubeTrans�̏�����ɁA�����B��Cube�̈ʒu���X�V����B
            for (int i = 0; i < CubeArray.Count; i++)
            {
                //CubeObj == null = ���ɔj�󂳂ꂽ�u���b�N�B�j�󂳂ꂽ�u���b�N�̍��W�X�V�͍s��Ȃ��B
                if (CubeArray[i].cubeObj != null)
                {
                    //CubeArray[i].pos = CubeArray[i].cubeObj.transform.position;
                    //CubeArray[i].trans = CubeArray[i].cubeObj.transform;
                    //CubeArray[i].rot = CubeArray[i].cubeObj.transform.rotation;
                    CubeTransform a;
                    a.Position = CubeArray[i].cubeObj.transform.position;
                    a.Rotation = CubeArray[i].cubeObj.transform.rotation;
                    CubeTrans[i] = a;

                }
            }
        }

        //Server�ȊO = ���̎󂯎�葤�B
        //�eCube�̍��W���X�V����B
        else
        {

            for (int i = 0; i < CubeArray.Count; i++)
            {
                if (CubeArray[i].cubeObj != null)
                {

                    //Vector3 pos = Vector3.Lerp(transform.position, m_ReceivedPosition, m_LerpRate * Time.deltaTime);
                    //Quaternion rot = Quaternion.Slerp(transform.rotation, m_ReceivedRotation, m_LerpRate * Time.deltaTime);

                    //CubeArray[i].cubeObj.transform.position = Vector3.Lerp(CubeArray[i].cubeObj.transform.position,CubeTrans[i].Position, 4f * Time.deltaTime) ;
                    //CubeArray[i].cubeObj.transform.rotation = CubeTrans[i].Rotation;

                    Vector3 pos = Vector3.Lerp(CubeArray[i].cubeObj.transform.position, CubeTrans[i].Position, 10f * Time.deltaTime);
                    Quaternion rot = Quaternion.Slerp(CubeArray[i].cubeObj.transform.rotation, CubeTrans[i].Rotation, 10f * Time.deltaTime);

                    //���ۂ̍X�V�B�����ɍX�V����ƂȂ������܂��s�����B
                    CubeArray[i].cubeObj.transform.SetPositionAndRotation(pos, rot);
                }
            }

        }
    }

    //�^���[���\�����Ă���u���b�N��S�Ĕz��Ɋi�[����ׂ̕ϐ��B
    void GetAllChild(Transform parent)
    {
        //�����̃I�u�W�F�N�g�̎q���擾�B
        //������J��Ԃ����ōŉ��w�܂őS�Ď擾����B
        children = parent.GetComponentInChildren<Transform>();

        foreach (Transform child in children)
        {
            //RigidBody�̑��݂Ńu���b�N�����m
            if (child.gameObject.GetComponent<Rigidbody>())
            {
                //CubeData�^�Ő錾�B�I�u�W�F�N�g�ƁACubeArray�̃J�E���g(=���̃I�u�W�F�N�g�ŉ��ڂ�)��n��
                CubeData b = new CubeData(child.transform.gameObject, CubeArray.Count);

                //miniTower�̓��_���O����
                if(this.gameObject.tag == "miniTower")
                {
                    b.Score = 0;
                }

                //CubeArray�ɒǉ��B
                CubeArray.Add(b);
                //�u���b�N�̃I�u�W�F�N�g�ɃA�N�Z�X�p��ID��ۑ�
                child.gameObject.GetComponent<C11CubeState>().arrayID = b.ArrayID;
                //Server�̂݁A�N���C�A���g�ւ̒ʒB�p�ɍ��W���i�[�B
                if (isServer)
                {
                    CubeTransform c = new CubeTransform();
                    c.Position = b.cubeObj.transform.position;
                    c.Rotation = b.cubeObj.transform.rotation;
                    CubeTrans.Add(c);
                }
            }
            //Rigidbody�������I�u�W�F�N�g=�I�u�W�F�N�g�̉��Ƀu���b�N�����݂���B
            //=�ēxGetAllChild���R�[��
            else
                GetAllChild(child);

        }
        //Debug.Log(CubeArray.Count);


    }


    //C11�ƁAC10�ɔz����q���֐��B
    //id�͂�����Ă񂾃u���b�N�ƁA�z��̃u���b�N���q����ׂ̂��́B
    //�v���p�e�B�I�ȕ��BC10�̔z�񂪊O������Q�Əo���Ȃ��ׁA���̂悤�Ȏ�@��������B
    public void ActDamage(int id, GameObject player, int power)
    {
        if (isServer)
        {
            CubeArray[id].Damage(player, power);

            if (CubeArray[id].HitPoint < 1)
            {
                //�j�󏈗�
                BreakBlock(SpawnCheck(), id);
                //RpcCubeDestroy(id);

            }
        }
    }
    //Player�����Ȃ����p(���R������)
    public void ActDamage(int id, int power)
    {
        if (isServer)
        {
            CubeArray[id].Damage(power);

            if (CubeArray[id].HitPoint < 1)
            {
                //�j�󏈗�
                BreakBlock(SpawnCheck(), id);
                //RpcCubeDestroy(id);

            }
        }
    }

    //�u���b�N�̔j�󏈗�
    void BreakBlock(int itemNum, int id)
    {
        //�j��G�t�F�N�g�B�e�v���C���[�ŏo���ׂ�Rpc
        RpcBreakEffect(id);

        //Switch��itemNum�̌��ʂ����[�U�[�ɂ���ĈقȂ����ׁA�A�C�e�����������܂��s����N����A�s�̗p�B
        switch (itemNum)
        {
            case 1:
                //item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
                //item.GetComponent<C02ItemManager>().itemNum = itemNum;
                //NetworkServer.Spawn(item);
                break;
            case 2:
                //item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
                //item.GetComponent<C02ItemManager>().itemNum = itemNum;
                //NetworkServer.Spawn(item);
                break;
            case 3:
                //item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
                //item.GetComponent<C02ItemManager>().itemNum = itemNum;
                //NetworkServer.Spawn(item);
                break;
            default:
                break;
        }

        //0����Ȃ���=�A�C�e������������Ă���
        //�ꂩ���ɓZ�߂ďd��Spawn��h�~�B
        if(itemNum != 0)
        {
            item = Instantiate(itemPrefab, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
            item.GetComponent<C02ItemManager>().itemNum = itemNum;
            //�������e�v���C���[�̐��E�ɒʒB���邽�߂�Spawn�֐��B
            NetworkServer.Spawn(item);
        }

        //�u���b�N�̔j��B�ʒB�ׂ̈�Rpc
        RpcCubeDestroy(id);

        //���_�v�Z�̓T�[�o�[�ōs���A���ʂ��N���C�A���g�ɑ��M����B
        if (isServer)
        {
            //lastPlayer�͓��_���l������v���C���[�BScore!=0�́A���_�v�Z����񂫂�ɂ���ׂ̂��́B
            if (CubeArray[id].lastPlayer != null && CubeArray[id].Score != 0)
            {
                PointGainer(CubeArray[id].lastPlayer, CubeArray[id].Score);
                CubeArray[id].Score = 0;
            }
            //���Ȃ��Ƃ��́ANoneScore�Ƃ����ϐ��ɏ����锤�������X�R�A���i�[����B
            else if (CubeArray[id].lastPlayer == null)
            {
                PointGainer(CubeArray[id].Score);
            }

            //�I�������̈�A�u���b�N�̔j�󊄍��p�̏����B
            if (breakCount > breakLimit)
            {
                breakend = true;
            }
            else
                breakend = false;
        }
    }

    //���_�v�Z�p�̊֐��B
    void PointGainer(GameObject Player, int Score)
    {
        //Player�̓��_�����Z���鏈���B
        //���炭�eplayer���ێ����Ă����񂾂Ǝv���̂ŁA�����𒼐ڎQ�Ƃ��ĉ��Z����B
        //(Aex.�Q�[���Ǘ��N���X�����݂����ꍇ�ł��A�u�N���󂵂����v�̏��͕K�v�ł��邽�߁A������player������)
        //�{�}�[�̂ݓ��_���Z
        if (Player.GetComponent<DefenderController>().enabled == false)
        {
            Player.GetComponent<P04ItemHolder>().Score += Score;
        }
        //�^���[�j�󏈗�

        if (Player.GetComponent<PlyerControlloer>().enabled == true)
        {

            M01.BreakCounter(Player);
        }

        //�j�󊄍��p
        breakCount++;
    }

    //�s����̂Ȃ��X�R�A�̉��Z
    void PointGainer(int Score)
    {
        M01.NoneScoreCount(Score);
        breakCount++;
    }

    //C01�������Ɠ���B
    int SpawnCheck()
    {
        int ans = Random.Range(0, 8);
        if (ans == 0)
        {
            //�A�C�e���o�������B
            ans = Random.Range(0, 100);
            if (ans < 60)
            {
                //Debug.Log("PowerUp");
                return 1;
            }
            else if (ans < 75)
            {
                //Debug.Log("CountUp");
                return 2;
            }
            else
            {
                //Debug.Log("SpeedUp");
                return 3;
            }
        }
        else
            return 0;
    }

    //�ȉ��A�ʒu�����Ŏg�p���Ă��������̖v��
    public Vector3 PositionUpdate(int id)
    {
        //�N���C�A���g�ւ̈ʒu�̑��M
        if (isClient)
        {
            return CubeArray[id].pos;
        }

        return CubeArray[id].cubeObj.transform.position;
    }
    public Vector3 GetCubePosition(int id)
    {
        return CubeArray[id].pos;
    }
    public Quaternion GetCubeRotation(int id)
    {
        return CubeArray[id].rot;
    }

    public void SetArrayID()
    {
        for (int i = 0; i < CubeArray.Count; i++)
        {
            CubeArray[i].cubeObj.GetComponent<C11CubeState>().arrayID = CubeArray[i].ArrayID;
        }
    }

    //ClientCheck
    //C11��NetworkBehavier���g���Ȃ��ׁA�N���C�A���g���ǂ����𔻒f������@�������Ǝv������ł����B
    //C10���Q�Ƃ��鎖�ŁA�T�[�o�[/�N���C�A���g�̂ǂ���ł��邩�����m���Ă���B
    public bool ClientCheck()
    {
        return isServer;
    }

    public void GetCubeTransform(int id)
    {
        if (isServer)
            RpcGetCubeTransform(id, CubeArray[id].pos, CubeArray[id].rot);

        //return CubeArray[id].cubeObj.transform;
    }
    [ClientRpc]
    public void RpcGetCubeTransform(int id, Vector3 pos, Quaternion rot)
    {
        CubeArray[id].cubeObj.transform.position = pos;
        CubeArray[id].cubeObj.transform.rotation = rot;
    }


    //�u���b�N�̔j�󏈗��B
    [ClientRpc]
    public void RpcCubeDestroy(int id)
    {
        //Instantiate(breakEffect, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
        blockcount -= 1;
        Destroy(CubeArray[id].cubeObj);
        //Effect
        //Destroy(x, 5f);

    }

    //�j��G�t�F�N�g�̐���
    [ClientRpc]
    public void RpcBreakEffect(int id)
    {
        Instantiate(breakEffect, CubeArray[id].cubeObj.transform.position, Quaternion.identity);
    }

    public void SendPlayerData(int reciverid, int Senderid)
    {
        if (isServer)
        {
            if (CubeArray[Senderid].lastPlayer != null)
            {
                CubeArray[reciverid].SendLastPlayer(CubeArray[Senderid].lastPlayer);
            }
        }
    }

    public GameObject GetLastPlayer(int id)
    {
        return CubeArray[id].lastPlayer;
    }

    //Score���擾����ׂ̃v���p�e�B
    public int GetCubeScore(int id)
    {
        return CubeArray[id].Score;
    }

    //���E���h�I�����A�c�u���b�N�̍��v�_���X�R�A�Ƃ��ĉ��Z�����Defender�p�̊֐��B
    //���v�_���Z�o���ĕԂ��BM01�Ŏg�p
    public int GetDefScore()
    {
        int AllScore = 0;

        for (int i = 0; i < CubeArray.Count; i++)
        {
            //Score��0����Ȃ��� = �j�󂳂�Ă��Ȃ��u���b�N
            if (CubeArray[i].Score != 0)
            {
                AllScore += CubeArray[i].Score;
                if (CubeArray[i].Score == PILLER_SCORE)
                {

                }
                if (CubeArray[i].Score == WALL_SCORE)
                {

                }
                if (CubeArray[i].Score == FLOOR_SCORE)
                {

                }
            }
        }

        return AllScore;
    }
}
