using UnityEngine;

public class PistolDeagle : Weapon
{
    protected override void InitializeWeaponStats()
    {
        weaponName = "USP-S";
        magSize = 7;
        reserveAmmo = 28;
        fireMode = FireMode.SemiAuto;
        fireRate = 6f; // per second, close to the 535 rpm the usp has in cs2
        reloadTime = 1.5f;
        damage = 52;
        penetrationPower = 3;
        penetrationRate = 0.75f;
    }

    public override void Shoot()
    {
        // Optional: usp recoil here maybe
        base.Shoot();
    }
}
