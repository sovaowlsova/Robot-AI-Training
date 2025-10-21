using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSwitchScript : MonoBehaviour
{
    [SerializeField] private GameObject interfaceUI;
    [SerializeField] private GameObject menu;
    private InputAction menuAction;
    private bool menuEnabled = false;

    private void Awake()
    {
        menuAction = InputSystem.actions.FindAction("Menu");
    }

    private void Update()
    {
        if (menuAction.ReadValue<float>() > 0)
        {
            if (!menuEnabled)
            {
                menuEnabled = true;
                interfaceUI.SetActive(false);
                menu.SetActive(true);
            }
        } else
        {
            if (menuEnabled)
            {
                menuEnabled = false;
                interfaceUI.SetActive(true);
                menu.SetActive(false);
            }
        }
    }
}
