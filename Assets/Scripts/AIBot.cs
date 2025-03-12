using UnityEngine;
using UnityEngine.AI;

public class AIBot : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float detectionRange = 20f;
    public float attackRange = 3f;
    public float respawnTime = 2f;
    public float maxHealth = 100f;

    private NavMeshAgent agent;
    private Vector3 spawnPoint;
    private float currentHealth;

    // I baked a NavMesh beforehand

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        spawnPoint = transform.position;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // set destination to player's position
            agent.SetDestination(player.position);

            // if close enough, stop moving
            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                // bot doesn't damage yet so it just stands still at this point
            }
            else
            {
                agent.isStopped = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Bot died! Respawning...");
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnTime);
        // wait respawnTime seconds until respawning
    }

    private void Respawn()
    {
        transform.position = spawnPoint;
        currentHealth = maxHealth;
        gameObject.SetActive(true);
    }
}
