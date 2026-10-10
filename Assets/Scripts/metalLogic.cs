using UnityEngine;

public class metalLogic : MonoBehaviour
{
    public int health = 3;
    public Collider2D targetCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            targetCollider.enabled = true;
            Destroy(gameObject);
        }
    }
}
