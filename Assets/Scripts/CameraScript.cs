using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float defaultZoom;
    [SerializeField] private Transform cameraObject;
    [SerializeField] private Vector3 pivotOffset;

    private InputAction lookAction;
    private InputAction zoomAction;
    private float zoom;

    private void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Look");
        zoomAction = InputSystem.actions.FindAction("Zoom");
    }

    private void Start()
    {
        zoom = defaultZoom;
    }

    private void Update()
    {

        zoom = Mathf.Clamp(zoom - zoomAction.ReadValue<Vector2>().y / zoomSpeed, minZoom, maxZoom);
        Vector3 pivotWithOffset = cameraObject.position + pivotOffset;

        transform.LookAt(pivotWithOffset);

        Vector2 lookRes = lookAction.ReadValue<Vector2>() * moveSpeed;
        Vector3 newAngle = transform.eulerAngles + new Vector3(-lookRes.y, lookRes.x, 0);
        if (newAngle.x > 180 && newAngle.x < 271)
        {
            newAngle.x = 271;
        }
        else if (newAngle.x > 89 && newAngle.x < 180)
        {
            newAngle.x = 89;
        }
        transform.eulerAngles = newAngle;

        transform.position = pivotWithOffset - transform.forward * zoom;
    }
}
