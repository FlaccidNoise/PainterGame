using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody myRigidbody;
    Vector3 velocity;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + (velocity * Time.fixedDeltaTime));
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    public void Knockback(float distance)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2);
        int i = 0;

        while (i < colliders.Length)
        {
            if (colliders[i].gameObject.tag == "Enemy")
            {
                Vector3 myVec = (transform.position -  colliders[i].gameObject.transform.position).normalized;
                myVec *= distance;
                myRigidbody.AddForce(myVec, ForceMode.Impulse);
            }
            i++;
        }
    }
}
