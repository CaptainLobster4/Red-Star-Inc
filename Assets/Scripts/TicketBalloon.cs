using UnityEngine;

// Goes on each ticket balloon PREFAB (needs a Collider2D).
// Make one prefab with ticketValue = 1 (low) and one with ticketValue = 2 (high).
public class TicketBalloon : MonoBehaviour
{
    [SerializeField] private int ticketValue = 1;

    private bool popped = false;

    // Called by BulletHole when a tap lands on this balloon
    public void Pop()
    {
        if (popped) return; // guards against two overlapping bullet holes double-paying
        popped = true;

        PlayerInventory.AddTickets(ticketValue);

        // Hook for a pop sound / particle effect later
        Destroy(gameObject);
    }
}