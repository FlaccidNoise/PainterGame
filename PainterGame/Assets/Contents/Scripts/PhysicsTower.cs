using UnityEngine;
using System.Collections;

public class PhysicsTower : MonoBehaviour {

    public int numRows = 3;
    public int numColumns = 3;
    public int numFloors = 1;
    public float scale = 0.33f;

    [SerializeField]
    GameObject physicsTowerCube;

    // Use this for initialization
    void Start()
    {
        for (int i = numFloors; i > 0; i--)
        {
            for (int j = numColumns; j > 0; j--)
            {
                for (int k = numRows; k > 0; k--)
                {
                    GameObject physicsTowerBit = Instantiate(physicsTowerCube, new Vector3(0, i * scale, 0), Quaternion.identity) as GameObject;
                    physicsTowerBit.transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
    }
}
