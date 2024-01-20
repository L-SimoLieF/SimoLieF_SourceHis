using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test01 : MonoBehaviour
{

    public GameObject othersPrefab;
    [SerializeField] GameObject Player;

    float deltaTime;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 PlayerPos = Player.transform.position;
        PlayerPos = PlayerPos + Vector3.forward * 100;
        Instantiate(othersPrefab,PlayerPos,Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {

        //this.transform.position += Vector3.forward * speed * Time.deltaTime;

        //deltaTime = Time.deltaTime;

        /*if(deltaTime > 120f) 
        {
          
        }*/

        /*if (Input.GetKeyDown(KeyCode.Space)) 
        {

        }*/
    }
}
