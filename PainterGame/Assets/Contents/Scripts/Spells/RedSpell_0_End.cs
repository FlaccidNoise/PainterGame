using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpell_0_End : MonoBehaviour {
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        int i = 0;

        while (i < numCollisionEvents)
        {
            Splatter.instance.Splat(collisionEvents[i].intersection, new Vector3());
            i++;
        }
    }

}
