using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private static float LIFESPAN = 5.0f;

    private float timeAlive = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        timeAlive += Time.deltaTime;
        if (timeAlive >= LIFESPAN)
        {
            Destroy(gameObject);
        }
		
	}
}
