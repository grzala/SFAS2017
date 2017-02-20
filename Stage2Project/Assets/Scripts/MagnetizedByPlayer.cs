﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagnetizedByPlayer : MonoBehaviour
{
    public enum Type { Attract, Repel }

    [SerializeField]
    private float RepelForce = 1000.0f;

    [SerializeField]
    private float MinimumDistance = 1.0f;

    [SerializeField]
    private Type MagnetizeType = Type.Repel;

    private Player mPlayer;
    private Rigidbody mBody;

    void Awake()
    {
        mPlayer = FindObjectOfType<Player>();
        mBody = GetComponent<Rigidbody>();
    }

	void Update()
    {
        //mPlayer is the closest of All players - should this be magnetized by all players, or just closest one?
        //All Magnetized are children of gamemanager
        GameManager gameManager = transform.parent.GetComponent<GameManager>();
        float distance = float.MaxValue;
        Player closestPlayer = null;

        foreach (Player p in gameManager.players) 
        {
            float temp = Vector3.Distance(p.transform.position, transform.position);
            if (temp < distance)
            {
                closestPlayer = p;
                distance = temp;
            }
        }

        mPlayer = closestPlayer;

        if (mPlayer != null)
        {
            Vector3 difference = MagnetizeType == Type.Repel ? transform.position - mPlayer.transform.position : mPlayer.transform.position - transform.position;
            if( difference.magnitude <= MinimumDistance )
            {
                mBody.AddForce(difference * RepelForce * Time.deltaTime);
            }
        }		
	}

    public Type getMagnetType()
    {
        return MagnetizeType;
    }
}
