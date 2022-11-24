using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A05ActionManager
//ギミック周りの入力を受け付けるスクリプト。
//FlowerClimbに関しては、A07に主な処理が記載されている。
public class A05ActionManager : MonoBehaviour
{
    [SerializeField] Collider ActionArea;   //アクションキーの入力受付範囲
    [SerializeField] GameObject Player;
    private float power = 12.0f;           //FlowerClimbのジャンプ力
    Rigidbody rb;

    [SerializeField] GameObject stepObj;

    //MushJump用の変数 距離とベクトル。
    Vector3 heading;
    Vector3 distance;
    Vector3 direction;
    bool MushAcF;

    //treehide用の変数
    public bool hideF = false;
    [SerializeField] GameObject mainCamera;
    Vector3 initDistance;
    GameObject HideArea;
    bool TreeAcF;

    //Animation
    Animator animator;
    float animSpeed;

    //ギミック入力の重複を防ぐ為のカウンタ。
    int a = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = Player.gameObject.GetComponent<Rigidbody>();
        animator = Player.transform.GetChild(4).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MushAcF == true)
        {
            a++;
            if (a > 100)
            {
                MushAcF = false;
            }
        }
    }


    //Playerが範囲内でアクションキーを押した場合、範囲を持つオブジェクト次第(tag,layer?)で処理を分岐。
    //各処理は下部に本体を記載。
    //(TriggerStayを使ってます。見れば分かると思います)

    private void OnTriggerStay(Collider Col) //
    {
        //
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown("joystick button 1"))
        {
            //Playerのみ判定
            if (Col.name == "Player")
            {
                Player.GetComponent<PlayerController>().Movecnt = 0;

                //FlowerClimb
                if (gameObject.tag == "flower" && Player.GetComponent<PlayerController>().ActionF == true)
                {

                    if (Player.GetComponent<BoxCollider>().isTrigger == false)
                    {

                        Debug.Log("FlowerClimb");

                        //入力の重複処理を防ぐのと、ジャンプ中の操作を拒否するための処理。
                        Player.GetComponent<PlayerController>().ActionF = false;
                        Player.GetComponent<PlayerController>().enabled = false;

                        //登り方の仕様変更
                        //FlowerClimb();
                        ProjectileMotion();

                    }
                }


                //MushJump
                if (gameObject.tag == "mushroom" && Player.GetComponent<PlayerController>().ActionF == true)
                {

                    //入力重複を防ぐための処理
                    Player.GetComponent<PlayerController>().ActionF = false;
                    if (MushAcF == false)
                    {
                        MushJump();
                        MushAcF = true;
                    }
                }

                //TreeHide
                if (gameObject.tag == "tree")
                {
                    TreeHide();
                }
            }
        }


    }

    //FlowerClimb
    //花弁登り
    //単純にPlayerをジャンプさせる奴。
    //完成版では使われていません。
    void FlowerClimb()
    {
        if (!D_1ClearScript.stageClear)
            sound.PlayerJumpSE = true;

        Vector3 Vec;

        animator.SetBool("Jump", true);
       
        Player.GetComponent<BoxCollider>().isTrigger = true;

        //仕様案その1 距離と方向を取得して、斜めに飛ばす
        //heading = Player.transform.position - transform.position;
        //heading.y = -3;
        //rb.velocity = -heading * 1.5f;
        //rb.velocity += new Vector3(0, power, 0);
        //Vec = transform.position - Player.transform.position;
        //Vec = Vec.normalized * 5 + new Vector3(0, power, 0);

        //仕様案その2。垂直ジャンプ。
        Vec = new Vector3(0, power, 0);
        rb.AddForce(Vec, ForceMode.Impulse);


    }

    //MushJump
    //キノコジャンプ。
    //オブジェクトとの距離、方向を算出、算出したベクトルに対して逆の力を掛ける(=オブジェクトの方向に飛んでいく)
    //跳躍前に座標を上にずらす事で、オブジェクトとの接触を回避している(はず)
    //→挙動がキモかったので消えました。それに、オブジェクトに当たり判定なかったので。
    //アニメーション用のSetを追加。
    void MushJump()
    {
        if (!D_1ClearScript.stageClear)
            sound.PlayerJumpSE = true;


        //キノコとプレイヤーの方向ベクトルを取得
        //取得した方向ベクトルの逆を掛ける。
        //ジャンプなので結構強めでよい。


        animator.SetBool("Jump", true);

        heading = Player.transform.position - transform.position;
        heading = heading.normalized;
        heading.y = -1.0f;
       
        //AddForceにより、初速を上げる。
        rb.AddForce(-heading * 20.0f, ForceMode.Impulse);


    }


    //TreeHide
    //木の裏に隠れる話
    void TreeHide()
    {

        HideArea = transform.parent.GetChild(11).gameObject;


        //木に登る(座標の変更)
        //追跡状態の変更(HarmLess,かな？Script呼べば解決しそう。→別の方法で管理。)
        //木に登っている間はプレイヤーは動けない。
        //ハイド中、別個にカメラが稼働出来る。辺りを見渡す事が出来ます。


        //木から降りる処理
        if (hideF && Player.GetComponent<PlayerController>().ActionF == false && TreeAcF == true)
        {

            Debug.Log("fall");
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Player.GetComponent<PlayerController>().enabled = true;

            //カメラ
            //A03に切り替える前にカメラの位置を戻さないといけない。
            //initDistanceはカメラが動く前のプレイヤーとカメラの距離。
            //これとLookatを使う事で、A03本来のカメラ位置を実現してからスクリプトを戻す。
            //(恐らく、Y軸の加算の際にカメラが追従しないのが原因。処理速度の問題か？)

            mainCamera.GetComponent<A11MovingCamera>().enabled = false;
            mainCamera.transform.position = Player.transform.position + initDistance;
            mainCamera.transform.LookAt(Player.transform);
            mainCamera.GetComponent<A03RotateCamera>().enabled = true;

            HideArea.SetActive(false);

            TreeAcF = false;

        }

        //木に登る処理
        else if (TreeAcF == false && Player.GetComponent<PlayerController>().ActionF == true)
        {

            Debug.Log("Upppp");

            HideArea.SetActive(true);
            //y軸を加算後、FreezePositionで座標を固定。Hide中は動かさない
            //予定だったがなぜか動くので、PlayerControllerのEnableを切る事で操作を受け付けないようにした。
            //天才の発想である。(自惚れ)
            //FreezeRotationの維持に、下記のような記述が必要らしいので記述。詳しくは聞いてください。

            //Player.transform.position += new Vector3(0, 15, 0);
            rb.AddForce(new Vector3(0, 15, 0), ForceMode.Impulse);

            //仕様変更により、木上での停止処理はA15に記載。

            Player.GetComponent<PlayerController>().enabled = false;




            //カメラの話
            //A03はプレイヤーを中心に回転するスクリプト A11は前後左右,旋回も自由なスクリプト。
            //initDistanceを保存しておかないと、降りた時にカメラ位置が戻らない。
            //A03,A11ともにカメラにアタッチ、enableで切り替える事で実現。

            initDistance = mainCamera.transform.position - Player.transform.position;
            mainCamera.GetComponent<A11MovingCamera>().enabled = true;
            mainCamera.GetComponent<A11MovingCamera>().GetInitPosition(mainCamera.transform.position);

            mainCamera.GetComponent<A03RotateCamera>().enabled = false;

            TreeAcF = true;


        }

        hideF = !hideF;

    }


    //FlowerClimbの新仕様。
    //垂直ジャンプから斜方ジャンプにする為のコード。
    //ProjectileMotionは斜方投射の意。
    public void ProjectileMotion()
    {


        //移動時の速度が残ってると嫌な挙動するので、飛ばす前に座標を静止させる。
        rb.constraints = rb.constraints = RigidbodyConstraints.FreezePosition
                                        | RigidbodyConstraints.FreezeRotationX
                                        | RigidbodyConstraints.FreezeRotationZ;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


        //stepObjは着地地点
        stepObj = transform.parent.GetChild(4).gameObject;


        //angleは跳ぶ角度。0-90の間でしか動かない(らしい)
        float angle = 80.0f;


        //XZ平面上でのPlayerと目標地点(花の中心)の距離を取得。
        Vector2 startPosXZ = new Vector2(Player.transform.position.x, Player.transform.position.z);
        Vector2 targetPosXZ = new Vector2(stepObj.transform.position.x, stepObj.transform.position.z);
        float distance = Vector2.Distance(targetPosXZ, startPosXZ);

        //斜方投射の計算式に必要な情報の取得。
        float x = distance;
        float g = Physics.gravity.y;
        float y0 = 1;
        float y = stepObj.transform.position.y;
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);


        //斜方投射の計算。及び変換
        float v0Square = g * x * x / (2 * cos * cos * (y - y0 - x * tan));
        float v0;
        //0未満の場合、与えられた角度の斜方投射では目標地点に届かない為、計算を行わない。(今後の処理も行わない)
        if (v0Square <= 0.0f)
        {
            Debug.Log("Failed");
            v0 = 0;
        }
        else
        {
            v0 = Mathf.Sqrt(v0Square);
        }


        //ジャンプに必要な処理
        Player.GetComponent<BoxCollider>().isTrigger = true;
        animator.SetBool("Jump", true);
        if (!D_1ClearScript.stageClear)
            sound.PlayerJumpSE = true;


        //斜方投射で届く場合処理を続行
        //追記。どちらにしても処理を行う。届かない場合は垂直ジャンプ。
        if (v0 != 0)
        {


            //上で導いた結果を力に再計算する。
            Vector3 startPos = Player.transform.position;
            Vector3 targetPos = stepObj.transform.position;
            startPos.y = 0.0f;
            targetPos.y = 0.0f;

            //Vector3への置換
            Vector3 dir = (targetPos - startPos).normalized;
            Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
            Vector3 vec = v0 * Vector3.right;

            //向きと、(重さの計算(今回はしていない))を整えて、AddForceで射出。
            vec = yawRot * Quaternion.AngleAxis(angle, Vector3.forward) * vec;
            Vector3 force = vec;

            Debug.Log(force.magnitude);
            if (force.magnitude < 15)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(new Vector3(0, 15, 0), ForceMode.Impulse);
            }


            //Debug.Log(stepObj.transform.position);

        }
        else
        {
            rb.AddForce(new Vector3(0, 15, 0), ForceMode.Impulse);
        }
    }

}




//FlowerClimbの抜け道。
//Raycastは現実的じゃない。そもそも、空中の操作性は？
//かといって、壁をいちいち取得するのも意味わかんないしな。
//Playerの当たり判定を消すのが一番スマートか。というかisTriggerでいいか。
//花伝いに移動するのも、isTriggerの操作でなんとか……できないすね。床面透けるじゃん馬鹿が。
//花の上を歩くっていうけど、花の形状如何では足を滑らす可能性がある。
//モデルに正直に判定をつけるのか、それとも空中に立たせるのか。
//モデルに正直に組むのであれば、アクションキーによる花伝いの移動を実装してもよい。
//ActionAreaの実装がきついか。降下キーと競合するから没。
//移動の制限は字面は簡単だけど、コードの可視性を考慮すると結構嫌なんだよな
//ギミックに関わる制限をPlayerControllerに記載する、ってのがマジで嫌。
//ActionはAction周りのスクリプトで完結してほしい。

//RigitBodyを消せば落ちない。花上で重力切れば落ちなくなるんよな。
//useGravityのOn/Offは、壁接触時に慣性が働く事で想定外の動きをするが……抜けたあと戻せば問題なさそう。
//wallColliderの中に存在する時のみ、useGravityを切る。とかか。
//無理じゃね？壁の接触判定をOnColliderでやって、そのままOnTriggerって出来る訳ある？？？
//無いだろ。無いな。え？無理じゃね？
//で　き　ま　し　た。
//なんならCollisionからのTriggerで上手くいったのでこの話は終わり、閉廷。
//

//床面が昇った花の部分しか生み出されない。
//→元々isTrigger=falseで、アクションキーでtrue,TriggerExitでfalseに戻せばいい。
//→降下処理も同様
//PlayerのisTrigger操作で床面を抜けるようにすればいい。

//花から花への移動と、花上移動の制限の兼ね合い。
//→Unityプロジェクト上の作業負担と、参照の複雑さを考慮すると『現実的じゃない』で全て没案になる。
//→出来ないことはない。(方法が無い訳ではない)　が、「やりたくない」が正しい。

//MushJumpによる壁の貫通
//isTriggerによる判定の透過が現状一番丸い。
//
