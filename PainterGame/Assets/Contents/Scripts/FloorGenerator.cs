using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour {

    public int width;
    public int length;

    [SerializeField]
    GameObject plane;

    // Use this for initialization
    void Awake() {
        /*for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                GameObject Plane = Instantiate(plane, new Vector3((10.0f * i) - 45.0f, 0, 10.0f * j - 45.0f), Quaternion.identity) as GameObject;
                Plane.transform.SetParent(GetComponentInParent<Transform>());
            }
        }*/
	}

}
