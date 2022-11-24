using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using NobleConnect.Mirror;
using NobleConnect.Examples.Mirror;

//Mirrorの理解が足りず、画面遷移で困っていた時の産物。
//今はMonoBehavierだが、NetworkManagerを継承してなんやかんややっていた。
//未使用

public class N01NetworkStart : MonoBehaviour
{

    // Start is called before the first frame update

    // Update is called once per frame


    public void ReplacePlayer(NetworkConnection conn, GameObject newPrefab)
    {
        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        // Include true for keepAuthority paramater to prevent ownership change
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPrefab), true);

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer);
    }

}
