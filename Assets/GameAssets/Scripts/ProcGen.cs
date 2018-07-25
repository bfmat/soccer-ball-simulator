using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProcGen : MonoBehaviour {

	[SerializeField] private GameObject _2x2Block, explosiveBlock, gravityBlock, disappearingBlock, turret, powerUp, tree, bullet, pig, winLocationParticleSystem, coin;
    private short difficulty;
	private int numberOfBlocks;
    private List<Vector3> blockLocations;
	private List<GameObject> blocks;
    private List<Vector3> rotatingPlatformLocations;
    private List<PlayerTriggeredEvent> events;
    public const string seed = "Seed", diff = "Difficulty";
    public static Vector3 winLocation;
    public static int totalCoins;
    private int minBlocksPerPlatform;
    private float probMovingPlatform, probStartingBlock, probGravityBlock, probRotatingPlatform, probPowerUp, probTree, probPTE, probPig, probCoin;

	void Awake() {
        difficulty = LoadProceduralGenerator.difficulty;
        print(LoadProceduralGenerator.difficulty);
        totalCoins = 0;
        numberOfBlocks = Mathf.RoundToInt(Mathf.Log(difficulty, 1.01f));
        minBlocksPerPlatform = 30 - Mathf.RoundToInt(0.1f * difficulty);
        probMovingPlatform = Mathf.Pow(difficulty, 1 / 3) * 0.01f;
        probStartingBlock = (0.0006f * difficulty) + 0.05f;
        probGravityBlock = (0.0002f * difficulty) + 0.01f;
        probRotatingPlatform = difficulty * 0.001f;
        probPowerUp = 0.01f;
        probTree = 0.05f;
        probPTE = probRotatingPlatform * 1.033f;
        probPig = probPTE * 1.033f;
        probCoin = 0.05f;

        if (PlayerPrefs.HasKey(seed))
        {
            UnityEngine.Random.seed = PlayerPrefs.GetInt(seed);
        } else
        {
            PlayerPrefs.SetInt(seed, UnityEngine.Random.seed);
            UnityEngine.Random.seed = PlayerPrefs.GetInt(seed);
        }
        PlayerPrefs.SetInt(diff, difficulty);
        blocks = new List<GameObject>();
        blockLocations = new List<Vector3>();
        rotatingPlatformLocations = new List<Vector3>();
        events = new List<PlayerTriggeredEvent>();
		GenerateStartingBlock (Vector3.zero);
        winLocation = blocks[blocks.Count - 1].transform.position;
        Instantiate(winLocationParticleSystem, winLocation, Quaternion.identity);
        StartCoroutine(HandlePlayerTriggeredEvents());
	}

	private void GenerateStartingBlock(Vector3 position) {
        ParentBlocks();
		GameObject startingBlock = Instantiate (_2x2Block);
		startingBlock.transform.position = position;
		startingBlock.transform.rotation = Quaternion.identity;
		blocks.Add (startingBlock);
        blockLocations.Add(startingBlock.transform.position);
		RecursivelyGenerateNextBlocks (startingBlock);
	}

	private void RecursivelyGenerateNextBlocks(GameObject block) {
		if (blocks.Count > numberOfBlocks) {
            ParentBlocks();
			return;
		}
        bool enoughBlocks = (CountUnparentedBlocks() >= minBlocksPerPlatform);
        float r = UnityEngine.Random.value;
        bool farEnoughFromOrigin = Vector3.Distance(block.transform.position, Vector3.zero) > 35;
        if (r < probMovingPlatform && farEnoughFromOrigin && enoughBlocks && CountUnparentedBlocks() < blocks.Count)
        {
            block = GenerateMovingPlatform(block);
        }
        else if (r < probMovingPlatform && enoughBlocks)
        {
            GenerateStartingBlock(block.transform.position + new Vector3(UnityEngine.Random.value * 10, UnityEngine.Random.value * 2, UnityEngine.Random.value * 10));
            return;
        }
        else if (r < probGravityBlock && farEnoughFromOrigin)
        {
            GenerateGravityBlock(block);
        } else if (r < probRotatingPlatform && enoughBlocks && farEnoughFromOrigin && CountUnparentedBlocks() < blocks.Count)
        {
            GenerateRotatingPlatform(block);
        } else if (UnityEngine.Random.value < probTree && farEnoughFromOrigin)
        {
            GenerateTree(block);
        } else if (r < probPTE && farEnoughFromOrigin)
        {
            GeneratePlayerTriggeredEvent(block);
        } else if (r < probPig)
        {
            GeneratePig();
        }
        if (UnityEngine.Random.value < probCoin)
        {
            GenerateCoin(block);
        }
        if (UnityEngine.Random.value < probPowerUp)
        {
            GeneratePowerUp(block);
        }
        GameObject newBlock = Generate2x2Block (block);
		blocks.Add(newBlock);
        blockLocations.Add(newBlock.transform.position);
		RecursivelyGenerateNextBlocks (newBlock);
	}

    private void ParentBlocks()
    {
        GameObject g = new GameObject();
        foreach (GameObject h in blocks)
        {
            if (h.transform.parent == null)
                h.transform.SetParent(g.transform, true);
        }
    }

    private GameObject GenerateMovingPlatform(GameObject block)
    {
        ParentBlocks();
        Vector3 endPoint = block.transform.position + new Vector3(UnityEngine.Random.value * 30, UnityEngine.Random.value * 1, UnityEngine.Random.value * 30);
        GameObject parent = block.transform.parent.gameObject;
        DynamicEnvironmentElement dyn = parent.AddComponent<DynamicEnvironmentElement>();
        dyn.translationWaypoints = new List<Vector3>();
        dyn.translationWaypoints.Add(endPoint);
        dyn.waypointCyclingVelocity = UnityEngine.Random.value * 6f;
        dyn.requiredWaypointAccuracy = dyn.waypointCyclingVelocity / 10f;
        GameObject temp = Instantiate(_2x2Block);
        temp.transform.position = endPoint;
        return temp;
    }

    private void GenerateRotatingPlatform(GameObject block)
    {
        foreach (Vector3 v in rotatingPlatformLocations)
        {
            if (Vector3.Distance(v, block.transform.position) < 30)
            {
                return;
            }
        }
        rotatingPlatformLocations.Add(block.transform.position);
        ParentBlocks();
        GameObject parent = block.transform.parent.gameObject;
        DynamicEnvironmentElement dyn = parent.AddComponent<DynamicEnvironmentElement>();
        float rotationCoefficient = UnityEngine.Random.value * 10;
        dyn.rotPerSecond = new Vector3((UnityEngine.Random.value - 0.5f) * rotationCoefficient, (UnityEngine.Random.value - 0.5f) * rotationCoefficient, (UnityEngine.Random.value - 0.5f) * rotationCoefficient);
        dyn.translationWaypoints = new List<Vector3>();
    }

	private GameObject Generate2x2Block(GameObject block) {
		float r = UnityEngine.Random.value;
		Vector3 direction = Vector3.zero;
		if (r > 0.6)
			direction = Vector3.forward;
		else if (r > 0.3)
			direction = Vector3.left;
		else
			direction = Vector3.right;
		Vector3 pos = block.transform.position + (2 * direction);
		if (blockLocations.Contains(pos))
			return Generate2x2Block(block);
        GameObject ret;
        if (UnityEngine.Random.value > 0.99) ret = Instantiate(explosiveBlock);
        else if (UnityEngine.Random.value > 0.9) ret = Instantiate(disappearingBlock);
        else
        {
            ret = Instantiate(_2x2Block);
            ret.transform.position = pos + new Vector3(0f, UnityEngine.Random.value / 2, 0f);
            return ret;
        }
		ret.transform.position = pos;
		return ret;
	}

    private void GenerateGravityBlock(GameObject block)
    {
        GameObject ret;
        if (UnityEngine.Random.value > 0.5f) ret = Instantiate(gravityBlock);
        else ret = Instantiate(turret);
        ret.transform.position = block.transform.position + (GravityBlockDirection() * 8);
        if (blockLocations.Contains(ret.transform.position)) Destroy(ret);
        else
        {
            blocks.Add(ret);
            blockLocations.Add(ret.transform.position);
        }
    }

    private Vector3 GravityBlockDirection()
    {
        float r = UnityEngine.Random.value;
        Vector3 direction = Vector3.zero;
        if (r > 0.7)
            direction = Vector3.up;
        else if (r > 0.5)
            direction = Vector3.left;
        else if (r > 0.3)
            direction = Vector3.right;
        else
            direction = Vector3.forward;
        if (UnityEngine.Random.value > 0.2)
        {
            direction += GravityBlockDirection();
        }
        return direction;
    }

    private void GeneratePowerUp(GameObject block)
    {
        GameObject temp = Instantiate(powerUp);
        temp.GetComponent<PowerUp>().powerUp = (PowerUpType) UnityEngine.Random.Range(1, Enum.GetNames(typeof(PowerUpType)).Length);
        temp.transform.position = block.transform.position + (Vector3.up * 3);
    }

    private void GenerateTree(GameObject block)
    {
        Vector3 topBlockCoords = block.transform.position;
        Vector3 blockPos = topBlockCoords;
        foreach (GameObject b in blocks)
        {
            if (b.transform.position.x == blockPos.x && b.transform.position.z == blockPos.z && b.transform.position.y > blockPos.y)
            {
                topBlockCoords = b.transform.position;
            }
        }
        topBlockCoords += Vector3.up * 4;
        GameObject temp = (GameObject) Instantiate(tree, topBlockCoords, Quaternion.identity);
        blocks.Add(temp);
        blockLocations.Add(temp.transform.position);
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey(seed);
        PlayerPrefs.DeleteKey(diff);
    }

    private void GeneratePlayerTriggeredEvent(GameObject block)
    {
        PlayerTriggeredEvent temp = new PlayerTriggeredEvent();
        temp.type = (PlayerTriggeredEventType) UnityEngine.Random.Range(0, Enum.GetNames(typeof(PlayerTriggeredEventType)).Length);
        temp.minimumPositionForTrigger = new Vector2(block.transform.position.x, block.transform.position.z);
        temp.blockIndex = blocks.IndexOf(block);
        temp.timeSpan = UnityEngine.Random.Range(1, 10);
        events.Add (temp);
    }

    private IEnumerator HandlePlayerTriggeredEvents()
    {
        List<PlayerTriggeredEvent> activatedEvents = new List<PlayerTriggeredEvent>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        while (true)
        {
            foreach (PlayerTriggeredEvent e in events)
            {
                if (player.transform.position.x >= e.minimumPositionForTrigger.x && player.transform.position.z >= e.minimumPositionForTrigger.y && e.activated == false)
                {
                    activatedEvents.Add(e);
                    e.activated = true;
                }
            }

            for (int i = activatedEvents.Count - 1; i >= 0; i--)
            {
                PlayerTriggeredEvent a = activatedEvents[i];
                a.timeActive++;
                if (a.type == PlayerTriggeredEventType.DisappearingWorld)
                {
                    GameObject nextBlock = blocks[a.blockIndex + (a.timeActive)];
                    try
                    {
                        DisappearingBlock db = nextBlock.AddComponent<DisappearingBlock>();
                        StartCoroutine(db.ShrinkAndDisappear());
                    }
                    catch { }
                } else if (a.type == PlayerTriggeredEventType.BulletRain)
                {
                    Instantiate(bullet, player.transform.position + new Vector3(0f, 10f, 0f), Quaternion.identity);
                }

                if (a.timeActive >= a.timeSpan)
                {
                    activatedEvents.Remove(a);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void GeneratePig()
    {
        Instantiate(pig, new Vector3(UnityEngine.Random.value * numberOfBlocks, UnityEngine.Random.value * numberOfBlocks, UnityEngine.Random.value * numberOfBlocks), Quaternion.identity);
    }

    private int CountUnparentedBlocks()
    {
        int number = 0;
        foreach (GameObject block in blocks)
        {
            if (block.transform.parent == null) number++;
        }
        return number;
    }

    private void GenerateCoin(GameObject block)
    {
        GameObject c = Instantiate(coin);
        c.transform.position = block.transform.position + (Vector3.up * 2);
        blocks.Add(c);
        blockLocations.Add(c.transform.position);
        totalCoins++;
    }

    private class PlayerTriggeredEvent
    {
        public PlayerTriggeredEventType type;
        public Vector2 minimumPositionForTrigger;
        public int timeSpan;
        public int timeActive = 0;
        public bool activated = false;
        public int blockIndex;
    }

    private enum PlayerTriggeredEventType
    {
        DisappearingWorld,
        BulletRain
    }
}