using UnityEngine;
using System.Collections;
using Q9Core;

[AddComponentMenu("Camera-Control/Mouse Orbit with Zoom")]
public class MouseOrbit : MonoBehaviour
{
    [System.Serializable]
    public enum CameraMode
    {
        overview,
        follow
    }

    public CameraMode cameraMode = CameraMode.overview;
    [System.Serializable]
    public class OverviewSettings
    {
        public Vector3 defaultPosition = new Vector3();
        public Vector3 wantedPosition = new Vector3();
        public Vector3 currentPosition = new Vector3();
        public float defaultDistance = 5.0f;
        public float zoomSpeed = 0.1f;
        public float wantedDistance = 5.0f;
        public float currentDistance = 5.0f;
        public float panSpeed = 0.1f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;
        public float zSpeed = 20.0f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        public float distanceMin = .5f;
        public float distanceMax = 15f;
        public Vector2 firstMousePosition;
        public int unlockThreshold = 5;
        public bool spinUnlocked = false;

        public Rigidbody rigidbody;

        public float x = 0.0f;
        public float y = 0.0f;
    }

    public OverviewSettings overviewSettings;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        overviewSettings.x = angles.y;
        overviewSettings.y = angles.x;

        overviewSettings.rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (overviewSettings.rigidbody != null)
        {
            overviewSettings.rigidbody.freezeRotation = true;
        }
        overviewSettings.currentPosition = overviewSettings.defaultPosition;
        overviewSettings.currentDistance = overviewSettings.defaultDistance;
        overviewSettings.wantedDistance = overviewSettings.defaultDistance;
    }


    void LateUpdate()
    {
        if (cameraMode == CameraMode.overview)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                overviewSettings.firstMousePosition = Input.mousePosition;
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (!overviewSettings.spinUnlocked)
                {
                    if (Vector2.Distance(Input.mousePosition, overviewSettings.firstMousePosition) >= overviewSettings.unlockThreshold)
                    {
                        overviewSettings.spinUnlocked = true;
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                overviewSettings.spinUnlocked = false;
            }


            if (overviewSettings.spinUnlocked)
            {
                overviewSettings.x += Input.GetAxis("Mouse X") * overviewSettings.xSpeed * 0.02f;
                overviewSettings.y -= Input.GetAxis("Mouse Y") * overviewSettings.ySpeed * 0.02f;
            }
        }
        else
        {

        }
        overviewSettings.y = ClampAngle(overviewSettings.y, overviewSettings.yMinLimit, overviewSettings.yMaxLimit);

        Quaternion rotation = Quaternion.Euler(overviewSettings.y, overviewSettings.x, 0);

        overviewSettings.wantedDistance = Mathf.Clamp(overviewSettings.wantedDistance - Input.GetAxis("Mouse ScrollWheel") * overviewSettings.zSpeed, overviewSettings.distanceMin, overviewSettings.distanceMax);

        overviewSettings.currentDistance = Mathf.Lerp(overviewSettings.currentDistance, overviewSettings.wantedDistance, overviewSettings.zoomSpeed);


        Vector3 negDistance = new Vector3(0.0f, 0.0f, -overviewSettings.currentDistance);

        overviewSettings.wantedPosition = overviewSettings.currentPosition;

        Vector3 position = rotation * negDistance + overviewSettings.currentPosition;


        transform.rotation = rotation;
        transform.position = position;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}