//Scene読み込みだけ。追加したよ。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Goal");
        SceneManager.LoadScene("GameClear");


        //下村追加部分。
/*#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif*/
    }
}
