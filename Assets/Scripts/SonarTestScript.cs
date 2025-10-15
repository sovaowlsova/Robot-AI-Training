using UnityEngine;

public class SonarTestScript : MonoBehaviour
{
    [SerializeField] private SonarScript sonar;

    [SerializeField] private Renderer lampRenderer;
    [SerializeField] private Light pointLight;

    [SerializeField] private Material lampOffMaterial;
    [SerializeField] private Material lampOnMaterial;

    private bool isOn = false;

    private void Update()
    {
        if (sonar.IsTriggered())
        {
            if (!isOn)
            {
                isOn = true;
                lampRenderer.material = lampOnMaterial;
                pointLight.enabled = true;
            }
        } else {
            if (isOn)
            {
                isOn = false;
                lampRenderer.material = lampOffMaterial;
                pointLight.enabled = false;
            }
        }
    }
}
