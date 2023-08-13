using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Selectable : MonoBehaviour
{
    private Material[] materials;
    private List<Color> defaultColors = new List<Color>();
    private List<Color> hoveredColors = new List<Color>();
    private List<Color> selectedColors = new List<Color>();
    // Components
    [SerializeField] private MeshRenderer meshRenderer;

    private bool hovered = false;
    private bool selected = false;

    void Start()
    {
        if(!meshRenderer)
        {
            // If no mesh renderer in component field, try to get mesh renderer from own component
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if(!meshRenderer)
        {
            Debug.LogError("S_Selectable:Start() - MeshRenderer not initialised");
        }
        materials = meshRenderer.materials;
        foreach (var material in materials)
        {
            defaultColors.Add(material.color);
            hoveredColors.Add(Color.Lerp(material.color, Color.white, 0.2f));
            selectedColors.Add(Color.Lerp(material.color, Color.green, 0.2f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelected()
    {
        Debug.Log("[S_Selectable] OnSelected:" + gameObject.name);
        selected = true;

        int matIndex = 0;
        foreach (var material in materials)
        {
            material.color = selectedColors[matIndex];
            matIndex++;
        }
    }

    public void OnDeselected()
    {
        Debug.Log("[S_Selectable] OnDeselected:" + gameObject.name);
        selected = false;

        int matIndex = 0;
        if (hovered)
        {
            foreach (var material in materials)
            {
                material.color = hoveredColors[matIndex];
                matIndex++;
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.color = defaultColors[matIndex];
                matIndex++;
            }
        }
    }

    public void OnHoverStart()
    {
        Debug.Log("[S_Selectable] OnHoverStart:" + gameObject.name);
        hovered = true;

        if(selected)
            return;
            
        int matIndex = 0;
        foreach (var material in materials)
        {
            material.color = hoveredColors[matIndex];
            matIndex++;
        }
    }

    public void OnHoverStop()
    {
        Debug.Log("[S_Selectable] OnHoverStop:" + gameObject.name);
        hovered = false;

        if(selected)
            return;

        int matIndex = 0;
        foreach (var material in materials)
        {
            material.color = defaultColors[matIndex];
            matIndex++;
        }
    }
}
