using System;
using UnityEngine;

public enum AbilityType
{
    Duckshot,
    SecondAbility   // placeholder for the 2nd purchasable ability - rename once it's designed
}

// Static save layer so tickets + purchased abilities survive scene loads
// (game scene <-> shop scene) and app restarts. Not a MonoBehaviour: nothing to attach.
//
// Abilities are single-round consumables: buying one arms it for the NEXT round,
// and GameManager calls ConsumeAllAbilities() when that round ends (win or lose).
public static class PlayerInventory
{
    private const string TicketsKey = "Tickets";
    private const string AbilityKey = "AbilityOwned_";

    // UI scripts subscribe to these so they refresh automatically
    public static event Action<int> OnTicketsChanged;
    public static event Action OnAbilitiesChanged;

    // ---------- Tickets ----------

    public static int Tickets => PlayerPrefs.GetInt(TicketsKey, 0);

    public static void AddTickets(int amount)
    {
        if (amount <= 0) return;
        SetTickets(Tickets + amount);
    }

    // Returns false (and changes nothing) if the player can't afford it
    public static bool TrySpendTickets(int cost)
    {
        if (cost < 0 || Tickets < cost) return false;
        SetTickets(Tickets - cost);
        return true;
    }

    private static void SetTickets(int value)
    {
        PlayerPrefs.SetInt(TicketsKey, value);
        PlayerPrefs.Save();
        OnTicketsChanged?.Invoke(value);
    }

    // ---------- Abilities (single-round consumables) ----------

    // True if the player has bought this ability and hasn't used it up yet
    public static bool HasAbility(AbilityType ability)
    {
        return PlayerPrefs.GetInt(AbilityKey + ability, 0) == 1;
    }

    // Spends tickets and arms the ability for the next round.
    // Returns false if already armed or the player can't afford it.
    public static bool TryPurchase(AbilityType ability, int cost)
    {
        if (HasAbility(ability)) return false;
        if (!TrySpendTickets(cost)) return false;

        PlayerPrefs.SetInt(AbilityKey + ability, 1);
        PlayerPrefs.Save();
        OnAbilitiesChanged?.Invoke();
        return true;
    }

    // Called when a round ends: every armed ability is used up and must be rebought
    public static void ConsumeAllAbilities()
    {
        bool anyConsumed = false;
        foreach (AbilityType ability in Enum.GetValues(typeof(AbilityType)))
        {
            if (HasAbility(ability))
            {
                PlayerPrefs.DeleteKey(AbilityKey + ability);
                anyConsumed = true;
            }
        }

        if (anyConsumed)
        {
            PlayerPrefs.Save();
            OnAbilitiesChanged?.Invoke();
        }
    }

    // Testing helper - wipes tickets and all abilities
    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(TicketsKey);
        foreach (AbilityType ability in Enum.GetValues(typeof(AbilityType)))
        {
            PlayerPrefs.DeleteKey(AbilityKey + ability);
        }
        PlayerPrefs.Save();
        OnTicketsChanged?.Invoke(0);
        OnAbilitiesChanged?.Invoke();
    }
}