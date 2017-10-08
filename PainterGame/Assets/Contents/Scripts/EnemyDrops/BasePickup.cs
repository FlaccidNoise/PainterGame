using UnityEngine;
using System.Collections;

public enum PickupType
{
    PaintAndColour= 0,
    Thanks
}

public class BasePickup : MonoBehaviour {
    
    [SerializeField]
    protected float colourValue = 5.0f;
    [SerializeField]
    protected int thanksValue = 5;

    [SerializeField]
    PickupType myPickupType;

    [SerializeField]
    private GameObject ground;

    private float rotateSpeed = 60.0f;
    private float lifetime = 20.0f;

    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("Floor");
        GameObject.Destroy(this.gameObject, lifetime);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // Store a reference to the player instance
            Player player = col.gameObject.GetComponent<Player>();

            // Add the appropriate paint or colour or thanks
            if (myPickupType == PickupType.PaintAndColour)
            {
                // Add paint/colour to player
                player.AddColour(colourValue);
                
            } else if (myPickupType == PickupType.Thanks)
            {
                player.AddThanks(thanksValue);
            }

            GameObject.Destroy(this.gameObject);
        }
    }

    void Update()
    {
        // Rotate around the World's Y axis
        transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed, Space.World);

        transform.position = new Vector3(transform.position.x, (ground.transform.position.y + 1.0f) + Mathf.Sin(Time.time * 3.0f) * 0.5f, transform.position.z);
    }

	
}
