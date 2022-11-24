using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//C10�ɑΉ�����CubeState
//�w�ǂ̏�����C10�ɔC���Ă���ׁA�d�v�ȏ����͖w�ǂȂ��B
//�B��A�����蔻��݂̂̓u���b�N��Collider���g���ׁA��������C10�̃_���[�W�֐����ĂԎ��őΏ����Ă���B
//�N���C�A���g�ŕ����v�Z���s���ƍ��W���o�O��̂ŁA�T�[�o�[����Ȃ��ꍇCube��RigidBody����菜���������s���Ă���B

public class C11CubeState : MonoBehaviour
{
    static int FLOOR_HP = 8192;
    static int WALL_HP = 1280;
    static int PILLER_HP = 1;
    static int FLOOR_SCORE = 9000;
    static int WALL_SCORE = 1500;
    static int PILLER_SCORE = 500;

    public int HitPoint;
    public int Score;
    public Rigidbody rb;

    //�j��҂̏���ێ�
    public GameObject Player;

    //�A�C�e���p��Prefab�̎擾
    public GameObject itemObj;
    GameObject item;

    //
    C10TowerAdmin C10;
    public int arrayID;

    // Start is called before the first frame update
    void Start()
    {
        if (HitPoint == PILLER_HP)
            Score = PILLER_SCORE;
        else if (HitPoint == WALL_HP)
            Score = WALL_SCORE;
        else if (HitPoint == FLOOR_HP)
            Score = FLOOR_SCORE;
        else
            Score = Score;

        //���u��
        /*if (this.gameObject.transform.position.y < 5)
        {
            HitPoint = HitPoint + HitPoint;
        }*/

        //Prefab�̃A�^�b�`���ȗ�
        itemObj = GameObject.Find("Item");

        //M01�̃A�^�b�`
        //M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();

        //TowerAdmin�̏��
        C10 = transform.root.gameObject.GetComponent<C10TowerAdmin>();


        //�N���C�A���g�̃V�[���̏ꍇ�A�����v�Z���s��Ȃ��B
        //�S�ăT�[�o�[��̌v�Z���ʂŏ������s���B
        if (C10 != null)
        {
            if (C10.ClientCheck() == false)
            {
                //this.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                //this.gameObject.GetComponent<Rigidbody>().useGravity = false;
                rb = this.gameObject.GetComponent<Rigidbody>();
                Destroy(rb);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //C10 = transform.root.gameObject.GetComponent<C10TowerAdmin>();
        if (C10 == null)
        {
            C10 = transform.root.gameObject.GetComponent<C10TowerAdmin>();
        }
        else
        {
            if (C10.ClientCheck() == false)
            {
                //this.gameObject.transform.position = this.gameObject.transform.position;
                //this.gameObject.transform.rotation = this.gameObject.transform.rotation;
            }

            if (C10.ClientCheck() == false)
            {
                //if (Vector3.Distance(this.transform.position, C10.GetCubePosition(arrayID)) > 1)
                //{
                //this.gameObject.transform.position = C10.PositionUpdate(arrayID);
                //this.gameObject.transform.position = C10.GetCubePosition(arrayID);
                //this.gameObject.transform.rotation = C10.GetCubeRotation(arrayID);
                //C10.GetCubeTransform(arrayID);
                //}
            }
            //this.gameObject.transform.position = C10.PositionUpdate(arrayID);

            Player = C10.GetLastPlayer(arrayID);
        }

    }


    //�ڐG����
    //C10�̊֐����R�[������B
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Field")
        {
            //HitPoint -= 8192;
            //Destroy(this.gameObject);
            C10.ActDamage(arrayID, 8192);
        }
        if (collision.gameObject.tag == "splinter")
        {
            //�ʏ픚�e
            /*if (isServer)
            {
                RpcDamage(collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 0);
            }*/
            C10.ActDamage(arrayID, collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 1);



        }
        if (collision.gameObject.tag == "splinter2")
        {
            //�S�����e
            /*if (isServer)
            {
                RpcDamage(collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 1);
            }*/
            C10.ActDamage(arrayID, collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 1);

        }

        if (collision.gameObject.tag == "Dfsplinter")
        {
            //�S�����e
            /*if (isServer)
            {
                RpcDamage(collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 1);
            }*/
            C10.ActDamage(arrayID, collision.gameObject.GetComponent<B01BombStatus>().bombOwner, 8192);

        }

        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Piller")
        {
            /*if(collision.gameObject.GetComponent<C01CubeState>().Player != null)
            {
                Player = collision.gameObject.GetComponent<C01CubeState>().Player;
            }*/
            C10.SendPlayerData(arrayID, collision.gameObject.GetComponent<C11CubeState>().arrayID);
        }
    }

    public int GetScore()
    {
        int temp = C10.GetCubeScore(arrayID);
        return temp;
    }
}
