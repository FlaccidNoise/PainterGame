using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

    public static bool CAMERA_LOCKED = true;

    [SerializeField]
    GameObject _cameraTarget;

    [SerializeField]
    GameObject _player;

    private float stretchFactor = 2.0f;
    private float dampTime = 0.075f;
    private Vector3 velocity = Vector3.zero;
    private float zSkew = -1.0f;

    void LateUpdate()
    {
        if (CAMERA_LOCKED)
        {
            this._LockedFollow();
        }
        else
        {
            this._UnlockedFollow();
        }
        if (Input.GetMouseButtonDown(2))
        {
            this._ToggleLock();
        }
    }

    void _ToggleLock()
    {
        CAMERA_LOCKED = !CAMERA_LOCKED;
    }

    void _LockedFollow()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(Camera.main.transform.position, point, Color.blue);
            this._cameraTarget.transform.position = this._player.transform.position;
        }
    }

    void _UnlockedFollow()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            float halfwayX = ((this._player.transform.position.x + point.x) / stretchFactor) * 0.9f;
            float halfwayZ = ((this._player.transform.position.z + point.z) / stretchFactor) + zSkew;
            Vector3 destination = new Vector3(halfwayX, point.y, halfwayZ);
            this._cameraTarget.transform.position = Vector3.SmoothDamp(this._cameraTarget.transform.position, destination, ref velocity, dampTime);
        }
    }
}