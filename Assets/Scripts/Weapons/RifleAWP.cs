using UnityEngine;

public class RifleAWP : Weapon
{
    protected override void InitializeWeaponStats()
    {
        weaponName = "AWP";
        maxAmmo = 10;
        fireMode = FireMode.SemiAuto;
        fireRate = 0.68f; //cs2 awp has an rpm of 41
        reloadTime = 3.5f;
        damage = 115;
        penetrationPower = 6;
        penetrationRate = 0.8f;
    }

    public override void Shoot()
    {
        // add scope features next
        // awp shouldn't really have a crosshair since no scope is random
        base.Shoot();
    }
}
