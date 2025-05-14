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
        // perfect accuracy, no random bloom spread
        // no movement inaccuracy
        // consistent spray patterns
        // better spray control when crouched maybe
        // recoil
        // camera punch when shooting
        base.Shoot();
    }

    // spray pattern test for now
    private Vector2[] akSprayPattern = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(0, 2),
        new Vector2(0, 3),
        new Vector2(0, 4),
        new Vector2(0, 5),
        new Vector2(0, 6),

    };

    protected override Vector3 GetSprayDirection()
    {
        if (shotCount >= akSprayPattern.Length)
            return new Vector3(0, 1.5f, 0); // fallback for long spray

        Vector2 pattern = akSprayPattern[shotCount];
        return new Vector3(-pattern.x, pattern.y, 0); // flip X for recoil direction
    }
}
