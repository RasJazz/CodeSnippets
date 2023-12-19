using UnityEngine;

public class Melee : EnemyBase
{
    void Start()
    {
        EnemyAI();
        chaseRange = initialChaseRange;
        health = 30.0f;
        name = "Melee";
    }
    
    void Update()
    {
        distanceToTarget = Vector3.Distance(target.position, transform.position);
        
        // If player is w/i x units of enemy, enemy engages player
        if (distanceToTarget <= chaseRange)
        {
            enemyAINavMeshAgent.SetDestination(target.position);
            chaseRange = leashRange;
        }
        else // when player out of range of enemy, chaseRange is reset
        {
            chaseRange = initialChaseRange;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var player = other.gameObject.GetComponent<PlayerMovement>();
        if (player == null) return;
        player.playerHealth -= 3; // using hardcoded value until SpellDamage class updated
        player.TakeDamageFromEnemy(); // checks if player health <= 0; if so, triggers restart/end game UI
    }
}
