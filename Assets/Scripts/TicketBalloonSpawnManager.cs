using System.Collections;
using UnityEngine;

public class TicketBalloonSpawnManager : MonoBehaviour
{
    public GameObject[] TicketBalloonTypes;

    public float TicketBalloonSpawnTimer = 6f;
    public float TicketBalloonDespawnTimer = 2f;

    [Tooltip("Max balloons spawned per round. 0 = keep spawning until the round ends.")]
    public int maxBalloonsPerRound = 0;

    void Start()
    {
        //Starts Balloon Spawning
        StartCoroutine(DelayedTicketBalloonSpawner());
    }

    private IEnumerator DelayedTicketBalloonSpawner()
    {
        if (TicketBalloonTypes == null || TicketBalloonTypes.Length == 0)
        {
            Debug.LogWarning("TicketBalloonSpawnManager: no balloon prefabs assigned.");
            yield break;
        }

        int spawned = 0;

        while (maxBalloonsPerRound <= 0 || spawned < maxBalloonsPerRound)
        {
            //Wait before each spawn
            yield return new WaitForSeconds(TicketBalloonSpawnTimer);

            //Randomly picks one of the balloon prefabs and spawns it at a random location
            GameObject clone = Instantiate(
                TicketBalloonTypes[Random.Range(0, TicketBalloonTypes.Length)],
                new Vector2(Random.Range(-1f, 1f), Random.Range(-3f, -1f)),
                Quaternion.identity
            );
            spawned++;

            //Balloon despawns on its own if nobody pops it in time.
            //If it's popped first, TicketBalloon.Pop() destroys it early.
            Destroy(clone, TicketBalloonDespawnTimer);

            yield return new WaitForSeconds(3);
        }
    }
}