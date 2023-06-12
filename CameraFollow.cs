using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    private new Transform camera;
    public float smoothing = 2f;
    public float shakeDuration = 0f;
    public float shakeAmount = 0.1f;

    Vector3 camera_position;

    void Awake()
    {
        Cursor.visible = false;
        if (camera == null)
        {
            camera = GetComponent<Transform>();
        }
    }

    void OnEnable()
    {
        camera_position = camera.localPosition;
    }

    private void Update()
    {
        Vector3 new_position = player.position;
        new_position.z = -10;
        transform.position = Vector3.Slerp(transform.position, new_position, smoothing * Time.deltaTime);

        if (shakeDuration > 0)
        {
            camera.localPosition = camera_position + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime;
        }
    }

    public void ShakeCamera()
    {
        camera_position = camera.localPosition;
        shakeDuration = 0.2f;
    }
}
