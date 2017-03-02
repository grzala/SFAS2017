using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Bullet is only allowed to last a few seconds 
 * this prevents swarms of bullets flying around for infinity */

public class Bullet : MonoBehaviour {

    private static float LIFESPAN = 5.0f;

    private float timeAlive = 0.0f;
	
	// Update is called once per frame
	void Update () {
        timeAlive += Time.deltaTime;
        if (timeAlive >= LIFESPAN)
        {
            Destroy(gameObject);
        }
	}
}
