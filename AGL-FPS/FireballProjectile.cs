/*
 * Note: This code was produced by Mystievous with collaboration by RasJazz
 * Document will contain comments on RasJazz's portions 
 */

using System.Linq;
using UnityEngine;

namespace Magic.SpellTypes.Fireball
{
    [RequireComponent(typeof(Rigidbody))]
    public class FireballProjectile : MonoBehaviour
    {

        private float _timeAlive;
        public FireballSpell FireballSpell { private get; set; }

        private void Start()
        {
            GameObject projectile = gameObject;
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.useGravity = false;
            projectileRb.velocity = Vector3.zero;
            projectileRb.AddRelativeForce(Vector3.forward * FireballSpell.speed * 10, ForceMode.VelocityChange);
        }

        // Update is called once per frame
        private void Update()
        {
            _timeAlive += Time.deltaTime;
            if (_timeAlive >= 5)
            {
                Destroy(gameObject);
            }
        }

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
    }
}
