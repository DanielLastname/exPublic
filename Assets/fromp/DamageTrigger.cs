using UnityEngine;
using UnityEngine.AI;

public class DamageTrigger : MonoBehaviour
{
    public float AttackPower = 10f;  // Amount of damage to apply
    //public float knockbackForce = 1f;  // Force to apply for knockback

    public Stats playerStats;

    public bool PhysicalAttack;
    public bool FireAttack;
    public bool iceAttack;

    private void Start()
    {
        if (playerStats != null) return;
        PlayerController pc = GetComponentInParent<PlayerController>();
        playerStats = pc.GetComponent<Stats>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object has the IDamagable interface
        IDamagable damagableObject = other.GetComponent<IDamagable>();

        if (damagableObject != null)
        {
            //Get enemy values
            var pwr = AttackPower;

            var enemyStats = other.GetComponent<Stats>();
            if (enemyStats != null && playerStats != null)
            {
                //Damage Calc

                float PlayerStrength;
                float PlayerFire;
                float PlayerIce;

                var c = playerStats.CritChance;
                c = Random.Range(1, c);
                c = Mathf.Clamp(c, 1, 249);

                var def = enemyStats.Def;
                var fireDefense = enemyStats.fireDef;
                var iceDefense = enemyStats.iceDef;

                var p = PhysicalAttack ? 1 : 0;
                var f = FireAttack ? 1 : 0;
                var i = iceAttack ? 1 : 0;

                PlayerStrength = playerStats.Str * p;
                PlayerFire = playerStats.fire * f;
                PlayerIce = playerStats.ice * i;

                // Damage calculation (simplified)
                float totalAttack = 250 - c + PlayerStrength + PlayerFire + PlayerIce;
                float totalDefense = 250 - c + def + fireDefense + iceDefense;

                pwr = totalAttack / totalDefense * pwr;  // Apply to the base power

                if (pwr <= 1) pwr = 1;
            }
            if (pwr <= 1) pwr = 1;
            // If it does, apply damage
            damagableObject.TakeDamage(pwr);
            Debug.Log("Damage applied to: " + other.name);

            // Apply knockback if the object has a Rigidbody component
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Disable NavMeshAgent if present
                NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    
                    var EC = other.GetComponent<EnemyController>();
                    if (EC != null)
                    {
                        Vector3 knockbackDirection = (other.transform.position - transform.position);
                        knockbackDirection.Normalize();
                        knockbackDirection *= playerStats.knockBack;
                        EC.DanielPhysics(knockbackDirection,playerStats.knockBack);
                        //EC.KnockBacker(knockbackDirection);
                        //EC.ApplyKnockback(knockbackDirection, playerStats.knockBack);
                        //EC.HandleCustomKnockback(knockbackDirection, playerStats.knockBack, 2);
                    }
                }

                // Calculate knockback direction
                

                // Apply force
                //rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
