using UnityEngine;
using UnityEngine.UI;


public class AmmoMeter : MonoBehaviour
{
    [SerializeField] private Image fillImage; 

    private void OnEnable()
    {
        PlayerInventory.OnAmmoChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        PlayerInventory.OnAmmoChanged -= Refresh;
    }

    private void Refresh()
    {
        if (fillImage == null) return;
        fillImage.fillAmount = (float)PlayerInventory.Ammo / PlayerInventory.MaxAmmo;
    }
}