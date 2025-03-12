using UnityEngine;

public class Knife : Weapon
{
    public Transform firePoint; // currently uses the camera as the origin for raycast rays
    public float attackRate;
    private float lastAttackTime;

    protected override void InitializeWeaponStats()
    {
        weaponName = "Knife";
        maxAmmo = 1; // not used, just a placeholder for the GUI for now
        fireRate = 1.5f;
        reloadTime = 0f; // no reloading needed since knife
        damage = 50;
    }

    public override void Shoot()
    {
        if (Time.time >= lastAttackTime + (1f / fireRate))
        {
            lastAttackTime = Time.time;
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
