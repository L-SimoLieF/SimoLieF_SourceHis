using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColiderMG : MonoBehaviour
{
    public bool contFlag;
    public bool endFlag;

    public float waitTimer01;
    public float waitTimer02;

    public Button btn;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(contFlag == true)
        {
            waitTimer01 += Time.deltaTime;
        }
        if(endFlag == true)
        {
            waitTimer02 += Time.deltaTime;
        }

        if(waitTimer01 > 5.0f)
        {
            Debug.Log("continue");
            btn.onClick.Invoke();
            contFlag = false;
            ChangeChoices();
            
        }
        if (waitTimer02 > 5.0f)
        {
            Debug.Log("end");
        }
    }

    public void ChangeChoices()
    { 
        waitTimer01 = 0f;
        waitTimer02 = 0f;

    }
}
