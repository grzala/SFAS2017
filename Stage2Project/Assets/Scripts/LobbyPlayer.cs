using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour {

    public int connectionId;
    public bool ready = false;
    private bool initialized = false;

	// Use this for initialization
	void Start () {
        print("localplayerspawning");
        GameObject list = GameObject.Find("List");
        transform.SetParent(list.transform, false);
	}

    public void SetReady()
    {
        print("ready");
        print(connectionId);
    }

	// Update is called once per frame
	void Update () {
        if (!initialized && hasAuthority)
        {
            InitializeLocal();

            initialized = true;
        }
	}

    public void OnClickReady()
    {
        CmdToggleReady(true);
    }

    [Command]
    public void CmdToggleReady(bool toggle)
    {
        print(connectionId);
    }

    public void InitializeLocal()
    {
        Button ready = transform.parent.parent.Find("Ready").GetComponent<Button>();
        ready.onClick.AddListener(() => OnClickReady());
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
