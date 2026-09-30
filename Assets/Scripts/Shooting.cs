using UnityEngine;
using TMPro;

//Tracks bullet count
// Shot count determined by target
// Need reference to target prefab

public class Shooting: MonoBehaviour
{
    [Header("Ammo Settings")]
    public int  maxAmmo = 30;
    public int currentAmmo;

    [Header("UI Reference")]
    public TextMeshProUGUI BulletCountText; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //bullet count
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input
    }
}
