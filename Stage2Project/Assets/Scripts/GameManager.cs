using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public enum State { Paused, Playing, Finished }

    [SerializeField]
    private GameObject [] SpawnPrefabs;

    [SerializeField]
    private GameObject BulletPrefab;

    [SerializeField]
    private Player PlayerPrefab;

    [SerializeField]
    private GameObject PowerupPrefab;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Arena Arena;

    [SerializeField]
    private float TimeBetweenSpawns;

    private Dictionary<Player, int> scores = new Dictionary<Player, int>();
    private float nextScoreCount;
    private const float SCORE_COUNT_FREQUENCY = 1.5f;
    private int scoreGoal = 100;
    private int MAX_CUBES = 40;
    private int MAX_POWERUPS = 10;

    private List<GameObject> mObjects;
    private Player mPlayer;
    private State mState;
    private float mNextSpawn;

    private float mNextPowerupSpawn;
    private float TimeBetweenPowerupSpawns = 5.0f;

    private int cubesLeft;

    public List<Player> players = new List<Player>();

    private float timePassed = 0.0f;
    private float gameTime = 120.0f; //in seconds

    private bool initialized = false;

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

        mNextSpawn = TimeBetweenSpawns;
        mNextPowerupSpawn = TimeBetweenPowerupSpawns;


    }

    void Start()
    {
        //Arena.Calculate();
		//mPlayer.enabled = false;
		//mState = State.Paused;
        mState = State.Playing;
        nextScoreCount = SCORE_COUNT_FREQUENCY;


    }

    private void DeleteInactivePlayers()
    {
        //delete disconnected players
        foreach (Player p in players)
        {
            if (p == null)
                players.Remove(p);
        }
    }

    private void UpdateScores()
    {
        //UpdatePoints
        nextScoreCount -= Time.deltaTime;
        if (nextScoreCount <= 0)
        {
            nextScoreCount = SCORE_COUNT_FREQUENCY;
            foreach (Player player in players)
            {
                if (player == null)
                {
                    if (scores.ContainsKey(player))
                    {
                        scores.Remove(player);
                    }
                    continue;
                }
                if (!scores.ContainsKey(player))
                {
                    scores[player] = 0;
                }

                scores[player] += GetPlayerCubes(player).Length;
                player.score = scores[player];

                if (scores[player] > scoreGoal)
                    EndGame();
            }
        }
    }

    private void UpdatePlayers()
    { 
        foreach (Player player in players)
        {
            if (player == null)
            {
                continue;
            }

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
    }

    private void UpdateCubes()
    {
        MagnetizedByPlayer[] cubes = GetAllCubes();
        foreach (MagnetizedByPlayer cube in cubes)
        {
            cube.UpdateMagnet(players);
        }

    }

    private void SpawnCubesAndPowerups()
    {
        mNextSpawn -= Time.deltaTime;
        mNextPowerupSpawn -= Time.deltaTime;
        if (mNextSpawn <= 0.0f && GetAllCubes().Length < MAX_CUBES)
        {
            if (mObjects == null)
            {
                mObjects = new List<GameObject>();
            }

            GameObject spawnObject = SpawnPrefabs[1];
            GameObject spawnedInstance = Instantiate(spawnObject);
            spawnedInstance.transform.parent = transform;
            SetRandomPos(spawnedInstance);

            mObjects.Add(spawnedInstance);
            mNextSpawn = TimeBetweenSpawns;
            NetworkServer.Spawn(spawnedInstance);   
        }

        if (mNextPowerupSpawn <= 0.0f && GetComponentsInChildren<Powerup>().Length < MAX_POWERUPS)
        {
            GameObject powerupSpawn = PowerupPrefab;
            Powerup powerupInstance = (Powerup)Instantiate(powerupSpawn).GetComponent<Powerup>();
            powerupInstance.transform.parent = transform;

            powerupInstance.GetComponent<Powerup>().SetRandomType();

            SetRandomPos(powerupInstance.gameObject);

            mNextPowerupSpawn = TimeBetweenPowerupSpawns;

            NetworkServer.Spawn(powerupInstance.gameObject);
            powerupInstance.RpcUpdateRenderTypes();
            //mObjects.Add(powerupInstance);


        }
    }

    void Update()
    {
        //run this only on server
        if (isLocalPlayer || !isServer)
            return;

        //print(isServer);

        DeleteInactivePlayers();

        if (mState == State.Playing)
        {
            if (!initialized)
            {

                initialized = true;
            }

            UpdateScores();

            UpdatePlayers();

            UpdateCubes();

           
            //update HUD
            //HUD hud = GameObject.Find("/HUD").GetComponent<HUD>();
            //hud.setCubes(childrenCubes.Count);

            SpawnCubesAndPowerups();

            timePassed += Time.deltaTime;

            if (timePassed >= gameTime)
            {
                mState = State.Finished;

                //GameObject.Find("ScreenManager").GetComponent<ScreenManager>().OnGameEnd();
                //Text resultText = (Text)GameObject.Find("Results").GetComponent<Text>();

                List<string> winners = new List<string>();
                Dictionary<string, int> playerPoints = new Dictionary<string, int>();

                foreach (Player p in players)
                {
                    winners.Add(p.name);
                    playerPoints.Add(p.name, scores[p]);
                }

                winners.Sort((x, y) => playerPoints[y].CompareTo(playerPoints[x]));
                string results = "";
                for (int i = 0; i < winners.Count; i++)
                {
                    results += (i + 1) + ". " + winners[i] + " scored: " + playerPoints[winners[i]];
                    if (i < winners.Count - 1)
                    {
                        results += "\n";
                    }
                }

                foreach (Player p in players)
                {
                    p.RpcDisplayResults(results);
                }

                //resultText.text = results;
            
            }
            
        }
        else if (mState == State.Finished)
        {

        }
    }

    public void SetPlayerPositions()
    {
        Transform[] positions = new Transform[4];
        GameObject SpawnPositions = GameObject.Find("SpawnPositions");
        for (int i = 0; i < SpawnPositions.transform.childCount; i++)
        {
            positions[i] = SpawnPositions.transform.GetChild(i);
        }

        print(players.Count);

        int spawned = 0;
        foreach (Player player in players)
        {
            player.RpcSetPos(positions[spawned].position);
            spawned++;
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

        //mPlayer.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        mNextSpawn = TimeBetweenSpawns;
        //mPlayer.enabled = true;
        mState = State.Playing;
    }

    private void EndGame()
    {
        //mPlayer.enabled = false;
        //mState = State.Paused;
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
            spawnedInstance.transform.position = new Vector3(0.0f, 4.0f, 0.0f);
            spawnedInstance.transform.position = cube.transform.position + new Vector3(0, cube.GetComponent<Renderer>().bounds.size.y, 0);
            spawnedInstance.GetComponent<Rigidbody>().velocity = cube.GetComponent<Rigidbody>().velocity;
        }
	}

    public void HalveCubes(Player p)
    {
        //THIS SCRIPTS CONTROLS  T H E  C U B E S
        //COMPARED TO IT, YOU ARE NOTHING

        MagnetizedByPlayer[] cubes = GetPlayerCubes(p);
        int cubesNo = cubes.Length;

        if (cubesNo < 2) return;

        List<int> todestroy = new List<int>();
        while (todestroy.Count < cubesNo / 2)
        {
            int rand = Random.Range(0, cubesNo);

            if (!todestroy.Contains(rand))
                todestroy.Add(rand);
        }

        foreach (int i  in todestroy)
        {
            Destroy(cubes[i].gameObject);
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

    public MagnetizedByPlayer[] GetAllCubes()
    {
        return GetComponentsInChildren<MagnetizedByPlayer>();
    }

    public void DeleteRandomPlayerCube(Player p)
    {
        MagnetizedByPlayer[] cubes = GetPlayerCubes(p);

        int index = Random.Range(0, cubes.Length);
        //Destroy(cubes[index].gameObject);
        NetworkServer.Destroy(cubes[index].gameObject);
    }

    //test again all transform children and also arena walls
    //cannot spawn inside walls
    public void SetRandomPos(GameObject obj)
    {
        do
        {
            Vector2 pos = Arena.GetRandomAvailableSpawnPoint();
            obj.transform.position = new Vector3(pos.x, obj.transform.position.y, pos.y);

        } while(!CanSpawn(obj));
    }

    public bool CanSpawn(GameObject obj)
    {
        bool canSpawn = true;

        List<GameObject> collidableObjects = Arena.GetAllWalls();
        List<GameObject> objectsInGame = new List<GameObject>();


        foreach (Transform t in transform)
        {
            objectsInGame.Add(t.gameObject);
        }

        collidableObjects.AddRange(objectsInGame);

        foreach (GameObject obj2 in collidableObjects)
        {
            if (obj2 == obj)
                continue;

            if (obj2.GetComponent<Collider>().bounds.Intersects(obj.GetComponent<Collider>().bounds))
            {
                canSpawn = false;
                break;
            }
        }

        return canSpawn;
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