using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//------�^���[���\������u���b�N�̃X�e�[�^�X���Ǘ�����ׂ̃X�N���v�g(���d�l)
//Mirror���C�u������NetworkIdentity,transform���g�����ŁA�ʐM�ɂ�铯�����������܂����B
//�`�[������HP�̊T�O��A�X�R�A�A�j��҂̏��ȂǁA�Q�[���I�ɏd�v�ȉӏ��ɂȂ��Ă��܂����B(�ߋ�)
//���݂�ROM�ł͎g�p���Ă��܂���B
public class C01CubeState : NetworkBehaviour
{
    //�e�萔�̐ݒ�B���ꂼ��u���b�N�ɂ���Ē�߂��Ă���B
    static int FLOOR_HP = 8192;
    static int WALL_HP = 1280;
    static int PILLER_HP = 1;
    static int FLOOR_SCORE = 9000;
    static int WALL_SCORE = 1500;
    static int PILLER_SCORE = 500;

    //SyncVar_HP�B�_���[�W�����L���Ȃ��ƁA�e�X�̃V�[���ňقȂ錋�ʂɂȂ�ׁB
    [SyncVar] public int HitPoint;
    //Score�B�j�󎞂ɓ�����X�R�A�B�ݒ��Start()���B
    public int Score;
   

    //�j��҂̏���ێ�
    [SyncVar] public GameObject Player;

    //�A�C�e���p��Prefab�̎擾
    public GameObject itemObj;
    GameObject item;

    //�Q�[���S�̂��Ǘ�����X�N���v�g�B�����ł̓^���[�j�󔻒�ׂ̈ɃA�N�Z�X����B
    M01GameManager M01;

    // Start is called before the first frame update
    void Start()
    {

        //�u���b�N�X�e�[�^�X�̏����ݒ�B�ŏ���HP�����āA�X�R�A��ݒ肵�Ă��܂����B
        if (HitPoint == PILLER_HP)
            Score = PILLER_SCORE;
        else if (HitPoint == WALL_HP)
            Score = WALL_SCORE;
        else if (HitPoint == FLOOR_HP)
            Score = FLOOR_SCORE;
        else
            Score = Score;


        //���u��
        //�^���[�̈�ԉ������Ȃ��悤�ɂ��鏈���B
        if (this.gameObject.transform.position.y < 5)
        {
            HitPoint = HitPoint + HitPoint;
        }

        //Prefab�̃A�^�b�`���ȗ�
        itemObj = GameObject.Find("Item");

        //M01�̃A�^�b�`
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();

    }

    // Update is called once per frame
    void Update()
    {

        //HP��1�ȉ�=�j�󂳂ꂽ��
        if (HitPoint < 1)
        {
            //Server����ARpc���R�[���B
            //Rpc=�S�N���C�A���g�ɏ������s�킹��@�\�B����̓u���b�N�̔j�󏈗�
            if (isServer)
                RpcBreakBlock(SpawnCheck(), this.gameObject.transform);

        }

    }

    //�ʐM�Ή��O�̔j��p�֐��B
    //�A�C�e����Spawn�ƁA�X�R�A�擾�̊֐����R�[�����āA���̃I�u�W�F�N�g��j�󂷂�B
    void BreakBlock()
    {

        ItemSpawn(this.gameObject.transform);
        Destroy(this.gameObject);
        PointGainer(Player);

    }



    //�A�C�e�������p�̊֐��B
    //�O��A�Ƃ����т̐��Ŏg�����A�C�e�����������̂܂ܗ��p�A���g�����ς��Ď������܂����B
    void ItemSpawn(Transform pos)
    {
        //SplitOfLight���痬�p�B

        //5���̊m���Ő����B
        int ans = Random.Range(0, 2);
        if (ans == 0)
        {
            item = Instantiate(itemObj, pos.position, Quaternion.identity);
            Debug.Log(item.transform.position);
            //�A�C�e���o�������B
            ans = Random.Range(0, 100);

            //��ނ�3�A5���A2���A3���̊m���B
            //ItemNum�́A�擾����Player�����Q�Ƃ���ׂɕK�v�B�������瑝������p�����[�^�����肷��B
            if (ans < 50)
            {
                Debug.Log("PowerUp");
                item.GetComponent<C02ItemManager>().itemNum = 0;
            }
            else if (ans < 70)
            {
                Debug.Log("CountUp");
                item.GetComponent<C02ItemManager>().itemNum = 1;
            }
            else
            {
                Debug.Log("SpeedUp");
                item.GetComponent<C02ItemManager>().itemNum = 2;
            }

        }
    }

    //�X�R�A���Z�p�̊֐��B
    //�n����Player��Score�����Z����ׂ̂��́B
    //�^���[�j�󔻒�Ɋւ��ẮAM01�ɋL�ځB
    void PointGainer(GameObject Player)
    {
     
        Player.GetComponent<P04ItemHolder>().Score += Score;

        //�^���[�j�󏈗�
        //�����́uBomber���ǂ����v�B�g���Ă�X�N���v�g�Ŕ��f
        if (Player.GetComponent<PlyerControlloer>().enabled == true)
        {
            M01.BreakCounter(Player);
        }
    }


    //�ڐG����
    //���e�A���A�ʃu���b�N�Ƃ̐ڐG���ɂ��ꂼ�ꏈ��������B
    private void OnCollisionEnter(Collision collision)
    {

        //field
        //���ɗ������ꍇ���j��BDestroy����Ȃ��̂̓X�R�A���Z�ׁ̈B
        if (collision.gameObject.tag == "Field")
        {
            HitPoint -= 8192;
            //Destroy(this.gameObject);
        }

        //splinter---���e�B�З͂��قȂ�ׂɕ���B
        if (collision.gameObject.tag == "splinter")
        {
            //�ʏ픚�e
            if (isServer)
            {
                RpcDamage(collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 0);
            }

        }
        if (collision.gameObject.tag == "splinter2")
        {
            //�S�����e
            if (isServer)
            {
                RpcDamage(collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 1);
            }

        }

        //�u���b�N
        //������΂��ꂽ�u���b�N�ɓ������āA���ɗ������ꍇ�̓��_�̍s�悪�Ȃ��ׁA���������u���b�N�̔j��ҏ���`�B����B
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Piller")
        {
            if (collision.gameObject.GetComponent<C01CubeState>().Player != null)
            {
                Player = collision.gameObject.GetComponent<C01CubeState>().Player;
            }
        }
    }

    //Network�̂��b
    //Cube�̏���ۑ����Ȃ��Ƃ����܂���B
    //�e�X������ɏ���������f�[�^���A�e�N���C�A���g�ɋ��L�����Ƃ��K�v�ł��B
    //�Ⴆ��HP,Player�̏�񓙂ł��B
    //���t���[���������s���̂͏�To���Ȃ̂ŁA�ύX���������������ɂ��܂��傤�B
    //�ύX���֐��Ăяo���ōs���A���̊֐��Ăяo�����T�[�o�[�ɒʒm����΁A�S�Ă���肭�s���܂��B


    //RpcDamage
    //�u���b�N�ւ̃_���[�W���e�v���C���[�̊��֓�������ׂ̂��́B
    //�Ō�ɐG�����l�ԂɃX�R�A������Ƃ����s����APlayer�̏��`�B���K�v�B
    [ClientRpc]
    void RpcDamage(GameObject Bomber, float i)
    {
        P04ItemHolder P04;
        P04 = Bomber.GetComponent<P04ItemHolder>();

        //if (i == 0f)
        //    HitPoint -= (float)Mathf.Pow(2.0f, P04.itemPower / 10.0f);
        //else if (i == 1.0f)
        //    HitPoint -= (float)Mathf.Pow(2.0f, P04.itemPower / 10.0f - 1.0f);
        //HitPoint -= 2 * collision.gameObject.GetComponent<BombStatus>().xxx
        HitPoint -= 1;
        Player = Bomber;
    }

    //�������邩�ǂ����̌���B
    //RpcBlock�̈����ɁA�A�C�e���̐������s�����ۂ��ƁA�A�C�e���̎�ނ�������ێ�������B
    //0�͐��������B1-3�ŃA�C�e���̋�ʁB

    //RpcBreakBlock
    //�u���b�N�j��p�̊֐��̊����ŁBNetworkServer.Destroy�ŒʐM�ɑΉ��B
    //Score�̉��Z�̓T�[�o�[�ōs���B
    [ClientRpc]
    void RpcBreakBlock(int itemNum, Transform pos)
    {

        switch (itemNum)
        {
            case 1:
                item = Instantiate(itemObj, pos.position, Quaternion.identity);
                item.GetComponent<C02ItemManager>().itemNum = itemNum;
                break;
            case 2:
                item = Instantiate(itemObj, pos.position, Quaternion.identity);
                item.GetComponent<C02ItemManager>().itemNum = itemNum;
                break;
            case 3:
                item = Instantiate(itemObj, pos.position, Quaternion.identity);
                item.GetComponent<C02ItemManager>().itemNum = itemNum;
                break;
            default:
                break;
        }
        NetworkServer.Destroy(this.gameObject);
        if (isServer)
        {
            if (Player != null)
            {
                PointGainer(Player);
            }
        }
    }


    //SpawnCheck
    //�A�C�e�����X�|�[�����邩�A����Ȃ�Ȃ�̎�ނ���Ԃ��֐��B
    //RpcBreakBlock�̈����ŃR�[�����A�����Ɍ��ʂ�Ԃ��B
    int SpawnCheck()
    {
        int ans = Random.Range(0, 2);
        if (ans == 0)
        {
            //�A�C�e���o�������B
            ans = Random.Range(0, 100);
            if (ans < 50)
            {
                //Debug.Log("PowerUp");
                return 1;
            }
            else if (ans < 70)
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


}

