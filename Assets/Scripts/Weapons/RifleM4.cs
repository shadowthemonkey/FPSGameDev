using UnityEngine;

public class RifleM4 : Weapon
{
    protected override void InitializeWeaponStats()
    {
        weaponName = "M4A4";
        maxAmmo = 30;
        fireMode = FireMode.FullAuto;
        fireRate = 11.1f; // m4 has an rpm of 666
        reloadTime = 2.3f;
        damage = 33;
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

    // I'll just have it rise for a little bit then stay around a certain area
    private Vector2[] m4SprayPattern = new Vector2[]
    {
        new Vector2(0, 0),
        new Vector2(-0.3f, 0.2f),
        new Vector2(-0.2f, 2f),
        new Vector2(0, 4f),
        new Vector2(-0.1f, 6f),
        new Vector2(0.4f, 8f),
        new Vector2(0.7f, 9f),
        new Vector2(1f, 10f),
        new Vector2(0.6f, 12f),
        new Vector2(-0.7f, 11.5f),
        new Vector2(-0.8f, 11.7f),
        new Vector2(-1.3f, 12.2f),
        new Vector2(-1.7f, 12.1f),
        new Vector2(-1.9f, 12.3f),
        new Vector2(-1.7f, 12.4f),
        new Vector2(-1.5f, 12.6f),
        new Vector2(-1f, 12.8f),
        new Vector2(0f, 13.1f),
        new Vector2(0.5f, 13.2f),
        new Vector2(0.8f, 13.4f),
        new Vector2(1f, 13.1f),
        new Vector2(1.5f, 13.2f),
        new Vector2(1.9f, 13f),
        new Vector2(1.4f, 12.9f),
        new Vector2(0.6f, 13.1f),
        new Vector2(-0.1f, 13.1f),
        new Vector2(-0.5f, 13.2f),
        new Vector2(-1f, 13.1f),
        new Vector2(-1.5f, 13.2f),
        new Vector2(-1.8f, 13.3f),
    };

    protected override Vector3 GetSprayPatternOffset()
    {
        /*
        if (shotCount >= akSprayPattern.Length)
            return new Vector3(0, 1.5f, 0); // fallback for long spray, 30 bullets are already calculated
        */

        Vector2 pattern = m4SprayPattern[shotCount];
        return new Vector3(-pattern.x, pattern.y, 0); // flip X for recoil direction
    }
}
