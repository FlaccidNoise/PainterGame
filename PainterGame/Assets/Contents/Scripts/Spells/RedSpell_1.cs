using UnityEngine;
using System.Collections;

public class RedSpell_1 : Spell, ISpell {

    private float moveSpeed = 20.0f;
    public LayerMask collisionMask;
    float skinWidth = 0.1f;
    float lifetime = 3.0f;
    float damage = 8.0f;
    float aoeDamage = 4.0f;
    float aoeRadius = 4.0f;
    float cameraShakeAmount = 0.08f;

    [SerializeField]
    private bool _canUpdate;

    [SerializeField]
    GameObject collideEffect;

    void Start()
    {
        Destroy(this.gameObject, lifetime);

        GetComponentInChildren<Renderer>().material.color = Colours.PG_RED;

        // Make spell point towards target
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            transform.position = new Vector3(point.x + 10.0f, Camera.main.transform.position.y, point.z - 10.0f);
            transform.LookAt(point);
        }

        _canUpdate = true;

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }
    }

    override public void Cast()
    {
        Debug.Log("CastRed1Spell");
    }

    void Update()
    {
        if (_canUpdate)
        {
            float travelDistance = moveSpeed * Time.deltaTime;
            CheckCollisions(travelDistance);
            transform.Translate(Vector3.forward * travelDistance);
        }
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



    public void OnHitObject(Collider c, Vector3 hitPoint)
    {
        // Find all objects inside the aoeRadius
        Collider[] aoeColliders = Physics.OverlapSphere(hitPoint, aoeRadius);
        int i = 0;
        while (i < aoeColliders.Length)
        {
            if (aoeColliders[i].gameObject.tag == "Enemy")
            {
                aoeColliders[i].GetComponent<Enemy>().TakeHit(aoeDamage, aoeColliders[i].transform.position, transform.forward);
            }
            i++;
        }

        // TODO: IDamageable interface
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        GameObject collideEffectObject = Instantiate(collideEffect, hitPoint, collideEffect.transform.rotation) as GameObject;
        Camera.main.GetComponent<CameraShake>().ShakeCamera(0.6f, 0.01f + cameraShakeAmount);

        GameObject.Destroy(this.gameObject);
    }

    public void setMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}
