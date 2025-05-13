using UnityEngine;

public class RifleAK : Weapon
{
    protected override void InitializeWeaponStats()
    {
        weaponName = "AK-47";
        maxAmmo = 30;
        fireMode = FireMode.FullAuto;
        fireRate = 10f; // per second, so this matches the 600rpm of the ak in cs2
        reloadTime = 2.5f;
        damage = 36 ;
        penetrationPower = 5;
        penetrationRate = 0.8f;
    }

    public override void Shoot()
    {
        // Optional: add AK-specific spread or recoil here
        base.Shoot();
    }
}
