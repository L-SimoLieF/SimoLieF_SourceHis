
//見たら消して良いよ---下村
//プレイヤーの操作を管理するスクリプト
//α版実装時点では、キーボードとコントローラで別のスクリプトを使用していたが、ファイナル版にて統合。
//その為現在、ContPlayerControllerに関しては全文がコメントアウトされています。
//追加したのは、各操作を受け取るif文条件の中です。
//コントローラ入力をorで取得しています。

//また、まきびしの投擲処理が正常に動作しなかったので、houdaiScriptに作った関数を呼ぶ形式に変更。
//→その際、本来キーボードの場合L→Kキーの順番で押す事で動作していたが、コントローラの仕様と同様にLキーの押下と解放でまきびしを投擲する様に変更してあります。
//また、隠れ身の仕様回数の減算処理の位置を変更してあります。
//→回数が残り一回の時に隠れ身を使うと、再度姿を現す時に入力が検知されなくなる(=使用回数0となる)為、減らすタイミングを「姿を現した時」に変更してある。
//しゃがみの当たり判定変化処理に関しては外してあります。
//視線判定がColliderを介したものではなく、正常に動作しない為。
//→しゃがみによる視線切りは実装されていません。無力な私を許してくれ
//また、しゃがみによるSpeed変化が+=,-=で記載されていましたが、コントローラ結合時に高速移動が可能になったので=5.0,=2.5に変更してあります。
//Speedを変える場合にコードの中身を書き換える必要があり、あまりやりたくない改造でしたが、高速移動バグの修正方法が思いつきませんでした。

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KeyPlayerContoller : MonoBehaviour
{
    //コントローラ対応の為に全てPublicにしました。(下村)
    //→12月26日追記　Publicにする必要無くなりましたが、変に戻して壊したら嫌なんでそのままにしてあります。本来はPublicじゃなくて大丈夫です。
    //directionをpublicにしました
    [SerializeField] public Vector3 direction;//移動方向
    [SerializeField] public float moveSpeed = 5.0f;//移動速度
    [SerializeField] public float applySpeed = 0.2f;//振り向く速度
    [SerializeField] public MainCamera mainCamera;//カメラのY軸を呼ぶための宣言
    [SerializeField] public int hengeKaisu = 2;//変化が出来る回数
    [SerializeField] public int kakuremiKaisu = 2;//隠れ身が出来る回数

    public bool makibishi = false;//まきびしのON、OFF,Lボタン
    public bool henge = false;//変化の術のON、OFF,Jボタン
    public bool kakuremi = false;//隠れ身の術ON、OFF、Hボタン

    //ここから↓

    GameObject defObject;//デフォルトスキン
    GameObject hengeObject;//変化後の武士スキン

    PlayerAnimationScript playerAnimation;
    public bool crouch = false;//しゃがみの状況を保存

    Animator animator;

    [SerializeField] public ParticleSystem particle;//パーティクルをセット

    public AudioClip ashioto;//足音
    AudioSource audioSource;

    //ここまで追加↑
    //Playerの子オブジェクトに侍を追加
    //▼Player
    //  　Houdai
    //  ▶chara_ninja
    //  ▶charater_samurai
    //こんな感じでお願いします



    BoxCollider boxCollider;//プレイヤーのボックスコライダー
    Vector3 tatiVector;//立ち状態のボックスコライダーのステータス
    Vector3 syagamiVector;//座り状態のボックスコライダーのステータス

    //追加しました。警戒ゲージ周りを操作する為の変数。
    GameObject AtGage;
    EnemyScript ES;

    GameObject GO;
    houdaiScript HS;

    [SerializeField] Text JutsuText;
    GameObject Jutsu;

    void Start()
    {
        //警戒ゲージ周りの処理を追加する為に必要。詳しく聞きたければ聞いてください。(いろんな所に書いてあります)
        AtGage = GameObject.Find("AttentionText");
        ES = AtGage.GetComponent<EnemyScript>();
        GO = GameObject.Find("Houdai");
        HS = GO.GetComponent<houdaiScript>();
        Jutsu = GameObject.Find("NinjutsuText");
        JutsuText = Jutsu.GetComponent<Text>();

        //ここから↓
        boxCollider = this.gameObject.GetComponent<BoxCollider>();
        syagamiVector = new Vector3(boxCollider.size.x, 0.7f, boxCollider.size.z);
        tatiVector = new Vector3(boxCollider.size.x, 1.7f, boxCollider.size.z);

        audioSource = GetComponent<AudioSource>();

        defObject = transform.GetChild(1).gameObject;
        hengeObject = transform.GetChild(2).gameObject;

        this.animator = GetComponent<Animator>();

        //ここまで追加↑
    }
    void Update()
    {
        float lrt = Input.GetAxis("L_R_Trigger");

        //しゃがむ時の減速

        if (!Input.GetKey(KeyCode.F) || moveSpeed == 2.5 && lrt < 1)
        {
            moveSpeed = 5.0f;
            //ここから↓
            //boxCollider.size = tatiVector;//コライダーを元の大きさに
            //boxCollider.center = new Vector3(0f, 0.2f, 0f);
            crouch = false;
            //ここまで追加↑
        }

        if (Input.GetKey(KeyCode.F)|| lrt == 1)
        {
            moveSpeed = 2.5f;
            //ここから↓
            //boxCollider.size = syagamiVector;//コライダーを小さく
            //boxCollider.center = new Vector3(0f, -0.2f, 0f);
            crouch = true;
            //ここまで追加↑
        }

        //まきびしの術 何も追加してません。まきびし関係は発射と弾頭の方に追加してます。
        if (Input.GetKeyDown(KeyCode.L)|| Input.GetKeyDown("joystick button 1"))//まきびしのON（押している間）
        {
            Debug.Log("まきびしの術on");
            makibishi = true;
            //audioSource.PlayOneShot(makibishiNage);
        }
        if (Input.GetKeyUp(KeyCode.L)||Input.GetKeyUp("joystick button 1"))//まきびしのOFF
        {
            HS.MakibishiThrow();
            Debug.Log("まきびしの術off");
            makibishi = false; 
        }

        //変化の術
        //追加しました、の部分を追加。
        //ES.StopGageを追加。これは次に呼ばれるまで、警戒ゲージの進行を停止させる関数。
        if(hengeKaisu>0)
        {
            if (Input.GetKeyDown(KeyCode.J)|| Input.GetKeyDown("joystick button 3"))//変化の術使用
            {


                henge = true;
                Debug.Log("変化の術on");

                //ここから↓

                animator.Play("NinzyutsuMotion");
                particle.Play();

                //defObject.SetActive(false);//忍者のスキンをoff
                //hengeObject.SetActive(true);//武士のスキンをon

                //ここ追加↑

                //追加しました。
                ES.StopGage();

                //指定された秒数後に変化をOFFにするメソッド
                StartCoroutine(hengeMethod(3.5f, () =>
                {
                    henge = false;

                    //ここから↓

                    defObject.SetActive(true);//忍者のスキンをoff
                    hengeObject.SetActive(false);//武士のスキンをon

                    //ここ追加↑

                    //追加しました。
                    ES.StopGage();

                    Debug.Log("変化の術off");
                }));
                hengeKaisu--;

                Debug.Log("変化回数");
                Debug.Log(hengeKaisu);
            }
        }

        //隠れ身の術 
        //追加しました、の部分を追加しました。
        //簡単に書くと、警戒ゲージ周りの処理を追加。ES(EnemyScript)で管理。
        //StopGageは次に呼ばれるまで警戒ゲージの進行を停止、Reduceは半分。
        if (kakuremiKaisu > 0)
        {
            if (Input.GetKeyDown(KeyCode.H)|| Input.GetKeyDown("joystick button 0"))//隠れ身の術使用、押すたびにモード切替
            {

                kakuremi = !(kakuremi);

                //ここから↓

                animator.Play("NinzyutsuMotion");
                particle.Play();

                //ここ追加↑

                if (kakuremi)
                {
                    Debug.Log("隠れ身の術on");
                    //kakuremiKaisu--;

                    //追加しました。
                    //警戒ゲージを半分にする奴です。
                    ES.StopGage();
                    ES.ReduceGage();
                    //ここから↓
                    defObject.SetActive(false);//忍者のスキンをoff
                     //ここから↓
                }
                else if(!kakuremi)
                {
                    //追加しました。
                    ES.StopGage();
                    //ここから↓
                    defObject.SetActive(true);//忍者のスキンをoff
                    //ここ追加↑
                    kakuremiKaisu--;

                    Debug.Log("隠れ身の術off");
                }
                
                Debug.Log("隠れ身回数");
                Debug.Log(kakuremiKaisu);
            }
        }


        //Playerの向きをキー入力
        direction = Vector3.zero;

        float lsh = Input.GetAxis("L_Stick_H");//Horizontal
        float lsv = Input.GetAxis("L_Stick_V");//Vertical

        if (Input.GetKey(KeyCode.W)|| lsv == -1)
            direction.z += 1;
        if (Input.GetKey(KeyCode.A)|| lsh == -1)
            direction.x -= 1;
        if (Input.GetKey(KeyCode.S)|| lsv == 1)
            direction.z -= 1;
        if (Input.GetKey(KeyCode.D)|| lsh == 1)
            direction.x += 1;
        direction = direction.normalized * moveSpeed * Time.deltaTime;

        //ここから↓
        if (!(direction == Vector3.zero))//足音追加しました
        {
            if(!(audioSource.isPlaying==ashioto))
            audioSource.PlayOneShot(ashioto);
        }
        //ここ追加↑

        if (direction.magnitude > 0)
        {
            //transform.rotationの更新
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mainCamera.yRotation * direction), applySpeed);
            //directionにマイナスが付いていた為Playerが反転していた。

            // transform.positionの更新
            if (!makibishi&&!kakuremi)//まきびしまたは隠れ身OFF
            {
                transform.position += mainCamera.yRotation * direction;
            }
        }
        JutsuText.text = "変化:" + hengeKaisu.ToString() + " 隠れ身:" + kakuremiKaisu.ToString();
    }

    //指定された秒数後に変化をOFFにするメソッド
    private IEnumerator hengeMethod(float waitTime,Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}