using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script controls The UI in multiplayer lobby */

public class LobbyUI : MonoBehaviour {

    [SerializeField]
    private Text status;
    [SerializeField]
    private Text hosting;

    [SerializeField]
    public Button startBtn;

    private GameNetwork net;
    private Canvas currentPanel;
    [SerializeField]
    public Canvas mainPanel;
    [SerializeField]
    public Canvas playerList;
    [SerializeField]
    public GameObject list;

	// Use this for initialization
	void Start () 
    {
        net = GetComponent<GameNetwork>();
        currentPanel = mainPanel;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Some canvases overlayed and prevented clicking on other canvases.
        GameObject.Find("TopPanel").GetComponent<GraphicRaycaster>().enabled = GetComponent<Canvas>().enabled;
	}

    //host game
    public void OnClickHost()
    {
        net.networkAddress = "127.0.0.1";
        net.StartHost();
    }

    //join game
    public void OnClickJoin()
    {
        string IP = GameObject.Find("IPField").GetComponent<InputField>().text;
        if (IP == "localhost" || IP == "")
            IP = "127.0.0.1";
        net.networkAddress = IP;
        net.StartClient();
    }

    public void OnClickStart()
    {
        net.StartGame();
    }

    public void OnClickBack()
    {
        net.QuitServer();

        if (currentPanel == mainPanel)
        {
            GameObject.Find("ScreenManager").GetComponent<ScreenManager>().GoToMain();
            return;
        }

        TransitionTo(mainPanel);
        UpdateInfo("offline", "none");
    }

    public void UpdateInfo(string status, string hosting)
    {
        this.status.text = status;
        this.hosting.text = hosting;

    }

    public void HideAllLobbyUI(List<LobbyPlayer> clients)
    {
        foreach (LobbyPlayer lp in clients)
        {
            lp.RpcGameUI();
        }
    }


    public void TransitionTo(Canvas next)
    {
        currentPanel.enabled = false;
        currentPanel = next;
        currentPanel.enabled = true;

        if (next == playerList)
        {
            foreach (LobbyPlayer player in net.GetPlayers())
            {
                if (!player.isServer)
                    continue;
                player.RpcToggleStartBtn(true);
            }
        }
        else
        {
            foreach (LobbyPlayer player in net.GetPlayers())
            {
                player.RpcToggleStartBtn(false);
            }
        }
    }

    public void ToggleStartBtn(bool toggle)
    {
        foreach (LobbyPlayer player in net.GetPlayers())
        {
            if (!player.isServer)
                continue;
            player.RpcToggleStartBtnInteract(toggle);
        }
    }

}
