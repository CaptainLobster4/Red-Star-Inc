using UnityEngine;
using UnityEngine.Assemblies;

public class BulletHole : MonoBehaviour
{
    private GameObject side1;
    private GameObject side2;
    private GameObject side3;

    private GameManager gameManager;


    private void Awake()
    {
        side1 = GameObject.Find("Side1");
        side2 = GameObject.Find("Side2");
        side3 = GameObject.Find("Side3");


        GameObject targetObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (targetObj != null)
        {
            gameManager = targetObj.GetComponent<GameManager>();
        }


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.name == "Side1")
        {
            Debug.Log("collided with side1,, amnt left is " + gameManager.side1Amount);

            if (gameManager.side1Amount > 0f)
            {
                gameManager.UpdateProgress(0.022f);
                gameManager.side1Amount = gameManager.DecreaseSideAmount(gameManager.side1Amount);
            }
        }

        if (collision.gameObject.name == "Side2")
        {
            Debug.Log("collided with side2,, amnt left is " + gameManager.side2Amount);

            if (gameManager.side2Amount > 0f)
            {
                gameManager.UpdateProgress(0.022f);
                gameManager.side2Amount = gameManager.DecreaseSideAmount(gameManager.side2Amount);
            }
        }

        if (collision.gameObject.name == "Side3")
        {
            Debug.Log("collided with side 3,, amnt left is " + gameManager.side3Amount);

            if (gameManager.side3Amount > 0f)
            {
                gameManager.UpdateProgress(0.022f);
                gameManager.side3Amount = gameManager.DecreaseSideAmount(gameManager.side3Amount);
            }
        }


    }





    void Update()
    {
        



    }
}
