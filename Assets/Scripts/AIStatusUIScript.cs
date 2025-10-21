using TMPro;
using UnityEngine;

public class AIStatusUIScript : MonoBehaviour
{
    [SerializeField] private GameObject foxy;

    private FoxyControllerAI AIController;
    private TMP_Text text;

    private void Awake()
    {
        AIController = foxy.GetComponent<FoxyControllerAI>();
        text = transform.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        text.color = AIController.enabled ? Color.green : Color.red;
    }
}
