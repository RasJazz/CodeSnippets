using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent enemyAINavMeshAgent; // variable for accessing NavMeshAgent AI features
    protected float distanceToTarget = Mathf.Infinity;
    
    [SerializeField] public Transform target; // player 
    [SerializeField] public float initialChaseRange;// Sets distance that enemy will follow player
    protected float chaseRange; // temp variable to hold initChaseRange, resets when player leashes enemy
    [SerializeField] public float leashRange; // distance enemy will aggro player until player escapes
    [SerializeField] public float health;
    protected string enemyName;
    
    protected EnemyBase()
    {
        initialChaseRange = 15.0f;
        chaseRange = 0.0f;
        leashRange = 20.0f;
        health = 0.0f;
        enemyName = "noName";
    }
    
    protected EnemyBase(float chaseRange, float leashRange, float health)
    {
        this.chaseRange = chaseRange;
        this.leashRange = leashRange;
        this.health = health;
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
