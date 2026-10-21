using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Goes on an object in the Shop scene (build index 3).
// Fill in the Abilities list in the Inspector - one entry per ability slot.
// Abilities are permanent upgrades: each purchase raises the level by 1 and the next level costs more.
public class ShopMenu : MonoBehaviour
{
    [Serializable]
    public class AbilitySlot
    {
        public AbilityType ability;
        public string displayName = "Duckshot";

        [Tooltip("Price of level 1.")]
        public int cost = 50;

        [Tooltip("Each level after the first costs this much more. Level 2 = cost + this, level 3 = cost + 2x this, etc.")]
        public int costIncreasePerLevel = 25;

        [Tooltip("Highest level the player can reach. 0 = no cap.")]
        public int maxLevel = 5;

        [Tooltip("Untick for abilities that aren't ready yet - the button shows 'Coming Soon' and is disabled.")]
        public bool available = true;

        public Button button;
        public TextMeshProUGUI nameLabel;   // optional - shows the name and current level
        public TextMeshProUGUI buttonLabel; // text on the button
    }

    [SerializeField] private AbilitySlot[] abilities;

    [Tooltip("Optional text for feedback like 'Not enough tickets'")]
    [SerializeField] private TextMeshProUGUI messageText;

    private void Start()
    {
        foreach (AbilitySlot slot in abilities)
        {
            AbilitySlot captured = slot;
            if (captured.button != null)
            {
                captured.button.onClick.AddListener(() => OnAbilityClicked(captured));
            }
        }

        RefreshAll();
        ShowMessage("");
    }

    private void OnEnable()
    {
        PlayerInventory.OnAbilitiesChanged += RefreshAll;
        PlayerInventory.OnTicketsChanged += OnTicketsChanged;
    }

    private void OnDisable()
    {
        PlayerInventory.OnAbilitiesChanged -= RefreshAll;
        PlayerInventory.OnTicketsChanged -= OnTicketsChanged;
    }

    private void OnTicketsChanged(int total)
    {
        RefreshAll();
    }

    // Price of the NEXT level for this slot
    private int NextCost(AbilitySlot slot)
    {
        return slot.cost + slot.costIncreasePerLevel * PlayerInventory.GetLevel(slot.ability);
    }

    private bool AtMax(AbilitySlot slot)
    {
        return slot.maxLevel > 0 && PlayerInventory.GetLevel(slot.ability) >= slot.maxLevel;
    }

    private void OnAbilityClicked(AbilitySlot slot)
    {
        if (!slot.available) return;

        if (AtMax(slot))
        {
            ShowMessage(slot.displayName + " is already at max level");
        }
        else if (PlayerInventory.TryUpgrade(slot.ability, NextCost(slot), slot.maxLevel))
        {
            ShowMessage(slot.displayName + " is now level " + PlayerInventory.GetLevel(slot.ability) + "!");
        }
        else
        {
            ShowMessage("Not enough tickets");
        }

        RefreshAll();
    }

    private void RefreshAll()
    {
        if (abilities == null) return;

        foreach (AbilitySlot slot in abilities)
        {
            int level = PlayerInventory.GetLevel(slot.ability);

            if (slot.nameLabel != null)
            {
                slot.nameLabel.text = slot.available && level > 0
                    ? slot.displayName + " (Lv " + level + ")"
                    : slot.displayName;
            }

            bool atMax = slot.available && AtMax(slot);

            if (slot.button != null)
            {
                slot.button.interactable = slot.available && !atMax;
            }

            if (slot.buttonLabel == null) continue;

            if (!slot.available)
            {
                slot.buttonLabel.text = "Coming Soon";
            }
            else if (atMax)
            {
                slot.buttonLabel.text = "Max";
            }
            else if (level == 0)
            {
                slot.buttonLabel.text = "Buy (" + NextCost(slot) + ")";
            }
            else
            {
                slot.buttonLabel.text = "Upgrade (" + NextCost(slot) + ")";
            }
        }
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    // Hook the shop's Back button to this
    public void BackButton()
    {
        SceneManager.LoadSceneAsync(0);
    }

    // Right-click the component header in Play Mode to use these while testing
    [ContextMenu("Debug: Add 100 Tickets")]
    private void DebugAddTickets()
    {
        PlayerInventory.AddTickets(100);
    }

    [ContextMenu("Debug: Reset Save Data")]
    private void DebugResetSaveData()
    {
        PlayerInventory.ResetAll();
    }
}