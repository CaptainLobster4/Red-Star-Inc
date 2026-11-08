using System;
using UnityEngine;

// Goes on any object in a level (or the shop) that has the ability sidebar.
// Each entry is one ability and the image/icon object that shows it is active.
// An image is shown while its ability is ACTIVE (owned AND switched on), and hidden otherwise,
// so any number of abilities can be showing at the same time.
// It reads PlayerInventory directly, so there is no button or flag to set.
public class AbilityIndicatorBar : MonoBehaviour
{
    [Serializable]
    public class Indicator
    {
        public AbilityType ability;

        [Tooltip("The image (or any object) shown while this ability is active.")]
        public GameObject image;
    }

    [SerializeField] private Indicator[] indicators;

    private void OnEnable()
    {
        PlayerInventory.OnAbilitiesChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        PlayerInventory.OnAbilitiesChanged -= Refresh;
    }

    private void Refresh()
    {
        if (indicators == null) return;

        foreach (Indicator indicator in indicators)
        {
            if (indicator.image != null)
            {
                indicator.image.SetActive(PlayerInventory.IsActive(indicator.ability));
            }
        }
    }
}