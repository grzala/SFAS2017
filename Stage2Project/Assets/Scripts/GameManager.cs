using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum State { Paused, Playing }

    [SerializeField]
    private GameObject [] SpawnPrefabs;

    [SerializeField]
    private Player PlayerPrefab;

    [SerializeField]
    private Arena Arena;

    [SerializeField]
    private float TimeBetweenSpawns;

    private List<GameObject> mObjects;
    private Player mPlayer;
    private State mState;
    private float mNextSpawn;

    private int cubesLeft;
    private 

    void Awake()
    {
        //mPlayer = Instantiate(PlayerPrefab);
		mPlayer = (Player)GameObject.Find("Player").GetComponent<Player>();
        mPlayer.transform.parent = transform;

		LevelCamera cam = FindObjectOfType<LevelCamera>();
		cam.enabled = true; //set as main
		cam.Follow(mPlayer.gameObject);

        ScreenManager.OnNewGame += ScreenManager_OnNewGame;
		ScreenManager.OnExitGame += ScreenManager_OnExitGame;

		/*
		for (int i = 0; i < 20; i++) {
			int indexToSpawn = 1;
			GameObject spawnObject = SpawnPrefabs[indexToSpawn];
			GameObject spawnedInstance = Instantiate(spawnObject);
			spawnedInstance.transform.parent = transform;
			//mNextSpawn = TimeBetweenSpawns;
		}
		*/
    }

    void Start()
    {
        Arena.Calculate();
		//mPlayer.enabled = false;
		//mState = State.Paused;
		mState = State.Playing;
    }

    void Update()
    {
        if( mState == State.Playing)
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

			if (Input.GetButtonDown("MouseClick")) {

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Input.mousePosition);
				Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);

				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 1000)) // spawn type one and delete type two
				{

                    for (int i = 0; i < childrenCubes.Count; i++)
                    {
                        MagnetizedByPlayer child = childrenCubes[i];
                        if (child.getMagnetType() == MagnetizedByPlayer.Type.Attract)
                        {
                            GameObject.Destroy(child.gameObject);
                            break;
                        }
                    }


					int indexToSpawn = 0;
                    GameObject spawnObject = SpawnPrefabs[indexToSpawn];
					GameObject spawnedInstance = Instantiate(spawnObject);


                    float height = spawnedInstance.GetComponent<Collider>().bounds.size.y;
                    Vector3 pos = new Vector3(hit.point.x, hit.point.y + height / 2, hit.point.z);
                    spawnedInstance.transform.position = pos;

                    spawnedInstance.transform.parent = transform; 
				}

            }

            //update HUD
            HUD hud = GameObject.Find("/HUD").GetComponent<HUD>();
            hud.setCubes(childrenCubes.Count);

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
