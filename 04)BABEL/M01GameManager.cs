using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//�Q�[���̓��_������Ǘ�����ׂ̃X�N���v�g�B
//RoundSet�Ƃ͈Ⴄ�Ӗ��ł́A�Q�[���i�s�̒����B
//�I��������A�{�[�i�X�|�C���g�̌v�Z�Ȃǂ��s���Ă��܂��B

public class M01GameManager : NetworkBehaviour
{
    //���E���h�I������(�j�󔻒肪�s��������)
    const int TOWER_LIMIT = 4;
    //�u���b�N�̔j���(�j�󔻒�ƌ��Ȃ�����)
    const int BREAK_LIMIT = 25;
    //�j�󔻒���p�����鎞��(���݂͍ŏ���1�`20�j�󂳂��܂ł̎���)
    const float BREAK_TIME = 7.0f;
    //�^���[�̔j�󔻒�ɂ��{�[�i�X�t�^��臒l
    const int CLUSH_BONUS = 5;

    [SyncVar] public int towerBreakcount = 0;
    [SyncVar] int blockBreakcount = 0;
    [SyncVar] public bool end;

    public GameObject localPlayer;
    //public SyncList<GameObject> user = new SyncList<GameObject>();
    [SyncVar] int resultScore_B;
    [SyncVar] int resultScore_D;

    [SyncVar] public int NoneScore = 0;

    float timer = 0f;

    public GameObject towerPrefab;
    GameObject tower;


    //�X�R�A�ۑ��p�ϐ��Q
    [SyncVar] public int timeScore;
    [SyncVar] public int remainScore;
    [SyncVar] public int bonusScore;
    [SyncVar] public int bonusNum;


    [SyncVar] public C10TowerAdmin C10;

    public struct PlayerData
    {
        public GameObject player;
        //Ture = Bomber False = Defender
        public bool side;
        public string name;
    }
    public SyncList<PlayerData> user = new SyncList<PlayerData>();

    public bool breakend;

    //singleton�p�B�g���Ȃ�����
    static bool existsInstance = false;

    // Start is called before the first frame update
    void Start()
    {

        //DontDestroyOnLoad(this.gameObject);

    }

    // ������
    //Awake�֐��B�V�[���J�ڂɂ����āASingleton��DontDestroyOnLoad���g�����ŏ�Ɏ������悤�Ƃ������A
    //UNET-Mirror�ɂ��o�O�A�d�l�ɂ��AAwake������ȃ^�C�~���O�ŌĂ΂ꂸ�A���Ғʂ�̓�������Ȃ��ׂɖ��g�p�ƂȂ����B
    /*void Awake()
    {
        // �C���X�^���X�����݂���Ȃ�j������
        if (existsInstance)
        {
            Destroy(gameObject);
            return;
        }

        // ���݂��Ȃ��ꍇ
        // ���g���B��̃C���X�^���X�ƂȂ�
        existsInstance = true;
        DontDestroyOnLoad(gameObject);
    }*/


    //�^���[�j�󔻒�̉��Z��A�u���b�N�j�󊄍��ł̏I������Ɍ��m���Ă���B
    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            if (blockBreakcount > 0)
            {
                timer += Time.deltaTime;
            }
            if (timer > BREAK_TIME)
            {
                blockBreakcount = 0;
                timer = 0f;
                Debug.Log("Count end");
            }
        }

        //Debug.Log(user[0].player.GetComponent<P04ItemHolder>().Score);

        if (C10.breakend == true)
        {
            breakend = true;
        }
        else
            breakend = false;


    }


    //�^���[�j�󔻒�̃X�R�A���Z�����B
    //�I�������̊m�F���s���Ă���B
    public void BreakCounter(GameObject Player)
    {
        Debug.Log("Count start");
        if (end == false)
        {
            blockBreakcount += 1;
            if (blockBreakcount > BREAK_LIMIT)
            {
                Player.GetComponent<P04ItemHolder>().Score += 150000;
                blockBreakcount = 0;
                towerBreakcount++;
                Debug.Log("Tower Break!");
            }
            if (towerBreakcount > TOWER_LIMIT)
            {
                end = true;
            }
        }
    }


    //���E���h�I�����̓��_�v�Z�p
    //���g�p
    [ClientRpc]
    public void RpcSetPlayerList(GameObject Player)
    {
        if (isServer)
        {
            //user.Add(Player);
        }
        Debug.Log(user);
    }

    //���_�v�Z�p�֐�(���g)
    //�w�c�ʂ̓��_�v�Z�B���s��L���s���̔���Ɏg���Ă���B
    public void ResultScoreCount()
    {
        resultScore_B = 0;
        resultScore_D = 0;

        for (int i = 0; i < user.Count; i++)
        {
            if (user[i].side == true)
            {
                resultScore_B += user[i].player.GetComponent<P04ItemHolder>().Score;
            }
            if (user[i].side == false)
            {
                resultScore_D += user[i].player.GetComponent<P04ItemHolder>().Score;
            }

        }

        Debug.Log("Score_B:" + resultScore_B);
        Debug.Log("Score_D:" + resultScore_D);
    }

    //�s����̂Ȃ��X�R�A�̉��Z
    //�{���́A�I������Bomber�̓_���ɉ��Z����\�肾����(�j��͂���Ă����)
    public void NoneScoreCount(int score)
    {
        NoneScore += score;
    }

    //�v���C���[���ڑ����Ă����ۂɁA���̃I�u�W�F�N�g���i�[����֐�
    //���_�v�Z��\���ȂǁA�����ō�����z�񂩂�v���C���[�����擾���Ă���B
    //���ɁARoundSet.cs�ŁA������user�z�񂪕p�o����B
    public int AddPlayerList(GameObject player, bool team)
    {
        int id = 9;
        if (isServer)
        {
            PlayerData p = new PlayerData();
            p.player = player;
            p.side = team;
            user.Add(p);
            id = user.Count - 1;
        }

        return id;
    }

    //���E���h�I�����̎c�u���b�N���ɂ�链�_�̌v�Z���s���ׂ̊֐��B
    //C10�̊֐����R�[�����A���ʂ�Ԃ��B
    public int DefScoreCount()
    {
        int BlockScore = 0;
        BlockScore = C10.GetDefScore();
        return BlockScore;
    }


    //���E���h�I�����̃{�[�i�X�X�R�A�̌v�Z���s���B
    //���ԁA�u���b�N���A�I��������3�̃{�[�i�X������B
    public void BonusSet(int endtime)
    {
        int time = endtime;
        //timeScore = 0;

        //���Ԃɂ��{�[�i�X�t�^
        if (time >= 0)
            timeScore = (180 - time) * 500;
        else
            timeScore = 0;

        //�c���u���b�N���ɂ��{�[�i�X�t�^
        remainScore = 0;
        remainScore = DefScoreCount();



        //�^���[�j�󔻒�̉񐔂ɂ��{�[�i�X�t�^
        if (towerBreakcount >= CLUSH_BONUS)
        {
            bonusScore = 500000;
            bonusNum = 1;
            end = false;
        }
        //�u���b�N�̔j�󊄍��ɂ��{�[�i�X�t�^
        else if (C10.breakend == true)
        {
            bonusScore = 100000;
            bonusNum = 2;
        }
        //�^���[�h�q�ɂ��{�[�i�X�t�^
        else
        {
            bonusScore = 300000;
            bonusNum = 3;
        }

    }

    //�L���s���A���s���m�F����֐��B
    public int JudgeScore()
    {
        ResultScoreCount();
        if (resultScore_B > resultScore_D)

            return 0;

        else if (resultScore_B < resultScore_D)

            return 1;

        else
            return 2;
    }
}
