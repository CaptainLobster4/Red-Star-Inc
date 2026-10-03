using TMPro;
using UnityEngine;

public class TicketTracker : MonoBehaviour
{
    public int tickets = 0;

    public TextMeshProUGUI TicketTotal;

    public void AddTickets (int ticketsAdded)
    {
        int tickets = this.tickets + ticketsAdded;
        TicketTotal.text = this.tickets.ToString();
    }
    

}
