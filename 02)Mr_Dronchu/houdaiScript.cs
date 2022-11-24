//Update内、Kキー入力で投射、の部分を全てカットしています。詳細と理由はPlayerControllerに。

using System.Collections;
using System.Collections.Generic;
//using UnityEditor.XR;
using UnityEngine;

public class houdaiScript : MonoBehaviour
{
    [SerializeField] GameObject ball;

   public static GameObject o;
    Rigidbody r;

    //ここから↓

    GameObject Player;
    KeyPlayerContoller playerContoller;

    public bool makibishiHasya = false;

    public AudioClip makibishiNage;//まきびしを投げた時のSE
    AudioSource audioSource;

    //ここ追加↑

    GameObject AtGage;
    EnemyScript ES;

    void Start()
    {

        AtGage = GameObject.Find("AttentionText");
        ES = AtGage.GetComponent<EnemyScript>();
        //o = Instantiate(ball);
        //Destroy(o, 0.1f);

        //ここから↓

        audioSource = GetComponent<AudioSource>();

        Player = GameObject.Find("Player");
        playerContoller = Player.GetComponent<KeyPlayerContoller>();

        //ここ追加↑

    }

    // Update is called once per frame
    void Update()
    {
        /*if (playerContoller.makibishi)
        {

            if (Input.GetKeyDown(KeyCode.K))
            {
                //ここから↓
                makibishiHasya = true;
                audioSource.PlayOneShot(makibishiNage);//まきびしを投げた時のSE追加しました。
                //ここ追加↑
                ES.AttentionGage += 50;

                o = Instantiate(ball);

                o.transform.position = transform.position;

                r = o.gameObject.GetComponent<Rigidbody>();

                //Vector3 v = ball.transform.TransformDirection(new Vector3(0, 200, 200));//吹っ飛ばし弱
                r.AddForce(transform.TransformDirection(Vector3.forward), ForceMode.Impulse);

                Destroy(o, 5f);


            }
            if(Input.GetKeyUp("joystick button 1"))
            {
                makibishiHasya = true;
                audioSource.PlayOneShot(makibishiNage);//まきびしを投げた時のSE追加しました。
                //ここ追加↑
                ES.AttentionGage += 50;

                o = Instantiate(ball);

                o.transform.position = transform.position;

                r = o.gameObject.GetComponent<Rigidbody>();

                //Vector3 v = ball.transform.TransformDirection(new Vector3(0, 200, 200));//吹っ飛ばし弱
                r.AddForce(transform.TransformDirection(Vector3.forward), ForceMode.Impulse);

                Destroy(o, 5f);
            }
        }
        //コントローラ用
        /*if (ContPlayerContoller.makibishi)
        {

            if (Input.GetKeyUp("joystick button 1"))
            {
                //追加しました。
                ES.AttentionGage += 50;

                o = Instantiate(ball);

                o.transform.position = transform.position;

                r = o.gameObject.GetComponent<Rigidbody>();

                //Vector3 v = ball.transform.TransformDirection(new Vector3(0, 200, 200));//吹っ飛ばし弱
                r.AddForce(transform.TransformDirection(Vector3.forward), ForceMode.Impulse);

                Destroy(o, 5f);
            }
        }*/
        //何故か出来なかったので、当初の予定通り関数予呼びます。
    }

    public void MakibishiThrow()
    {
        //追加しました。
        ES.AttentionGage += 50;

        makibishiHasya = true;
        audioSource.PlayOneShot(makibishiNage);//まきびしを投げた時のSE追加しました。

        o = Instantiate(ball);

        o.transform.position = transform.position;

        r = o.gameObject.GetComponent<Rigidbody>();

        //Vector3 v = ball.transform.TransformDirection(new Vector3(0, 200, 200));//吹っ飛ばし弱
        r.AddForce(transform.TransformDirection(Vector3.forward), ForceMode.Impulse);

        Destroy(o, 5f);
    }
}