//タイトル画面とかの奴。
//特に弄ってない。強いて言えば、キーボード用に隠しコマンドを追加したのと、LoadSceneの中身をSampleSceneにかえたくらい。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scene関係の処理に必要なライブラリ
using UnityEngine.SceneManagement;

public class Arrow : MonoBehaviour
{
    public RectTransform arrow;

    public RectTransform start;
    public RectTransform rule;

    [SerializeField] GameObject RuleScreen;
    void Start()
    {
        Transform arrowTransform = arrow.transform;
        Transform startTransform = start.transform;

        Vector3 aPos = arrowTransform.position;
        Vector3 sPos = startTransform.position;

        aPos.x = sPos.x - 5;
        arrowTransform.position = aPos;
    }
    void Update()
    {
        float lsh = Input.GetAxis("L_Stick_H");//Horizontal

        Transform arrowTransform = arrow.transform;
        Transform startTransform = start.transform;
        Transform ruleTransform = rule.transform;

        Vector3 aPos = arrowTransform.position;
        Vector3 sPos = startTransform.position;
        Vector3 rPos = ruleTransform.position;

        if (lsh == 0)
        {
            aPos.x += 0.0f;
        }
        else if (lsh == -1)
        {
            aPos.x = sPos.x-5;
        }
        else if(lsh == 1)
        {
            aPos.x = rPos.x-5;
        }
        //矢印の位置
        arrowTransform.position = aPos;

        //キーボード用隠しコマンド
        if (Input.GetKeyDown(KeyCode.S))
        {
            SceneManager.LoadScene("SampleScene");
            Debug.Log("Game Start");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            RuleScreen.SetActive(true);
            Debug.Log("表示");
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            RuleScreen.SetActive(false);
            Debug.Log("非表示");
        }


        if (aPos.x == sPos.x - 5)
        {
            //ゲームスタート
            if (Input.GetKeyDown("joystick button 0"))
            {
                SceneManager.LoadScene("SampleScene");
                Debug.Log("Game Start");
            }
        }
        else if (aPos.x == rPos.x - 5)
        {
            //表示
            if (Input.GetKeyDown("joystick button 0"))
            {
                RuleScreen.SetActive(true);
                Debug.Log("表示");
            }
            //非表示
            if (Input.GetKeyUp("joystick button 0"))
            {
                RuleScreen.SetActive(false);
                Debug.Log("非表示");
            }
        }
    }
}
