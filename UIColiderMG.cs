using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIColiderMG : MonoBehaviour
{
    public bool contFlag;
    public bool endFlag;

    public float waitTimer01;
    public float waitTimer02;

    public Button btn;

    public C01GOMamager C01; //C01 アタッチしろ


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (contFlag == true)
        {
            waitTimer01 += Time.deltaTime; 
            Debug.Log("continue");
            btn.onClick.Invoke();
            contFlag = false;
            ChangeChoices();
        }
        if (endFlag == true)
        {
            waitTimer02 += Time.deltaTime;
            SceneManager.LoadScene("ResultScene");
            Debug.Log("end");
        }

        /*if (waitTimer01 > 5.0f)
        {
            Debug.Log("continue");
            btn.onClick.Invoke();
            contFlag = false;
            ChangeChoices();

        }
        if (waitTimer02 > 5.0f)
        {
            SceneManager.LoadScene("ResultScene");
            Debug.Log("end");
        }*/

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("continue");
            btn.onClick.Invoke();
            contFlag = false;
            ChangeChoices();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("ResultScene");
        }
    }

    public void ChangeChoices()
    {
        waitTimer01 = 0f;
        waitTimer02 = 0f;

    }
}