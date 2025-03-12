using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;
    public Weapon meleeWeapon;

    private Weapon currentWeapon;

    public float MovementSpeedMultiplier { get; private set; } = 0.8f; // rifle value as default for now

    // accessor made for GUI access
    public Weapon GetCurrentWeapon() => currentWeapon;

    private void Start()
    {
        EquipWeapon(primaryWeapon); // start holding primary weapon by default for now
    }

    public void Shoot()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Shoot();
        }
    }

    public void Reload()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Reload();
        }
    }

    public void SetPrimary()
    {
        // don't do unnecessary code if primary is already out, same goes for all the others
        if (currentWeapon != primaryWeapon)
        {
            EquipWeapon(primaryWeapon);
            MovementSpeedMultiplier = 0.8f;
        }
    }
    public void SetSecondary()
    {
        if (currentWeapon != secondaryWeapon)
        {
            EquipWeapon(secondaryWeapon);
            MovementSpeedMultiplier = 0.9f;
        }
    }

    public void SetMelee()
    {
        if (currentWeapon != meleeWeapon)
        {
            EquipWeapon(meleeWeapon);
            MovementSpeedMultiplier = 1.0f; // you run faster with a knife
        }
    }

    private void EquipWeapon(Weapon newWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false); // hide previous weapon
        }

        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true); // show new weapon
        //Debug.Log("Switched to: " + currentWeapon.weaponName);
    }
}

