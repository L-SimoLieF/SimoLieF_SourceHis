using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChoiceCol : MonoBehaviour
{
    public UIColiderMG UIMG;//UIColider �A�^�b�`����
    public bool L_R;//colider��� true�͍��Afalse���E�B

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(L_R == true)
            {
                UIMG.endFlag = true;
                UIMG.contFlag = false;

            }

            if (L_R == false)
            {
                UIMG.endFlag = false;
                UIMG.contFlag = true;

            }
            UIMG.ChangeChoices();
        }
    }
}
