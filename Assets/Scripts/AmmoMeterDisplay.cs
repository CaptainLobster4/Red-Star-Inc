using UnityEngine;
using UnityEngine.UI;

// place on Ammo Cannister object (the parent one under Canvas)
// use ammo meter for image
public class AmmoMeterDisplay : MonoBehaviour
{
    [SerializeField] private Image[] fills;

    [Tooltip("How fast the bar moves to its new level. 0 = instant.")]
    [SerializeField] private float smoothSpeed = 6f;

    private float shown = 1f;

    private void Awake()
    {
        
        foreach (Image fill in fills)
        {
            if (fill == null) continue;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Vertical;
            fill.fillOrigin = (int)Image.OriginVertical.Bottom;
        }
    }

    private void OnEnable()
    {
        PlayerInventory.OnAmmoChanged += Snap;
        PlayerInventory.OnAbilitiesChanged += Snap;
        Snap();
    }

    private void OnDisable()
    {
        PlayerInventory.OnAmmoChanged -= Snap;
        PlayerInventory.OnAbilitiesChanged -= Snap;
    }

    private float Target()
    {
        int max = PlayerInventory.MaxAmmo;
        return max > 0 ? Mathf.Clamp01((float)PlayerInventory.Ammo / max) : 0f;
    }

    // in case smoothing is set to 0
    private void Snap()
    {
        if (smoothSpeed <= 0f)
        {
            shown = Target();
            Apply();
        }
    }

    private void Start()
    {
        shown = Target(); // start where the ammo really is (full on a fresh game)
        Apply();
    }

    private void Update()
    {
        if (smoothSpeed <= 0f) return;

        float target = Target();
        if (Mathf.Approximately(shown, target)) return;

        shown = Mathf.MoveTowards(shown, target, smoothSpeed * Time.unscaledDeltaTime * 0.25f);
        Apply();
    }

    
    private void Apply()
    {
        if (fills == null || fills.Length == 0) return;

        float total = shown * fills.Length;
        for (int i = 0; i < fills.Length; i++)
        {
            if (fills[i] != null)
            {
                fills[i].fillAmount = Mathf.Clamp01(total - i);
            }
        }
    }
}