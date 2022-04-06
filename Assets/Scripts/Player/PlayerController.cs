using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

    private Vector2 mouseDelta;

    //Komponentit
    private Rigidbody rb;

    private void Awake()
    {
        //Yhdistä rigidbody muuntajaan rb
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraLook();
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rb.velocity.y;

        //
        rb.velocity = dir;
    }

    void CameraLook()
    {
        //Katso ylös ja alas(ylös ja alas hiiren sirtämällä)
        camCurXRot += mouseDelta.y * lookSensitivity;

        //Kiinnitä arvot min look ja max look
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);

        //Yhdistää sen kamera containeriin
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        //Jos A,W,S,D näppäimet on painettu
        if (context.phase == InputActionPhase.Performed)
        {
            //Liiku Vector2-arvon suuntaan
            curMovementInput = context.ReadValue<Vector2>();
        }
        //Ei enää paina mitään näppäintä
        else if (context.phase == InputActionPhase.Canceled)
        {
            //Pysähdy
            curMovementInput = Vector2.zero;
        }
    }
}
