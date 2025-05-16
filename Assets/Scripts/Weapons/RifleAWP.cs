using UnityEngine;

public class RifleAWP : Weapon
{
    protected override void InitializeWeaponStats()
    {
        weaponName = "AWP";
        maxAmmo = 10;
        fireMode = FireMode.Sniper;
        fireRate = 0.68f; //cs2 awp has an rpm of 41
        reloadTime = 3.5f;
        damage = 115;
        penetrationPower = 6;
        penetrationRate = 0.8f;
    }

    public override void Shoot()
    {
        if (isReloading) return; // reloading
        if (Time.time < lastShotTime + (1f / fireRate)) return; // not ready to fire yet
        if (currentAmmo <= 0) { Reload(); return; }

        currentAmmo--;
        lastShotTime = Time.time;

        Vector3 direction;

        if (isScoped)
        {
            // accurate shot when scoped
            direction = firePoint.forward;
        }
        else
        {
            // no scope inaccurate
            float noScopeSpread = 0.1f;
            // spread area is inside unit circle * noScopeSpread
            Vector2 randomSpread = UnityEngine.Random.insideUnitCircle * noScopeSpread;
            direction = firePoint.forward
                + firePoint.up * randomSpread.y
                + firePoint.right * randomSpread.x;
            direction.Normalize();
        }

        FireRaycast(damage, direction);

        shotCount++;
        lastRecoilTime = Time.time;
    }
}
