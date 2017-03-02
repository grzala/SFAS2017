using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* script for updating on screen information */

public class HUD : MonoBehaviour {

    public void setCubes(int count) 
    {
        Text text = transform.Find("CubesCount").GetComponent<Text>();
        text.text = "Cubes Left: " + count.ToString();
    }

    public void setShots(int count) 
    {
        Text text = transform.Find("ShotsCount").GetComponent<Text>();
        text.text = "Shots Left: " + count.ToString();
    }

    public void setShield(int percent)
    {
        percent = Mathf.Max(percent, 0);
        Text text = transform.Find("ShieldCount").GetComponent<Text>();
        text.text = "Shield Charge: " + percent.ToString() + "%";
    }

    public void setScore(int score)
    {
        Text text = transform.Find("ScoreCount").GetComponent<Text>();
        text.text = "Score: " + score.ToString();
    }

    public void setTimeLeft(string time)
    {
        Text text = transform.Find("TimeCount").GetComponent<Text>();
        text.text = "Time Left: " + time;
    }
}
