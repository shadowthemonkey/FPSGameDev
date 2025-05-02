    using UnityEngine;

public class RifleAK : Weapon
{
    public Transform firePoint; // currently uses the camera as the origin for raycast rays
    private float lastShotTime;

    protected override void InitializeWeaponStats()
    {
        weaponName = "AK-47";
        maxAmmo = 30;
        fireMode = FireMode.FullAuto;
        fireRate = 10f; // per second, so this matches the 600rpm of the ak in cs2
        reloadTime = 2.5f;
        damage = 36 ;
    }

    public override void Shoot()
    {
        if (isReloading) return;

        if (Time.time >= lastShotTime + (1f / fireRate))
        {
            if (currentAmmo > 0)
            {
                currentAmmo--;
                lastShotTime = Time.time;

                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 100f))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        AIBot bot = hit.collider.GetComponent<AIBot>(); // bot
                        if (bot != null)
                        {
                            bot.TakeDamage(damage);
                        }
                    }
                }
            }
            else
            {
                // call reload if trying to fire without ammo
                Reload();
            }
        }
    }
}
