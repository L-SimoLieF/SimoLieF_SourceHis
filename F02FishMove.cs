using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�V�d�l ���~�T�C��

public class F02FishMove : MonoBehaviour
{
    GameObject target;//player


    //�x�W�F�Ȑ��p�ϐ� (�x�W�F�Ȑ� = �g���ƒe���Ȃ���Z�p)
    //�J�n�ʒu�A���ԁA�I�_
    Vector3 bezieStart;
    Vector3 bezieCenter;
    Vector3 bezieEnd;

    //�Ȑ���`�����߂̒��ԓ_�B
    //�V�d�l�ł͂Ƃ肠������������ƉE�������ɂ̂ݑ��݁B
    Vector3 bezieMiddle;

    //�e�̋O���Ǘ��p�ϐ��Q
    float bezieT;      //�x�W�F�Ȑ��̐i�s�x


    //�x�W�F�Ȑ��̕ω��l�B
    //(�e�ۂ̑��x���w�肷��ׁA�x�W�F�ɂ����W�ړ����s���Ă��Ȃ�)
    Vector3 beziePos;

    int ferocity = 0;//���\��
    public float  fishSpeed;//���x
    bool reTarget;//�ڋߎ��̕⊮
    bool homFlag;//�ǔ����邩�ǂ����BW02����󂯎�����l���g�p�B
    int a;//�x�W�F�̐i�s��x�点��ׂ̕��B���s����̌��ʁB���̂Ƃ���g���ĂȂ��B

    float timer;

    //�␳�p
    Vector3[] linePosition;
    int lineCount;

    //�����ɉ����ĕ␳��ύX���邽�߂̃t���O
    bool crossFlag;

    //�����ɉ����ē��B���Ԃ𒲐����邽�߂ɕK�v
    //W01������������Ă���
    public float distanceHP;
    int distanceCount;

    //x(���B����)�̃X�R�[�v�������ׁABezieNew�Ŏg���Ȃ��B
    //�g���ׂ�Public�ϐ��B
    public float xBox;

    //2���ڈȍ~�A����/���s�����ɒe���o����������ׂ̕ϐ��B
    int dir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //�x�W�F�̐i�s�Ǘ��B�ڍׂ�BezieController�ցB
        //����A�J�[�u���I������i�K�ŕ␳�����Ɉڍs�B
        //�␳�����̓^�[�Q�b�g�̍Đݒ�B�܂��ǔ��e/�����_���e�Ń^�[�Q�b�g���W��ς��Ă���B

        //BezieControllerNew
        //���׍H��M���������ǑS���g���ĂȂ��B�P����BezieNew���Ă�ł邾���B
        
        if (/*Vector3.Distance(target.transform.position, this.transform.position) > 30*/(bezieT/xBox) < 1.0f)
        {
            
            timer += Time.deltaTime;
            //�i�s�̒x�������B�e���Ƃ̌��ˍ����ŕK�v��������Ȃ��B
            //���͎g���ĂȂ��B
            if (timer > 2f)
            {
                timer = 0;
                //BezieController();
                //Constructer();

            }
            //BezieController();
            BezieControllerNew();
           
        }
        else
        {

            BezieControllerNew();
            //�^�[�Q�b�g�̍Đݒ�
            if(reTarget == false)
            {
                //
                //Debug.Log("aaaaaaaaaa");
                //ResetTarget(homFlag);
                //Constructer();
            }
           
            if (homFlag == true)
            {
                //beziePos = new Vector3(target.transform.position.x + 10, target.transform.position.y, target.transform.position.z);
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position + new Vector3(5,0,0) - this.transform.position), 30.0f * Time.deltaTime);
                //transform.LookAt(target.transform.position);

                //BezieController();
                //BezieControllerNew();
                /*Vector3 s = target.transform.position - this.transform.position;
                Quaternion q = Quaternion.LookRotation(s);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.3f);*/
            }
            else
            {
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(beziePos - this.transform.position), 20.0f * Time.deltaTime);
                //BezieController();
            }
        }

        //�e�̑O�i�����B�������x�őO�ɐi�܂��Ă邾���B
        //�x�W�F�Ȑ���̍��W�ɑ΂�Lookat�����Ă���ׁA�Ȑ���`���Ĕ��ł���B
        //transform.position += transform.forward * fishSpeed * Time.deltaTime;

        //transform.position += transform.forward * fishSpeed;
     }




    //�킟�A�������O�̊֐������B
    //���Ԑ����g���Ă܂��B�ǉ�����Ă���ϐ��͉񐔂ƒǔ����邩�ۂ��B
    //���ꂼ��W02�Ŏw��B�{�̂͂��̊֐��̍Ō�ŃR�[���B
    public void Constructer(GameObject player, int a, bool b,Vector3[] array,int randomDir)
    {
        //�ǔ����邩�ǂ����B
        homFlag = b;

        //Lineposition
        linePosition = array;

        dir = randomDir;

        //1��ڂ͒��i�A2��ڂ͍�������A3��ڂ͉E�������ɋȂ���B
        if (a == 0)
        {
            bezieMiddle = new Vector3(0, 0, 0);
        }
        else if (a == 1)
        {
            bezieMiddle = new Vector3(0, 20, 20);
        }
        else if (a == 2)
        {
            bezieMiddle = new Vector3(0, -20, -20);
        }

        if(homFlag == false)
        {
            bezieMiddle = new Vector3(0, Random.Range(-10, 10), Random.Range(-10, 10));
        }

        //�{�̕����̃R�[���B
        Constructer(player);
    }


    //�������֐��B�R�[�������͓̂��X�N���v�gConstructer�֐����B
    //W02�ŌĂ�ł���͓̂����̕ʂ̊֐��B�ڂ����͕ʂ̕��ŋL�ځB
    //��{�����͂����B
    void Constructer(GameObject player)
    {

        //W02����n���ꂽplayer��target�Ƃ��Ďw��B
        target = player;

        //���x�v�Z�B���͎d�l������B
        fishSpeed = 15 + (ferocity / 2);
        if (fishSpeed > 23)
            fishSpeed = 23;

        //�x�W�F�̒��ԓ_�w��B�����ʊ֐��ōs���Ă���׃R�����g�A�E�g�B
        //bezieMiddle = new Vector3(0, 30,0);

        //�x�W�F�Ȑ��p�̍��W�w��B

        //�J�n�n�_
        bezieStart = transform.position;

        //�I���n�_
        //���x�ɍ��킹���΍��Z�o
        //��肭�����Ȃ��̂łƂ肠�����g���ĂȂ��B
        //float x = 100f/*Vector3.Distance(this.transform.position, target.transform.position)*/;
        //x = x / fishSpeed * 15;


        //�������p true�͖{��
        float y, z;
        if (homFlag == true)
        {
            //�I���n�_���Y�����������e�����ۂ������サ���̂ŁA����p�ɑg�ށB
            y = Random.Range(-1, 1);
            z = Random.Range(-1, 1);
        }
        //�����e�By,z���ʂɃ����_���Ȓl�����A�ڕW���W�̃Y����\������B
        //�e�����ŌŒ�BW02�ŕ��������肵�Ă���B
        else
        {
            if (dir == 0)
            {
                y = Random.Range(-20, 20);
                z = 0;
            }
            else
            {
                y = 0;
                z = Random.Range(-20, 20);
            }
        }




        //�΍����݂̏I���n�_�Bx�����Z�����̂�player�̑O�i���������Ă̕��B
        Vector3 aaa /*= new Vector3(target.transform.position.x + 100, target.transform.position.y + y, target.transform.position.z + z)*/;

        //CenterLine�̈ړ����l�������΍��␳
        //aaa = aaa + ReviseBezieEnd();



        //aaa = new Vector3(aaa.x + 7, aaa.y + y, aaa.z + z);




        //���Ԓn�_ Lerp�ɂ���āA�n�_�ƏI�_�����񂾐���5���n�_���Z�o�A����ɑ傫���O�ꂽ���ԓ_�����Z�B
        //���ʋȂ���l�ɂȂ�B�ڂ����̓x�W�F�Ȑ��̎d�g�݂𒲂ׂ�B


        Vector3 t1 = new Vector3(target.transform.position.x, 0, 0);
        Vector3 t2 = new Vector3(transform.position.x, 0, 0);

        if (Vector3.Distance(t1, t2) > 50)
        {
            aaa = PredictionPosition();
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;
            crossFlag = false;


            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);

            transform.LookAt(Vector3.Lerp(a, b, 0.5f));
        }
        else
        {
            //+fishSpeed = fishSpeed * 2;
            aaa = PredictionPosition();
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + new Vector3(10,0,0);
            crossFlag = true;
            transform.LookAt(bezieEnd);
           
        }

        //Debug.Log(crossFlag + "/" + (t1 - t2).magnitude);

        //�i�s�x�̃��Z�b�g�B
        bezieT = 0;
    }



    //public void Constructer()
    //{
    //    //�J�n�n�_
    //    bezieStart = transform.position;

    //    //�I���n�_
    //    //���x�ɍ��킹���΍��Z�o
    //    //
    //    Vector3 q = new Vector3(transform.position.x, 0, 0);
    //    Vector3 e = new Vector3(target.transform.position.x, 0, 0);

    //    float x = Vector3.Distance(q, e);
    //    x = x / fishSpeed * 15;

    //    //�I���n�_���Y�����������e�����ۂ������サ���̂ŁA����p�ɑg�ށB
    //    float y = Random.Range(-3, 3);
    //    float z = Random.Range(-3, 3);

    //    //�΍����݂̏I���n�_�Bx�����Z�����̂�player�̑O�i���������Ă̕��B
    //    Vector3 aaa = new Vector3(target.transform.position.x + 10, target.transform.position.y + y, target.transform.position.z + z);

    //    //CenterLine�̈ړ����l�������΍��␳
    //    aaa = aaa + ReviseBezieEnd();
    //    //aaa = aaaaa();

    //    bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);

    //    //���Ԓn�_ Lerp�ɂ���āA�n�_�ƏI�_�����񂾐���5���n�_���Z�o�A����ɑ傫���O�ꂽ���ԓ_�����Z�B
    //    //���ʋȂ���l�ɂȂ�B�ڂ����̓x�W�F�Ȑ��̎d�g�݂𒲂ׂ�B
    //    bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;

    //}


    //�x�W�F�Ȑ��̌v�Z�֐�
    //�V���v���ȏ����B
    void BezieController()
    {
        bezieT += 0.2f * Time.deltaTime;
        Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
        Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
        beziePos = Vector3.Lerp(a, b, bezieT);


        //���x���f�p�̏���
        //�x�W�F�Ȑ��ɂ����W���������ł͂Ȃ��A�ړ�������W�Ɍ����Ē��i�����鏈��
        //�e�ۂ̑��x�ω�����������ׂɕK�v�������B(�����ɂ͍��W�̒��ڏ��������ł��o����Ǝv���B�l����̂��ʓ|�������B)
        //transform.LookAt(beziePos);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(beziePos- this.transform.position),35.0f * Time.deltaTime);
        //Vector3 c = beziePos - this.transform.position;
        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(c), 1f * Time.deltaTime);


        if (crossFlag == false)
        {
            Vector3 s = beziePos - this.transform.position;
            Quaternion q = Quaternion.LookRotation(s);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.2f * Time.deltaTime);
            Debug.Log("������!");
        }
        else if (crossFlag == true)
        {
            Vector3 s = beziePos - this.transform.position;
            Quaternion q = Quaternion.LookRotation(s);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.015f);
            Debug.Log("�߂���!");
        }

    }

    //�^�[�Q�b�g�̍Đݒ�
    //Constructer�֐��Ɠ��������A���]�Ȑ܂����čŌ�Ɉ��Bezie���Ă�ł���B
    //transform.Lookat�������������BBeziePos�̌v�Z���֐����ׁ̈A�Ă񂾕����ǂ��Ɣ��f�B
    //����͊֌W�Ȃ��B
    void ResetTarget(Vector3 targetPos)
    {
        //Vector3 aaa = new Vector3(target.transform.position.x + 5, target.transform.position.y, target.transform.position.z);
        bezieStart = transform.position;
        bezieEnd = Vector3.Lerp(transform.position, targetPos, 1.0f);
        bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f);
        bezieT = 0;
        reTarget = true;
        BezieController();
    }

    //Constructer�֐��Ɠ��l�A�����ʊ֐��B
    //�z�[�~���O���A�����_�����ŏI���n�_�̍��W���Ⴄ�B
    //�����ς���ׂɗp�ӂ����B
    //�Ō��ResetTarget���R�[���B
    void ResetTarget(bool a)
    {
        Vector3 aaa;
        //�z�[�~���O����ꍇ
        if (a == true)
        {
            float d = 100f;
            d = d / fishSpeed * 15;

            aaa = new Vector3(target.transform.position.x + d, target.transform.position.y, target.transform.position.z);
        }
        //���Ȃ��ꍇ�Bplayer�̕t�߂��烉���_���ɐݒ�B
        else
            aaa = new Vector3(target.transform.position.x + 10, target.transform.position.y + Random.Range(-5, 5), target.transform.position.z + Random.Range(-5, 5));
        //�Z�o�������W��ResetTarget�ɓn���Ă���B
        ResetTarget(aaa);

    }

    //CenterLine�̏��
    //x���W���猻�݂̋�Ԃ��擾�A�����W���玟�̍��W�܂ł̌X�����擾�A�␳�Ɏg�p�B
    //�X���ɑ΂��đ��x���|����Β�������H
    //�␳��̍��W��Random.range�Ń����_������ǉ��H

    //X���W�ɑ΂��ČX���͈��B�Đݒ莞�̍��W���猻�݈ʒu�ɂ�����X�����Z�o����K�v������B
    //�e�e�ŎZ�o����̂̓X�}�[�g����Ȃ��B���A�e�e�ŎZ�o����̂���ԑ����Ƃ����b������B
    //Find�͒x���炵���B�I�u�W�F�N�g�Q�Ƃ���W02�œn���������B
    //����n���H�z��H�I�u�W�F�N�g�f�[�^�H
    //W02��LinePosition�ɑ΂���Q�Ƃ�p�ӂ�������ǂ��̂ł́H
    //LinePosition�̎Q�Ƃ��������ɓn�������ōs���܂��傤�B

    void SetLinepos()
    {
        //position�̎Q��
        //LinePos.x��pos.x�𒴂���܂ŁB
        //�������u�Ԃ�count���X���v�Z�Ɏg�p�B
        //count��count + 1�B

        int count = 0;

        while (true)
        {
            if (this.transform.position.x > linePosition[count].x)
            {
                count++;
            }
            else
            {
                lineCount = count;
                break;
            }
        }

    }


    //Revise = �␳
    Vector3 ReviseBezieEnd()
    {
        SetLinepos();

        Vector3 a = new Vector3(0, linePosition[lineCount].y, linePosition[lineCount].z);
        Vector3 b = new Vector3(0, linePosition[lineCount + 1].y, linePosition[lineCount + 1].z);

        Vector3 c = b - a;
        c = c.normalized;

        //c = �X��
        //����ɒ��e�܂ł̎��ԂŐi�ދ������|����B
        //���e�܂ł̎��� = x�̍� / ���x�B

        //���x�ɍ��킹���΍��Z�o
        //
        Vector3 q = new Vector3(transform.position.x, 0, 0);
        Vector3 e = new Vector3(target.transform.position.x, 0, 0);

        float x = Vector3.Distance(q, e);
        x = Mathf.Abs(x);
        x = x / fishSpeed * 15;

        //x = ���e�܂ł̎���
        //x * fishSpeed * time.deltatime

        //c = c * (x * fishSpeed * Time.deltaTime);


        return c;
    }

    //Prediction = �\��
    //�v���C���[�̖����ʒu���Z�o����֐��B
    Vector3 PredictionPosition()
    {
      

        //Vector3 n = Vector3.Lerp(linePosition[count], linePosition[count + 1], linePosition[count + 1].x / this.transform.position.x);

        Vector3 q = new Vector3(transform.position.x, transform.position.y,transform.position.z);
        Vector3 e = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);

        float x = Vector3.Distance(q, e);
        x = Mathf.Abs(x);

        //�Œ�l���g���܂��Bx = �|���鎞��
        x = 4f/*x / ((fishSpeed + 15) * Time.deltaTime)*/;//�����܂łɊ|���鎞��
        x = x - ( (100.0f - distanceHP) / 25.0f) *0.25f;

        //Debug.Log(x);
        //Debug.Log(distanceHP);

        xBox = x;


       


        float r = target.transform.position.x + x * 15 /** Time.deltaTime*/;//��������player��x���W
        //Debug.Log(r);

        //centerLine��̈ʒu�������B
        //[count].x < r < [count + 1]�ɂȂ�悤��count���Z�o�B
        int count = GetLinePosCount(r);
        //���ݒn�̊������Z�o
        //�S�̂̍��ɑ΂��A���݂ǂ̒��x�̈ʒu�ɂ���̂������̔�r�ŎZ�o
        float u = linePosition[count + 1].x - linePosition[count].x;
        float y = linePosition[count + 1].x - r;
        Vector3 n = Vector3.Lerp(linePosition[count + 1],linePosition[count], y/u);//lerp,���W�̎Z�o CenterLine��̖ڕW�ʒu

        //n�ɑ΂���player��x���W���瓱��CenterLine�ォ��̃Y�������Z����Ε␳����������B

        //player.x�����CenterLine.x
        //centerLine����player���ǂꂾ������Ă��邩�̎Z�o = SA
        // n �� SA�𑫂��Α��얳���ł̒��e�ʒu���Z�o�ł�����Ď��H�H�H�H�H


        //////////////player�̍��W��///////////////

        int k = GetLinePosCount(target.transform.position.x);
        //���ݒn�̊������Z�o
        //�S�̂̍��ɑ΂��A���݂ǂ̒��x�̈ʒu�ɂ���̂������̔�r�ŎZ�o
        float g1 = linePosition[k + 1].x - linePosition[k].x;
        float g2= linePosition[k + 1].x - target.transform.position.x;
        Vector3 f = Vector3.Lerp(linePosition[k + 1], linePosition[k], g2 / g1);//lerp,���W�̎Z�o CenterLine���player�̈ʒu

        //Debug.Log(f);

        Vector3 offset = target.transform.position - f;//player�̑���ɂ���āAcenterline���猻���W���ǂꂾ���Y���Ă��邩

        n = n + offset;//�Y������ڕW�ʒu�ɑ����B



        //�Z�o���@�̕ύX

        Vector3 l = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 h = n;

        float j = Vector3.Distance(l, h);

        float missileSpeed = j / x;

        fishSpeed = missileSpeed;


        return n;

    }

    //�߂����̂��
    //���Ԏw�肾�Ɩ���������P���ɑ��x�Ŕ�΂����߂̂��
    Vector3 PredictionPosition(float speed)
    {


        //Vector3 n = Vector3.Lerp(linePosition[count], linePosition[count + 1], linePosition[count + 1].x / this.transform.position.x);

        Vector3 q = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        Vector3 e = new Vector3(target.transform.position.x,target.transform.position.y,target.transform.position.z);

        float x = Vector3.Distance(q, e);
        x = Mathf.Abs(x);

        //�Œ�l���g���܂���Bx =
        x = x / (15 * Time.deltaTime);//�����܂łɊ|���鎞��(
        //x = x - ((100.0f - distanceHP) / 25.0f) * 0.25f;

        //Debug.Log(x);
        //Debug.Log(distanceHP);

        //xBox = x;





        float r = target.transform.position.x + x * 15 * Time.deltaTime;//��������player��x���W
        //Debug.Log(r);

        //centerLine��̈ʒu�������B
        //[count].x < r < [count + 1]�ɂȂ�悤��count���Z�o�B
        int count = GetLinePosCount(r);
        //���ݒn�̊������Z�o
        //�S�̂̍��ɑ΂��A���݂ǂ̒��x�̈ʒu�ɂ���̂������̔�r�ŎZ�o
        float u = linePosition[count + 1].x - linePosition[count].x;
        float y = linePosition[count + 1].x - r;
        Vector3 n = Vector3.Lerp(linePosition[count + 1], linePosition[count], y / u);//lerp,���W�̎Z�o CenterLine��̖ڕW�ʒu

        //n�ɑ΂���player��x���W���瓱��CenterLine�ォ��̃Y�������Z����Ε␳����������B

        //player.x�����CenterLine.x
        //centerLine����player���ǂꂾ������Ă��邩�̎Z�o = SA
        // n �� SA�𑫂��Α��얳���ł̒��e�ʒu���Z�o�ł�����Ď��H�H�H�H�H


        //////////////player�̍��W��///////////////

        int k = GetLinePosCount(target.transform.position.x);
        //���ݒn�̊������Z�o
        //�S�̂̍��ɑ΂��A���݂ǂ̒��x�̈ʒu�ɂ���̂������̔�r�ŎZ�o
        float g1 = linePosition[k + 1].x - linePosition[k].x;
        float g2 = linePosition[k + 1].x - target.transform.position.x;
        Vector3 f = Vector3.Lerp(linePosition[k + 1], linePosition[k], g2 / g1);//lerp,���W�̎Z�o CenterLine���player�̈ʒu

        //Debug.Log(f);

        Vector3 offset = target.transform.position - f;//player�̑���ɂ���āAcenterline���猻���W���ǂꂾ���Y���Ă��邩

        n = n + offset;//�Y������ڕW�ʒu�ɑ����B



        return n;

    }



    //CenterLine�ɒu����ʒu���擾����B
    //�^����ꂽx���W�ɑ΂��A�v�f�ԍ�a < x < �v�f�ԍ�a+1�ƂȂ�a��Ԃ��B
    int GetLinePosCount(float xPos)
    {
        int count = 0;
        for (int i = 0; i < linePosition.Length; i++)
        {
            if (xPos > linePosition[i].x)
            {
                count = i;
            }
            else
                break;

        }

        return count;

    }


    //�x�W�F�Ȑ��̐i�s�Ǘ��p�֐��B
    //�V�d�l�B���B���Ԃ����肷��悤�ɂȂ������߁A���W�ړ����x�W�F�ōs����悤�ɂȂ����B

    void BezieControllerNew()
    {
        Vector3 a; 
        Vector3 b;

        if (crossFlag == false)
        {
            bezieT += 1.0f * Time.deltaTime;
            a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            transform.position = Vector3.Lerp(a, b, bezieT / xBox);


            if (bezieT < 3.0f)
            {
                //Vector3 s = target.transform.position - this.transform.position;
                Vector3 s = Vector3.Lerp(a, b, bezieT + Time.deltaTime / xBox)/*target.transform.position*/ - this.transform.position;
                Quaternion q = Quaternion.LookRotation(s);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.05f);
                Debug.Log("������!");
            }
            else
            {
                Vector3 s = target.transform.position - this.transform.position;
                //Vector3 s = Vector3.Lerp(a, b, bezieT + Time.deltaTime / xBox)/*target.transform.position*/ - this.transform.position;

                Quaternion q = Quaternion.LookRotation(s);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.05f);
                Debug.Log("�߂���!");
            }



        }
        else
        {
            //bezieT = 3.1f;
            bezieT += 1.0f * Time.deltaTime;
            a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            transform.position = Vector3.Lerp(a, b, bezieT/2);

            //transform.position += transform.forward * 15.0f * Time.deltaTime;

            Vector3 s = target.transform.position - this.transform.position;
            //Vector3 s = Vector3.Lerp(a, b, bezieT + Time.deltaTime / xBox)/*target.transform.position - this.transform.position;*/

            Quaternion q = Quaternion.LookRotation(s);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q, 0.05f);
        }


        //���x���f�p�̏���
        //�x�W�F�Ȑ��ɂ����W���������ł͂Ȃ��A�ړ�������W�Ɍ����Ē��i�����鏈��
        //�e�ۂ̑��x�ω�����������ׂɕK�v�������B(�����ɂ͍��W�̒��ڏ��������ł��o����Ǝv���B�l����̂��ʓ|�������B)
        //transform.LookAt(beziePos);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(beziePos- this.transform.position),35.0f * Time.deltaTime);
        //Vector3 c = beziePos - this.transform.position;
        //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(c), 1f * Time.deltaTime);

        

    }

}
