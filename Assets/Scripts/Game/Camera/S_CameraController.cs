using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CameraController : MonoBehaviour
{
   private enum CameraState
    {
        MAIN,
        OTHER,
    }
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 25.0f;

    [SerializeField] private float minimumCameraSize = 3.0f;
    [SerializeField] private float maximumCameraSize = 20.0f;

    private Camera mainCamera;
    private Camera currentCamera;
    public Camera CurrentCamera
    {
        get => currentCamera;
    }
    CameraState cameraState;
    Transform cameraPivot;

	private Vector3 dragOrigin;
	private Vector3 dragDifference;
	private bool drag = false;

	private Vector2 rotateDragOrigin;
	private Vector2 rotateDragDifference;
	private float rotateDragDistance;
	private bool rotateDrag = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        mainCamera.orthographicSize = 15;
        cameraState = CameraState.MAIN;
        currentCamera = mainCamera;
        cameraPivot = GameObject.FindGameObjectWithTag("T_CameraPivot").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraState == CameraState.MAIN)
        {
            CalculatePivotPoint();
            if (Input.GetAxis("Mouse ScrollWheel") != 0f ) // forward
            {
                mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - (Input.GetAxis("Mouse ScrollWheel") * zoomSpeed), minimumCameraSize, maximumCameraSize);
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime * 2.0f);
            }

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime * 2.0f);
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.RotateAround(cameraPivot.position, transform.up, -rotateSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.RotateAround(cameraPivot.position, transform.up, rotateSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.R))
            {
                mainCamera.transform.Rotate(Vector3.right, -rotateSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.F))
            {
                mainCamera.transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
            }
        }
    }

	private void LateUpdate()
	{
		Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("L_CameraPlane")))
		{
			dragDifference = hit.point - transform.position;
			if(!drag)
			{
				drag = true;
				dragOrigin = hit.point;
			}
		}

		if(Input.GetMouseButton(2))
		{
			Debug.Log("Mouse 2 down");
			Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("L_CameraPlane")))
			{
				dragDifference = hit.point - transform.position;
				if(!drag)
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

		if(Input.GetMouseButton(1))
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
			transform.RotateAround(cameraPivot.position, transform.up, rotateDragDifference.x * 0.1f * Time.deltaTime);
			mainCamera.transform.Rotate(Vector3.right, rotateDragDifference.y * 0.1f * Time.deltaTime);
			float rotationClamped = Mathf.Clamp(mainCamera.transform.rotation.eulerAngles.x, 10, 80);
			mainCamera.transform.rotation = Quaternion.Euler(rotationClamped, mainCamera.transform.rotation.eulerAngles.y, mainCamera.transform.rotation.eulerAngles.z);
		}
	}
    private void CalculatePivotPoint()
    {
        Ray ray = new Ray();
        ray.direction = currentCamera.transform.forward;
        ray.origin = currentCamera.transform.position;
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("L_CameraPlane")))
		{
			//float distance = Vector3.Distance(currentCamera.ScreenToWorldPoint(Input.mousePosition), hit.point);
            cameraPivot.position = hit.point;
		}
    }
    public void SwitchToCamera(Camera newCamera)
    {
        if (currentCamera == newCamera)
        {
            Debug.LogError("SwitchToCamera: Trying to switch to current camera");
            return;
        }
        mainCamera.enabled = false;
        newCamera.enabled = true;
        cameraState = CameraState.OTHER;
        currentCamera = newCamera;
    }

    public void SwitchToMainCamera()
    {
        if (cameraState == CameraState.MAIN)
        {
            Debug.LogError("SwitchToMainCamera: Main camera is already enabled");
            return;
        }
        currentCamera.enabled = false;
        mainCamera.enabled = true;
        cameraState = CameraState.MAIN;
        currentCamera = mainCamera;
    }
}
