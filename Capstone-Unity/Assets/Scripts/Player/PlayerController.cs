using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float mouseMultiplier = .05f;
    public float moveSpeed = 1;
    // public float sprintMultiplier;

    float vAxis = 0;
    float hAxis = 0;

    private Vector3 _inputVector;
    public Rigidbody rb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    // Update is called once per frame
    void Update()
    {
        hAxis += Input.GetAxis("Mouse X") * mouseMultiplier;
        vAxis += Input.GetAxis("Mouse Y") * mouseMultiplier * -1;
        vAxis = Mathf.Clamp(vAxis, -90, 90);
        transform.localRotation = Quaternion.Euler(vAxis, hAxis, 0);

        // bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        _inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {

        Vector3 movementBase = _inputVector.z * new Vector3(transform.forward.x, 0f, transform.forward.z).normalized + _inputVector.x * new Vector3(transform.right.x, 0f, transform.right.z).normalized;

        if (Input.GetKey(KeyCode.Space))
        {
            movementBase += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementBase += new Vector3(0, -1, 0);
        }

        Vector3 movementFinal = movementBase * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movementFinal);
    }
}
