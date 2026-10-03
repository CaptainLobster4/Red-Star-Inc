using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Goes on an object in the Shop scene (build index 3).
// Fill in the Abilities list in the Inspector - one entry per ability slot.
public class ShopMenu : MonoBehaviour
{
    [Serializable]
    public class AbilitySlot
    {
        public AbilityType ability;
        public string displayName = "Duckshot";
        public int cost = 50;

        [Tooltip("Untick for abilities that aren't ready yet - the button shows 'Coming Soon' and is disabled.")]
        public bool available = true;

        public Button button;
        public TextMeshProUGUI nameLabel;   // optional
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

    private void OnAbilityClicked(AbilitySlot slot)
    {
        if (!slot.available) return;

        if (PlayerInventory.HasAbility(slot.ability))
        {
            ShowMessage(slot.displayName + " is already ready for your next round");
        }
        else if (PlayerInventory.TryPurchase(slot.ability, slot.cost))
        {
            ShowMessage(slot.displayName + " ready for your next round!");
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
            if (slot.nameLabel != null)
            {
                slot.nameLabel.text = slot.displayName;
            }

            bool owned = slot.available && PlayerInventory.HasAbility(slot.ability);

            if (slot.button != null)
            {
                // Can't buy a second copy while one is already waiting for the next round
                slot.button.interactable = slot.available && !owned;
            }

            if (slot.buttonLabel == null) continue;

            if (!slot.available)
            {
                slot.buttonLabel.text = "Coming Soon";
            }
            else if (owned)
            {
                slot.buttonLabel.text = "Ready";
            }
            else
            {
                slot.buttonLabel.text = "Buy (" + slot.cost + ")";
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

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
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