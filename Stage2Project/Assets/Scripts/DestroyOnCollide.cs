using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyOnCollide : MonoBehaviour {

    bool toDestroy = false;
    int framesLeft = 2; // to ensure that bullet does not get deleted before the collision is carried out

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (toDestroy)
        {
            framesLeft--;
        }

        if (framesLeft <= 0)
        {
            NetworkServer.Destroy(gameObject);
            Destroy(this);
        }


	}

    private void OnCollisionEnter(Collision collision) {
        //if collided with something else than arena, delete the bullet
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Bounds") && collision.collider.gameObject.layer != LayerMask.NameToLayer("Shields")) {
            toDestroy = true;
        }
    }
}
