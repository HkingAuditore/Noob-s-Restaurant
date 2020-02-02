using UnityEngine;

public class MouseOrbitDemo : MonoBehaviour
{
    //Copied from the standard assets and renamed to MouseOrbitDemo to avoid any conflicts. Feel free to delete this file.

    public Transform target = null;
    public float distance = 8.0f;

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;

    private float x = 0.0f;
    private float y = 0.0f;

    private void Start ()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody> ())
            GetComponent<Rigidbody> ().freezeRotation = true;
    }

    private void LateUpdate ()
    {
        if (target)
        {
            x += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle (y, yMinLimit, yMaxLimit);

            var rotation = Quaternion.Euler (y, x, 0);
            var position = rotation * new Vector3 (0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private static float ClampAngle (float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp (angle, min, max);
    }
}