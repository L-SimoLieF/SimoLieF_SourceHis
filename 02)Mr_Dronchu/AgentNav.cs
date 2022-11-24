

//多分、人が触った所は弄ってないと思う。

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

using UnityEngine.SceneManagement;

public class AgentNav : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] GameObject Player;//プレイヤーオブジェクトをUnity側でアタッチしてください。
    public Transform[] points;         //巡回場所です。Inspecter内のSizeで巡回場所の数を設定後、Elementでオブジェクト(Empty?)をアタッチしてください。
    private int destPoint = 0;
    private bool mF; //敗北宣言。誘引処理に関わる物。
    //追加しました。/
    GameObject AtGage;
    EnemyScript ES;

    //追加したよ
    public bool idle = false;
    public bool run = false;
    public bool kyoro = false;


    // Start is called before the first frame update
    void Start()
    {


        agent = GetComponent<NavMeshAgent>();
        //autoBrakingは目標地点の近くで減速するかどうか。
        agent.autoBraking = false;

        //追加しました。
        AtGage = GameObject.Find("AttentionText");
        ES = AtGage.GetComponent<EnemyScript>();


        UpdateDestination();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 a = agent.destination - transform.position;
        if (mF == false)
        {
            if (Vector3.Dot(transform.forward, a / a.magnitude) < 0.5f)
            {

                agent.speed = 0;
                Quaternion targetRotation = Quaternion.LookRotation(a);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / 1.5f);
                idle = true;   //追加したよ

            }
            else
            {
                agent.speed = 5f;
                if (ES.AttentionGage / 100 < 4)//if(enemy.keikai.>5){}
                {
                    //Quaternion q = Quaternion.FromToRotation(transform.forward,points[destPoint].transform.forward);
                    //transform.rotation = Quaternion.FromToRotation(transform.forward, agent.destination);
                    //transform.Rotate(q.eulerAngles);
                    if (!agent.pathPending && agent.remainingDistance < 0.5f)
                    {
                        //agent.velocity = Vector3.zero;
                        agent.isStopped = true;
                        agent.updatePosition = false;
                        agent.updateRotation = false;



                        //Debug.Log("aaaaa");
                        UpdateDestination();

                        //Quaternion targetRotation = Quaternion.LookRotation(transform.position - agent.destination);
                        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);



                        agent.isStopped = false;
                        agent.updateRotation = true;
                        agent.updatePosition = true;
                        //agent.speed = 5f;
                    }
                    run = false;   //追加したよ
                }
                else
                {
                    ChasingPlayer();
                    run = true;   //追加したよ
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, agent.destination) < 0.5f)
            {
                Debug.Log("www");
                StartCoroutine("ReturnNav");
                //agent.destination = points[destPoint].position;
                //destPoint = (destPoint + 1) % points.Length;
                mF = false;
            }
        }

        //AIの動きの調整

        // 位置の更新
        //transform.position = agent.nextPosition;
        //transform.position += transform.forward * 5 * Time.deltaTime;

    }

    void UpdateDestination()
    {

        // 地点がなにも設定されていないときに返します
        if (points.Length == 0)
            return;

        // エージェントが現在設定された目標地点に行くように設定します
        agent.destination = points[destPoint].position;

        // 配列内の次の位置を目標地点に設定し、
        // 必要ならば出発地点にもどります
        destPoint = (destPoint + 1) % points.Length;


        //Quaternion targetRotation = Quaternion.LookRotation(agent.destination - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);




        /*// 次の位置への方向を求める
        var dir = agent.destination - transform.position;

        // 方向と現在の前方との角度を計算（スムーズに回転するように係数を掛ける）
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        var angle = Mathf.Acos(Vector3.Dot(transform.forward, dir.normalized)) * Mathf.Rad2Deg * smooth;

        // 回転軸を計算
        var axis = Vector3.Cross(transform.forward, dir);


        if (angle > 120)
        {
            // 回転の更新
            var rot = Quaternion.AngleAxis(angle, axis);
            transform.forward = rot * transform.forward;
        }*/

        agent.isStopped = false;
    }

    void ChasingPlayer()
    {
        agent.destination = Player.transform.position;

    }

    //追加しました。@Shimomura
    //座標を受け取ってNavMeshAgentの目標地点を引数座標に指定する関数。
    //destPoint = 0 は、初期地点に戻る為の物。

    public void MakibishiAttract(Vector3 FallPoint)
    {
        //Debug.Log("b");
        agent.destination = FallPoint;
        destPoint = 0;
        mF = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && ES.AttentionGage / 100 > 3)
        {
            SceneManager.LoadScene("GameOver");
            /*#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
                  UnityEngine.Application.Quit();
            #endif*/
        }
    }

    IEnumerator ReturnNav()
    {
        idle = true;   //追加したよ
        kyoro = true;   //追加したよ
        agent.isStopped = true;
        Debug.Log("sto");
        yield return new WaitForSeconds(3);
        kyoro = false;   //追加したよ
        agent.isStopped = false;
        Debug.Log("sta");
        yield break;
    }
}
