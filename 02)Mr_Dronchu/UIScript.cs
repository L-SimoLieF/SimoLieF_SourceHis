//中身は記載していませんが、本プロジェクト実装に際して多少の改良を行っています。

//UI関係の処理を纏めたスクリプト。
//特に何か弄った訳じゃないが、HengeScreen1,2を配列にしたりなど、ちょっとした改造をしてる。
//警戒ゲージは0.8倍されていて、その理由も分かるんだけど、300で満タンになったので0.8倍の処理を取り除いています。
//→多分、ゲージ外(警の部分)も上書きされてるんだろうけど、見えないので問題ないです。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] GameObject[] HengeScreen = new GameObject[2];
    [SerializeField] GameObject[] KakuremiScreen = new GameObject[2];
    [SerializeField] private Image KeikaiGaugeR;
    float keikaiMax;

    GameObject Player;
    KeyPlayerContoller KPC;
    GameObject AtGage;
    EnemyScript ES;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        KPC = Player.GetComponent< KeyPlayerContoller>();
        AtGage = GameObject.Find("AttentionText");
        ES = AtGage.GetComponent<EnemyScript>();
        keikaiMax = 400;
        //警戒ゲージの幅は0.2~1までだから×0.8する

    }

    // Update is called once per frame
    void Update()
    {
        if (KPC.hengeKaisu == 1)
        {
            HengeScreen[1].SetActive(false);
        }
        if (KPC.hengeKaisu== 0)
        {
            HengeScreen[0].SetActive(false);
        }

        if (KPC.kakuremiKaisu== 1)
        {
            KakuremiScreen[1].SetActive(false);
        }
        if (KPC.kakuremiKaisu == 0)
        {
            KakuremiScreen[0].SetActive(false);
        }


        KeikaiGaugeR.fillAmount = ES.AttentionGage / keikaiMax;

    }
}
