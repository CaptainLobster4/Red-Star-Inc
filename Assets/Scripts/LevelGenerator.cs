using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class spawnTargets
{
    public string targetName;
    public GameObject prefab;
    [UnityEngine.Range(0f, 1f)] public float spawnChance = 0.45f;

}
public class LevelGenerator : MonoBehaviour
{

    //Target prefab spwanning inspector area set up
    [Header("Target Spawns")]
    [SerializeField] private GameObject targetPrefab;
    [Tooltip("Target prefabs. Singular targets are picket at random for each spawn")]


    //Target prefab array lsit
    [SerializeField] private List<spawnTargets> targetTable = new List<spawnTargets>();

    //Where to spawn logic
    //Target Spawn area
    // --------- [SerializeField] private Vector2 areaSize = new Vector2 

    //Debug-ing optionals

    [Header("Debug options")]
    [SerializeField] private bool startOnPlay = true;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    //Target Spawner Logic
    public void SpawnTargets()
    {
        //Sets the bounds in which a target can spawn
        int spawnPointX = Random.Range(-5, 5);
        int spawnPointY = Random.Range(0, 10);


        Vector2 spawnPosition = new Vector2(spawnPointX, spawnPointY);

        Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
    }

}
