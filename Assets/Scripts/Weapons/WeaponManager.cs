using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject knifePrefab;
    public GameObject uspPrefab;
    public GameObject deaglePrefab;
    public GameObject akPrefab;
    public GameObject m4Prefab;
    public GameObject awpPrefab;
    public GameObject shotgunPrefab;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    private Weapon meleeWeapon;

    public Transform weaponHolder;

    private Weapon currentWeapon;
    [SerializeField] private Transform firePoint;

    public float MovementSpeedMultiplier { get; private set; }

    // accessor made for GUI access
    public Weapon GetCurrentWeapon() => currentWeapon;

    private void Start()
    {
        // instantiate and equip default weapons
        //primary starts off as null    
        secondaryWeapon = InstantiateWeapon(uspPrefab);
        meleeWeapon = InstantiateWeapon(knifePrefab);

        SetSecondary(); // start with pistol, since you don't have primary
    }

    private Weapon InstantiateWeapon(GameObject prefab)
    {
        GameObject weaponInstance = Instantiate(prefab, weaponHolder); // place it in holder
        Weapon weaponComponent = weaponInstance.GetComponent<Weapon>();
        weaponComponent.firePoint = firePoint; // assign player camera as firePoint
        return weaponComponent;
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
        if (currentWeapon != primaryWeapon && primaryWeapon != null)
        {
            EquipWeapon(primaryWeapon);
            MovementSpeedMultiplier = 0.8f;
        }
    }
    public void SetSecondary()
    {
        if (currentWeapon != secondaryWeapon && secondaryWeapon != null)
        {
            EquipWeapon(secondaryWeapon);
            MovementSpeedMultiplier = 0.9f;
        }
    }

    public void SetMelee()
    {
        if (currentWeapon != meleeWeapon && meleeWeapon != null)
        {
            EquipWeapon(meleeWeapon);
            MovementSpeedMultiplier = 1.0f; // you run faster with a knife
        }
    }

    private void EquipWeapon(Weapon newWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.CancelReload(); // just in case of the user reloading then switching weapons before completion
            currentWeapon.gameObject.SetActive(false); // hide previous weapon
        }

        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true); // show new weapon
        //Debug.Log("Switched to: " + currentWeapon.weaponName);
    }

    public bool BuyWeapon(string weaponName)
    {
        GameObject prefab = null;
        //boolean for now, might have to switch to a better system if other weapon types are added
        bool isPrimary = false;

        // cases reference in UIManager
        switch (weaponName)
        {
            case "USP":
                prefab = uspPrefab;
                isPrimary = false;
                break;
            case "Deagle":
                prefab = deaglePrefab;
                isPrimary = false;
                break;
            case "AK":
                prefab = akPrefab;
                isPrimary = true;
                break;
            case "M4":
                prefab = m4Prefab;
                isPrimary = true;
                break;
            case "AWP":
                prefab = awpPrefab;
                isPrimary = true;
                break;
        }

        if (prefab == null) return false;

        // instantiate
        Weapon newWeapon = InstantiateWeapon(prefab);

        if (isPrimary)
        {
            if (primaryWeapon != null)
                Destroy(primaryWeapon.gameObject);

            primaryWeapon = newWeapon;
            EquipWeapon(primaryWeapon);
            MovementSpeedMultiplier = 0.8f;
        }
        else
        {
            if (secondaryWeapon != null)
                Destroy(secondaryWeapon.gameObject);

            secondaryWeapon = newWeapon;
            EquipWeapon(secondaryWeapon);
            MovementSpeedMultiplier = 0.9f;
        }

        return true;
    }
}

