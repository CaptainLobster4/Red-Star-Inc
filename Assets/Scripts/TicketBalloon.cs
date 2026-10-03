using System.Collections;
using UnityEngine;

public class TicketBalloons : MonoBehaviour
{
    public TicketTracker ticketTracker;
    public int HighValue = 2;
    public int LowValue = 1;
    void Start()
    {
        //Finds Ticket Tracker Script
        ticketTracker = FindAnyObjectByType<TicketTracker>();
    }
    //Adds Ticket based on tier 2 balloon value
    public void HighGivenTickets()
    {
        ticketTracker.AddTickets(HighValue);
    }
    //Gives Tickets based on tier 1 balloon value
    public void LowGivenTickets()
    {
        ticketTracker.AddTickets(LowValue);
    }
}