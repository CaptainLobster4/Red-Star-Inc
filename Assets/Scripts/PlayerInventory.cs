using System;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    Duckshot,
    DoubleShot   // placeholder for the 2nd purchasable ability - rename once it's designed
}

// Static save layer so tickets + purchased abilities survive scene loads
// (game scene <-> shop scene) and app restarts. Not a MonoBehaviour: nothing to attach.
//
// Abilities are single-round consumables:
//   - Buying one "arms" it (saved, shows as Ready in the shop).
//   - GameManager.Start() calls BeginRound(): armed abilities are used up on the spot
//     (saved copy deleted) and become ACTIVE for that round only (memory only).
//   - BulletHole etc. check IsActive(), never HasAbility().
// Because it's consumed when the round STARTS, leaving mid-round can't keep it.
public static class PlayerInventory
{
    private const string TicketsKey = "Tickets";
    private const string AbilityKey = "AbilityOwned_";

    // Abilities live for the current round. Never saved to disk.
    private static readonly HashSet<AbilityType> activeThisRound = new HashSet<AbilityType>();

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

    // Bought and waiting for the next round (what the shop shows as "Ready")
    public static bool HasAbility(AbilityType ability)
    {
        return PlayerPrefs.GetInt(AbilityKey + ability, 0) == 1;
    }

    // Working right now, this round only (what gameplay scripts should check)
    public static bool IsActive(AbilityType ability)
    {
        return activeThisRound.Contains(ability);
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

    // Call once when a round starts. Starts from a clean slate, then moves every
    // armed ability into this round's active set and deletes the saved copy,
    // so it has to be rebought for the next round.
    public static void BeginRound()
    {
        activeThisRound.Clear();

        bool anyConsumed = false;
        foreach (AbilityType ability in Enum.GetValues(typeof(AbilityType)))
        {
            if (HasAbility(ability))
            {
                activeThisRound.Add(ability);
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

    // Call when the round's scene is torn down
    public static void EndRound()
    {
        activeThisRound.Clear();
    }

    // Testing helper - wipes tickets and all abilities
    public static void ResetAll()
    {
        activeThisRound.Clear();
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