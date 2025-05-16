using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private int _currentHealth = 100;

    [SerializeField]
    private int maxHealth = 100;

    public int CurrentHealth => _currentHealth;

    // Manual sync method
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        UpdateHealthOnClients(_currentHealth);
    }

    [ObserversRpc]
    private void UpdateHealthOnClients(int newHealth)
    {
        _currentHealth = newHealth;
        // Optionally update UI or effects here
    }
}