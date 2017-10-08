using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTrap : MonoBehaviour {

    [SerializeField]
    GameObject explosionEffect;

    [SerializeField]
    GameObject player;

    [SerializeField]
    float damage = 15;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

	void OnTriggerEnter(Collider col)
    {
        player.GetComponent<Player>().TakeDamage(damage);
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject;
        Camera.main.gameObject.GetComponent<CameraShake>().ShakeCamera(0.5f, 1);

        GameObject.Destroy(this.gameObject);
    }
}
