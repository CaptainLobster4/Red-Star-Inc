using TMPro;
using UnityEngine;

// Put this on any object in a scene that shows the ticket count (game scene AND shop scene).
// It reads the saved total and refreshes the text whenever tickets change.
public class TicketTracker : MonoBehaviour
{
    public TextMeshProUGUI TicketTotal;

    [Tooltip("Optional text shown before the number, e.g. \"Tickets: \"")]
    public string prefix = "";

    public int Tickets => PlayerInventory.Tickets;

    private void OnEnable()
    {
        PlayerInventory.OnTicketsChanged += Refresh;
        Refresh(PlayerInventory.Tickets); // show the correct number as soon as the scene loads
    }

    private void OnDisable()
    {
        PlayerInventory.OnTicketsChanged -= Refresh;
    }

    public void AddTickets(int ticketsAdded)
    {
        PlayerInventory.AddTickets(ticketsAdded);
    }

    private void Refresh(int total)
    {
        if (TicketTotal != null)
        {
            TicketTotal.text = prefix + total.ToString();
        }
    }
}