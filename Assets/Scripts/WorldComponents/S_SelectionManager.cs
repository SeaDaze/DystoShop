using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SelectionManager : MonoBehaviour
{
    S_Selectable hoveredObject;
    S_Selectable selectedObject;
    Camera currentCamera;
	void Start () {
        currentCamera = Camera.main;
	}

	void Update () {
		Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
		{
            S_Selectable selectable = hit.collider.gameObject.GetComponent<S_Selectable>();
			if (selectable && selectable != hoveredObject)
			{
                if(hoveredObject)
                {
                    hoveredObject.OnHoverStop();
                }
                hoveredObject = selectable;
                hoveredObject.OnHoverStart();
			}
			else if (!selectable && hoveredObject)
			{
                hoveredObject.OnHoverStop();
                hoveredObject = null;
			}

            if (Input.GetMouseButtonDown(0))
            {
                if (hoveredObject && selectedObject != hoveredObject)
                {
                    selectedObject = hoveredObject;
                    selectedObject.OnSelected();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if(selectedObject)
                {
                    selectedObject.OnDeselected();
                    selectedObject = null;
                }
            }
		}
	}

}
