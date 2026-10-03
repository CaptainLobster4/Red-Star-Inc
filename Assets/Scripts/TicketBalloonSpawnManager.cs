using System.Collections;
using UnityEngine;

public class TicketBalloonSpawniManager : MonoBehaviour
{
    public GameObject[] TicketBalloonTypes;
    public GameObject CloneActor;

    public float Duration=3f;
    public float TicketsGiven;
    public float TicketBalloonSpawnTimer = 6f;
    public float TicketBalloonDespawnTimer = 2f;
    



    void Start() 
   {

        //Starts Balloon Spawning
        StartCoroutine(DelayedTicketBalloonSpawner());
       

       

   }

    private IEnumerator DelayedTicketBalloonSpawner()
    {
        int chances = 3;

        while (chances > 0)
        {
            //Wait for 3 seconds to start
            //Clones ticket balloon based on array of balloons given
            //Randomizes between the 2 options
            //Spawns at a random location
            //Keeps transformation rotation
            //Destroys after despawn float ammount
            yield return new WaitForSeconds(TicketBalloonSpawnTimer);

            GameObject clone = Instantiate(
            TicketBalloonTypes
            [Random.Range(0,
           TicketBalloonTypes.Length)],
            new Vector2(Random.Range(-1f, 1f), Random.Range(-3f, -1f)),
            Quaternion.identity
            );

           CloneActor = clone;

            Destroy(clone, TicketBalloonDespawnTimer);

            yield return new WaitForSeconds(3);


            chances--;
        }
    }

    //Pops clone in case of collision earlier than the routine
  public  void popped()
    {
        Destroy(CloneActor);
    }
}

