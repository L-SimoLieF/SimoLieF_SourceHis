using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�����_���Z�o����A���\���������B
//felocity = felocity - �U���֐�
//3��̃R�[���AIEnumerator�ōs���K�v������
//3�̑I�o�p�֐��B�I�o�p�֐��̕Ԃ�l��int�ɂ��āAIEnumerator����felocity -= ���s���B
//�I�o�p�֐����Ŕ��ˊ֐����R�[���BW02��������ɕt�����Ă邩����Ȃ��B

//08/25 �ǋL
//�U���V�X�e���̃A�b�v�f�[�g�BWhaleAttack2nd�֐��ցB
//11/01 �ǋL
//�U���V�X�e���̃A�b�v�f�[�g�BWhaleAttack3rd�ցB

public class W02WhaleAttack : MonoBehaviour
{
    public GameObject player;
    public GameObject gem;

    //�e���̔��ˊԊu((�V�V�X�e��)�ړ��J�n���甭�˂܂ł̎���)
    public float atkInterval = 0.5f;
    //�����m��񂯂Ǐ�肭�����Ȃ������̂Ŏg���ĂȂ��B
    //WhaleAttack2nd�̒��AWaitforSeconds��ς��Ă��������B

    //CenterLine�p
    public GameObject centerLine;
    Vector3[] linePositions;
    int lineCount;

    W01WhaleMoving W01;

    int randomDir;

    W04WhaleAnimator W04;

    // Start is called before the first frame update
    void Start()
    {
        //Player Object�̃A�^�b�`
        player = GameObject.Find("Board");

        //�U���΍��p��LinePosition�̃A�^�b�`
        //CenterLine�p
        centerLine = GameObject.Find("CenterLine");
        linePositions = new Vector3[centerLine.GetComponent<LineRenderer>().positionCount];
        centerLine.GetComponent<LineRenderer>().GetPositions(linePositions);

        W01 = GetComponent<W01WhaleMoving>();

        W04 = GetComponent<W04WhaleAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack01(player);
        }
        if (Input.GetKeyDown(KeyCode.H))
            HorizontalShotBase(11);
    }


    //�U������
    //�{�̗pver. W02��IEnumerator�֐����R�[���B���̊֐����̂�W01�ŌĂяo���Ă���B
    //(�ʃN���X����̃R�[���ɐ���ɔ������Ȃ��\������B)
    //�V�U���V�X�e�� --- Whale Attack2nd.
    //�V�U���V�X�e�� --- Whale Attack3rd.
    public void Attack(int ferocity)
    {
        //StartCoroutine("WhaleAttack", ferocity);
        //StartCoroutine("WhaleAttack2nd");
        StartCoroutine("WhaleAttack3rd",ferocity);
    }


    //�U������
    //3�x�ڂ̐����B0.5�b����1�񂸂A�v�O��̒e�����˂��s���B
    //�����1��ڂƎ����悤�ȏ����B
    IEnumerator WhaleAttack3rd(int ferocity)
    {
        //�|�W�V�����`�F���W�̑ҋ@���ԁB
        yield return new WaitForSeconds(3f);
        //Debug.Log(Vector3.Distance(player.transform.position, transform.position));

        SeSystem.WhaleMissile = true;

        //Barrage3rd(ferocity);

        randomDir = Random.Range(0, 2);

        //Barrage3rd���R�[���B
        //�����͋��\���Ɖ񐔁B

        yield return new WaitForSeconds(atkInterval);
       
        Barrage3rd(ferocity,0);

        yield return new WaitForSeconds(atkInterval);
        
        Barrage3rd(ferocity,1);

        yield return new WaitForSeconds(1.2f);
       
        Barrage3rd(ferocity,2);

       

    }

    //Barrage3rd(Barrage = �e�� --3rd)
    //�d�l���ɋL�ځB
    //�Œ�e����ammo,�Z�o�ׂ̈̎���add
    void Barrage3rd(int ferocity,int count)
    {

        if (W04.damageFlag == true)
            return;

        int ammo = 1;
        int add = 1 * (ferocity / 6);
        ammo = ammo + add;

        //���ꂼ��ꔭ�͌Œ�ׁ̈A�ʘg�ŃR�[���B
        //True�̓z�[�~���O�Afalse�̓����_���e�̈ӁB
        //2�����������B
        Shot3rd(count,true);
        //Shot3rd(count,false);
        ammo -= 1;

        //�e���̓��󌈒�
        //�c�e���A�J��Ԃ����[�v�B
        for(int i = 0; i < ammo; i++)
        {
            int a = Random.Range(0, 5);
            //homing
            if(a < 2) 
            {
                Shot3rd(count, false);
            }
            //random
            else
            {
                Shot3rd(count, false);
            }


        }
    }

    public Transform missileParent;

    //3��ڒe���V�X�e���p�����֐��B
    //�e������āA�e�̃X�N���v�g�ɏ���n���Ă邾���B
    void Shot3rd(int count,bool d)
    {
        //�����ʒu�������_���ɂ��鎖�ŁA�e�����ۂ��̉��o
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + Random.Range(-3, 3), transform.position.z + Random.Range(-5, 5));

        GameObject ins = Instantiate(gem,pos, Quaternion.identity, missileParent);

        ins.GetComponent<F02FishMove>().distanceHP = W01.distanceHP;
        //F02 �����͍�����^�[�Q�b�g�B�e���̉񐔁A�z�[�~���O���邩�ۂ��B
        ins.GetComponent<F02FishMove>().Constructer(player,count,d,linePositions,randomDir);
    }

    //8���X�V �V�U���V�X�e��
    //�ړ��̎��Ԃ��҂��ׂ�WaitforSeconds,W01���Ő��䂵�Ă��ǂ��B
    //���@�_���ƁA���@������͈͓��������_���ɑ_�����i�e�̍\���B
    IEnumerator WhaleAttack2nd()
    {

        //�ړ����� lerpRange�Ɠ�������
        yield return new WaitForSeconds(3f);

        //Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //�ǔ��e �n���Ă�͔̂��ˈʒu
        Homing(transform.position);

        //���i�e for�̉񐔂��e�̐�
        //�n���Ă�͔̂��ˈʒu�Ay��z���ɗ��������ގ��Œe�����ۂ������B
        for (int i = 0; i < 12; i++)
        {
            Straight(new Vector3(transform.position.x, player.transform.position.y + Random.Range(-20, 20), transform.position.z + Random.Range(-30, 30)));
        }
    }

    //------------------------------
    //WhaleAttack2nd�ŗp�����֐��Q
    //�e�ۂ̐����A�������F01.Constructer�֐��ŏ����ݒ���s���Ă���B
    //NowArea.Number�̓N�W���̌��݈ʒu�B�O�����Ȃ���̂ɕK�v�B0��n���Ă�̂͑��Ԑ��ׁ̈B

    //���ˈʒu��n���Ēǔ�������B
    void Homing(Vector3 pos)
    {
        GameObject ins = Instantiate(gem, pos, Quaternion.identity);
        ins.GetComponent<F01FishMove>().Constructer(player, this.gameObject.GetComponent<W01WhaleMoving>().NowArea.Number);
    }
    //���ˈʒu��n���āA�������璼�i
    void Straight(Vector3 pos)
    {
        GameObject ins = Instantiate(gem, pos, Quaternion.identity);
        ins.GetComponent<F01FishMove>().Constructer(player, this.gameObject.GetComponent<W01WhaleMoving>().NowArea.Number, 0);
    }

    //-------------------------------



  
    //-------------------------------- �ȉ��A���U���V�X�e��

    //�U�������̊�b����
    //3�e���A���ꂼ�ꃉ���_���BWaitforSeconds���g���ׂ�IEnumerator
    IEnumerator WhaleAttack(int ferocity)
    {
        //3��U��
        //�eAttack���Œe���̌���A�y�є��˂��s���A�g�������\���̗ʂ�Ԃ��B
        //�Ԃ��Ă������\�����A�S�̂�������Ď��̒e���̌���ɗ��p
        //(�v���k) 3��ł��؂������_��0�ɂȂ�悤�Ɏg���؂�B

        ferocity = ferocity - FirstAttack(ferocity);

        yield return new WaitForSeconds(atkInterval);

        ferocity = ferocity - SecondAttack(ferocity);

        yield return new WaitForSeconds(atkInterval);

        ferocity = ferocity - ThirdAttack(ferocity);


        //���߁A�������͎g���؂�Ȃ������ꍇ�Ƀ��b�Z�[�W��\��(�f�o�b�O�p)
        if (ferocity != 0)
        {
            Debug.LogError("ferocity is valied ---SimoLieF");
        }

        //Homing
        //�V�U���V�X�e���̃e�X�g�p WhaleAttack2nd�ɈڐA�ς�
        //HomingBase();


    }


    

    //�e��̍U���p�֐�
    //(06/16) �e���v�[�������u���ׁ̈A�d�g�݂����L�ځB
    //Case�̒��ɂ��ꂼ��ɑΉ������e���֐����L�q����B
    int FirstAttack(int ferocity)
    {
        //�g��felocity�����肵�āA����Switch�̒��ŕ��e�������肷�遨����
        //���_:felocity�ʂ̑I�𐔂͕ς��Ȃ����A��felocity�̕���������Α����قǊm��������B

        int rund = Random.Range(0, ferocity + 1);

        switch (rund)
        {
            case 0:
                Debug.Log("0 Attack");
                break;
            case 1:
                //���������
                //HorizontalShot1Line(11, 3, 10);
                Debug.Log("1 Attack");
                break;
            case 2:
                //HorizontalShot2Line(19, 2, 5);
                Debug.Log("2 Attack");
                break;
            case 3:
                Debug.Log("3 Attack");
                break;
            case 4:
                //HorizontalShot4Line(19, 0,5);
                Debug.Log("4 Attack");
                break;
            case 5:
                Debug.Log("5 Attack");
                break;
            case 6:
                Debug.Log("6 Attack");
                break;
            case 7:
                Debug.Log("7 Attack");
                break;
            case 8:
                Debug.Log("8 Attack");
                break;
            case 9:
                Debug.Log("9 Attack");
                break;
            case 10:
                Debug.Log("10 Attack");
                break;

            //felocity >=11 �̎�
            default:
                Debug.Log("11 Attack");
                break;
        }

        //HorizontalShot4Line(19, 0, 5);
        //HomingBase();

        //�Ԃ�l�͎g�����R�X�g(���\��)�̐�
        return rund;
    }

    int SecondAttack(int ferocity)
    {
        int rund = Random.Range(0, ferocity + 1);

        switch (rund)
        {
            case 0:
                Debug.Log("0 Attack");
                break;
            case 1:
                Debug.Log("1 Attack");
                break;
            case 2:
                Debug.Log("2 Attack");
                break;
            case 3:
                Debug.Log("3 Attack");
                break;
            case 4:
                Debug.Log("4 Attack");
                break;
            case 5:
                Debug.Log("5 Attack");
                break;
            case 6:
                Debug.Log("6 Attack");
                break;
            case 7:
                Debug.Log("7 Attack");
                break;
            case 8:
                Debug.Log("8 Attack");
                break;
            case 9:
                Debug.Log("9 Attack");
                break;
            case 10:
                Debug.Log("10 Attack");
                break;

            //felocity >=11 �̎�
            default:
                Debug.Log("11 Attack");
                break;
        }


        //HorizontalShot4Line(19, -6, 5);
        //HomingBase();

        return rund;
    }

    int ThirdAttack(int ferocity)
    {
        //���\��(felocity)�̒l���g���؂�Ȃ���΂����Ȃ��ȏ�A3��ڂ̍U���͗^����ꂽfelocity�̂���Ă̂݌���t������B
        //���ׁ̈A�O���ōs���Ă���random�����͕K�v�Ȃ��B


        switch (ferocity)
        {
            case 0:
                Debug.Log("0 Attack");
                break;
            case 1:
                Debug.Log("1 Attack");
                break;
            case 2:
                Debug.Log("2 Attack");
                break;
            case 3:
                Debug.Log("3 Attack");
                break;
            case 4:
                Debug.Log("4 Attack");
                break;
            case 5:
                Debug.Log("5 Attack");
                break;
            case 6:
                Debug.Log("6 Attack");
                break;
            case 7:
                Debug.Log("7 Attack");
                break;
            case 8:
                Debug.Log("8 Attack");
                break;
            case 9:
                Debug.Log("9 Attack");
                break;
            case 10:
                Debug.Log("10 Attack");
                break;

            //felocity >=11 �̎�
            default:
                Debug.Log("11 Attack");
                break;
        }

        //HorizontalShot5Line();
        //HomingBase();

        return ferocity;
    }


    ///////////////////�e���p�֐���

    //������
    //�{�v���W�F�N�g�ł̐i�s������x���ʂ������ׁAfor�̒���Vector3�̐��l��M��܂����B
    void HorizontalShotBase(int num)
    {
        Vector3 pos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);

        for (int x = num / 2; x > (num / 2) - num; x--)
        {
            GameObject ins = Instantiate(gem, pos + new Vector3(0, 0, x * 10), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }
    }

    //�������(���A�����A�Ԃ�����)
    void HorizontalShot1Line(int num, int height, int space)
    {
        Vector3 pos = new Vector3(transform.position.x, player.transform.position.y + height, transform.position.z);

        for (int x = num / 2; x > (num / 2) - num; x--)
        {
            GameObject ins = Instantiate(gem, pos + new Vector3(0, 0, x * space), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }
    }

    //�������
    void HorizontalShot2Line(int num, int height, int space)
    {
        HorizontalShot1Line(num, height, space);
        HorizontalShot1Line(num, height + height, space);
    }

    //�����O��
    //�v�[���\�Ɉ�����Ȃ������ג��ڐݒ�
    void HorizontalShot3Line()
    {
        HorizontalShot1Line(7, 2, 5);
        HorizontalShot1Line(11, 0, 5);
        HorizontalShot1Line(7, -2, 5);
    }

    //�����l��
    void HorizontalShot4Line(int num, int height, int space)
    {
        for (int i = 0; i < 4; i++)
        {
            HorizontalShot1Line(num, height, space);
            height = height + 2;
        }
    }

    //�����ܗ�
    //�v�[���\�Ɉ�����Ȃ�������
    void HorizontalShot5Line()
    {
        HorizontalShot1Line(7, 4, 5);
        HorizontalShot1Line(7, -4, 5);
        HorizontalShot1Line(11, 2, 5);
        HorizontalShot1Line(11, -2, 5);
        HorizontalShot1Line(19, 0, 5);
    }


    //Homing�̃e�X�g
    //�ǔ��e�ƁA���̎���Ɋ���̒����e�B
    void HomingBase()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Homing(pos);
        for (int i = 0; i < 12; i++)
        {
            Straight(new Vector3(transform.position.x, player.transform.position.y + Random.Range(-20,20) ,transform.position.z + Random.Range(-30,30)));
        }
    }

  


    /////////////////////////�e���p�֐� �����܂ŁB



    //���ăV���[�Y
    //�e����1 ���������ɂ΂�T���z�B
    public void HorizontalShotTest()
    {
        int a = 10;
        for (int x = a / 2; x > (a / 2) - a; x--)
        {
            GameObject ins = Instantiate(gem, transform.position + new Vector3(x * 10, 0, 0), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }

    }
    //HorizontalShot�̃I�[�o�[���C�h�B�����Ɍ������B
    void HorizontalShotTest(int num)
    {
        int a = num;

        for (int x = a / 2; x > (a / 2) - a; x--)
        {
            GameObject ins = Instantiate(gem, transform.position + new Vector3(x * 2, 0, 0), Quaternion.identity);
            ins.GetComponent<F01FishMove>().target = ins.gameObject;
        }
    }

    //�U�� ���g
    void Attack01(GameObject player)
    {
        GameObject ins = Instantiate(gem, transform.position, Quaternion.identity);
        //ins.transform.LookAt(player.transform);
        //Vector3 dir = player.transform.position - ins.transform.position;
        //dir = new Vector3(dir.x, dir.y, dir.z + 50f);
        //ins.GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
        ins.GetComponent<F01FishMove>().target = player;
    }
}
