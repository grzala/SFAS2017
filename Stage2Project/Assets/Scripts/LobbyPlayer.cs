using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour {

    public int connectionId;
    public bool ready = false;
    public 

	// Use this for initialization
	void Start () {
        print("localplayerspawning");
        transform.SetParent(GameObject.Find("List").transform, false);
	}

    public override void OnStartLocalPlayer()
    {
    }

	// Update is called once per frame
	void Update () {
		
	}

    [ClientRpc]
    public void RpcGameUI() {
        print("test");
        GameObject.Find("ScreenManager").GetComponent<ScreenManager>().StartGame();

        Canvas[] canvasses = GameObject.Find("Lobby").GetComponentsInChildren<Canvas>();
        foreach (Canvas c in canvasses)
        {
            c.enabled = false;
        }
    }

    [ClientRpc]
    public void RpcToggleStartBtn(bool toggle)
    {
        Button btn = GameObject.Find("Lobby").GetComponent<LobbyUI>().startBtn;

        if (isServer)
        {
            btn.gameObject.SetActive(toggle);
        }
    }
}
