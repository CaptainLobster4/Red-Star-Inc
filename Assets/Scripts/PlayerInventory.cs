using System;
using UnityEngine;

public enum AbilityType
{
    Duckshot,
    DoubleShot,  // 2nd purchasable ability
    ExtraAmmo    // permanent upgrade: more bullets every level (always on, not a toggle)
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

    // ---------- Ammo (one shared pool for the whole game) ----------
    // The player starts the game with StartingAmmo bullets. Every shot spends one, in every level,
    // and leftovers carry over. More are bought in the shop (AmmoPrice tickets each) up to MaxAmmo.
    // The Extra Ammo upgrade raises MaxAmmo by MaxAmmoPerLevel per level; it does not give bullets by itself.

    public const int StartingAmmo = 50;
    public const int BaseMaxAmmo = 50;
    public const int MaxAmmoPerLevel = 5;
    public const int AmmoPrice = 1;

    private const string AmmoKey = "Ammo";

    public static event Action OnAmmoChanged;

    public static int MaxAmmo => BaseMaxAmmo + MaxAmmoPerLevel * GetLevel(AbilityType.ExtraAmmo);

    // Unset (first launch) = StartingAmmo
    public static int Ammo => PlayerPrefs.GetInt(AmmoKey, StartingAmmo);

    public static void SetAmmo(int value)
    {
        PlayerPrefs.SetInt(AmmoKey, Mathf.Max(0, value));
        PlayerPrefs.Save();
        OnAmmoChanged?.Invoke();
    }

    // Returns how many were actually bought (0 = none).
    public static int BuyAmmo(int wanted)
    {
        int room = MaxAmmo - Ammo;
        int affordable = AmmoPrice > 0 ? Tickets / AmmoPrice : wanted;
        int count = Mathf.Min(wanted, Mathf.Min(room, affordable));
        if (count <= 0) return 0;

        if (!TrySpendTickets(count * AmmoPrice)) return 0;
        SetAmmo(Ammo + count);
        return count;
    }

    // ---------- Legacy (kept so older scripts still compile; they do nothing now) ----------

    public static void BeginRound() { }
    public static void EndRound() { }
    public static void ClearAllAbilities() { }

    // Testing helper - wipes tickets and all abilities
    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(TicketsKey);
        PlayerPrefs.DeleteKey(AmmoKey);
        foreach (AbilityType ability in Enum.GetValues(typeof(AbilityType)))
        {
            PlayerPrefs.DeleteKey(AbilityLevelKey + ability);
            PlayerPrefs.DeleteKey(AbilityEnabledKey + ability);
            PlayerPrefs.DeleteKey(OldAbilityKey + ability);
        }
        PlayerPrefs.Save();
        OnTicketsChanged?.Invoke(0);
        OnAbilitiesChanged?.Invoke();
        OnAmmoChanged?.Invoke();
    }
}