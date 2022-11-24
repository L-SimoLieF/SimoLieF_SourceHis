using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A13GetItem : MonoBehaviour
{
    A12ItemManager A12;
    // Start is called before the first frame update
    void Start()
    {
        A12 = GameObject.Find("FCCheck").GetComponent<A12ItemManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "Player")
        {
            sound.Mitu = true;
            A12.ItemGet();
            this.gameObject.SetActive(false);
        }
    }
}
