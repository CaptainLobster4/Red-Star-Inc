using System;
using UnityEngine;

public enum AbilityType
{
    Duckshot,
    SecondAbility   // placeholder for the 2nd purchasable ability - rename once it's designed
}

// Static save layer so tickets + upgrades survive scene loads
// (game scene <-> shop scene) and app restarts. Not a MonoBehaviour: nothing to attach.
//
// Abilities are PERMANENT, stackable upgrades (roguelike style):
//   - Each purchase raises the ability's level by 1 (level 0 = not owned).
//   - Levels are saved and never expire; they carry into every level.
//   - Each owned ability can be switched ON/OFF before a round (default ON).
//   - Gameplay scripts check IsActive() (owned AND switched on) and GetLevel() for strength.
public static class PlayerInventory
{
    private const string TicketsKey = "Tickets";
    private const string AbilityLevelKey = "AbilityLevel_";
    private const string AbilityEnabledKey = "AbilityEnabled_";
    private const string OldAbilityKey = "AbilityOwned_"; // from the old one-round version, only cleared by ResetAll

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

    // ---------- Abilities (permanent, stackable upgrades) ----------

    // 0 = not owned, 1+ = owned at that level
    public static int GetLevel(AbilityType ability)
    {
        return PlayerPrefs.GetInt(AbilityLevelKey + ability, 0);
    }

    // Owned at any level (what the shop and the in-level buttons check)
    public static bool HasAbility(AbilityType ability)
    {
        return GetLevel(ability) > 0;
    }

    // The player's on/off switch. Defaults to ON.
    public static bool IsEnabled(AbilityType ability)
    {
        return PlayerPrefs.GetInt(AbilityEnabledKey + ability, 1) == 1;
    }

    public static void SetEnabled(AbilityType ability, bool enabled)
    {
        PlayerPrefs.SetInt(AbilityEnabledKey + ability, enabled ? 1 : 0);
        PlayerPrefs.Save();
        OnAbilitiesChanged?.Invoke();
    }

    // Working right now: owned AND switched on (what gameplay scripts should check)
    public static bool IsActive(AbilityType ability)
    {
        return HasAbility(ability) && IsEnabled(ability);
    }

    // Spends tickets and raises the ability one level (buying it the first time = level 1).
    // maxLevel <= 0 means no cap. Returns false if at max level or the player can't afford it.
    public static bool TryUpgrade(AbilityType ability, int cost, int maxLevel)
    {
        int level = GetLevel(ability);
        if (maxLevel > 0 && level >= maxLevel) return false;
        if (!TrySpendTickets(cost)) return false;

        PlayerPrefs.SetInt(AbilityLevelKey + ability, level + 1);
        if (level == 0)
        {
            PlayerPrefs.SetInt(AbilityEnabledKey + ability, 1); // newly bought starts switched on
        }
        PlayerPrefs.Save();
        OnAbilitiesChanged?.Invoke();
        return true;
    }

    // ---------- Legacy (kept so older scripts still compile; they do nothing now) ----------
    // The one-round ability system was replaced by permanent upgrades.

    public static void BeginRound() { }
    public static void EndRound() { }
    public static void ClearAllAbilities() { }

    // Testing helper - wipes tickets and all abilities
    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(TicketsKey);
        foreach (AbilityType ability in Enum.GetValues(typeof(AbilityType)))
        {
            PlayerPrefs.DeleteKey(AbilityLevelKey + ability);
            PlayerPrefs.DeleteKey(AbilityEnabledKey + ability);
            PlayerPrefs.DeleteKey(OldAbilityKey + ability);
        }
        PlayerPrefs.Save();
        OnTicketsChanged?.Invoke(0);
        OnAbilitiesChanged?.Invoke();
    }
}