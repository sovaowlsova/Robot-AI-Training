using TMPro;
using UnityEngine;

public class StatusUIScript : MonoBehaviour
{
    [SerializeField] private MonoBehaviour script;

    private TMP_Text text;

    private void Awake()
    {
        text = transform.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        text.color = script.enabled ? Color.green : Color.red;
    }
}
