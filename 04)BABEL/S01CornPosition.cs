using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���G�p��Corn�̍��W���APlayer�ɒǏ]������ׂ̃X�N���v�g�B

public class S01CornPosition : MonoBehaviour
{
    public GameObject Player;
    public Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = Player.transform.position + direction;
    }
}
