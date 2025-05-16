using System;
using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public int magSize;
    public int reserveAmmo;
    protected int currentAmmo;
    protected bool isReloading = false;
    private Coroutine reloadCoroutine;
    public enum FireMode { SemiAuto, FullAuto, Sniper}
    public FireMode fireMode;
    public float fireRate;
    public float reloadTime;
    public int damage;
    public int penetrationPower; // number of layers
    public float penetrationRate; //damage reduction per layer

    // variables for recoil and spray control
    protected int shotCount = 0;
    protected float recoilResetTime = 0.3f; // time after which spray resets
    protected float lastRecoilTime = 0f;
    protected float lastShotTime;

    protected float horizontalSprayScale = 0.02f;
    protected float verticalSprayScale = 0.02f;

    private Vector2 totalRecoil = Vector2.zero;
    
    public Transform firePoint; // currently uses the camera as the origin for raycast rays
    protected PlayerLook playerLook;
    public Camera playerCamera;

    // sniper zoom behaviour
    protected bool isScoped = false;
    protected int zoomLevel = 0; // goes between 0 to 2 for the 2 different scope levels
    //public Sprite scopeOverlay;
    private float defaultFOV;
    private float[] zoomLevels = {30f, 15f};

    public WeaponManager weaponManager;


    // getter methods to access ammo count and weapon name for GUI
    public string GetWeaponName() => weaponName;
    public int GetCurrentAmmo() => currentAmmo;
    public int GetTotalAmmo() => reserveAmmo;

    protected virtual void Awake()
    {
        InitializeWeaponStats();
        currentAmmo = magSize;
        playerLook = FindFirstObjectByType<PlayerLook>();

        // cache original field of view before scoped
        if (playerCamera == null)
            playerCamera = Camera.main;

        defaultFOV = playerCamera.fieldOfView;

    }

    protected virtual void Update()
    {
        // spray end
        if (Time.time - lastRecoilTime > recoilResetTime)
            shotCount = 0;
            totalRecoil = Vector2.zero;
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


        Vector2 sprayOffset = GetSprayPatternOffset(); // renamed for clarity
        ApplyRecoil(sprayOffset);

        Vector3 direction = firePoint.forward
        + firePoint.up * sprayOffset.y * verticalSprayScale
        + firePoint.right * sprayOffset.x * horizontalSprayScale;
        direction.Normalize();
        FireRaycast(damage, direction);

        shotCount++;
        lastRecoilTime = Time.time;

        //FireRaycast(damage);
    }

    // overloaded method
    protected virtual void FireRaycast(float startingDamage)
    {
        FireRaycast(startingDamage, firePoint.forward); // default case
    }

    protected virtual void FireRaycast(float startingDamage, Vector3 direction)
    {
        Vector3 origin = firePoint.position;

        // list of all raycast hits, so we can deal with wallbangs and collaterals (hitting enemies behind walls and other enemies)
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, 100f);
        Debug.DrawRay(origin, direction * 100f, Color.red, 5.0f);
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
                    print($"Bot takes {Mathf.RoundToInt(currentDamage)} damage");
                    bot.TakeDamage(Mathf.RoundToInt(currentDamage));
                    penetrations++;
                }
            }
            else if (hit.collider.CompareTag("Player")) // if hit an enemy
            {
                AIBot bot = hit.collider.GetComponent<AIBot>();
                if (bot != null)
                {
                    print($"Player takes {Mathf.RoundToInt(currentDamage)} damage");
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

    protected virtual Vector3 GetSprayPatternOffset()
    {
        return Vector3.zero; // default: no spray, example weapons like non-automatic weapons would not have sprays
    }

    protected virtual void ApplyRecoil(Vector2 sprayOffset)
    {
        if (playerLook != null)
        {
            playerLook.ApplyRecoil(new Vector2(sprayOffset.x / 2, sprayOffset.y / 2));
        }
    }

    public virtual void ToggleScope()
    {
        UnityEngine.UI.Image scopeOverlay = weaponManager.GetScopeImage();
        // 0 = not scoped, 1 = scoped level 1, 2 = scoped level 2
        zoomLevel++;
        // zoomed past level 2, unscope
        if (zoomLevel > 2)
        {
            zoomLevel = 0;
            isScoped = false;
            playerCamera.fieldOfView = defaultFOV;
            if (scopeOverlay != null)
                scopeOverlay.enabled = false;
        }
        else
        {
            // zoom will be 1 or 2
            isScoped = true;
            // if zoomlevel is 1, then we are 1 scope in, if 2, then we are 2
            float zoom = zoomLevel == 1 ? zoomLevels[0] : zoomLevels[1];
            playerCamera.fieldOfView = zoom;
            if (scopeOverlay != null)
            {
                scopeOverlay.enabled = true;
                //Debug.Log("Scope image enabled: " + scopeOverlay.enabled);
            }
        }
    }

    public virtual void Reload()
    {
        // reload logic, can't reload if full
        if (!isReloading && currentAmmo < magSize)
        {
            if (!isReloading && reserveAmmo >= 0)
            {
                // coroutine because of the wait
                reloadCoroutine = StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log($"{weaponName} reloading...");

        yield return new WaitForSeconds(reloadTime);

        int neededAmmo = magSize - currentAmmo;
        // pick the smallest, if reserve ammo is smaller than needed ammo, that means the player doesn't have enough to get to full magazine size
        int ammoToReload = Mathf.Min(neededAmmo, reserveAmmo);

        // ammo goes from reserve to current
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        Debug.Log($"{weaponName} reloaded.");
    }

    // call cancel when switching weapons
    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        isReloading = false;
    }
}
