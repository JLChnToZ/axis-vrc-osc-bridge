using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Axis.Elements;

public class UIController : MonoBehaviour {
    [SerializeField] AxisBrain axisBrain;
    [SerializeField] Button connectVRCButton;
    [SerializeField] Button disconnectVRCButton;
    [SerializeField] TMP_Text bodyHeightDisplay;
    [SerializeField] Button showMannequinButton;
    [SerializeField] Button hideMannequinButton;
    [SerializeField] Slider bodyHeightSlider;
    [SerializeField] TMP_InputField ipInput;
    [SerializeField] TMP_InputField portInput;
    AxisVRChatOscBridge bridge;

    void Awake() {
        connectVRCButton.onClick.AddListener(OnConnectClick);
        disconnectVRCButton.onClick.AddListener(OnDisconnectClick);
        showMannequinButton.onClick.AddListener(OnShowMannequinClick);
        hideMannequinButton.onClick.AddListener(OnHideMannequinClick);
        bodyHeightSlider.onValueChanged.AddListener(OnBodyHeightChanged);
        bridge = new AxisVRChatOscBridge();
    }

    void OnDestroy() => bridge.Disconnect();

    void OnConnectClick() {
        try {
            bridge.Connect(ipInput.text, int.Parse(portInput.text));
            connectVRCButton.interactable = false;
            ipInput.interactable = false;
            portInput.interactable = false;
            disconnectVRCButton.interactable = true;
        } catch (Exception ex) {
            Debug.LogException(ex);
        }
    }

    void OnDisconnectClick() {
        bridge.Disconnect();
        connectVRCButton.interactable = true;
        ipInput.interactable = true;
        portInput.interactable = true;
        disconnectVRCButton.interactable = false;
    }

    void OnBodyHeightChanged(float height) {
        bridge.Scale = height;
        bodyHeightDisplay.text = height.ToString("0.00");
    }
    void OnHideMannequinClick() {
        if (axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        axisBrain.axisMannequin.SetVisibility(false);
        hideMannequinButton.interactable = false;
        showMannequinButton.interactable = true;
    }

    void OnShowMannequinClick() {
        if (axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        axisBrain.axisMannequin.SetVisibility(true);
        hideMannequinButton.interactable = true;
        showMannequinButton.interactable = false;
    }
}

