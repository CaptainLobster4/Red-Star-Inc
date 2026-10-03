using UnityEngine;
using UnityEngine.Assemblies;

public class BulletHole : MonoBehaviour
{

    // area around the actual shape that bullets count in. should be a child of shape
    public GameObject TargetArea;
    public GameObject HighTicketBalloon;
    public GameObject LowTicketBalloon;
    public TicketBalloons ticketBalloons;
    public TicketBalloonSpawniManager ticketBalloonSpawnManager;
    
    // gamemanager script
    private GameManager gameManager;

    // amount of bullets needed to cut out shape (should be turned into a percentage?)
    //      also should be called from gamemanager
    //private float shapeAmount = 40f;

    private void Awake()
    {

        TargetArea = GameObject.Find("TriangleArea");
        HighTicketBalloon = GameObject.FindGameObjectWithTag("HighBalloon");
        LowTicketBalloon = GameObject.FindGameObjectWithTag("LowBalloon");

        // find and get gamemanager script
        GameObject targetObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (targetObj != null)
        {
            gameManager = targetObj.GetComponent<GameManager>();
        }

        //Finds Balloon Spawn Manager
        GameObject ticketBalloonSpawnManager = GameObject.FindGameObjectWithTag("MainCamera");
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // when this object collides with the target area, decreases amount required to shoot out the shape 
        //      and updates progress bar
        if (collision.gameObject == TargetArea)
        {
            Debug.Log("collided with shape,, amnt left is " + gameManager.shapeAmount);

            //if (gameManager.shapeAmount > 0f)
            //{
                gameManager.UpdateProgress(0.025f);
                gameManager.shapeAmount = gameManager.DecreaseSideAmount(gameManager.shapeAmount);
            //}




            //LOGIC FOR TRACKING WHEN HITTING TICKET BALLOONS TO EXECUTE ADDING TICKETS BASED ON VALUE OF BALLOON
        }
        if (collision.gameObject == HighTicketBalloon)
        {
            Debug.Log("Added Higher Tickets " );

            //Calls when hitting ticket balloon
            ticketBalloons.HighGivenTickets();
            ticketBalloonSpawnManager.popped();


        }
        if (collision.gameObject == LowTicketBalloon)
        {
            Debug.Log("Added Lower Tickets ");

            //Calls when hitting ticket balloon
            ticketBalloons.LowGivenTickets();
            ticketBalloonSpawnManager.popped();
        }

    }

}
