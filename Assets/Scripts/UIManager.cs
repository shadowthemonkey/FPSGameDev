using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerMovement playerMovement; // speedometer in this script
    public WeaponManager weaponManager; // weapon details in this script

    [Header("UI Elements")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI weaponInfoText;
    // currently just using GUI to show the speed of the player, and the weapon info

    private void Update()
    {
        UpdateSpeedometer();
        UpdateWeaponInfo();
    }

    private void UpdateSpeedometer()
    {
        if (playerMovement == null) return;

        // get the current speed from PlayerMovement.LogSpeed, changed to a return float
        speedText.text = $"Speed: {playerMovement.LogSpeed():F2} m/s";
    }

    private void UpdateWeaponInfo()
    {
        if (weaponManager == null || weaponManager.GetCurrentWeapon() == null) return;

        // get weapon details of the current weapon from the weapon manager
        string weaponName = weaponManager.GetCurrentWeapon().GetWeaponName();
        int ammoInMag = weaponManager.GetCurrentWeapon().GetCurrentAmmo();
        int totalAmmo = weaponManager.GetCurrentWeapon().GetTotalAmmo();

        // update UI (Weapon name Ammo: x / y)
        weaponInfoText.text = $"{weaponName} Ammo: {ammoInMag} / {totalAmmo}";
    }
}
