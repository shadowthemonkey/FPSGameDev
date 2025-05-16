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
        new Vector2(-0.2f, 0.2f),
        new Vector2(-0.2f, 2f),
        new Vector2(0, 4f),
        new Vector2(-0.1f, 6f),
        new Vector2(0.4f, 8f),
        new Vector2(0.1f, 9f),
        new Vector2(-0.4f, 10f),
        new Vector2(-0.3f, 10f),
        new Vector2(-0.8f, 10.3f),
        new Vector2(-0.8f, 10.5f),
        new Vector2(-1f, 10.4f),
        new Vector2(-0.9f, 10.6f),
        new Vector2(-0.9f, 10.4f),
        new Vector2(-0.7f, 10.4f),
        new Vector2(-0.5f, 10.6f),
        new Vector2(-0.3f, 10.8f),
        new Vector2(0f, 11.1f),
        new Vector2(-0.2f, 11.2f),
        new Vector2(-0.4f, 11.3f),
        new Vector2(-0.6f, 11.1f),
        new Vector2(-0.5f, 11.2f),
        new Vector2(-0.5f, 11f),
        new Vector2(-0.4f, 11.9f),
        new Vector2(-0.6f, 11.1f),
        new Vector2(-0.1f, 11.1f),
        new Vector2(-0.5f, 11.2f),
        new Vector2(-0.4f, 11.1f),
        new Vector2(-0.5f, 11.2f),
        new Vector2(-0.8f, 11.3f),
    };

    protected override Vector3 GetSprayPatternOffset()
    {
        /*
        if (shotCount >= m4SprayPattern.Length)
            return new Vector3(0, 1.5f, 0); // fallback for long spray, 30 bullets are already calculated
        */

        Vector2 pattern = m4SprayPattern[shotCount];
        return new Vector3(-pattern.x, pattern.y, 0); // flip X for recoil direction
    }
}
