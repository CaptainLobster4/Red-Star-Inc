using UnityEngine;
using UnityEngine.Assemblies;

public class BulletHole : MonoBehaviour
{

    // area around the actual shape that bullets count in. should be a child of shape
    public GameObject TargetArea;
    
    // gamemanager script
    private GameManager gameManager;

    // amount of bullets needed to cut out shape (should be turned into a percentage?)
    //      also should be called from gamemanager
    //private float shapeAmount = 40f;

    private void Awake()
    {

        TargetArea = GameObject.Find("TriangleArea");

        // find and get gamemanager script
        GameObject targetObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (targetObj != null)
        {
            gameManager = targetObj.GetComponent<GameManager>();
        }

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
        }

    }

}
