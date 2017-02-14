using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [SerializeField]
	private float Speed;

    [SerializeField]
    private GameObject BulletPrefab;

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


    public void shoot(float angle) {
        GameObject spawnObject = BulletPrefab;
        GameObject spawnedInstance = Instantiate(spawnObject);


        float height = spawnedInstance.GetComponent<Collider>().bounds.size.y;
        //Vector3 pos = new Vector3(hit.point.x, hit.point.y + height / 2, hit.point.z);
        float distanceFromPlayer = 2.0f;

        spawnedInstance.transform.parent = transform.parent; 

        Vector3 pos = new Vector3(Mathf.Cos(angle) * distanceFromPlayer, 0.0f, Mathf.Sin(angle) * distanceFromPlayer);
        spawnedInstance.transform.position = transform.position;
        spawnedInstance.transform.position = spawnedInstance.transform.position + pos;

        float speed = 3000;
        Vector3 vel = new Vector3(Mathf.Cos(angle) * speed, 0.0f, Mathf.Sin(angle) * speed);
        spawnedInstance.GetComponent<Rigidbody>().velocity = spawnedInstance.GetComponent<Rigidbody>().velocity + (vel * Time.deltaTime);

    }
}
