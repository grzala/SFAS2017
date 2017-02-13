using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField]
	private float Speed;
	private const float MAX_SPEED = 1000;

    private Rigidbody mBody;

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;



        if (Input.GetKey(KeyCode.A))
        {
            direction = -Vector3.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = Vector3.right;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction += -Vector3.forward;
        }

        mBody.AddForce(direction * Speed * Time.deltaTime);

		Vector3 vec = GetComponent<Rigidbody>().velocity;
		Vector2 vel = new Vector2(vec.x, vec.z);

		float velocity = vel.SqrMagnitude();

		if (velocity > MAX_SPEED) {
			Vector2 newvel;
			float dif = velocity - MAX_SPEED;
			float percent = dif / velocity;
			newvel = vel - (vel * percent);
			Vector3 newvec = new Vector3(newvel.x, vec.y, newvel.y);
			GetComponent<Rigidbody>().velocity = newvec;
		}

    }
}
