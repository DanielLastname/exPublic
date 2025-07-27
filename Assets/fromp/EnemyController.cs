using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IDamagable
{
    public Stats UnitStats;
    public float currentHealth = 1;
    public float attackRange = 2f;

    private NavMeshAgent agent;
    private Transform target;
    private List<PlayerController> playerStatsList = new List<PlayerController>();

    public Animator anim;

    public GameObject attackHitboxPrefab;  // Assign in inspector
    public Transform attackSpawnPoint;     // Where to spawn the hitbox (e.g., in front of the enemy)
    public float attackCooldown = 1f;

    private float attackTimer = 0f;

    public float height = 1;

    public bool Dumb = false;
    bool start = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        UnitStats = GetComponent<Stats>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        StartCoroutine(FindPlayersCoroutine());

        if (UnitStats != null) currentHealth = UnitStats.Hp;
        else Debug.LogError("not stats component on Unit to set it's health");
    }

    public void TakeDamage(float dmg)
    {
        UnitStats.Hp -= dmg;
        Debug.Log("Enemy took damage! Health: " + UnitStats.Hp);

    }

    void HandleKO()
    {
        CollectibleSpawner CS = GetComponent<CollectibleSpawner>();

        if (CS != null)
        {
            CS.Spawn();
        }

        if (UnitStats.Hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FindPlayersCoroutine()
    {
        while (playerStatsList.Count == 0)
        {
            // Find all PlayerStats in the scene
            PlayerController[] foundPlayers = FindObjectsOfType<PlayerController>();

            if (foundPlayers != null && foundPlayers.Length > 0)
            {
                playerStatsList.AddRange(foundPlayers);
                SetNearestTarget();
            }
            else
            {
                yield return new WaitForSeconds(0.5f); // Wait and check again
            }
        }
    }
    private void SetNearestTarget()
    {
        float minDistSq = Mathf.Infinity;
        Transform nearest = null;

        Vector3 myPos = transform.position;

        foreach (PlayerController ps in playerStatsList)
        {
            if (ps == null) continue;

            float distSq = (ps.transform.position - myPos).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                nearest = ps.transform;
            }
        }

        if (nearest != null)
            target = nearest;
        else
            Debug.LogWarning($"{gameObject.name}: No valid targets found in SetNearestTarget");
    }

    private void Update()
    {
        if (isKnockedBack)
        {
            return; // Don't run normal AI while recovering
        }


        if (Dumb == true) DumbPathing();
        else
        {
            // new code@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            if (target == null) return;
            if (start == false)
            {
                PickWanderPoint();
                start = true;
            }
            else
            {
                HandleStateLogic();
                CheckStuck();
                CheckOffNavMesh();
            }

        }

        if (attackTimer > 0) attackTimer -= Time.deltaTime;

    }

    private void DumbPathing()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > attackRange)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath();
            Attack();
        }

        if (transform.position.y < -100f)
        {
            Destroy(gameObject); // or teleport to a safe location
            Debug.LogWarning("Unit went off mesh and was destroyed");
        }
    }
    

    private Vector3 lastTargetPos = Vector3.zero;

    private void DefaultPathing()
    {
        if (target == null) return;

        float distSqr = (transform.position - target.position).sqrMagnitude;
        float rangeSqr = attackRange * attackRange;

        if (distSqr > rangeSqr)
        {
            if ((target.position - lastTargetPos).sqrMagnitude > 0.1f)
            {
                agent.SetDestination(target.position);
                lastTargetPos = target.position;
            }
        }
        else
        {
            if (!agent.isStopped)
            {
                agent.ResetPath();
            }
            Attack();
        }

        if (transform.position.y < -100f)
        {
            Debug.LogWarning($"{gameObject.name} fell off the nav mesh!");
            Destroy(gameObject); // or teleport to safe zone
        }
    }
    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


    private enum State { Wandering, Chasing }
    private State currentState = State.Wandering;

    public float wanderRadius = 10f;
    public float decisionInterval = 3f;
    public float stuckCheckTime = 2f;

    private float stuckTimer;
    private float decisionTimer;

    private void PickWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void CheckStuck()
    {
        if (!agent.hasPath || agent.pathPending)
            return;

        if (agent.velocity.sqrMagnitude < 0.1f)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckCheckTime)
            {
                Debug.LogWarning($"{gameObject.name} is stuck. Picking new wander point.");
                stuckTimer = 0f;

                if (currentState == State.Wandering)
                    PickWanderPoint();
                else if (currentState == State.Chasing)
                    agent.SetDestination(target.position);
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }


    private void CheckOffNavMesh()
    {
        if (transform.position.y < -100f)
        {
            Debug.LogWarning($"{gameObject.name} fell off the nav mesh!");
            Destroy(gameObject);
        }
    }


    private void HandleStateLogic()
    {
        if (target == null) return;

        float distSqr = (transform.position - target.position).sqrMagnitude;
        float rangeSqr = attackRange * attackRange;

        // Always chase if in range
        if (distSqr <= rangeSqr)
        {
            if (currentState != State.Chasing)
            {
                currentState = State.Chasing;
                Debug.Log($"{gameObject.name} has locked onto player!");
            }

            DefaultPathing();
            return;
        }

        // Handle decision to switch back to wandering
        decisionTimer -= Time.deltaTime;
        if (currentState == State.Chasing && decisionTimer <= 0)
        {
            decisionTimer = decisionInterval;

            if (Random.value < 0.33f)
            {
                currentState = State.Wandering;
                PickWanderPoint();
                Debug.Log($"{gameObject.name} lost interest and is wandering again.");
            }
        }

        switch (currentState)
        {
            case State.Chasing:
                DefaultPathing();
                break;

            case State.Wandering:
                // wander movement already handled via PickWanderPoint()
                break;
        }
    }


    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@22


    public void KnockBacker(Vector3 force)
    {
        StartCoroutine(KnockBack(force));
    }
    
    private IEnumerator KnockBack(Vector3 force)
    {
        yield return null;
        yield return new WaitForFixedUpdate();
        isKnockedBack = true;
        agent.enabled = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        //rb.constraints = RigidbodyConstraints.FreezePositionY;
        //force = force * 100;
        force.y += 5f;
        rb.AddForce(force);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.linearVelocity.magnitude < 0.5f);
        yield return new WaitForSeconds(0.25f);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        agent.Warp(transform.position);

        yield return null;

        agent.enabled = true;

        yield return null;

        StartCoroutine(FindPlayersCoroutine());
        isKnockedBack = false;
    }

    private bool isKnockedBack = false;
    private Rigidbody rb;


    public void DanielPhysics(Vector3 dir, float dist)
    {
        StartCoroutine(DanielPhys(dir, dist));
    }

    private IEnumerator DanielPhys(Vector3 dir, float dist, float knockbackDuration = 0.25f)
    {
        yield return null;

        isKnockedBack = true;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + dir.normalized * dist;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 10f, NavMesh.AllAreas))
        {
            Vector3 finalPos = hit.position;
            finalPos.y += height;

            agent.enabled = false;

            float elapsed = 0f;
            while (elapsed < knockbackDuration)
            {
                transform.position = Vector3.Lerp(currentPos, finalPos, elapsed / knockbackDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure exact position at the end
            transform.position = finalPos;

            // Warp to sync with NavMeshAgent
            agent.enabled = true;
            agent.Warp(finalPos);

            //Debug.DrawLine(currentPos, finalPos, Color.green, 2f);
        }
        else
        {
            Debug.LogWarning("No valid NavMesh position found in that direction.");
        }

        isKnockedBack = false;

        HandleKO();

    }

    // Called when in range
    private void Attack()
    {
        if (attackTimer > 0) return; // still cooling down

        anim.SetTrigger("attack");
        
        if (attackHitboxPrefab != null && attackSpawnPoint != null)
        {
            GameObject hitbox = Instantiate(attackHitboxPrefab, attackSpawnPoint.position, attackSpawnPoint.rotation);
            // Optional: auto-destroy after a short time
            Destroy(hitbox, 0.5f);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} tried to attack but has no prefab or spawn point assigned.");
        }

        attackTimer = attackCooldown;
    }



}
