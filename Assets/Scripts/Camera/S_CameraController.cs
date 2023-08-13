using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 25.0f;

    [SerializeField] private float minimumCameraSize = 3.0f;
    [SerializeField] private float maximumCameraSize = 20.0f;

    private Vector3 dragOrigin;
	private Vector3 dragDifference;
	private bool drag = false;

	private Vector2 rotateDragOrigin;
	private Vector2 rotateDragDifference;
	private float rotateDragDistance;
	private bool rotateDrag = false;

    Camera cameraComponent;

    Vector3 cameraPivot;

    // Start is called before the first frame update
    void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateCameraPivot();
        HandleInputZoom();
        HandleInputTranslation();
        HandleInputRotation();
    }

    void LateUpdate()
    {
        HandleDragTranslation();
        HandleDragRotation();
    }
    void CalculateCameraPivot()
    {
        Ray ray = new()
        {
            origin = transform.position,
            direction = (transform.forward - transform.up).normalized
        };

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("FloorPlane")))
        {
            cameraPivot = hit.point;
        }
    }

    void HandleInputRotation()
    {
        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(cameraPivot, Vector3.up, -rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(cameraPivot, Vector3.up, rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.R))
        {
            transform.Rotate(Vector3.right, -rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.F))
        {
            transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
        }
    }

    void HandleInputTranslation()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + (movementSpeed * Time.deltaTime * -transform.right);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + (movementSpeed * Time.deltaTime * transform.right);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 forwardTransform = transform.forward;
            forwardTransform.y = 0;
            transform.position = transform.position + (2.0f * movementSpeed * Time.deltaTime * forwardTransform);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 forwardTransform = transform.forward;
            forwardTransform.y = 0;
            transform.position = transform.position + (2.0f * movementSpeed * Time.deltaTime * -forwardTransform);
        }
    }

    void HandleInputZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f )
        {
            cameraComponent.orthographicSize = Mathf.Clamp(cameraComponent.orthographicSize - (Input.GetAxis("Mouse ScrollWheel") * zoomSpeed), minimumCameraSize, maximumCameraSize);
        }
    }

    void HandleDragTranslation()
    {
        if(Input.GetMouseButton(1))
		{
            Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("FloorPlane")))
            {
                dragDifference = hit.point - transform.position;
                if (!drag)
                {
                    drag = true;
                    dragOrigin = hit.point;
                }
            }
        }
        else
        {
            drag = false;
        }

        if(drag)
        {
            transform.position = dragOrigin - dragDifference;
        }
    }

    void HandleDragRotation()
    {
        if(Input.GetMouseButton(2))
        {
            if(!rotateDrag)
            {
                rotateDrag = true;
                rotateDragOrigin = Input.mousePosition;
            }
            rotateDragDifference = new Vector2(Input.mousePosition.x - rotateDragOrigin.x, Input.mousePosition.y - rotateDragOrigin.y);
        }
        else
        {
            rotateDrag = false;
        }

        if(rotateDrag)
        {
            transform.RotateAround(cameraPivot, Vector3.up, rotateDragDifference.x * 0.1f * Time.deltaTime);
            transform.Rotate(Vector3.right, rotateDragDifference.y * 0.1f * Time.deltaTime);
            float rotationClamped = Mathf.Clamp(transform.rotation.eulerAngles.x, 10, 80);
            transform.rotation = Quaternion.Euler(rotationClamped, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
