using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    float speed;
    public LayerMask collisionMask;
    float damage = 1;
    float lifetime = 3;
    float skinWidth = 0.1f;
    
    void Start()
    {
        Destroy(this.gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }
    }
    
    void Update()
    {
        float travelDistance = speed * Time.deltaTime;
        CheckCollisions(travelDistance);
        transform.Translate(Vector3.forward * travelDistance);

    }

    void CheckCollisions(float travelDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, travelDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(this.gameObject);
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
