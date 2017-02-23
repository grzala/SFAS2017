using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {

    private float rotationSpeed = 30.0f;

    private float bounce_bottom = 0.5f;
    private float bounce_top = 2.5f;
    private float current_bounce;
    private bool rising = true;
    private float bounce_speed = 0.8f;

    [SerializeField]
    private Material selfMaterial;

    [SerializeField]
    private Material restMaterial;

    [SerializeField]
    private Material arenaMaterial;

    private Type type;
    private Target target;

    //SELF, REST, ARENA

    public enum Type
    {
        SPEED_UP,
        SLOW_DOWN,
		INVERSE,
		DOUBLE_CUBES
    }

    public enum Target
    {
        SELF,
        REST,
        ARENA
    }

    private Dictionary<Type, string> textureNames = new Dictionary<Type, string>();

	// Use this for initialization
	void Awake () {
        current_bounce = bounce_bottom;		
        PopulateTexNames();

        type = Type.SPEED_UP;
        target = Target.SELF;

        SetColor();
        SetIcon();
	}

    private void PopulateTexNames()
    {
        textureNames.Add(Type.SPEED_UP, "speed_up");
		textureNames.Add(Type.SLOW_DOWN, "slow_down");
		textureNames.Add(Type.INVERSE, "inverse");
        textureNames.Add(Type.DOUBLE_CUBES, "double");
    }

    public void SetType(Type type, Target target)
    {
        this.type = type;
        this.target = target;

        SetColor();
        SetIcon();
    }

    public void SetRandomType()
    {
        System.Array types = Type.GetValues(typeof(Type));
        Type rntype = (Type)types.GetValue(Random.Range(0, types.Length));

        System.Array targets = Target.GetValues(typeof(Target));
        Target rntargert = (Target)targets.GetValue(Random.Range(0, 2));

        SetType(rntype, rntargert);

    }

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

    private void SetIcon()
    {
        Sprite s = Resources.Load(textureNames[type], typeof(Sprite)) as Sprite;

        GetComponentInChildren<SpriteRenderer>().sprite = s;
    }
	
	// Update is called once per frame
	void Update () {

        //transform.GetChild(0).Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));


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

    private void OnTriggerEnter(Collider collision) {
        //only players collect powerups
        if (collision.gameObject.tag == "Player") {
            //Resources.Load("COLLECT");

            transform.parent.GetComponent<PowerupHandler>().ActivatePowerup(type, target, collision.GetComponent<Player>());

            Destroy(gameObject, 0.1f);
        }
    }
}
