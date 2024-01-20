
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//�Q�[���I�[�o�[���̈Ó]�A�y�эĊJ�p�X�N���v�g
public class UIGameOver : MonoBehaviour
{
    //�ė����̈Ó]�p

    public GameObject gameOverUI; //game over��Canvas
    public Image fadeImage;//�Ó]�p��Image
    public Text goText; //game over text.
    public GameObject conButton; //���g���C�p�̃{�^���B

    public bool flag;
    float lerpTime;//�Ó]�p
    float lerpRange = 4f;//�Ó]�p

    Vector3 endPosition; //�ė����̍��W �ĊJ�n�_�̌���Ɏg�p�B
    Vector3 restartPos;//�ĊJ�n�_

    public bool conflag;//�R���e�j���[�����p�̃t���O�B
    float conLerp = 3f; //�ĊJ���̖��]

    public GameObject whaleObject; //�N�W���B�ĊJ���Ɉʒu��ݒ肵�����̂ɕK�v�B

    public GameObject camObject; //MainCamera�̃I�u�W�F�N�g�B�ė����ɃJ���������Ƃ��ׂɕK�v�B
    Vector3 cameraOffset = new Vector3(0, 1.36144257f, 0); // �����J�����̍��W�̃Y���B
    public Rigidbody a; //�J������Rigidbody �ė�����̂Ɏg�p�B

    public GameObject XRrig; //�O�i���x��0�ɂ���ׂɕK�v�B

    Vector3 boardOffset = new Vector3(0, 1.36143994f, 0);

    //public GameObject uiColider;//UIColider Continue�����p �A�^�b�`����

    public W05WhaleTutorial W05;

    public Material fademat;

    float alphaPlus;

    public C01GOMamager C01; //�A�^�b�`����

    public EnergyCharge energyCharge;//�A�^�b�`

    ///public Material fadeMat;

  


    // Start is called before the first frame update
    void Start()
    {
        //fadeImage = gameOverUI.GetComponent<Image>();
        W05 = whaleObject.GetComponent<W05WhaleTutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        //�Q�[���I�[�o�[�B PlayerDamaged��OnTrigger��flag���Z�b�g�B
        if (flag == true)
        {
            //UI Canvas��false�̏ꍇ(����ōŏ��̃t���[��)
            //UI�I�u�W�F�N�g��\���B�I�����̍��W��ۑ��B
            if (gameOverUI.activeSelf == false)
            {
                gameOverUI.SetActive(true);
                gameOverUI.transform.GetChild(0).gameObject.SetActive(true);
                endPosition = this.transform.position;

            }

            //�Ó]�p��Timer�̉��Z�B
            //�Ó]����
            if (lerpTime < lerpRange)
            {
                //�����͂���shot�p
                energyCharge.IsEnd = true;
                lerpTime = lerpTime + Time.deltaTime;
                alphaPlus += 0.35f * Time.deltaTime;

                fademat.color = new Color(0, 0, 0, alphaPlus);

                if(this.transform.position.y < 20)
                {
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<UIGameOver>().a.useGravity = false;//a = mainCamera�B�J������ė������Ȃ��Ǝ��_�����Ȃ��ׁB

                    a.useGravity = false;


                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                    a.velocity = Vector3.zero;
                }


            }
            //���߂�ꂽ���Ԃ��o�߂����ꍇ(�Ó]�I��)
            //�e�L�X�g�ƃ{�^����\���B
            else
            {
                W05.startFlag = false;
                lerpTime = lerpRange;
                goText.enabled = true;
                conButton.SetActive(true);

                //alphaPlus += 50.0f * Time.deltaTime;

                //�Ó]�̏I���ɍ��킹�ė������~�߂�
                GetComponent<Rigidbody>().useGravity = false;
                a.useGravity = false;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                a.velocity = Vector3.zero;

                //
                //uiColider.SetActive(true);

                GetComponent<PlayerDamaged>().HP = 5;

                //flag = false;

                //�Q�[���I�[�o�[���[���ɓ]������ׂ̏����B�ȍ~��C01�B
                if (C01.fadeOutFlag == false)
                {
                    //FSShaderScript.SetBlendMode(fademat, FSShaderScript.Mode.Cutout);
                    C01.fadeOutFlag = true;

                }
            }

            //�t�F�[�h�A�E�g
            fadeImage.color = new Color(0, 0, 0, lerpTime / 3);
            if (C01.fadeOutFlag == false)
            {
                
            }
        }

        //�R���e�B�j���[����
        if (conflag == true)
        {

            //���] 0�ɂȂ����疾�]����
            if (lerpTime > 0)
                lerpTime = lerpTime - Time.deltaTime;
            //�Q�[���X�^�[�g
            //Canvas���I�t�A���x�����ʂ�A������̈ʒu���Đݒ�B
            else
            {
                lerpTime = 0;
                gameOverUI.SetActive(false);
                gameOverUI.transform.GetChild(0).gameObject.SetActive(false);

                XRrig.GetComponent<FollowLine>().speed = 15f;
                XRrig.GetComponent<FollowLine>().ResetLineOrder();

                conflag = false;
                //flag = false;
               


                //uiColider
                //uiColider.SetActive(false);
                W05.startFlag = true;

                //shot�񕜗p
                energyCharge.IsEnd = false;
            }
            //�t�F�[�h�C��
            fadeImage.color = new Color(0, 0, 0, lerpTime);
        }
    }


    //Button�ɃA�^�b�`�B
    //�J�n�ʒu�A���]�p�̃^�C�}�[�A�d�͂̉����ȂǁB
    public void Continue()
    {
        //mizunoSE
        SeSystem.GameOver = false;
        //�J�n�ʒu�̎w��
        //�I�����̃v���C���[��x���W���Q�ƁB
        if (endPosition.x < 1200)
            restartPos = new Vector3(0, 100, 50);
        else if (endPosition.x < 2400)
            restartPos = new Vector3(1200, 100, 50);
        else
            restartPos = new Vector3(2400, 100, 50);

        //���]�p�̎���
        lerpTime = 3f;
        //�������n�߂�ׂ̃t���O�B
        conflag = true;

        //�e�L�X�g�A�{�^���̍폜�B
        goText.enabled = false;
        conButton.SetActive(false);

        //HP�̍Đݒ�A�d�͂̉����B�����̍폜�ׂ̈�velociy��0�ɁB
        GetComponent<PlayerDamaged>().HP = 5;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //�J�����̕������l�B
        a.useGravity = false;
        a.velocity = Vector3.zero;

        //�Ó]�����̉���
        flag = false;


        //������̈ʒu
        whaleObject.transform.position = new Vector3(restartPos.x + 100, 100, 50);
        whaleObject.GetComponent<W01WhaleMoving>().ResetParam(W05.player);

        //�ĊJ�ʒu�B�����̏C���B
        XRrig.transform.position = restartPos;
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        this.gameObject.transform.position = restartPos + cameraOffset;

        //�J�����̈ʒu�����l�B
        camObject.transform.position = this.gameObject.transform.position + cameraOffset;
        camObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        //GameOverRoom����̃t���O�̃��Z�b�g�B
        C01.ResetParam();

       
    }



    
}
