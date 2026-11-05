using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefabs;

    //Target prefab spwanning inspector area set up
    [Header("Target Spawns")]
    [SerializeField] private GameObject targetPrefab;
    [Tooltip("Target prefabs. Singular targets are picket at random for each spawn")]
    //Target prefab array lsit
    [SerializeField] private List<spawnTargets> targetTable = new List<spawnTargets>();










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

        Instantiate(targetPrefabs, spawnPosition, Quaternion.identity);
    }

}
