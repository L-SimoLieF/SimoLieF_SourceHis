using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C01GOMamager : MonoBehaviour { 

    public bool fadeOutFlag;

    float timer;

    public GameObject player;//�A�^�b�`����
    public GameObject XRRig;//�A�^�b�`����
    public Material fadeMate;

    public bool teleportFlag;
    public float alphaminus;

    public GameObject missileParant;//�A�^�b�`����

    Vector3 warpPosition = new Vector3(-500, -503, -500);
    // Start is called before the first frame update
    void Start()
    {
        FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Fade);
        fadeMate.color = new Color(0, 0, 0, 0);
        alphaminus = 255;
    }

    // Update is called once per frame
    void Update()
    {

        //UIGameOver����t���O���Z�b�g
        //�Ó]�A�y�ї����̏I������X�^�[�g�B
        if(fadeOutFlag == true)
        {

            //���]���� Ending�p�̃~�T�C����������������ɔq�؂����B
            timer += Time.deltaTime;
            missileParant.SetActive(false);

            //�Ó]�p�̗\���ҋ@����
            //�e���|�[�g�̏u�Ԃ��f���Ȃ��ׂ̏���
            if(timer < 2.0f)
            {
                //alphaminus = 255;
                //�e���|�[�g
                player.transform.position = warpPosition;
            }

            //���]
            if(timer > 2.0f)
            {
                //XRRig���t���Ă��Ȃ��ƈړ������œ����Ȃ��Ȃ�
                if (teleportFlag == false)
                {
                    //alphaminus��0-1�͈̔́B
                    alphaminus = 1;
                    XRRig.transform.position = warpPosition;
                    player.transform.position = XRRig.transform.localPosition;//�J������e��0,0,0�ɍ��킹��
                    FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Fade);
                }
                teleportFlag = true;
                
            }
            //�e���|�[�g�I����
            //���]
            if(teleportFlag == true)
            {
                //0�ȉ���0�ɌŒ�
                if (alphaminus < 0)
                {
                    alphaminus = 0;
                    fadeMate.color = new Color(0, 0, 0, 0);

                }
                if (alphaminus > 0)
                {
                    alphaminus -= Time.deltaTime * 0.2f;
                    fadeMate.color = new Color(0, 0, 0, alphaminus);
                    //fadeMate.color = new Color(0, 0, 0, 0);
                }
                if(alphaminus == 0)
                {
                    //FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Cutout);
                    fadeMate.color = new Color(0, 0, 0, alphaminus);
                }

                //fadeMate.color = new Color(0, 0, 0, 0);
                
                
            }
        }
    }

    public void ResetParam()
    {
        timer = 0f;
        fadeOutFlag = false;
        alphaminus = 255;
        teleportFlag = false;
        FSShaderScript.SetBlendMode(fadeMate, FSShaderScript.Mode.Fade);
        missileParant.SetActive(true);
    }
}
