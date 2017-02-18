using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (isLocalPlayer)
        {
            //GetComponent<Player>().enabled = true;
        }
        else
        {
            //GetComponent<Player>().enabled = false;
        }
		
	}
}
