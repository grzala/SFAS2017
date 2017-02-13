using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HUD : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setCubes(int count) {
        Text text = transform.Find("CubesCount").GetComponent<Text>();
        text.text = "Magnetized Cubes Left: " + count.ToString();

    }
}
