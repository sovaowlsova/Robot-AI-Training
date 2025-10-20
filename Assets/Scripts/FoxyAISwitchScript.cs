using UnityEngine;
using UnityEngine.InputSystem;

public class FoxyAISwitchScript : MonoBehaviour
{
    private InputAction AISwitchAction;
    private FoxyController foxyController;
    private FoxyControllerAI foxyControllerAI;

    private void Awake()
    {
        AISwitchAction = InputSystem.actions.FindAction("AISwitch");
        foxyController = transform.GetComponent<FoxyController>();
        foxyControllerAI = transform.GetComponent<FoxyControllerAI>();
    }

    private void Update()
    {
        bool AISwitchPressed = AISwitchAction.triggered && AISwitchAction.ReadValue<float>() > 0;
        if (AISwitchPressed)
        {
            foxyController.enabled = !foxyController.enabled;
            foxyControllerAI.enabled = !foxyControllerAI.enabled;   
        }
    }
}
