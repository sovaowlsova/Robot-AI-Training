using TMPro;
using UnityEngine;

public class SonarUIScript : MonoBehaviour
{
    [SerializeField] private SonarScript sonar;

    private TMP_Text text;

    private void Start()
    {
        text = transform.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        text.text = sonar.GetDistance().ToString("0.00 m");
    }
}
