using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{

    [SerializeField]
    public Material[] mats;

    [SerializeField]
	private float Speed;

    [SerializeField]
    private GameObject BulletPrefab;

    [SerializeField]
    private GameObject ShieldPrefab;

    [SyncVar]
    public string name = "player";

    private NetworkInstanceId shieldId;

    public const float MAX_SPEED = 1000;
    public const float bulletDistanceFromPlayer = 2.0f; //distance of new spawned bullets from player
    public const float shieldDistanceFromPlayer = 12.0f;
    public const float accel = 6000;

    [SyncVar]
    public bool shielding = false;
    [SyncVar]
    public float shieldingTimeLeft;
    public const float SHIELDING_TIME = 1.0f; //in seconds

    [SyncVar]
    public float angle = 0.0f;

    [SyncVar]
    public int cubesLeft = 0;
    [SyncVar]
    public int shotsLeft = 0;
    public const int SHOTS_PER_RELOAD = 5;

    [SyncVar]
    public int score = 0;
    [SyncVar]
    public int materialIndex = 0;

    //BUFFS
    [SyncVar]
    public bool speedBuff = false;
    [SyncVar]
    public bool slowBuff = false;
    public const float SPEED_BUFF_MODIF = 2.5f;
	[SyncVar]
	public bool inverted = false;

    private Shield shield;

    private Rigidbody mBody;

    private HUD hud;

    private bool initialized = false;

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer() {
        CmdCreateShield();

        GameObject.Find("LevelCamera").GetComponent<LevelCamera>().Follow(this.gameObject);
        //shield = NetworkServer.FindLocalObject(shieldId).GetComponent<Shield>();
        //shield.transform.parent = transform;

        hud = GameObject.Find("HUD").GetComponent<HUD>();
        shotsLeft = SHOTS_PER_RELOAD;
        shieldingTimeLeft = SHIELDING_TIME;
    }

    [Command]
    private void CmdCreateShield()
    {
        GameObject toInstantiate = ShieldPrefab;
        GameObject shieldObject = Instantiate(toInstantiate);
        //shieldObject.name = "Shield";
        shieldObject.transform.parent = transform; //child of game arena, sibling of player
        shieldObject.transform.position = new Vector3(0, 0, 0);
        NetworkServer.Spawn(shieldObject);

        shieldId = shieldObject.GetComponent<NetworkIdentity>().netId;
    }

    [Command]
    private void CmdUpdateShield(float angle)
    {
        shield = NetworkServer.FindLocalObject(shieldId).GetComponent<Shield>();

        shield.gameObject.SetActive(shielding);
        if (!shielding)
            return;

        Vector3 offset = new Vector3(Mathf.Cos(angle) * shieldDistanceFromPlayer, 0.0f, Mathf.Sin(angle) * shieldDistanceFromPlayer);
        shield.transform.position = transform.position;
        shield.transform.position = shield.transform.position + offset;

        float degrees = Mathf.Atan2(-offset.z, offset.x);
        degrees *= (180 / Mathf.PI);
        //this prevents from rotating wrong way

        Vector3 rot = shield.transform.eulerAngles;
        shield.transform.rotation = Quaternion.Euler(rot.x, 0, degrees);
    }

    [Command]
    public void CmdSetShielding(bool s, float a)
    {
        shielding = s;
        angle = a;
    }
        

    void Update()
    {
        if (isLocalPlayer)
        {
            if (!initialized)
            {
                CmdSetName(name);
                CmdSetMaterial(materialIndex);
                initialized = true;
            }
            
            InterpretInput();

			UpdateHUD(); //HUD is updated on clientside
        }
    }

	void UpdateHUD()
	{
        hud.setCubes(cubesLeft);
        hud.setShots(shotsLeft);

        int shieldPercent = (int)((shieldingTimeLeft / SHIELDING_TIME) * 100);
        hud.setShield(shieldPercent);

        hud.setScore(score);
	}

    private void InterpretInput() 
    {
        Vector3 direction = Vector3.zero;

        //left = right = up = down = false;

        if (Input.GetKey(KeyCode.A))
        {
		    direction += !inverted ? Vector3.left : Vector3.right;
            //left = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction += !inverted ? Vector3.right : Vector3.left;
            //right = true;
        }

        if (Input.GetKey(KeyCode.W))
		{
            direction += !inverted ? Vector3.forward : -Vector3.forward;
            //up = true;
        }
        else if (Input.GetKey(KeyCode.S))
		{
            direction += !inverted ? -Vector3.forward : Vector3.forward;
            //down = true;
        }

        float speed_to_add = accel;


        if (speedBuff)
            speed_to_add *= SPEED_BUFF_MODIF;
        if (slowBuff)
            speed_to_add *= 1/SPEED_BUFF_MODIF;
        
        
        mBody.AddForce(direction * speed_to_add * Time.deltaTime);
        //maintainMaxSpeed();
        //Accelerate(direction * speed_to_add * Time.deltaTime);

        Ray ray = GameObject.Find("LevelCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition); //Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000)) // spawn type one and delete type two
        {

            Vector2 shootAngle = new Vector2(hit.point.x - transform.position.x, hit.point.z - transform.position.z);
            float angle = Mathf.Atan2(shootAngle.y, shootAngle.x);

            if (Input.GetButtonDown("Shoot"))
            {
                CmdShoot(angle);
            }


            if (Input.GetButton("Shield"))
            {
                //shielding = true;
                CmdSetShielding(true, angle);
            }

            //CmdUpdateShield(angle);

        }
        if (!Input.GetButton("Shield"))
        {
            //shielding = false;
            CmdSetShielding(false, 0.0f);
        }
    }

    private void maintainMaxSpeed() {
        Vector3 vec = GetComponent<Rigidbody>().velocity;
        Vector2 vel = new Vector2(vec.x, vec.z);

        float velocity = vel.SqrMagnitude();

        float modif_max_speed = MAX_SPEED;
        if (speedBuff)
            modif_max_speed *= SPEED_BUFF_MODIF;
        if (slowBuff)
            modif_max_speed *= 1/SPEED_BUFF_MODIF;

        if (velocity > modif_max_speed) {

            float angle = Mathf.Atan2(vec.z, vec.x);
            Vector3 newvec = new Vector3(Mathf.Cos(angle) * modif_max_speed * Time.deltaTime, vec.y, Mathf.Sin(angle) * modif_max_speed * Time.deltaTime);
            GetComponent<Rigidbody>().velocity = newvec;
        }
    }

    private void Accelerate(Vector3 acc)
    {
        Vector3 vec = GetComponent<Rigidbody>().velocity;
        vec += acc;
        Vector2 vel = new Vector2(vec.x, vec.z);

        float velocity = vel.SqrMagnitude();

        float modif_max_speed = MAX_SPEED;
        if (speedBuff)
            modif_max_speed *= SPEED_BUFF_MODIF;
        if (slowBuff)
            modif_max_speed *= 1/SPEED_BUFF_MODIF;

        if (velocity > modif_max_speed)
        {
            float angle = Mathf.Atan2(vec.z, vec.x);
            Vector3 newvec = new Vector3(Mathf.Cos(angle) * modif_max_speed * Time.deltaTime, vec.y, Mathf.Sin(angle) * modif_max_speed * Time.deltaTime);
            GetComponent<Rigidbody>().velocity = newvec;
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(acc);
        }
    }

    [Command]
    public void CmdShoot(float angle) {
        if (shotsLeft <= 0)
            return; //no shots, need to reload

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

		NetworkServer.Spawn(spawnedInstance);

        shotsLeft--;
        //Destroy(spawnedInstance);
    }

    [Command]
    public void CmdSetMaterial(int index)
    {
        materialIndex = index;
        GetComponent<Renderer>().material = mats[index];
        RpcSetMaterial(index);
    }


    [ClientRpc]
    public void RpcSetMaterial(int index)
    {
        materialIndex = index;
        GetComponent<Renderer>().material = mats[index];
    }

    [ClientRpc]
    public void RpcDisplayResults(string resultString)
    {
        GameObject.Find("ScreenManager").GetComponent<ScreenManager>().OnGameEnd();
        Text resultText = (Text)GameObject.Find("Results").GetComponent<Text>();
        resultText.text = resultString;
    }

    [ClientRpc]
    public void RpcSetName(string name)
    {
        UpdateName(name);
    }

    [Command]
    public void CmdSetName(string name)
    {
        UpdateName(name);
        RpcSetName(name);
    }

    public void UpdateName(string name)
    {
        this.name = name;
        transform.FindChild("PlayerName").GetComponent<TextMesh>().text = name;
    }

    [ClientRpc]
    public void RpcSetPos(Vector3 pos)
    {
        transform.position = pos;
    }
}
