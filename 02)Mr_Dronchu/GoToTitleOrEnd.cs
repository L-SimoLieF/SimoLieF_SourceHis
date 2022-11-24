//キーボード用隠しコマンドと、「もう一度」「戻る」の画像処理を追加しました。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Scene関係の処理に必要なライブラリ
using UnityEngine.SceneManagement;

public class GoToTitleOrEnd : MonoBehaviour
{
    public RectTransform arrow;

    public RectTransform title;
    public RectTransform end;
    [SerializeField] GameObject T;
    [SerializeField] GameObject E;
    void Start()
    {
        Transform arrowTransform = arrow.transform;
        Transform titleTransform = title.transform;

        Vector3 aPos = arrowTransform.position;
        Vector3 tPos = titleTransform.position;

        aPos.x = tPos.x - 5;
        arrowTransform.position = aPos;
    }
    void Update()
    {
        float lsh = Input.GetAxis("L_Stick_H");//Horizontal

        Transform arrowTransform = arrow.transform;
        Transform titleTransform = title.transform;
        Transform endTransform = end.transform;

        Vector3 aPos = arrowTransform.position;
        Vector3 tPos = titleTransform.position;
        Vector3 ePos = endTransform.position;

        //コントローラー入力
        if (lsh == 0)
        {
            aPos.x += 0.0f;
        }
        else if (lsh == -1)
        {
            T.SetActive(true);
            E.SetActive(false);
            aPos.x = tPos.x - 5;
        }
        else if (lsh == 1)
        {
            T.SetActive(false);
            E.SetActive(true);
            aPos.x = ePos.x - 5;
        }
        //矢印の位置
        arrowTransform.position = aPos;


        //キーボード用隠しコマンド
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Title");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
        }


        if (aPos.x == tPos.x - 5)
        {
            //Titleに戻る
            if (Input.GetKeyDown("joystick button 0"))
            {
                SceneManager.LoadScene("Title");
            }
        }
        else if(aPos.x == ePos.x - 5)
        {
            //ゲーム終了
            if (Input.GetKeyDown("joystick button 0"))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
                //Application.Quit();
            }
        }
    }
}
