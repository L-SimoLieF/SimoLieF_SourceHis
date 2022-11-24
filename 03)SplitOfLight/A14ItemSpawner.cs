using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A14ItemSpawner : MonoBehaviour
{
    bool itemEnable;//使ってない。
    [SerializeField] GameObject itemPrehab; //ItemのPrehab。
    GameObject item;//Itemのオブジェクト。花と1-1で対応する。
    int countTime;
    int secTime;


    // Start is called before the first frame update
    void Start()
    {
        item = Instantiate(itemPrehab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        item.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //花が咲いているかどうか。
        if (gameObject.activeSelf == true)
        {
            countTime++;

            if (countTime > 59)
            {
                countTime = 0;
                secTime++;
            }

            //10秒経過かつ、蜜玉が存在しない場合
            if (secTime > 9 && item.activeSelf == false)
            {

                //アイテム生成の関数と、経過時間を0に。確率は1/2
                RandomSpawn();
                secTime = 0;

            }

        }


    }

    void RandomSpawn()
    {
        int ans = Random.Range(0, 2);
        if (ans == 0)
        {

            //itemEnableは必要ない。
            //アイテムを「見せるか」「見せないか」で出現を管理。
            itemEnable = true;
            item.SetActive(true);
        }

    }
}
