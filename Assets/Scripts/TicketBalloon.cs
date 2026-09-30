using UnityEngine;

public class TicketBalloon : MonoBehaviour
{
    public GameObject TicketBalloonSprite;
    public float SpawnTimer; 
    public float Duration;
    public float TicketsGiven;

    private Vector2 SpawnPosition;

   void Start() 
   {
       //Wait for x duration to spawn TicketBalloonSprite

       //Wait for x duration to despawn TicketBalloonSprite

       //Randomize ammount of tickets given to player based on time taken to hit balloon

       //Randomize spawn location under a certain screen threshold

   }
    void Update()
    {

    }

    private void spawnBalloon() 
    {

    }

}

//Spawning Logic
// -- Instantiate(TicketBalloon, SpawnPosition, Quaternion.identity)
