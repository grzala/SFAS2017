using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Powerup prefab
public class Powerup : NetworkBehaviour {

    //this is deprecated
    private float rotationSpeed = 30.0f;

    //up/down motion
    private float bounce_bottom = 0.5f;
    private float bounce_top = 2.5f;
    private float current_bounce;
    private bool rising = true;
    private float bounce_speed = 0.8f;

    //depending on target, powerups have different colors, therefore different materials
    [SerializeField]
    private Material selfMaterial;
    [SerializeField]
    private Material restMaterial;
    [SerializeField]
    private Material arenaMaterial;

    //SELF, REST, ARENA
    //In the beggining, I wanted to create powerups
    //That not only affect the player, but also the arena / game itself
    //I decided not to do this, However I left the code in case I wanted
    //to implement that feature later

    //Types of powerups
    public enum Type
    {
        SPEED_UP,
        SLOW_DOWN,
		INVERSE,
		DOUBLE_CUBES,
        HALVE_CUBES
    }

    public enum Target
    {
        SELF,
        REST,
        ARENA
    }

    //set default type
    [SyncVar]
    private Type type = Type.SPEED_UP;
    [SyncVar]
    private Target target = Target.SELF;

    private Dictionary<Type, string> textureNames = new Dictionary<Type, string>();

	// Use this for initialization
	void Awake () {
        current_bounce = bounce_bottom;		
        PopulateTexNames();

        SetColor();
        SetIcon();
	}

    //every type has different icon. TextureNames allows to check out the name of required sprite
    private void PopulateTexNames()
    {
        textureNames.Add(Type.SPEED_UP, "speed_up");
		textureNames.Add(Type.SLOW_DOWN, "slow_down");
        textureNames.Add(Type.INVERSE, "inverse");
        textureNames.Add(Type.DOUBLE_CUBES, "double");
        textureNames.Add(Type.HALVE_CUBES, "halve");
    }

    //set type and proper material and sprite
    public void SetType(Type type, Target target)
    {
        this.type = type;
        this.target = target;

        SetColor();
        SetIcon();
    }

    //set material and icon on server
    [ClientRpc]
    public void RpcUpdateRenderTypes()
    {
        SetColor();
        SetIcon();
    }

    //create a random powerup from possible types and targets
    public void SetRandomType()
    {
        System.Array types = Type.GetValues(typeof(Type));
        Type rntype = (Type)types.GetValue(Random.Range(0, types.Length));

        System.Array targets = Target.GetValues(typeof(Target));
        Target rntargert = (Target)targets.GetValue(Random.Range(0, 2));

        SetType(rntype, rntargert);
    }

    //set material adequate to target
    private void SetColor()
    {
        Material [] mats = new Material[1];

        switch (target) {
            case Target.SELF:
                mats[0] = selfMaterial;
                break;
            case Target.REST:
                mats[0] = restMaterial;
                break;
            case Target.ARENA:
                mats[0] = arenaMaterial;
                break;
        }

        GetComponent<Renderer>().materials = mats;
    }

    //Set sprite icon adequate to target
    private void SetIcon()
    {
        Sprite s = Resources.Load(textureNames[type], typeof(Sprite)) as Sprite;

        GetComponentInChildren<SpriteRenderer>().sprite = s;
    }
	
	// Update is called once per frame
	void Update () {

        float cur_speed;
        if (current_bounce >= bounce_top)
        {
            rising = false;
        }
        else if (current_bounce <= bounce_bottom)
        {
            rising = true;
        }

        if (rising)
        {
            cur_speed = bounce_speed;
        }
        else
        {
            cur_speed = -bounce_speed;
        }

        transform.position = transform.position + new Vector3(0, cur_speed * Time.deltaTime, 0);
        current_bounce = transform.position.y;
	}

    //collect powerup
    private void OnTriggerEnter(Collider collision) {
        //only players collect powerups
        if (collision.gameObject.tag == "Player") {
            //Resources.Load("COLLECT");

            //run powerup picking only on server side
            if (!isServer)
                return;

            transform.parent.GetComponent<PowerupHandler>().ActivatePowerup(type, target, collision.GetComponent<Player>());

            Destroy(gameObject, 0.1f);
        }
    }
}
