using TMPro;
using UnityEngine;

public class RecordStatusUIScript : MonoBehaviour
{
    [SerializeField] private RecordScript recordScript;

    private TMP_Text text;

    private void Awake()
    {
        text = transform.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        text.color = recordScript.isRecording() ? Color.green : Color.red;
    }
}
