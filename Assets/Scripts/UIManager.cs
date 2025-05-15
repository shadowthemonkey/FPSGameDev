using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerMovement playerMovement; // speedometer in this script
    public WeaponManager weaponManager; // weapon details in this script

    [Header("UI Elements")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI weaponInfoText;

    public GameObject buyMenuPanel;
    public static bool IsBuyMenuOpen { get; private set; }

    public Button uspButton;
    public Button deagleButton;

    public Button akButton;
    public Button m4Button;
    public Button awpButton;

    private void Start()
    {
        buyMenuPanel.SetActive(false); // hide on start

        uspButton.onClick.AddListener(() => Purchase("USP"));
        deagleButton.onClick.AddListener(() => Purchase("Deagle"));

        akButton.onClick.AddListener(() => Purchase("AK"));
        m4Button.onClick.AddListener(() => Purchase("M4"));
        awpButton.onClick.AddListener(() => Purchase("AWP"));
    }

    public void ToggleBuyMenu()
    {
        // active --> inactive --> active
        bool isMenuOpen = !buyMenuPanel.activeSelf;
        buyMenuPanel.SetActive(isMenuOpen);
        IsBuyMenuOpen = isMenuOpen; // accessed in playerinputs
        // cursor no longer locked to centre if buy menu opened, also visible
        Cursor.lockState = buyMenuPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = buyMenuPanel.activeSelf; // uf buy menu is up, then cursor visible
    }

    private void Purchase(string weaponName)
    {
        bool success = weaponManager.BuyWeapon(weaponName);
        // kinda useless class for now, just considering if any other logic needs to happen here
    }

    private void Update()
    {
        UpdateSpeedometer();
        UpdateWeaponInfo();
    }

    private void UpdateSpeedometer()
    {
        if (playerMovement == null) return;

        // get the current speed from PlayerMovement.LogSpeed, changed to a return float
        speedText.text = $"Speed: {playerMovement.LogSpeed():F2} m/s";
    }

    private void UpdateWeaponInfo()
    {
        if (weaponManager == null || weaponManager.GetCurrentWeapon() == null) return;

        // get weapon details of the current weapon from the weapon manager
        string weaponName = weaponManager.GetCurrentWeapon().GetWeaponName();
        int ammoInMag = weaponManager.GetCurrentWeapon().GetCurrentAmmo();
        int totalAmmo = weaponManager.GetCurrentWeapon().GetTotalAmmo();

        // update UI (Weapon name Ammo: x / y)
        weaponInfoText.text = $"{weaponName} Ammo: {ammoInMag} / {totalAmmo}";
    }
}
