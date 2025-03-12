using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public int maxAmmo;
    public float fireRate;
    public float reloadTime;
    public int damage;

    protected int currentAmmo;
    protected bool isReloading = false;

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

    public abstract void Shoot();

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
