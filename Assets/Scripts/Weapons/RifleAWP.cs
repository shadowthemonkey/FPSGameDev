using UnityEngine;

public class RifleAWP : Weapon
{

    [Header("AWP Scope Settings")]
    public GameObject scopeOverlay;
    public GameObject normalCrosshair;
    public float[] zoomLevels = { 30f, 15f };

    private int currentZoomIndex = -1; // starts at -1
    private float originalFOV;
    private bool isScoped = false;

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
        // add scope features next
        // awp shouldn't really have a crosshair since no scope is random
        base.Shoot();
    }
}
