using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPS_CamController : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float zoomLerpSpeed = 10f;
    public float minDist = 3f;
    public float MaxDist = 15f;

    private PlayerControls controls;
    private CinemachineOrbitalFollow orbital;
    private CinemachineCamera cam;
    Vector2 scrollDelta;
    private float targetZoom;
    private float currentZoom;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new PlayerControls();
        controls.Enable();
        controls.CameraControls.MouseZoom.performed += HandleMouseScroll;
        

        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();
        targetZoom = currentZoom = orbital.Radius;

    }

    private void HandleMouseScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();

    }

    // Update is called once per frame
    void Update()
    {
        if (scrollDelta.y != 0)
        {
            if (orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDist, MaxDist);
                scrollDelta = Vector2.zero;
            }

            float bumperDelta = controls.CameraControls.GamePadZoom.ReadValue<float>();
            if (bumperDelta != 0)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - bumperDelta * zoomSpeed, minDist, MaxDist);
            }

            currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLerpSpeed);
            orbital.Radius = currentZoom;
        }
    }
}
