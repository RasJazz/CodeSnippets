using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent enemyAINavMeshAgent; // variable for accessing NavMeshAgent AI features
    [SerializeField] public Transform target; // player 
    
    protected float distanceToTarget;
    protected float chaseRange; // temp variable to hold initChaseRange, resets when player leashes enemy
    [SerializeField] public float initialChaseRange;// Sets distance that enemy will follow player
    [SerializeField] public float leashRange; // distance enemy will aggro player until player escapes
    [SerializeField] public float health;
    
    protected EnemyBase()
    {
        distanceToTarget = Mathf.Infinity;
        chaseRange = initialChaseRange;
    }

    protected virtual void EnemyAI()
    {
        enemyAINavMeshAgent = GetComponent<NavMeshAgent>();
    }
        
    [HideInInspector] public void TakeDamage(float health)
    { 
        // decreases health by damage amount
        if (health <= 0) // if enemy is dead, destroy object
        {
            Destroy(gameObject);
        }
    }
}
