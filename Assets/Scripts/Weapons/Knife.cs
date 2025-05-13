using UnityEngine;

public class Knife : Weapon
{
    public float attackRate;

    protected override void InitializeWeaponStats()
    {
        weaponName = "Knife";
        maxAmmo = 1; // never spent, just a placeholder for the GUI for now
        fireMode = FireMode.FullAuto; // you can hold left click and keep swinging
        fireRate = 1.5f;
        reloadTime = 0f; // no reloading needed since knife
        damage = 50;
    }

    public override void Shoot()
    {
        if (Time.time >= lastShotTime + (1f / fireRate))
        {
            lastShotTime = Time.time;
            RaycastHit hit;
            //range of 5, since melee
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 5f))
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
    }

    public override void Reload()
    {
        // knives don't reload, override to do nothing
    }
}
