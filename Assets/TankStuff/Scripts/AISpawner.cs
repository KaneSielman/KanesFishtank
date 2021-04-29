using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[System.Serializable]
public class AIObjects
{
    //-------->
    //declare our variables
    public string AIGroupName { get { return m_aiGroupName; } }
    public GameObject objectPrefab { get { return m_prefab; } }
    public int maxAI { get { return m_maxAI; } }
    public int spawnAmount { get { return m_maxSpawnAmount; } }
    public bool randomizeStats { get { return m_randomizeStats; } }
    public bool enableSpawner { get { return m_enableSpawner; } }
    public bool useRandomTarget { get { return m_useRandomTarget; } }

    //serliaze the private variables
    [Header("AI Group Stats")]
    [SerializeField]
    private string m_aiGroupName;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    [Range(0f, 40f)]
    private int m_maxAI;
    [SerializeField]
    [Range(0f, 10f)]
    private int m_maxSpawnAmount;

    //new variables
    [Header("Main Settings")]
    [SerializeField]
    private bool m_enableSpawner;
    [SerializeField]
    private bool m_randomizeStats;
    [SerializeField]
    private bool m_useRandomTarget;

    public AIObjects(string Name, GameObject Prefab, int MaxAI, int SpawnAmount, bool RandomizeStats)
    {
        this.m_aiGroupName = Name;
        this.m_prefab = Prefab;
        this.m_maxAI = MaxAI;
        this.m_maxSpawnAmount = SpawnAmount;
        this.m_randomizeStats = RandomizeStats;
    }

    public void setValues(int MaxAI, int SpawnAmount)
    {
        this.m_maxAI = MaxAI;
        this.m_maxSpawnAmount = SpawnAmount;
    }
}

public class AISpawner : MonoBehaviour
{

    //-------->
    //declare our variables

    //note: using list because we dont know the size of it, array would need to set size first
    public List<Transform> Waypoints = new List<Transform>();

    public float spawnTimer { get { return m_SpawnTimer; } } //global value for how often we run the spawner
    public Vector3 spawnArea { get { return m_SpawnArea; } }
    //serliaze the private variables
    [Header("Global Stats")]
    [Range(0f, 600f)]
    [SerializeField]
    private float m_SpawnTimer; //global value for how often we run the spawner
    [SerializeField]
    private Color m_SpawnColor = new Color(1.000f, 0.000f, 0.000f, 0.300f); //use the colour for the gizmo - set nice red default
    [SerializeField]
    private Vector3 m_SpawnArea = new Vector3(20f, 10f, 20f);



    //create array from new class
    [Header("AI Groups Settings")]
    public AIObjects[] AIObject = new AIObjects[5];


    // Use this for initialization
    void Start()
    {
        GetWaypoints();
        RandomiseGroups();
        CreateAIGroups();
        InvokeRepeating("SpawnNPC", 0.5f, spawnTimer);
    }

    void SpawnNPC()
    {
        //loop through all of the AI groups
        for (int i = 0; i < AIObject.Count(); i++)
        {
            //check to make sure spawner is enabled
            if (AIObject[i].enableSpawner && AIObject[i].objectPrefab != null)
            {
                //make sure that AI group doesnt have max NPCs //need a better way to find the GO for the AI Group
                GameObject tempGroup = GameObject.Find(AIObject[i].AIGroupName);
                if (tempGroup.GetComponentInChildren<Transform>().childCount < AIObject[i].maxAI)
                {
                    //spawn random number of NPCs from 0 to Max Spawn Amount
                    for (int y = 0; y < Random.Range(0, AIObject[i].spawnAmount); y++)
                    {
                        //get random rotation
                        Quaternion randomRotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(0, 360), 0);
                        //create spawned gameobject
                        GameObject tempSpawn;
                        tempSpawn = Instantiate(AIObject[i].objectPrefab, RandomPosition(), randomRotation);
                        //put spawned NPC as child of group
                        tempSpawn.transform.parent = tempGroup.transform;
                        //Add the AIMove script and class to the new NPC
                        tempSpawn.AddComponent<AIMove>();
                    }
                }
            }
        }
    }

    //public method for Random Position within the Spawn Area
    public Vector3 RandomPosition()
    {
        //get a random position within our Spawn Area
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z)
         );
        randomPosition = transform.TransformPoint(randomPosition * .5f);
        return randomPosition;
    }

    //public method for getting a Random Waypoint
    public Vector3 RandomWaypoint()
    {
        int randomWP = Random.Range(0, (Waypoints.Count - 1));
        Vector3 randomWaypoint = Waypoints[randomWP].transform.position;
        return randomWaypoint;
    }

    //Method for putting random values in the AI Group setting
    void RandomiseGroups()
    {
        //randomise
        for (int i = 0; i < AIObject.Count(); i++)
        {
            //if array item has randomizeStats ticked true
            if (AIObject[i].randomizeStats)
            {
                //using constructor 
                //AIObject[i] = new AIObjects(AIObject[i].AIGroupName, AIObject[i].objectPrefab, Random.Range(1, 30), Random.Range(1, 10), AIObject[i].randomizeStats);

                //using method
                AIObject[i].setValues(Random.Range(1, 30), Random.Range(1, 10));
            }
        }
    }

    //Method for creating the empty worldobject groups
    void CreateAIGroups()
    {
        for (int i = 0; i < AIObject.Count(); i++)
        {
            //Empty Game Object to keep our AI in
            GameObject AIGroupSpawn;

            //check to make sure Group has a name
            if (AIObject[i].AIGroupName != null)
            {
                //create a new game object            
                AIGroupSpawn = new GameObject(AIObject[i].AIGroupName);
                AIGroupSpawn.transform.parent = this.gameObject.transform;
            }
        }
    }

    void GetWaypoints()
    {
        // list using standard library      
        // look through nested children 
        Transform[] wpList = this.transform.GetComponentsInChildren<Transform>(); //note: 'this' is not required any more 
        for (int i = 0; i < wpList.Length; i++)
        {
            if (wpList[i].tag == "waypoint")
            {
                //add to the list
                Waypoints.Add(wpList[i]);
            }
        }
    }

    //show the gizmos in colour
    void OnDrawGizmosSelected()
    {
        Gizmos.color = m_SpawnColor;
        Gizmos.DrawCube(transform.position, spawnArea);
    }

}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[System.Serializable]
public class AIObjects 
{
    //Declaration of Variables
    public string AIGroupName { get { return m_aiGroupName;  } }
    public GameObject objectPrefab { get { return m_prefab; } }
    public int maxAI { get { return m_maxAI; } }
    public int spawnRate { get { return m_spawnRate; } }
    public int spawnAmount { get { return m_maxSpawnAmount; } }
    public bool randomizeStats { get { return m_randomizeStats; } }
    public bool enableSpawner { get { return m_enableSpawner;  } }

    //Serialize the private Variables
    [Header ("AI Group Stats")]
    [SerializeField]
    private string m_aiGroupName;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    [Range(0f, 40f)]
    private int m_maxAI;
    [SerializeField]
    [Range(0f, 40f)]
    private int m_spawnRate;
    [SerializeField]
    [Range(0f, 10f)]
    private int m_maxSpawnAmount;

    [Header("Main Settings")]
    [SerializeField]
    private bool m_enableSpawner;
    [SerializeField]
    private bool m_randomizeStats;

    //Constructor
    public AIObjects(string Name, GameObject Prefab, int MaxAI, int SpawnRate, int SpawnAmount, bool RandomizeStats)
    {
        m_aiGroupName = Name;
        m_prefab = Prefab;
        m_maxAI = MaxAI;
        m_spawnRate = SpawnRate;
        m_maxSpawnAmount = SpawnAmount;
        m_randomizeStats = RandomizeStats;
    }
}


public class AISpawner : MonoBehaviour { 

    //Declatrion of Variables

    //List of New Waypoints. Keeping as a list as I do not have a set size. This allows Easy expansion of the game
    public List<Transform> Waypoints = new List<Transform>();

    public float spawnTimer { get { return m_SpawnTimer; } }
    public Vector3 spawnArea { get { return m_SpawnArea; } }

    //Serialize the Private Variables
    [Header("Global Stats")]
    [Range(0f, 600f)]
    [SerializeField]
    private float m_SpawnTimer; // How long the spawner is ran
    [SerializeField]
    private Color m_SpawnColor = new Color(1.000f, 0.000f, 0.000f, 0.300f); //Set a default color
    [SerializeField]
    private Vector3 m_SpawnArea = new Vector3(0f, 0f, 0f);

    //Create array from class
    [Header ("AI Groups Settings")]
    public AIObjects[] AIObject = new AIObjects[5];

    // Start is called before the first frame update
    void Start()
    {
        GetWaypoints();
        RandomsiseGroups();
        CreateAIGroups();
        InvokeRepeating("SpawnNPC", 0.5f, spawnTimer); //Spawn half a second after start
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnNPC()
    { 
    //Set to loop through all AI Groups
    for (int i = 0; i < AIObject.Count(); i++)
        {
            //Check spaner is enabled
            if (AIObject[i].enableSpawner && AIObject[i].objectPrefab != null)
            {
                //Check AI group does not have max NPC already
                GameObject tempGroup = GameObject.Find(AIObject[i].AIGroupName);
                if(tempGroup.GetComponentInChildren<Transform>().childCount < AIObject[i].maxAI)
                {
                    //Spawn number of NPC (Ranged to Max amount)
                    for (int y = 0; y < Random.Range(0,AIObject[i].spawnAmount); y++)
                    {
                        //Random Rotation
                        Quaternion randomRotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(0, 360), 0);
                        //Spawn game object
                        GameObject tempSpawn;
                        tempSpawn = Instantiate(AIObject[i].objectPrefab, RandomPosition(), randomRotation);
                        //put spaned NPC as child of group
                        tempSpawn.transform.parent = tempGroup.transform;
                        //Add AIMove scrip and clss to the new NPC
                        tempSpawn.AddComponent<AIMove>();
                    }

                }
            }
        }
    }
    //Public method to create a random position
    public Vector3 RandomPosition()
    {
        //Gets random position within spawn area
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z)
            );
        randomPosition = transform.TransformPoint(randomPosition * .5f);
        return randomPosition;
    }
    //Public method for getting a random waypoint
    public Vector3 RandomWaypoint()
    {
        int randomWP = Random.Range(0, (Waypoints.Count - 1));
        Vector3 randomWaypoint = Waypoints[randomWP].transform.position;
        return randomWaypoint;
    }
    //Test methods for putting random values into the AI Group Settings
    void RandomsiseGroups()
    {
        //Randomise
        for (int i = 0; i < AIObject.Count(); i++)
        {
            //if array item has randomizeStats ticked true
            if (AIObject[i].randomizeStats)
            {
                AIObject[i] = new AIObjects(AIObject[i].AIGroupName, AIObject[i].objectPrefab, Random.Range(1, 30), Random.Range(1, 20), Random.Range(1, 10), AIObject[i].randomizeStats);
            }
        }
    
    }
    void GetWaypoints() 
    {
        //Create list using standard library 
        //Loops through nested Children
        Transform[] wpList = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < wpList.Length; i++)
        {
            if (wpList[i].tag == "waypoint")
            {
                //Adds Waypoint to list
                Waypoints.Add(wpList[i]);
            } 
        }
    }
    //Shows the gixmos in colour
    void onDrawGizmosSelected()
    {
        Gizmos.color = m_SpawnColor;
        Gizmos.DrawCube(transform.position, spawnArea);

    }

    //Create empty world object groups
    void CreateAIGroups() 
    { 
        for (int i = 0; i < AIObject.Count(); i++)
        {
            //Empty Game Object to keep game AI in
            GameObject m_AIGroupSpawn;
            
            //Create a new game object
            m_AIGroupSpawn = new GameObject(AIObject[i].AIGroupName);
            m_AIGroupSpawn.transform.parent = gameObject.transform;
        }
    }
}
*/