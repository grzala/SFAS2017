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

    [SerializeField]
    private GameObject ShieldPrefab;

    private Shield shield;

	private const float MAX_SPEED = 1000;
    private const float bulletDistanceFromPlayer = 2.0f; //distance of new spawned bullets from player
    private const float shieldDistanceFromPlayer = 12.0f;

    private Rigidbody mBody;

    private bool shielding = false;

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();

        shield = Instantiate(ShieldPrefab).GetComponent<Shield>();
        shield.transform.parent = transform.parent; //child of game arena
        shield.gameObject.SetActive(false);
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

        maintainMaxSpeed();
            
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000)) // spawn type one and delete type two
        {

            Vector2 shootAngle = new Vector2(hit.point.x - transform.position.x, hit.point.z - transform.position.z);
            float angle = Mathf.Atan2(shootAngle.y, shootAngle.x);

            if (Input.GetButtonDown("Shoot"))
            {
                shoot(angle);
            }

            if (Input.GetButton("Shield"))
            {
                shield.gameObject.SetActive(true);

            }



            shield.transform.position = transform.position;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * shieldDistanceFromPlayer, 0.0f, Mathf.Sin(angle) * shieldDistanceFromPlayer);
            shield.transform.position = shield.transform.position + offset;


            float degrees = Mathf.Atan2(-offset.z, offset.x);
            degrees *= (180 / Mathf.PI);
            //this prevents from rotating wrong way

            Vector3 rot = shield.transform.eulerAngles;
            shield.transform.rotation = Quaternion.Euler(rot.x, 0, degrees);
        }
        if (!Input.GetButton("Shield"))
        {
            shield.gameObject.SetActive(false);
        }


    }

    private void maintainMaxSpeed() {
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

        spawnedInstance.transform.parent = transform.parent; 

        Vector3 offset = new Vector3(Mathf.Cos(angle) * bulletDistanceFromPlayer, 0.0f, Mathf.Sin(angle) * bulletDistanceFromPlayer);
        spawnedInstance.transform.position = transform.position;
        spawnedInstance.transform.position = spawnedInstance.transform.position + offset;

        float speed = 60;
        Vector3 vel = new Vector3(Mathf.Cos(angle) * speed, 0.0f, Mathf.Sin(angle) * speed);
        spawnedInstance.GetComponent<Rigidbody>().velocity = spawnedInstance.GetComponent<Rigidbody>().velocity + (vel);

    }
}
