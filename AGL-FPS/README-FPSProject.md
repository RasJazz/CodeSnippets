### Overview
The main purpose of this document is to discuss my code contributions to Team 2's Aztec Game Lab FPS Workshop Project. It also discusses the collaborative efforts of Team 2 as well as the game's background information. Unfortunately, due to timing, Team 2 was unable to finish the final project. However, it was a great learning experience and taught everyone on the team valuable skills.
### Team Members
Our programming team consisted of:
- JonnyTheKing-3
- Mystievous
- RasJazz (me)
### Table of Contents
[[#Game Description]]
[[#Code Discussion]]
- [[#Enemies]]
	- [[#`EnemyBase.cs`|EnemyBase.cs]]
		- Logic
		- My Code Contributions
	- [[#`Caster.cs`|Caster.cs]]
		- Logic
		- My Code Contributions
	- [[#`Melee.cs`|Melee.cs]]
		- Logic
		- My Code Contributions
- [[#Spells]]
	- [[#`FireballProjectile.cs`|FireballProjectile.cs]]
		- Logic
		- My Code Contributions
- [[#Conclusion]]
### Timeframe: 2-3 weeks
### Game Description
Team 2's FPS was intended to have elements of fantasy, taking inspiration from Shoot-Em-Up styled games. Enemies consisted of flying casters based on biblical depictions of angels and melee ground units. The game ended when the Player lost all health, giving the player to restart or quit the game.
### Code Discussion
The following portions are what I contributed to the project. Some parts I was individually responsible for, while others were collaboration efforts with my team.
### Enemies
#### `EnemyBase.cs`
The `EnemyBase` class is the base class for all enemies.
```
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
	        
	[HideInInspector] public void TakeDamage(float objectHealth)  
	{
		// decreases object's health by damage amount  
		if (objectHealth <= 0) // if enemy is dead, destroy object  
		{  
			Destroy(gameObject);  
		}    
	}
}
```
##### Logic
Enemies are created as a `NavMeshAgent` to give them AI behavior. The `distanceToTarget` variable is used in the logic involved that causes enemies to target the Player and continue targeting until the Player leaves `leashRange`. `initialChaseRange` is the initial distance it takes for the Player to aggro the enemy, with `chaseRange` acting as a temp variable that initially holds the value of `initialChaseRange` and updates to `leashRange` once enemy is engaged. 
##### My Code Contributions
I was responsible for this base class.
#### `Caster.cs`

```
using Magic;
  
[RequireComponent(typeof(EnemySpellController))]  
public class Caster : EnemyBase  
{  
    [SerializeField] public float backUpDistance;  
    private NavMeshAgent _casterAI;  
    private EnemySpellController _spellController; 
  
    [SerializeField] public GameObject casterEnemy;
      
    protected override void EnemyAI()  
    {        
	    _casterAI = GetComponent<NavMeshAgent>();  
        _spellController = GetComponent<EnemySpellController>();  
    }  
    void Start()  
    {        
	    EnemyAI();  
        health = 25.0f;  
        chaseRange = initialChaseRange; 
    }
        
    void Update()  
    { 
        Vector3 casterPos = transform.position;  
        Vector3 targetPos = target.position;  
        Vector3 flatTargetPos = new Vector3(targetPos.x, casterPos.y, targetPos.z);   
          
		distanceToTarget = Vector3.Distance(flatTargetPos, casterPos);   
        // If distance to player is w/i chase range, spells are turned on  
		// If Caster enemy is not at target distance, approaches player until it is 
		// Else, Caster enemy is too close to Player and backs up        
		
		if (distanceToTarget <= chaseRange)  
        {            
	        _spellController.isActive = true; // spells are turned on    
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
        }  
        else  
        {  
            _spellController.isActive = false; // spells stop firing and chase range is reset to init  
            chaseRange = initialChaseRange; // chase range reset  
        }  
    }
}
```
Finally, when disengaged and out of range, the `chaseRange` 
#### `Melee.cs`
### Spells
#### `FireballProjectile.cs`
The method for Fireball collision was created by Mystievous, with contributions by RasJazz. Below is one method in the `FireballProjectile` class.
```
private void OnTriggerEnter(Collider other)  
{  
    if (FireballSpell.ignoreTags.Any(other.CompareTag))  
    {        
	    return;  
    }    
    if (other.TryGetComponent(out Melee melee))  
    {   
	    // Spell hit a melee enemy  
        melee.health -= FireballSpell.damage;  
        melee.TakeDamage(melee.health);  
    }
    else if (other.TryGetComponent(out Caster caster))  
    {   
	    // Spell hit a caster enemy  
        caster.health -= FireballSpell.damage;  
        caster.TakeDamage(caster.health);  
    }    
    // logic for Player being hit  
    if (other.CompareTag("Player"))  
    {   
	    // Handle player collision  
        var playerMovement = other.GetComponentInParent<PlayerMovement>();  
        if (playerMovement == null) return;  
        playerMovement.playerHealth -= FireballSpell.damage;  
        playerMovement.TakeDamageFromEnemy();  
        return;  
    }    
    
    Destroy(gameObject);  
}
```
##### Logic
When the Fireball object collides with a Rigidbody, it checks what other object it collided with. 
- If not a valid target, the method is exited.
- If the Fireball spell collides with a melee enemy when used by the Player, it calculates the health decrease from melee's base health and passes it in as an argument to the `TakeDamage` method (located in the `EnemyBase` base class), decreasing the enemy's health accordingly. 
- There is logic to do the same for Caster enemies and the Player.
##### My Code Contributions
- Calling `TakeDamage` and passing in melee health decrease
`melee.TakeDamage(melee.health);`
- Calling `TakeDamage` and passing in caster health decrease
`caster.TakeDamage(caster.health);`
- Player Damage logic
```
// logic for Player being hit  
if (other.CompareTag("Player"))  
{  
    // Handle player collision  
    var playerMovement = other.GetComponentInParent<PlayerMovement>();  
    if (playerMovement == null) return;  
    playerMovement.playerHealth -= FireballSpell.damage;  
    playerMovement.TakeDamageFromEnemy();  
    return;  
}
```
### Conclusion
After working on this project, I learned a lot about the following:
- Time management
	Due to the timing of the project, it was difficult for us all as full time students to find the time to work on this project. It is extremely important to manage time effectively, especially when deadlines are involved. To mitigate this in the future, I believe it would be best to make a general plan to get a working prototype out. Then, if time allows, details can be added on.
- Effective communication
	Our team did an amazing job at staying connected. When one person needed assistance on a portion, we all made sure that task was able to be reallocated to someone else on the team to ensure it was finished. We also had extensive talks on the interconnectivity of each part and spent some time working on requirements gathering.
- Programming enemies and their behavior
	Although I still have a lot of learning to do on this subject, this project was a great introduction to Enemy AI for me. I learned how to utilize `NavMesh` tools to some degree. 
- Object-Oriented Programming concepts
	This was also my first time learning how to compose classes in C#. My main programming language is C++, so taking what I know and applying it in this situation was extremely rewarding. I learned how to apply OOP concepts such as inheritance, abstraction, etc. in the context of Unity development, a first for me in my history of my programming journey!
Overall, this was such a great semester, and I am happy to have been a part of this project. I hope to take everything I have learned so far and build upon it for future projects.