using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Renderer))]
public class PhysicsTowerCube : MonoBehaviour {

	void Start () {
        Color newColor = new Color(Random.value, Random.value, Random.value, 1.0f);
        GetComponent<Renderer>().material.color = newColor;
    }
}
