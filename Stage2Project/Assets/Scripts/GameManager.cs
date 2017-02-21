using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum State { Paused, Playing }

    [SerializeField]
    private GameObject [] SpawnPrefabs;

    [SerializeField]
    private GameObject BulletPrefab;

    [SerializeField]
    private Player PlayerPrefab;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Arena Arena;

    [SerializeField]
    private float TimeBetweenSpawns;

    private List<GameObject> mObjects;
    private Player mPlayer;
    private State mState;
    private float mNextSpawn;

    private int cubesLeft;

    public List<Player> players = new List<Player>();

    void Awake()
    {
        //mPlayer = Instantiate(PlayerPrefab);
		//mPlayer = (Player)GameObject.Find("Player").GetComponent<Player>();
        //mPlayer.transform.parent = transform;

		//LevelCamera cam = FindObjectOfType<LevelCamera>();
		//cam.enabled = true; //set as main
		//cam.Follow(mPlayer.gameObject);

        ScreenManager.OnNewGame += ScreenManager_OnNewGame;
		ScreenManager.OnExitGame += ScreenManager_OnExitGame;


    }

    void Start()
    {
        //Arena.Calculate();
		//mPlayer.enabled = false;
		//mState = State.Paused;
		mState = State.Playing;

    }

    void Update()
    {
        if (mState == State.Playing)
        {


            MagnetizedByPlayer[] temp = GetComponentsInChildren<MagnetizedByPlayer>();
            List<MagnetizedByPlayer> childrenCubes = new List<MagnetizedByPlayer>();
            foreach (MagnetizedByPlayer cube in temp)
            {
                if (cube.getMagnetType() == MagnetizedByPlayer.Type.Attract)
                {
                    childrenCubes.Add(cube);
                }
            }


            foreach (Player player in players)
            {
                if (player == null)
                    continue;

                //UPDATE CUBES
                player.cubesLeft = GetPlayerCubes(player).Length;

                //RELOAD
                if (player.shotsLeft <= 0 && player.cubesLeft > 0)
                {
                    DeleteRandomPlayerCube(player);
                    player.shotsLeft += Player.SHOTS_PER_RELOAD;
                }
                
                //UPDATE SHIELD
                //shield.transform.position = new Vector3(0, 100, 0);
                bool shielding = player.shielding;
                Shield shield = null; // = player.GetComponentInChildren<Shield>();
                //shield.gameObject.SetActive((bool)shielding);

                foreach (Transform child in player.transform)
                {
                    
                    shield = child.GetComponent<Shield>();
                    if (shield != null)
                        break;
                    
                }

                if (shield != null)
                {
                    //reload
                    if (player.shieldingTimeLeft <= 0 && GetPlayerCubes(player).Length > 0)
                    {
                        DeleteRandomPlayerCube(player);
                        player.shieldingTimeLeft += Player.SHIELDING_TIME;
                    }

                    if (!shielding || player.shieldingTimeLeft <= 0)
                    {
                        shield.transform.position = new Vector3(0, -300, 0);
                    }
                    else
                    {
                        shield.transform.position = player.transform.position;

                        Vector3 offset = new Vector3(Mathf.Cos(player.angle) * Player.shieldDistanceFromPlayer, 0.0f, Mathf.Sin(player.angle) * Player.shieldDistanceFromPlayer);
                        //shield.transform.position = transform.position;
                        shield.transform.position = shield.transform.position + offset;

                        float degrees = Mathf.Atan2(-offset.z, offset.x);
                        degrees *= (180 / Mathf.PI);
                        //this prevents from rotating wrong way

                        Vector3 rot = shield.transform.eulerAngles;
                        shield.transform.rotation = Quaternion.Euler(rot.x, 0, degrees);

                        player.shieldingTimeLeft -= Time.deltaTime;

                    }
                }
            }

            //update HUD
            //HUD hud = GameObject.Find("/HUD").GetComponent<HUD>();
            //hud.setCubes(childrenCubes.Count);

			/*
                mNextSpawn -= Time.deltaTime;
                if( mNextSpawn <= 0.0f )
                {
                    if (mObjects == null)
                    {
                        mObjects = new List<GameObject>();
                    }

                    int indexToSpawn = Random.Range(0, SpawnPrefabs.Length);
                    GameObject spawnObject = SpawnPrefabs[indexToSpawn];
                    GameObject spawnedInstance = Instantiate(spawnObject);
                    spawnedInstance.transform.parent = transform;
                    mObjects.Add(spawnedInstance);
                    mNextSpawn = TimeBetweenSpawns;
                }
            */

        }
    }

    private void BeginNewGame()
    {
        if (mObjects != null)
        {
            for (int count = 0; count < mObjects.Count; ++count)
            {
                Destroy(mObjects[count]);
            }
            mObjects.Clear();
        }

        mPlayer.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        mNextSpawn = TimeBetweenSpawns;
        mPlayer.enabled = true;
        mState = State.Playing;
    }

    private void EndGame()
    {
        mPlayer.enabled = false;
        mState = State.Paused;
    }

    private void ScreenManager_OnNewGame()
    {
        BeginNewGame();
    }

    private void ScreenManager_OnExitGame()
    {
        EndGame();
    }

	public void DoubleCubes(Player p)
	{
		//THIS SCRIPTS CONTROLS  T H E  C U B E S
		//COMPARED TO IT, YOU ARE NOTHING

        //for every cube, create a new one on top of it

        foreach (MagnetizedByPlayer cube in GetPlayerCubes(p))
        {
            GameObject spawnedInstance = Instantiate(SpawnPrefabs[1], transform);
            spawnedInstance.transform.position = cube.transform.position + new Vector3(0, cube.GetComponent<Renderer>().bounds.size.y, 0);
            spawnedInstance.GetComponent<Rigidbody>().velocity = cube.GetComponent<Rigidbody>().velocity;
        }
	}

    public MagnetizedByPlayer[] GetPlayerCubes(Player p)
    {
        MagnetizedByPlayer[] cubes = GetComponentsInChildren<MagnetizedByPlayer>();
        List<MagnetizedByPlayer> temp = new List<MagnetizedByPlayer>();
        foreach (MagnetizedByPlayer cube in cubes)
        {
            float dist = Vector3.Distance(p.transform.position, cube.transform.position);
            if (dist < cube.GetMagnetRange())
            {
                temp.Add(cube);
            }
        }

        return temp.ToArray();
    }

    public void DeleteRandomPlayerCube(Player p)
    {
        MagnetizedByPlayer[] cubes = GetPlayerCubes(p);

        int index = Random.Range(0, cubes.Length);
        //Destroy(cubes[index].gameObject);
        NetworkServer.Destroy(cubes[index].gameObject);
    }
}

/*
for (int i = 0; i < 20; i++) {
    int indexToSpawn = 1;
    GameObject spawnObject = SpawnPrefabs[indexToSpawn];
    GameObject spawnedInstance = Instantiate(spawnObject);
    spawnedInstance.transform.parent = transform;
    //mNextSpawn = TimeBetweenSpawns;
}
*/

/*
Vector3 offset = new Vector3(Mathf.Cos(angle) * shieldDistanceFromPlayer, 0.0f, Mathf.Sin(angle) * shieldDistanceFromPlayer);
//shield.transform.position = transform.position;
//shield.transform.position = shield.transform.position + offset;

float degrees = Mathf.Atan2(-offset.z, offset.x);
degrees *= (180 / Mathf.PI);
//this prevents from rotating wrong way

Vector3 rot = shield.transform.eulerAngles;
shield.transform.rotation = Quaternion.Euler(rot.x, 0, degrees);
*/