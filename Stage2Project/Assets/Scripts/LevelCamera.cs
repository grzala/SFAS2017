using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCamera : MonoBehaviour {

	private GameObject following;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Transform target = following.transform;

		//follow        
		transform.position = target.position + offset;

	}

	public void Follow(GameObject obj) {
		following = obj;
		offset = transform.position - obj.transform.position;
	}
}
