using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class PersistentUIUtils {
    public static void PersistentMemorize(this Slider slider, string key) {
        if (slider == null) throw new ArgumentNullException(nameof(slider));
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(key, slider.value));
        slider.onValueChanged.AddListener(v => {
            PlayerPrefs.SetFloat(key, v);
            PlayerPrefs.Save();
        });
    }

    public static void PersistentMemorize(this Toggle toggle, string key) {
        if (toggle == null) throw new ArgumentNullException(nameof(toggle));
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        toggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(key, toggle.isOn ? 1 : 0) == 1);
        toggle.onValueChanged.AddListener(v => {
            PlayerPrefs.SetInt(key, v ? 1 : 0);
            PlayerPrefs.Save();
        });
    }

    public static void PersistentMemorize(this Toggle[] toggles, string key) {
        if (toggles == null) throw new ArgumentNullException(nameof(toggles));
        if (toggles.Length > 32) throw new ArgumentException("Too many toggles");
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        int value = 0;
        if (PlayerPrefs.HasKey(key)) {
            value = PlayerPrefs.GetInt(key);
            for (int i = 0; i < toggles.Length; i++)
                if (toggles[i] != null)
                    toggles[i].SetIsOnWithoutNotify((value & (1 << i)) != 0);
        } else {
            for (int i = 0; i < toggles.Length; i++)
                if (toggles[i] != null && toggles[i].isOn)
                    value |= 1 << i;
            PlayerPrefs.SetInt(key, value);
        }
        for (int i = 0; i < toggles.Length; i++) {
            if (toggles[i] == null) continue;
            int mask = 1 << i;
            toggles[i].onValueChanged.AddListener(isOn => {
                if (isOn) value |= mask;
                else value &= ~mask;
                PlayerPrefs.SetInt(key, value);
                PlayerPrefs.Save();
            });
        }
    }

    public static void PersistentMemorize(this InputField inputField, string key) {
        if (inputField == null) throw new ArgumentNullException(nameof(inputField));
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        inputField.text = PlayerPrefs.GetString(key, inputField.text);
        inputField.onEndEdit.AddListener(v => {
            PlayerPrefs.SetString(key, v);
            PlayerPrefs.Save();
        });
    }

    public static void PersistentMemorize(this TMP_InputField inputField, string key) {
        if (inputField == null) throw new ArgumentNullException(nameof(inputField));
        if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
        inputField.text = PlayerPrefs.GetString(key, inputField.text);
        inputField.onEndEdit.AddListener(v => {
            PlayerPrefs.SetString(key, v);
            PlayerPrefs.Save();
        });
    }
}
