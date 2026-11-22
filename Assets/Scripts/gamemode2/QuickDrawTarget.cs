using UnityEngine;

// You don't need to put this on your target prefabs - QuickDrawManager adds it
// automatically when a target spawns. It only handles moving + bouncing.
// Speed 0 = the target just sits where it spawned.
public class QuickDrawTarget : MonoBehaviour
{
    private Vector2 areaMin;
    private Vector2 areaMax;
    private Vector2 direction;
    private float speed;

    public void Init(Vector2 min, Vector2 max, float moveSpeed)
    {
        areaMin = min;
        areaMax = max;
        speed = moveSpeed;

        direction = Random.insideUnitCircle.normalized;
        if (direction == Vector2.zero) direction = Vector2.right;
    }

    private void Update()
    {
        if (speed <= 0f) return;

        Vector3 p = transform.position + (Vector3)(direction * speed * Time.deltaTime);

        // bounce off the edges of the spawn area
        if (p.x < areaMin.x) { p.x = areaMin.x; direction.x = Mathf.Abs(direction.x); }
        else if (p.x > areaMax.x) { p.x = areaMax.x; direction.x = -Mathf.Abs(direction.x); }

        if (p.y < areaMin.y) { p.y = areaMin.y; direction.y = Mathf.Abs(direction.y); }
        else if (p.y > areaMax.y) { p.y = areaMax.y; direction.y = -Mathf.Abs(direction.y); }

        transform.position = p;
    }
}
