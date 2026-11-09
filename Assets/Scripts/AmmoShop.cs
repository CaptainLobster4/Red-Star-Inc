using TMPro;
using UnityEngine;

// Goes on an object in the Shop scene. Sells bullets from the player's shared ammo.
// Hook buttons to it: a "+1" button -> BuyAmmo (int = 1), a "+10" button -> BuyAmmo (int = 10),
// a "Fill" button -> FillAmmo. The price (AmmoPrice) and the cap (MaxAmmo) live in PlayerInventory.
public class AmmoShop : MonoBehaviour
{
    [Tooltip("Shows e.g. 'Ammo: 12 / 50'")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Tooltip("Optional text for feedback like 'Not enough tickets'")]
    [SerializeField] private TextMeshProUGUI messageText;

    private void OnEnable()
    {
        PlayerInventory.OnAmmoChanged += Refresh;
        PlayerInventory.OnAbilitiesChanged += Refresh;
        Refresh();
        ShowMessage("");
    }

    private void OnDisable()
    {
        PlayerInventory.OnAmmoChanged -= Refresh;
        PlayerInventory.OnAbilitiesChanged -= Refresh;
    }

    // Button On Click: pass the amount in the Inspector (1, 10, ...)
    public void BuyAmmo(int amount)
    {
        if (PlayerInventory.Ammo >= PlayerInventory.MaxAmmo)
        {
            ShowMessage("Ammo is full");
            return;
        }

        int bought = PlayerInventory.BuyAmmo(amount);

        if (bought <= 0)
        {
            ShowMessage("Not enough tickets");
        }
        else if (bought < amount && PlayerInventory.Ammo < PlayerInventory.MaxAmmo)
        {
            ShowMessage("Bought " + bought + " (That's all you can get)");
        }
        else
        {
            ShowMessage("Bought " + bought);
        }
    }

    // Button On Click: buys as many as the player can afford, up to the max
    public void FillAmmo()
    {
        BuyAmmo(int.MaxValue);
    }

    private void Refresh()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + PlayerInventory.Ammo + " / " + PlayerInventory.MaxAmmo;
        }
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }
}