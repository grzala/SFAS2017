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
        text.text = "Cubes Left: " + count.ToString();
    }

    public void setShots(int count) {
        Text text = transform.Find("ShotsCount").GetComponent<Text>();
        text.text = "Shots Left: " + count.ToString();
    }

    public void setShield(int percent) {
        percent = Mathf.Max(percent, 0);
        Text text = transform.Find("ShieldCount").GetComponent<Text>();
        text.text = "Shield Charge: " + percent.ToString() + "%";
    }
}
