
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;

    private bool controlsEnabled = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!controlsEnabled)
            return;

        Move();
        LookAround();
    }

    public void SetControlsEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 movement =
            transform.right * x +
            transform.forward * z;

        controller.Move(
            movement.normalized * moveSpeed * Time.deltaTime
        );

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        playerCamera.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
