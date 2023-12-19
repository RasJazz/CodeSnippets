using Magic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemySpellController))]
public class Caster : EnemyBase
{
    [SerializeField] public float backUpDistance;
    private NavMeshAgent _casterAI;
    private EnemySpellController _spellController; // Code by Mystievous

    [SerializeField] public GameObject casterEnemy;
    
    protected override void EnemyAI()
    {
        _casterAI = GetComponent<NavMeshAgent>();
        _spellController = GetComponent<EnemySpellController>();  // Code by Mystievous
    }
    void Start()
    {
        EnemyAI();
        health = 25.0f;
        chaseRange = initialChaseRange;
        name = "Caster";
    }
    
    void Update()
    {
        // Section of code by Mystievous
        Vector3 casterPos = transform.position;
        Vector3 targetPos = target.position;
        Vector3 flatTargetPos = new Vector3(targetPos.x, casterPos.y, targetPos.z); 
        
        distanceToTarget = Vector3.Distance(flatTargetPos, casterPos);
        // End section of code by Mystievous
        
        // If distance to player is w/i chase range, spells are turned on
        // If Caster enemy is not at target distance, approaches player until it is
        // Else, Caster enemy is too close to Player and backs up
        if (distanceToTarget <= chaseRange)
        {
            _spellController.isActive = true; // spells are turned on
            
            // Section of code refactored by Mystievous
            if (distanceToTarget >= backUpDistance)
            {
                _casterAI.SetDestination(target.position);
                _casterAI.stoppingDistance = backUpDistance; // Stops enemy short of player; casters only
                
                chaseRange = leashRange;
            }
            else
            {
                Vector3 targetToCaster = casterPos - flatTargetPos;
                Vector3 backupPos = (targetToCaster.normalized * 10) + flatTargetPos;
                _casterAI.SetDestination(backupPos);
                _casterAI.stoppingDistance = 0;
            }
            // End section of code by Mystievous
        }
        else
        {
            _spellController.isActive = false; // spells stop firing and chase range is reset to init
            chaseRange = initialChaseRange; // chase range reset
        }
    }
}

