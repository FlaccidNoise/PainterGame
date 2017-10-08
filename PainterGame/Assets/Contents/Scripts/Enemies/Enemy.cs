using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Enemy : Entity {

    // State of the enemy
    public enum State
    {
        Idle,
        Chasing,
        PreAttack,
        Attacking,
        Thanking,
        RunningOffscreen
    }

    [SerializeField]
    Material mat;

    State currentState;
    UnityEngine.AI.NavMeshAgent pathfinder;
    Entity targetEntity;
    Transform target;
    Transform offscreenTarget;
    Material skinMaterial;

    Color originalColour;

    [SerializeField]
    Color finalColour;

    float attackDistanceThreshold = 0.75f;
    float timeBetweenAttacks = 1.25f;
    float preAttackDelayTime = 0.8f;
    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    float damage = 3.48f;
    float lifetime = 4.0f;

    bool hasTarget;
    [SerializeField]
    bool hasGivenDrops = false;

    [SerializeField]
    GameObject[] dropsOnDeath;

    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColour = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            offscreenTarget = GameObject.FindGameObjectWithTag("OffscreenTarget").transform;
            hasTarget = true;
            targetEntity = target.GetComponent<Entity>();
            targetEntity.OnDeath += OnTargetDeath;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks + preAttackDelayTime;
                    StartCoroutine(PreAttack());
                }
            } 
        }
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= health)
        {
            StartCoroutine(GiveThanks());
        }
        // TODO: Use hitPoint and hitDirection to create a particle effect on the enemy
        TakeDamage(damage);

        float percentHealth = health / startingHealth;
        skinMaterial.color = Color.Lerp(originalColour, finalColour, 1 - percentHealth);
    }
    
    public override void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            
        }
    }

    void CreateDeathDrops()
    {
        hasGivenDrops = true;
        for (int i = 0; i < dropsOnDeath.Length; i++)
        {
            GameObject thanksObject = Instantiate(dropsOnDeath[i], transform.position + new Vector3(i, 0.0f, i), Quaternion.identity) as GameObject;
        }
    }

    IEnumerator PreAttack()
    {
        currentState = State.PreAttack;
        pathfinder.enabled = false;
        float preAttackDelayRemaining = preAttackDelayTime;

        while (preAttackDelayRemaining > 0.0f)
        {
            preAttackDelayRemaining -= Time.deltaTime;
            if (preAttackDelayRemaining <= 0.0f)
            {
                StartCoroutine(Attack());
            }
            yield return null;
        }

    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        originalColour = skinMaterial.color;
        skinMaterial.color = Colours.PG_RED;

        Vector3 targetPosition = target.position;

        Vector3 originalPosition = transform.position;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = targetPosition - directionToTarget * (myCollisionRadius);

        float percent = 0.0f;
        float attackSpeed = 3.0f;

        bool hasAppliedDamage = false;

        while(percent <= 1)
        {
            if (percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            // Animate Lunge
            percent += Time.deltaTime * attackSpeed;
            float interpolationValue = (-Mathf.Pow(percent, 2.0f) + percent) * 4.0f;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolationValue);


            yield return null;
        }

        skinMaterial.color = originalColour;
        pathfinder.enabled = true;
        currentState = State.Chasing;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            } else if (currentState == State.RunningOffscreen)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator GiveThanks()
    {
        currentState = State.Thanking;
        pathfinder.enabled = false;

        if (!hasGivenDrops)
        {
            StartCoroutine(ThankingJump());
            CreateDeathDrops();
        }

        pathfinder.enabled = true;
        target = offscreenTarget;
        currentState = State.RunningOffscreen;
        GameObject.Destroy(this.gameObject, lifetime);
        yield return null;
    }

    IEnumerator ThankingJump()
    {
        Vector3 originalPosition = transform.position;
        Vector3 jumpPosition = originalPosition + new Vector3(0.0f, 2.0f, 0.0f);
        float percent = 0;
        float jumpSpeed = 3;

        bool hasReachedPeak = false;

        while (percent <= 1)
        {
            if (percent >= 0.5f && !hasReachedPeak)
            {
                hasReachedPeak = true;
            }
            percent += Time.deltaTime * jumpSpeed;
            float interpolationValue = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, jumpPosition, interpolationValue);

            // Animate jump XD

            yield return null;
        }
    }
}
