using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;
    
    // TODO: Events throughout the game! <3
    public event System.Action OnDeath;


    protected virtual void Start()
    {
        health = startingHealth;
    }

    // From interface IDamageable
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // TODO: Some stuff here with the hit variable (Particle Effects)
        TakeDamage(damage);
    }

    // From interface IDamageable
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        Destroy(this.gameObject);
    }
}
