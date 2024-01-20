using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G02DropGem : MonoBehaviour
{
    float timer;
    Vector3 dir;

    bool getFlag;
    GameObject player;

    float lerpTimer;

    public GameObject backet;
    public GameObject gem2;

    Vector3 randomDir;

    Vector3 bezieStart;
    Vector3 bezieCenter;
    Vector3 bezieEnd;
    float bezieT;

    bool crossFlag;
    GameObject target;

    float xBox;
    float dropTimer;

    Vector3[] linePos;

    // Start is called before the first frame update
    void Start()
    {
        //dir = new Vector3(0f, 0f, Random.Range(-2, 2));

        //dir = player.transform.position - this.transform.position;

        
    }

    // Update is called once per frame
    void Update()
    {
        //timer += Time.deltaTime;

        if ((bezieT / xBox) <= 1.5f)
        {
            BezieController();
        }

        /*if (timer < 30.0f)
        {
            //transform.Rotate(new Vector3(8f, 15f, 10f));
            //transform.position += dir * Time.deltaTime;

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), 360.0f * Time.deltaTime);

            //transform.position =  * 4f * Time.deltaTime;

            //lerpTimer += Time.deltaTime * 0.01f;
            //transform.position = Vector3.Lerp(this.transform.position, dir, lerpTimer/5);
           



            if (/*Vector3.Distance(this.transform.position, dir) > 0.5f*//*(bezieT/xBox) < 1.0f){

                //BezieController();

                /*transform.LookAt(dir);
                this.transform.position += transform.forward * 5f * Time.deltaTime;

            }



            

           
        }*/

        if (timer > 50f)
            Destroy(this.gameObject);
      
        if(getFlag == true)
        {
            GameObject ins = Instantiate(gem2, backet.transform.position, Quaternion.identity);
            ins.transform.parent = backet.transform;
            Destroy(this.gameObject);

            //GameObject ins = Instantiate(this.gameObject,backet.transform.position,Quaternion.identity);
            //ins.transform.localScale = ins.transform.localScale / 2;
            //ins.transform.parent = backet.transform;
            //ins.GetComponent<G02DropGem>().enabled = false;
            //this.transform.position = backet.transform.position;/*new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 2)*/;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SeSystem.reer = true;
            SeSystem.GemGet = false;
            SeSystem.GemGet = true;
            //Destroy(this.gameObject);
            Debug.Log("get");
            getFlag = true;
            player = other.gameObject;
            //�X�R�A�p
            JewelScore.jewelCount += 1;
        }
    }

    //Prediciton = �\��
    public void PredictionPosition(Vector3[] linePosition)
    {
        target = GameObject.Find("Board");
        //linePos = new Vector3[linePosition.Length];
        //linePos = linePosition;
        //Vector3 n = Vector3.Lerp(linePosition[count], linePosition[count + 1], linePosition[count + 1].x / this.transform.position.x);
        //Vector3 q = new Vector3(transform.position.x, 0, 0);
        //Vector3 e = new Vector3(target.transform.position.x, 0, 0);

        float x /*= Vector3.Distance(q, e)*/;
        /*x = Mathf.Abs(x);
        x = x / (15 * Time.deltaTime);//�����܂łɊ|���鎞��*/


        Vector3 t1 = new Vector3(target.transform.position.x, 0, 0);
        Vector3 t2 = new Vector3(transform.position.x, 0, 0);


        if (Vector3.Distance(t1,t2) > 50.0f)
        {
            x = 6.0f;
        }
        else
            x = 2.0f;
        xBox = x;


        float r = target.transform.position.x + x * 15 * Time.deltaTime;//��������player��x���W

        r = r + 60.0f;

        //centerLine��̈ʒu�������B
        //[count].x < r < [count + 1]�ɂȂ�悤��count���Z�o�B
        int count = GetLinePosCount(r,linePosition);
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

        int k = GetLinePosCount(target.transform.position.x,linePosition);
        //���ݒn�̊������Z�o
        //�S�̂̍��ɑ΂��A���݂ǂ̒��x�̈ʒu�ɂ���̂������̔�r�ŎZ�o
        float g1 = linePosition[k + 1].x - linePosition[k].x;
        float g2 = linePosition[k + 1].x - target.transform.position.x;
        Vector3 f = Vector3.Lerp(linePosition[k + 1], linePosition[k], g2 / g1);//lerp,���W�̎Z�o CenterLine���player�̈ʒu

        //Debug.Log(f);

        Vector3 offset = target.transform.position - f;//player�̑���ɂ���āAcenterline���猻���W���ǂꂾ���Y���Ă��邩

        n = n + offset;//�Y������ڕW�ʒu�ɑ����B

        //transform.LookAt(n);

        dir = n;

        //randomDir = RandomShoot(dir);
        BezieSet(linePosition);

    }


    int GetLinePosCount(float xPos,Vector3[] linePosition)
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

    Vector3 RandomShoot(Vector3 v)
    {
        float a = Random.Range(0, 3);
        float b = Random.Range(-3, 3);
        float c = Random.Range(-5, 5);

        return new Vector3(v.x + a, v.y + b, v.z + c);
    }




    //bezieMiddle�̒� = �Ȑ��̑傫��
    //y,z = ���e�_�͈̔�

    void BezieSet(Vector3[] linePosition)
    {

        //�x�W�F�̒��ԓ_�w��B�����ʊ֐��ōs���Ă���׃R�����g�A�E�g�B
        Vector3 bezieMiddle = new Vector3(Random.Range(0,10), Random.Range(-3,3),Random.Range(-3,3));

        //�x�W�F�Ȑ��p�̍��W�w��B

        //�J�n�n�_
        bezieStart = transform.position;




        float y = Random.Range(-2, 2);
        float z = Random.Range(-5, 5);



        //CenterLine�̈ړ����l�������΍��␳
        //aaa = aaa + ReviseBezieEnd();
        Vector3 aaa = dir;






        Vector3 t1 = new Vector3(target.transform.position.x, 0, 0);
        Vector3 t2 = new Vector3(transform.position.x, 0, 0);

        if (Vector3.Distance(t1, t2) > 50)
        {
            aaa = dir;
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;
            crossFlag = false;


            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);

            //transform.LookAt(Vector3.Lerp(a, b, 0.5f));
        }
        else
        {
            //+fishSpeed = fishSpeed * 2;
            aaa =dir;//dir;
            aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);
            bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);
            bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f);
            crossFlag = true;
            //transform.LookAt(bezieEnd);

        }









        //aaa = new Vector3(aaa.x, aaa.y + y, aaa.z + z);


        //bezieEnd = Vector3.Lerp(transform.position, aaa, 1.0f);

        //���Ԓn�_ Lerp�ɂ���āA�n�_�ƏI�_�����񂾐���5���n�_���Z�o�A����ɑ傫���O�ꂽ���ԓ_�����Z�B
        //���ʋȂ���l�ɂȂ�B�ڂ����̓x�W�F�Ȑ��̎d�g�݂𒲂ׂ�B
        //bezieCenter = Vector3.Lerp(bezieStart, bezieEnd, 0.5f) + bezieMiddle;

        //�i�s�x�̃��Z�b�g�B
        bezieT = 0;

    }

    //bezieT = ��΂̑��x

    void BezieController()
    {

        if (crossFlag == false)
        {
            bezieT += 1.0f * Time.deltaTime;
            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            this.transform.position = Vector3.Lerp(a, b, bezieT/xBox);
        }

        if(crossFlag == true)
        {
            bezieT += 1.0f * Time.deltaTime;
            Vector3 a = Vector3.Lerp(bezieStart, bezieCenter, bezieT);
            Vector3 b = Vector3.Lerp(bezieCenter, bezieEnd, bezieT);
            this.transform.position = Vector3.Lerp(a, b, bezieT / xBox);


        }


    }
}
