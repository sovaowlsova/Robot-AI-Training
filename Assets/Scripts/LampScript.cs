using UnityEngine;

public class LampScript : MonoBehaviour
{
    [SerializeField] private Material spiralOffMaterial;
    [SerializeField] private Material spiralOnMaterial;
    [SerializeField] private Material glassOffMaterial;
    [SerializeField] private Material glassOnMaterial;

    [SerializeField] private Renderer spiral;
    [SerializeField] private Renderer glass;

    [SerializeField] private Light pointLight;

    private bool state = false;

    public void Toggle(bool newState)
    {
        if (newState && !state)
        {
            state = true;
            pointLight.enabled = true;
            spiral.material = spiralOnMaterial;
            glass.material = glassOnMaterial;
        }
        else if (!newState && state)
        {
            state = false;
            pointLight.enabled = false;
            spiral.material = spiralOffMaterial;
            glass.material = glassOffMaterial;
        }
    }
}
