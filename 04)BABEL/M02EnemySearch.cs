using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


//M02EnemySearch
//���O�̒ʂ�A���G�p�̃X�N���v�g�ł��B
//�S�āA�������S�����܂����B
public class M02EnemySearch : NetworkBehaviour
{
    [SerializeField] GameObject director;
    [SerializeField] GameObject directorPrehab;
    public GameObject target;
    public bool SetDir;
    int time;
    Vector3 dir;

    M01GameManager M01;
    public GameObject MyPlayer;
    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        M01 = GameObject.Find("GameMG").GetComponent<M01GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
       

        //���������삵�Ă���L�����N�^�[���i�[�B
        //����ȊO�����e�𔭎˂����ۂɁA���G�@�\���I���ɂ���ׂ̏����B
        if (MyPlayer == null)
        {
            for (int i = 0; i < M01.user.Count; i++)
            {
                if (M01.user[i].player.GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                    MyPlayer = M01.user[i].player;
                }
            }
        }
        else
        {
            //director = MyPlayer.transform.GetChild(24).gameObject;
            //target = ���e�𔭎˂����v���C���[�I�u�W�F�N�g
            //���������삵�Ă���L�����N�^�[����Ȃ��ꍇ�Atarget�̕���������Corn��Instatiate����B
            //Corn�͎��Ԍo�߂ŏ��ŁB
            if (target != MyPlayer)
            {
                if (SetDir)
                {
                    if (time == 0)
                    {
                        //director.SetActive(true);
                        //MyPlayer.transform.GetChild(24).gameObject.SetActive(true);
                        direction = (target.transform.position - MyPlayer.transform.position).normalized;
                        Transform tsm = directorPrehab.transform;
                        tsm.position = MyPlayer.transform.position + direction;
                        tsm.LookAt(target.transform.position);
                        director = Instantiate(directorPrehab,tsm);
                        director.GetComponent<S01CornPosition>().Player = MyPlayer;
                        director.GetComponent<S01CornPosition>().direction = direction;
                        Destroy(director,3f);
                    }
                    //Quaternion rotation = Quaternion.LookRotation(target.transform.position, Vector3.up);
                    //director.transform.LookAt(target.transform.position);
                    //dir = (target.transform.position - this.gameObject.transform.position).normalized;
                    //director.transform.Rotate(dir);
                    //director.transform.rotation = rotation;
                    //director.transform.LookAt(target.transform);
                    //director.transform.position = MyPlayer.transform.position + direction;
                    time++;
                    if (time > 100)
                    {
                        SetDir = false;
                        //MyPlayer.transform.GetChild(24).gameObject.SetActive(false);
                        time = 0;
                    }

                }
            }
        }
    }
}

//���G�ɂ���
//���̃v���C���[�����e�����˂�������ʒB����K�v������B
//���̃v���C���[����A���g�̃v���C���[�I�u�W�F�N�g�ւ̏��̒ʒB
//�����ꂪ�ʓ|�BM02�Ƃ������ʂ̎󂯎M��p�ӂ��鎖�őΉ��BM02��Corn��Active��؂�ւ�����j�B
//M02�Ɏ�����LocalPlayer�̃I�u�W�F�N�g��ێ����Ă����āA���˖���M02�ɔ��ˎ҂̏��𑗐M����B
//���ˎ҂�Local�̃I�u�W�F�N�g�ƈقȂ����ꍇ�ɁA���G�@�\���N������B
