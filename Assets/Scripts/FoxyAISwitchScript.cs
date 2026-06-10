using UnityEngine;
using UnityEngine.InputSystem;

public class FoxyAISwitchScript : MonoBehaviour
{
    private AIController AIScript;
    [SerializeField] private ManualController manualControllerScript;

    private InputAction AISwitchAction;

    private void Awake()
    {
        AISwitchAction = InputSystem.actions.FindAction("AISwitch");
        AIScript = transform.GetComponent<AIController>();
    }

    private void Update()
    {
        bool AISwitchPressed = AISwitchAction.WasPressedThisFrame();
        if (AISwitchPressed)
        {
            manualControllerScript.enabled = !manualControllerScript.enabled;
            AIScript.enabled = !AIScript.enabled;   
        }
    }
}
