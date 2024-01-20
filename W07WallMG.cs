using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W07WallMG : MonoBehaviour
{
    public GameObject[] walls = new GameObject[4];
    W01WhaleMoving W01;
    float timer;
    bool gearFlag;//true = 加速 false = 減速 

    public GameObject player;//アタッチしろ 無限加速編対策 

    // Start is called before the first frame update
    void Start()
    {
        W01 = transform.root.gameObject.GetComponent<W01WhaleMoving>();
    }

    // Update is called once per frame
    void Update()
    {
       if(W01.normalSet == false)
        {
            timer += Time.deltaTime;
            if(timer > 8.0f)
            {
                if (gearFlag == true)
                    W01.speed = 7.5f;
                else
                    W01.speed = 30.5f;
                gearFlag = !gearFlag;
                timer = 0.0f;
            }

            if((this.transform.position.x - player.transform.position.x) > 120)
            {
                AccelStop();
            }

            
        }
        else
        {
            timer = 0.0f;
        }
    }

    public void wallActive(int distanceHP,float speed)
    {
        W01.normalSet = false;
        W01.speed = speed;

        if (speed > 10.0f)
        {
            gearFlag = true;
        }
        else
            gearFlag = false;

        switch (distanceHP)
        {
            case 100:
                transform.GetChild(4).gameObject.SetActive(true);
                break;
            case 75:
                transform.GetChild(3).gameObject.SetActive(true);
                break;
            case 50:
                transform.GetChild(2).gameObject.SetActive(true);
                break;
            case 25:
                transform.GetChild(1).gameObject.SetActive(true);
                break;
            case 0:
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void AccelStop()
    {
        W01.speed = 15.0f;
        W01.normalSet = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
    }
}
