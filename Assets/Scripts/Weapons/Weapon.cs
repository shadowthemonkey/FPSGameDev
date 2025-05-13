using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public int maxAmmo;
    public enum FireMode { SemiAuto, FullAuto}
    public FireMode fireMode;
    public float fireRate;
    public float reloadTime;
    public int damage;
    public int penetrationPower; // number of layers
    public float penetrationRate; //damage reduction per layer

    protected int currentAmmo;
    protected bool isReloading = false;

    protected float lastShotTime;
    public Transform firePoint; // currently uses the camera as the origin for raycast rays

    // getter methods to access ammo count and weapon name for GUI
    public string GetWeaponName() => weaponName;
    public int GetCurrentAmmo() => currentAmmo;
    public int GetTotalAmmo() => maxAmmo;

    protected virtual void Awake()
    {
        InitializeWeaponStats();
        currentAmmo = maxAmmo;
    }

    // each weapon will define its own stats inside this method
    protected abstract void InitializeWeaponStats();

    public virtual void Shoot()
    {
        if (isReloading) return; // reloading
        if (Time.time < lastShotTime + (1f / fireRate)) return; // not ready to fire yet
        if (currentAmmo <= 0) { Reload(); return; }

        currentAmmo--;
        lastShotTime = Time.time;

        FireRaycast(damage);
    }

    protected virtual void FireRaycast(float startingDamage)
    {
        Vector3 origin = firePoint.position;
        Vector3 direction = firePoint.forward;

        // list of all raycast hits, so we can deal with wallbangs and collaterals (hitting enemies behind walls and other enemies)
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, 100f);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance)); // sorted by distance

        Vector3 finalHitPoint = origin + direction * 100f;

        int penetrations = 0;
        float currentDamage = startingDamage;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.isTrigger) continue; // ignore triggers

            if (hit.collider.CompareTag("Enemy")) // if hit an enemy bot, deal the damage and add a penetration
            {
                AIBot bot = hit.collider.GetComponent<AIBot>();
                if (bot != null)
                {
                    print("Bot takes " + currentDamage);
                    bot.TakeDamage(Mathf.RoundToInt(currentDamage));
                    penetrations++;
                }
            }
            else if (hit.collider.CompareTag("Wall")) // if hit a wall, add a penetration
            {
                BulletHoleManager.Instance.SpawnBulletHole(hit);
                penetrations++;
            }
            else
            {
                // unpenetrable surface, give up
                BulletHoleManager.Instance.SpawnBulletHole(hit);
                break;
            }


            // if this is the last wall the bullet penetrates because of penetration power, don't spawn an exit hole
            if (penetrations == penetrationPower)
            {
                break;
            }

            Vector3 nextEntry; // define nextEntry here

            //Debug.log(hits.Length);

            // this would be the last hit wall, the bullet should still penetrate
            if (penetrations == hits.Length)
            {
                nextEntry = finalHitPoint;
            }
            else // acts as normal
            {
                RaycastHit nextHit = hits[penetrations];
                nextEntry = nextHit.point;
            }
            Vector3 entry = hit.point;

            // spawn an exit hole between the two entry points
            SpawnExitHole(nextEntry, entry);

            currentDamage *= penetrationRate; // damage falloff per penetration, AK example of 80%: 100 to 80 to 64...
            if (penetrations >= penetrationPower)
                break;
        }
    }


    // the other side of a wall should have a bullet hole of the bullet leaving the wall
    // we shouldn't have an exit hole if it is the final penetration
    private void SpawnExitHole(Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from).normalized;
        float dist = Vector3.Distance(from, to);

        RaycastHit hit;
        if (Physics.Raycast(from + dir * 0.01f, dir, out hit, dist + 0.02f))
        {
            if (hit.collider.CompareTag("Wall")) // I'll tag all the walls in any maps we are playing, outside 3D objects would not be needed to be tagged as wall as it needs no wallbangs
            {
                BulletHoleManager.Instance.SpawnBulletHole(hit);
            }
        }
    }



    public virtual void Reload()
    {
        // reload logic, can't reload if full
        if (!isReloading && currentAmmo < maxAmmo)
        {
            // coroutine because of the wait
            StartCoroutine(ReloadCoroutine());
        }
    }

    private System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log($"{weaponName} reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log($"{weaponName} reloaded.");
    }
}
