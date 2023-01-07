using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class CopyrightNotice : MonoBehaviour {
    void Awake() {
        var text = GetComponent<TMP_Text>();
        text.text = string.Format(text.text, Application.productName, Application.version, Application.companyName);
    }
}
