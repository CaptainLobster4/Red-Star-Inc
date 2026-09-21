using UnityEngine;
using UnityEngine.Assemblies;

public class BulletHole : MonoBehaviour
{

    public GameObject TargetArea;
    
    // gamemanager script
    private GameManager gameManager;

    private float shapeAmount = 40f;

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
            Debug.Log("collided with shape,, amnt left is " + shapeAmount);

            if (shapeAmount > 0f)
            {
                gameManager.UpdateProgress(0.025f);
                shapeAmount = gameManager.DecreaseSideAmount(shapeAmount);
            }
        }

    }

}
