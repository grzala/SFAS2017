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
                /*
                if (player == null)
                    continue;
                

                //shield.transform.position = new Vector3(0, 100, 0);
                bool shielding = player.shielding;
                print(shielding);
                Shield shield = player.GetComponentInChildren<Shield>();
                //shield.gameObject.SetActive((bool)shielding);

                if (shielding)
                {
                    shield.gameObject.SetActive(true);
                }*/
            }

			/*if (Input.GetButtonDown("MouseClick")) {

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Input.mousePosition);
				Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);

				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 1000)) // spawn type one and delete type two
				{
					print ("game");
					print (hit.point);
                    for (int i = 0; i < childrenCubes.Count; i++)
                    {
                        MagnetizedByPlayer child = childrenCubes[i];
                        if (child.getMagnetType() == MagnetizedByPlayer.Type.Attract)
                        {
                            //GameObject.Destroy(child.gameObject);
                            break;
                        }
                    }

                    Vector2 shootAngle = new Vector2(hit.point.x - mPlayer.transform.position.x, hit.point.z - mPlayer.transform.position.z);
                    float angle = Mathf.Atan2(shootAngle.y, shootAngle.x);

                    mPlayer.shoot(angle);

				}

            }*/

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