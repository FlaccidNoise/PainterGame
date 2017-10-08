using UnityEngine;
using System.Collections;


public class RedSpell_0 : Spell, ISpell {

    private float moveSpeed = 25.0f;
    public LayerMask collisionMask;
    float skinWidth = 0.1f;
    float lifetime = 0.6f;
    float damage = 2.0f;
    float cameraShakeAmount = 0.02f;

    [SerializeField]
    private bool _canUpdate;

    [SerializeField]
    GameObject collideEffect;

    void Start()
    {
        Destroy(this.gameObject, lifetime);

        GetComponentInChildren<Renderer>().material.color = Colours.PG_RED;

        _canUpdate = true;

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }
    }

    override public void Cast()
    {
        Debug.Log("CastRed0Spell");
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
        // TODO: IDamageable interface
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }

        GameObject collideEffectObject = Instantiate(collideEffect, hitPoint, transform.rotation) as GameObject;
        Camera.main.GetComponent<CameraShake>().ShakeCamera(0.2f, 0.01f + cameraShakeAmount);

        GameObject.Destroy(this.gameObject);
    }

    public void setMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}
