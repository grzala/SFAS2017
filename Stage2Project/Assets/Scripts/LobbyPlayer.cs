using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkBehaviour {

    public static Color[] colors =
    {
        Color.red,
        Color.yellow,
        Color.cyan,
        (Color)new Vector4(0.3f, 0, 0.5f, 1),
        Color.blue,
    };

    public int connectionId;
    private bool initialized = false;

    [SyncVar]
    public bool ready = false;

    [SyncVar]
    public int colorIndex = -1;

    [SyncVar]
    public string name = "player";

    [SerializeField]
    private Text readyText;
    [SerializeField]
    private Button ColorButton;
    [SerializeField]
    private Text NameText;


	// Use this for initialization
	void Start () {
        print("localplayerspawning");
        GameObject list = GameObject.Find("List");
        transform.SetParent(list.transform, false);

        GetComponent<Canvas>().enabled = false;
        GetComponent<Canvas>().enabled = true;

        readyText.text = ready ? "Ready" : "Not Ready";
        ColorButton.GetComponent<Image>().color = colors[colorIndex];
        NameText.text = name;
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
        CmdToggleReady(!ready);
    }

    [Command]
    public void CmdToggleReady(bool toggle)
    {
        ready = toggle;

        RpcUpdateReadyText(ready ? "Ready" : "Not Ready");
    }

    [ClientRpc]
    public void RpcUpdateReadyText(string text)
    {
        readyText.text = text;
    }

    public void InitializeLocal()
    {
        Button readyBtn = transform.parent.parent.Find("Ready").GetComponent<Button>();
        readyBtn.onClick.AddListener(() => OnClickReady());

        ColorButton.onClick.AddListener(() => OnClickColor());

        readyText.text = ready ? "Ready" : "Not Ready";
        ColorButton.GetComponent<Image>().color = colors[colorIndex];
        NameText.text = name;
    }

    public void OnClickColor()
    {
        CmdChangeColor();
    }

    [Command]
    public void CmdChangeColor()
    {
        colorIndex = GameObject.Find("Lobby").GetComponent<GameNetwork>().NextAvailableColor(colorIndex);

        RpcRecolor(colorIndex);
    }

    [ClientRpc]
    public void RpcRecolor(int index)
    {
        colorIndex = index;
        ColorButton.GetComponent<Image>().color = colors[colorIndex];
    }


    [ClientRpc]
    public void RpcGameUI() {
        print("test");
        GameObject.Find("ScreenManager").GetComponent<ScreenManager>().GoToGame();

        /*
        Canvas[] canvasses = GameObject.Find("Lobby").GetComponentsInChildren<Canvas>();
        foreach (Canvas c in canvasses)
        {
            c.enabled = false;
        }
        */
    }

    [ClientRpc]
    public void RpcToggleStartBtn(bool toggle)
    {
        Button btn = GameObject.Find("Lobby").GetComponent<LobbyUI>().startBtn;

        btn.gameObject.SetActive(toggle);
       
    }

    [ClientRpc]
    public void RpcToggleStartBtnInteract(bool toggle)
    {
        Button btn = GameObject.Find("Lobby").GetComponent<LobbyUI>().startBtn;

        if (isServer)
        {
            btn.interactable = toggle;
        }
    }

    [ClientRpc]
    public void RpcGetNameFromInput()
    {
        string name = GameObject.Find("UsernameField").GetComponent<InputField>().text;
        this.name = name;

        //transform.FindChild("Name").GetComponent<Text>().text = name;
        CmdSetNameOnServer(name);

    }

    [Command]
    public void CmdSetNameOnServer(string name)
    {
        this.name = name;
        transform.FindChild("Name").GetComponent<Text>().text = name;
    }
}
