### Overview
The main purpose of this document is to discuss my code contributions to Team 2's Aztec Game Lab FPS Workshop Project. It also discusses the collaborative efforts of Team 2 as well as the game's background information. Unfortunately, due to timing, Team 2 was unable to finish the final project. However, it was a great learning experience and taught everyone on the team valuable skills.
### Team Members
Our programming team consisted of:
- JonnyTheKing-3
- Mystievous
- RasJazz (me)
### Table of Contents
<a href="#Code-Discussion">Code Discussion</a>
<ul>
    <li><a href="#Enemies">Enemies</a>
	<ul>
	    <li><a href="#EnemyBase.cs">EnemyBase.cs</a>
		<ul>
		    <li><a href="#EnemyBaseLogic">Logic</a></li>
		    <li><a href="#EnemyContribution">My Code Contributions</a></li>
		</ul>
	    </li>
		<li><a href="#Caster.cs">Caster.cs</a>
		<ul>
		    <li><a href="#CasterLogic">Logic</a></li>
		    <li><a href="#CasterContribution">My Code Contributions</a></li>
		</ul>
	    </li>
		<li><a href="#Melee.cs">Melee.cs</a>
		<ul>
		    <li><a href="#MeleeLogic">Logic</a></li>
		    <li><a href="#MeleeContribution">My Code Contributions</a></li>
		</ul>
	    </li>
	</ul>
    </li>
    <li><a href="#Spells">Spells</a>
	<ul>
	    <li><a href="#FireballProjectile.cs">FireballProjectile.cs</a>
		<ul>
		    <li><a href="#FireballLogic">Logic</a></li>
		    <li><a href="#FireballContribution">My Code Contributions</a></li>
		</ul>
	    </li>
	</ul>
    </li>
</ul>  
<a href="#Conclusion">Conclusion</a>

##### Timeframe: 2-3 weeks

### Game Description
Team 2's FPS was intended to have elements of fantasy, taking inspiration from Shoot-Em-Up styled games. Enemies consisted of flying casters based on biblical depictions of angels and melee ground units. The game ended when the Player lost all health, giving the player to restart or quit the game.

<h3 id="Code-Discussion">Code Discussion</h3>
The following portions are what I contributed to the project. Some parts I was individually responsible for, while others were collaboration efforts with my team.

---
<h3 id="Enemies">Enemies</h3>

<h4 id="EnemyBase.cs"><code>EnemyBase.cs</code></h4>
The <code>EnemyBase</code> class is the base class for all enemies.<br>
<a href="AGL-FPS/EnemyBase.cs">EnemyBase.cs file</a>

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
<h5 id="EnemyBaseLogic">Logic</h5>
Enemies are created as a <code>NavMeshAgent</code> to give them AI behavior. The <code>distanceToTarget</code> variable is used in the logic involved that causes enemies to target the Player and continue targeting until the Player leaves <code>leashRange</code>. <code>initialChaseRange</code> is the initial distance it takes for the Player to aggro the enemy, with <code>chaseRange</code> acting as a temp variable that initially holds the value of <code>initialChaseRange</code> and updates to <code>leashRange</code> once enemy is engaged. 
<h5 id="EnemyContribution">My Code Contributions</h5>
I was responsible for this base class.

<h4 id="Caster.cs"><code>Caster.cs</code></h4>
The <code>Caster</code> class is a derived class of <code>EnemyBase</code> class. It implementes attacking and movement logic for Caster Enemies which are flying units.<br>
<a href="AGL-FPS/Caster.cs">Caster.cs file</a>

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

<h5 id="CasterLogic">Logic</h5>
In line 3 of the <code>Update()</code> method in the<code>Caster</code> class, the Caster Enemy is set to have a different y-position than the Player because it is a floating enemy. The distance in between the Player and the Enemy is calculated. If the distance between the two points is less than or equal to the <code>chaseRange</code>, the Enemy's spells are turned on and it starts attacking the Player. When the Enemy's distance is at the <code>backUpDistance</code>, it stays in position. If the Player approaches it, it begins to back up to the <code>backUpDistance</code>. <code>chaseRange</code> is then updated to equal the <code>leashRange</code> value. Finally, when the Player's distance from the Enemy is greater than the <code>leashRange</code>, <code>chaseRange</code> is reverted back to its default value and the Enemy stops firing.

<h5 id="CasterContribution">My Code Contributions</h5>
<pre><code>
protected override void EnemyAI(): _casterAI = GetComponent<NavMeshAgent>();
void Start()
{
	EnemyAI();
	health = 25.0f;
	chaseRange = initialChaseRange;
	name = "Caster";
}
void Update():
if (distanceToTarget <= chaseRange)
{
	_spellController.isActive = true; // spells are turned on
	// Rest of logic by Mystievous
}
else
{
	_spellController.isActive = false; // spells stop firing and chase range is reset to init
	chaseRange = initialChaseRange; // chase range reset
}
</code></pre>

<h4 id="Melee.cs"><code>Melee.cs</code></h4>
The <code>Melee</code> class is a derived class of <code>EnemyBase</code> class. It implementes attacking and movement logic for Melee Enemies which are ground units.<br>
<a href="AGL-FPS/Melee.cs">Melee.cs file</a>

```
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
```

<h5 id="MeleeLogic">Logic</h5>
<p>The <code>Melee</code> class starts by calculating the distance between the Melee Enemy and the Player. Like the <code>Caster</code> class, it contains logic that drives the enemy to start attacking the Player when the Player comes into range. <code>chaseRange</code> is updated to take on the <code>leashRange</code> value and resets to the initial value when the Player escapes. The method <code>OnCollisionEnter()</code> handles attacking the Player and checks to see if the Player's health reaches 0.</p>
<h5 id="MeleeContribution">My Code Contributions</h5>
<p>I was responsible for this entire class, including the <code>TakeDamageFromEnemy()</code> method (not included in this document) called in <code>OnCollisionEnter()</code>.</p>

---
<h3 id="Spells">Spells</h3>
<h4 id="FireballProjectile.cs"><code>FireballProjectile.cs</code></h4>
The method for Fireball collision was created by Mystievous, with contributions by RasJazz. Below is one method in the <code>FireballProjectile</code> class.

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
<h5 id="FireballLogic">Logic</h5>
When the Fireball object collides with a Rigidbody, it checks what other object it collided with. 
<ul>
	<li>If not a valid target, the method is exited.</li>
	<li>If the Fireball spell collides with a Melee Enemy when used by the Player, it calculates the health decrease from Melee's base health and passes it in as an argument to the <code>TakeDamage()</code> method (located in the <code>EnemyBase</code> base class), decreasing the Enemy's health accordingly.</li> 
	<li>There is logic to do the same for Caster Enemies and the Player.</li>
</ul>
<h5 id="FireballContribution">My Code Contributions</h5>
<ul>
	<li>Calling <code>TakeDamage</code> and passing in Melee health decrease: <code>melee.TakeDamage(melee.health);</code></li>
	<li>Calling <code>TakeDamage</code> and passing in Caster health decrease: <code>caster.TakeDamage(caster.health);</code></li>
	<li>Player Damage logic</li>
</ul>

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
<h3 id="Conclusion">Conclusion</h3>
<p>After working on this project, I learned a lot about the following:</p>
<ul>
	<li>Time management</li>
	Due to the timing of the project, it was difficult for us all as full time students to find the time to work on this project. It is extremely important to manage time effectively, especially when deadlines are involved. To mitigate this in the future, I believe it would be best to make a general plan to get a working prototype out. Then, if time allows, details can be added on.
	<li>Effective communication</li>
	Our team did an amazing job at staying connected. When one person needed assistance on a portion, we all made sure that task was able to be reallocated to someone else on the team to ensure it was finished. We also had extensive talks on the interconnectivity of each part and spent some time working on requirements gathering.
	<li>Programming enemies and their behavior</li>
	Although I still have a lot of learning to do on this subject, this project was a great introduction to Enemy AI for me. I learned how to utilize NavMesh tools to some degree. 
	<li>Object-Oriented Programming concepts</li>
	This was also my first time learning how to compose classes in C#. My main programming language is C++, so taking what I know and applying it in this situation was extremely rewarding. I learned how to apply OOP concepts such as inheritance, abstraction, etc. in the context of Unity development, a first for me in my history of my programming journey!
</ul>
<p>Overall, this was such a great semester, and I am happy to have been a part of this project. I hope to take everything I have learned so far and build upon it for future projects.</p>
