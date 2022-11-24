//何も変わってないし、誰も弄ってない。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;//Text操作用。


//警戒ゲージを操作、管理する為だけのスクリプト。
//現在は警戒度のTextUIにアタッチしてますが、別に何処でも問題ありません。
//アタッチ先を変更する場合は、参照先スクリプトのGetComponentに問題が生じるので分からなきゃ聞いてください。

 //警戒ゲージの説明。
 //(仮置き)
 //加算は1フレーム毎。警戒度と別に警戒レベルを追加し、1-5で管理。5でプレイヤーを誘導する様に。
 //各レベルは100で区切られてます。(レベル1:0-99,レベル2:100-199...レベル5:400)
 //まきびしは+50。変化の使用時に加算は行わない。(視線判定スクリプト参照(EnemyVision)
 //隠れ身はその時点から半分。(現在は警戒度MAXのみ使用可能という制限が存在しない為。)
 //変化と同様に加算は行わない。(方法も変化と同じ)

 //・メンバの説明。
 //attentionGage:警戒ゲージ。0-400で遷移。各スクリプトから参照されるが、イキってアクセッサ(Getter,Setterって奴。プロパティとも言うかも)使いました。
 //"A"ttentionGage:警戒ゲージのアクセッサ。privateな変数を外部から参照する為の奴。機密保持が云々。使う時はこっち呼んでください。
 //Text 警戒度とか書いてあるUIを呼ぶ奴。数字の書き換えに使用。(usingでUnityEngine.UI呼ばないと使えないので注意。)
 //HengeChecker 変化の確認。後述のStopGageで呼ばれる。外部から参照する為にpublic。(暇だったら警戒ゲージみたいにするかも)
 //Stop,ReduceGage それぞれ変化と隠れ身の時に呼ぶ関数。そんな難しい事書いてないけど、強いて言えば停止と半減。

public class EnemyScript : MonoBehaviour
{
    //警戒ゲージの上昇をさせない仕様。(変化)
    //半分(隠れ身)
    int attentionGage;
    public Text text;

    public bool HengeChecker = false;

    //イキりアクセッサ。授業で習った事を使う優等生の鑑。
    public int AttentionGage
    {
        set
        {
            attentionGage = value;
        }
        get
        {
            return attentionGage;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        attentionGage = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "警戒度:" + attentionGage.ToString() + "  警戒レベル:" + (attentionGage / 100 + 1).ToString();
    }

    //変化用関数。それだけ。
    public void StopGage()
    {
        if (HengeChecker == false)
        {
            HengeChecker = true;
        }
        else
        {
            HengeChecker = false;
        }
    }

    //隠れ身用関数。それだけ。
    public void ReduceGage()
    {
        attentionGage /= 2;
    }


}
