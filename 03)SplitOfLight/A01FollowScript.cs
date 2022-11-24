using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A01FollowScript
//仲間キャラクターの追従、移動処理
//
public class A01FollowScript : MonoBehaviour
{
    //基本の処理に必要な変数群
    //追従先のオブジェクト、また移動経路を保存する為のスクリプト。
    public GameObject TrackingObject;
    public A02PositionUpdate PositionManager;
    public int nextIndex;

    public float speed = 4.5F;
    public float startTime = 0;
    public float journeyLength;

    public Vector3 startPosition;
    public Vector3 endPosition;

    public float waitTime = 0.1f;   // sec
    public float range = 0.001f;


    //追跡先の自動切り替え用
    //forwardA01は「自身の前方に位置するオブジェクトにアタッチされたスクリプト」を示す。
    public A01FollowScript forwardA01;
    public bool friendState = true;

    //当たり判定を持ってくれるオブジェクトの参照。
    GameObject colObject;

    //号令処理
    float accelSpeed = 5.0f;
    bool accelflag = false;
    [SerializeField] GameObject Player;
    int callCount = 0;
    int calltime = 60;

    //散開処理用
    [SerializeField] private bool spreadFlag = false;
    bool spreaded = false;
    float spreadPower = 3.0f;
    A02PositionUpdate A02PlayerPM;

    //Animation
    Animator animator;
    new Animation animation;
    [SerializeField] float animSpeed = 1.0f;
    bool RebornF;

    //スピードアップアイテム
    A12ItemManager A12;
    bool dualCheck = false;

    //画像処理
    GameObject playCanvas;
    [SerializeField] GameObject jumpUI;

    // Start is called before the first frame update
    void Start()
    {

        //初期設定
        PositionManager = TrackingObject.GetComponent<A02PositionUpdate>();

        startPosition = transform.position;
        endPosition = PositionManager.GetCurrentPositon(out nextIndex);

        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, endPosition);

        forwardA01 = TrackingObject.GetComponent<A01FollowScript>();


        //子オブジェクトのタグを引っ張ってくるために必要な処理。
        colObject = transform.GetChild(2).gameObject;

        //プレイヤーのオブジェクト情報の取得
        Player = GameObject.Find("Player");
        //拡散処理の為に、PositionManagerも必要になった。
        A02PlayerPM = Player.transform.GetChild(6).GetComponent<A02PositionUpdate>();

        //Animatorの取得
        animator = transform.GetChild(0).GetComponent<Animator>();
        animation = transform.GetChild(0).GetComponent<Animation>();

        //アイテム
        A12 = GameObject.Find("FCCheck").GetComponent<A12ItemManager>();

        playCanvas = GameObject.Find("PlayCanvas");
        jumpUI = playCanvas.transform.Find("jumpup").gameObject;
        //jumpUI = GameObject.Find("jumpup");

    }

    // Update is called once per frame
    void Update()
    {

        //集合時の拡散処理に関わるフラグ管理。
        //Playerとの距離を測り、近い時に拡散処理に分岐する為のフラグを立てる。
        //解除時に、ある条件を指定する事でスムーズな隊列編成を実現した。
        
        if (Vector3.Distance(Player.transform.GetChild(6).position, A02PlayerPM.GetLatePosition(1)) == 0)
        {
            animator.SetBool("Move", false);
            spreadFlag = true;

        }
        else
        {

            //距離が遠い場合にフラグをfalseにする。
            //if内は、拡散してから「一回目」の判定である事を知る為の条件
            //→必要な理由は、Playerが移動を再開した際に、スムーズな隊列変化を可能にするため。

            spreadFlag = false;
            if (spreadFlag == false && spreaded == true)
            {
                endPosition = PositionManager.ResetPosition(out nextIndex);
                gameObject.GetComponent<A02PositionUpdate>().ResetPosition(out _);
                SetPosition();
                spreaded = false;

            }

            animator.SetBool("Move", true);
        }


        //目標座標との距離が近づいた場合、目標座標を更新する。
        //拡散処理、加速からの減速処理等はこのタイミングで行う。

        if (Vector3.Distance(transform.position, endPosition) < range)
        {

            //集合と再生時の合流処理による加速を元に戻します。また、減速時に通った座標に戻させないようにするため、
            //PositionManagerの中身を現在地でリセットします。
            if (accelflag)
            {
                accelflag = false;
                speed = speed / accelSpeed;
                endPosition = PositionManager.ResetPosition(out nextIndex);

                //3.0fの判定を使うと、なぜか号令が正常に機能しない為、ここでもsetPositionをコールする。
                SetPosition();

            }

            //拡散処理
            //拡散はFriendSpreadに記載。
            //
            if (spreadFlag)
            {

                if (spreaded == false)
                {
                    FriendSpread(Player.transform.position);
                    spreaded = true;
                }
                
            }
            else
            {


                //集合状態から移動を開始した際のごちゃ付きを回避する為、3f以上距離が開いている状態でしか座標更新を行わない。
                if (Vector3.Distance(transform.position, TrackingObject.transform.position) > 3.0f)
                {
                    SetPosition();
                }


            }


            //アイテム取得による加減速のチェック。
            //dualCheckは、加速を一回きりにするために必要な条件。
            //getFは取得しているかどうか、dualCheckは既に加速しているかどうか。
            //SetFloatはAnimator関係。速度に応じてアニメーションの実行速度を変更する。
            if (A12.getF == true && dualCheck == false)
            {
                animator.SetFloat("Speed", A12.UpSpeed * animSpeed);
                speed = speed * A12.UpSpeed;
                dualCheck = true;
            }
            else if (A12.getF == false && dualCheck == true)
            {
                animator.SetFloat("Speed", animSpeed);
                speed = speed / A12.UpSpeed;
                dualCheck = false;
            }

        }


        //Range外、つまり目標座標との距離が開いている場合。
        //紆余曲折あり、距離が空いている場合のみ回転処理を行う事で上手くいった。
        //仲間がブレる問題の原因をここに記す。
        //1.前方との距離が極近距離になると、算出ベクトルが0に近くなり、Quaternionによる回転の生成が上手くいかない。
        //2.目標地点に到着した状態で回転処理を行うと、算出ベクトルが反転し、回転が真逆になる。
        //3.意図しないタイミングでSpreadFlagが立ち、Lookatや拡散処理が行われる為。
        else
        {
            if (friendState)
            {
                Vector3 dir = TrackingObject.transform.position - transform.position;

                if (Vector3.zero != dir)
                {
                    transform.rotation = Quaternion.LookRotation(dir);
                }
            }


        }

        //移動処理
        //上の条件に関わらず必ず処理を行う。
        //Lerpによる移動。詳しくは口頭で説明します。

        float distCovered = (Time.time - startTime) * speed;

        float fractionOfJourney = distCovered / journeyLength;

        //joutneyLengthは目標地点との距離と、現在の進行度を比べた差。
        //0の場合、到着している為移動しない。   
        if (0 < journeyLength)
        {
            if (friendState == true && RebornF == false)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
            }
        }


        //friendState は仲間の状態。あくまで仮置きの為、boolで管理。死亡状態をfalseとしている。
        if (forwardA01.friendState == false)
        {
            //forwardA01によって、「前の仲間が」追跡しているオブジェクトを参照し、自身の追跡先として再定義する。

            SetTarget(forwardA01.TrackingObject);



            //座標の更新
            //SetPosition();

        }

        //死亡判定が子オブジェクトのコライダーで行っているらしい。そこは要修正。
        //colObject.tag == death
        if (colObject.tag == "Death")
        {

            //死んだ直後を判定するための条件。
            if (animator.enabled == true)
            {
                animator.enabled = false;

                //死亡時に死体を地面に落とす為の処理
                //Rigidbodyをアタッチして、重力と当たり判定を作用させる。
                gameObject.AddComponent<Rigidbody>();
                gameObject.AddComponent<BoxCollider>();
                gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                gameObject.GetComponent<BoxCollider>().size = new Vector3(0.1f, 0.5f, 0.1f);
               
            }
            friendState = false;


        }



        //号令処理

        //クールタイム処理
        callCount++;
        if (callCount > 10000)
        {
            callCount = calltime;
        }

        if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {

            //号令のクールタイム処理。
            if (callCount > calltime)
            {
                callCount = 0;
                //水野サウンド用
                if (sound.soundTime[4] > 0)
                {
                    sound.FriendGatherSE = false;
                    sound.startSound[4] = false;
                    sound.soundTime[4] = 0f;
                    GameObject.Find("FriendGatherSe").SetActive(false);
                }
                sound.FriendGatherSE = true;
                //ここまで
                //FriendAggregateは集合処理の関数
                FriendAggregate();
            }
        }

        //アクションキーが押された際に、号令を掛ける処理
        //プレイヤーがエリア内に存在する場合のみ、号令を掛ける。
        //判定方法はInAreaCheckerに記載……難しいのでしてません。聞いてください。
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 1"))
        {

            if (InAreaChecker())
            {
                //animator.SetBool("Jump", true);
                FriendAggregate();
            }
        }


        //Animation
        //Jumpモーションの再生条件。
        //前方の仲間が宙に浮いた場合 とする。
        if (Player.transform.position.y > 9)
        {
            //宙に浮いている間集合処理を掛け続ける。
            FriendAggregate();
            //FriendSpread(Player.transform.position);
     
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
        }

        //Bボタンを押せる場所でBボタンのUIを表示する。
        if (InAreaChecker() && jumpUI != null)
        {
            jumpUI.SetActive(true);
        }
        else
        {
            jumpUI.SetActive(false);
        }

        //登場時のアニメーション。
        //fellowshispawnでアニメーションを起動するため、こちらでは終了を検知する必要がある。
        //rebornFはA01側で登場直後であることを認識する為に必要。rebornFとisPlayingの終了で動き出しを検知する。
        if (animation.IsPlaying("MLanding"))
        {
            //friendState = false;
            RebornF = true;
        }

        if (RebornF == true && animation.IsPlaying("MLanding") == false)
        {
            RebornF = false;
            animator.enabled = true;
            friendState = true;
            transform.LookAt(Player.transform.position);
        }


        
        if (friendState && animator.enabled == false && RebornF == false)
        {
            animator.enabled = true;
            //transform.position += new Vector3(0, 5, 0);
            //transform.LookAt(Player.transform.position);
            //transform.GetChild(0).localRotation = Quaternion.Euler(-90, 0, 0);
            //transform.GetChild(0).position = transform.position;
        }


    }


    //FriendAggregate
    //集合用スクリプト。呼ばれた場合、一時的な追従先をPlayerに合わせ、速度をaccelSpeed倍する。
    //Playerに接近したらもとに戻る。
    public void FriendAggregate()
    {
        if (accelflag == false)
        {
            accelflag = true;
            speed = speed * accelSpeed;
        }

        SetPosition(Player.transform.position);

        Player.GetComponent<PlayerController>().Movecnt = 0;
    }


    //Setシリーズ。コピぺしていた部分を関数化しただけ。SetTargetによって、他のスクリプトから呼びやすくなったかも？
    //SetPositionは多態性を使用。追従先を引数で指定出来るようにした。
    public void SetTarget(GameObject obj)
    {
        TrackingObject = obj.transform.gameObject;
        PositionManager = TrackingObject.GetComponent<A02PositionUpdate>();
        forwardA01 = TrackingObject.GetComponent<A01FollowScript>();
        SetPosition();
    }

    private void SetPosition()
    {


        startTime = Time.time;
        startPosition = transform.position;
        endPosition = PositionManager.GetPositon(nextIndex, out nextIndex);
        //endPosition.y = startPosition.y;    // Y座標を合わせる


        //旋回処理。
        /*Vector3 dir = endPosition - startPosition;
        Quaternion Q = Quaternion.LookRotation(dir);
        //transform.rotation = Quaternion.Slerp(transform.rotation,Q,0.5f);
        transform.rotation = Quaternion.LookRotation(dir);*/

        //transform.LookAt(endPosition);

        journeyLength = Vector3.Distance(startPosition, endPosition);
    }

    private void SetPosition(Vector3 tracePosition)
    {
        startTime = Time.time;

        startPosition = transform.position;
        endPosition = tracePosition;
        //endPosition.y = startPosition.y;    // Y座標を合わせる

        //transform.LookAt(endPosition);

        journeyLength = Vector3.Distance(startPosition, endPosition);
    }



    //集合時の散開処理。
    //プレイヤーとのDistanceを取り(もしくは小さなCollider)、一定距離未満の場合にSetPositionを散らべる(???)。
    //Playerの周囲16方向から、乱数で決定。距離は一定or乱数で定める。
    //次の処理(SetPosition)を行う条件は、散開処理に入った時のplayerの座標と、現在の座標の比較によって行う。
    //

    //散開処理の仕様を変更する。
    //SetPositionによる座標指定の後、乱数で導かれた分だけendPositionをずらす。
    //
    private void FriendSpread(Vector3 playerPos)
    {
        //playerPos.y += 1;

        var angle = Random.Range(0, 360);
        var f = angle * Mathf.Deg2Rad;
        var direction = new Vector3(Mathf.Cos(f), 0, Mathf.Sin(f));
        direction = direction * spreadPower;

        //endPosition += direction;

        //transform.LookAt(Player.transform.position);

        //修正必須
        SetPosition(playerPos += direction);


    }


    //アクションキーの入力受付。
    //Playerがアクションエリア内に存在するかどうかをチェックします。
    //PhysicsOverlapSphereを使ってます。詳しくは聞いてください。
    //一定範囲内に「ActionArea」という名前のColliderがある場合、trueを返す。
    private bool InAreaChecker()
    {
        bool hitF = false;
        Collider[] hitCol = Physics.OverlapSphere(Player.transform.position, 1);
        for (int i = 0; i < hitCol.Length; i++)
        {
            if (hitCol[i].name == "ActionArea")
            {
                hitF = true;
            }
        }
        return hitF;
    }



    //死体の落下処理用。
    private void OnCollisionEnter(Collision collision)
    {
        
        if (gameObject.name != "Player")
        {
            if (collision.collider.tag == "stage" || collision.collider.name == "StepArea")
            {
                Destroy(gameObject.GetComponent<Rigidbody>());
                Destroy(gameObject.GetComponent<BoxCollider>());
            }
        }
    }
}

