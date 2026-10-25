using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Goes on any object in a level (the Canvas is fine).
// Shows one on/off button per ability, but ONLY for abilities the player owns -
// unowned ones stay hidden until bought in the shop.
// Switch them before shooting; once the first shot is fired the buttons lock for the round.
public class AbilityToggleHUD : MonoBehaviour
{
    [Serializable]
    public class AbilityToggle
    {
        public AbilityType ability;
        public string displayName = "Duckshot";

        [Tooltip("The button object. It is hidden when the ability isn't owned.")]
        public Button button;

        [Tooltip("Text on the button, shows e.g. 'Duckshot Lv 2: ON'")]
        public TextMeshProUGUI label;
    }

    [SerializeField] private AbilityToggle[] toggles;

    [Header("Look")]
    [SerializeField] private Color onColor = new Color(0.55f, 0.9f, 0.55f);
    [SerializeField] private Color offColor = new Color(0.65f, 0.65f, 0.65f);

    [Header("Rules")]
    [Tooltip("Lock the buttons once the player fires their first shot of the round.")]
    [SerializeField] private bool lockAfterFirstShot = true;

    private PlayerCharacter player;
    private bool lockRefreshed = false;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerCharacter>();

        foreach (AbilityToggle toggle in toggles)
        {
            AbilityToggle captured = toggle;
            if (captured.button != null)
            {
                captured.button.onClick.AddListener(() => OnToggleClicked(captured));
            }
        }

        Refresh();
    }

    private void OnEnable()
    {
        PlayerInventory.OnAbilitiesChanged += Refresh;
    }

    private void OnDisable()
    {
        PlayerInventory.OnAbilitiesChanged -= Refresh;
    }

    private void Update()
    {
        // cheap check so the buttons lock the moment the first shot is fired
        if (!lockRefreshed && IsLocked())
        {
            lockRefreshed = true;
            Refresh();
        }
    }

    private bool IsLocked()
    {
        return lockAfterFirstShot && player != null && player.ShotsFired > 0;
    }

    private void OnToggleClicked(AbilityToggle toggle)
    {
        if (IsLocked() || !PlayerInventory.HasAbility(toggle.ability)) return;

        PlayerInventory.SetEnabled(toggle.ability, !PlayerInventory.IsEnabled(toggle.ability));
        // SetEnabled raises OnAbilitiesChanged, which calls Refresh()
    }

    private void Refresh()
    {
        if (toggles == null) return;

        bool locked = IsLocked();

        foreach (AbilityToggle toggle in toggles)
        {
            if (toggle.button == null) continue;

            bool owned = PlayerInventory.HasAbility(toggle.ability);
            toggle.button.gameObject.SetActive(owned);
            if (!owned) continue;

            bool on = PlayerInventory.IsEnabled(toggle.ability);
            toggle.button.interactable = !locked;

            if (toggle.button.targetGraphic != null)
            {
                toggle.button.targetGraphic.color = on ? onColor : offColor;
            }

            if (toggle.label != null)
            {
                toggle.label.text = toggle.displayName + " Lv " + PlayerInventory.GetLevel(toggle.ability) + ": " + (on ? "ON" : "OFF");
            }
        }
    }
}