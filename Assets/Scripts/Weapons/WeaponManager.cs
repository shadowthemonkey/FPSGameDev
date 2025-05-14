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

    [Header("Runtime Weapons")]
    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;
    public Weapon meleeWeapon;

    public Transform weaponHolder;

    private Weapon currentWeapon;

    [SerializeField] private Transform firePoint;

    public float MovementSpeedMultiplier { get; private set; }

    // accessor made for GUI access
    public Weapon GetCurrentWeapon() => currentWeapon;

    private void Start()
    {
        // instantiate and equip default weapons
        primaryWeapon = null; // no primary at start
        secondaryWeapon = InstantiateWeapon(uspPrefab);
        meleeWeapon = InstantiateWeapon(knifePrefab);

        EquipWeapon(secondaryWeapon); // start with USP
        MovementSpeedMultiplier = 0.9f;
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
            currentWeapon.gameObject.SetActive(false); // hide previous weapon
        }

        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true); // show new weapon
        //Debug.Log("Switched to: " + currentWeapon.weaponName);
    }
}

