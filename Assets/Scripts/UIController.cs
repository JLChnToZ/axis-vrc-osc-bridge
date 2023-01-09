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
    [SerializeField] TMP_Text bodyWidthDisplay;
    [SerializeField] Toggle showMannequinButton;
    [SerializeField] Toggle showVRChatTrackers;
    [SerializeField] Toggle hasTrackerToggle;
    [SerializeField] Button manualSyncHeadButton;
    [SerializeField] Slider bodyHeightSlider;
    [SerializeField] Slider bodyWidthSlider;
    [SerializeField] TMP_InputField ipInput;
    [SerializeField] TMP_InputField portInput;
    [SerializeField] VRCTrackerDisplay[] displays;
    [SerializeField, Range(0, 80)] float mannequinShoulderBreath = 42;
    [SerializeField, Range(0, 200)] float mannequinBodyHeight = 180;

    AxisVRChatOscBridge bridge;

    void Awake() {
        bridge = new AxisVRChatOscBridge();
        connectVRCButton.onClick.AddListener(OnConnectClick);
        disconnectVRCButton.onClick.AddListener(OnDisconnectClick);
        showMannequinButton.onValueChanged.AddListener(OnToggleMannequinClick);
        showVRChatTrackers.onValueChanged.AddListener(OnShowVRCTrackerClick);
        bodyHeightSlider.onValueChanged.AddListener(OnBodyHeightChanged);
        bodyWidthSlider.onValueChanged.AddListener(OnBodyWidthChanged);
        hasTrackerToggle.onValueChanged.AddListener(UpdateHasTrackerToggle);
        manualSyncHeadButton.onClick.AddListener(bridge.SyncHeadRotation);
        UpdateWidthText();
        UpdateHeightText();
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
        bridge.ScaleY = height;
        UpdateHeightText();
    }

    void OnBodyWidthChanged(float width) {
        bridge.ScaleX = width;
        UpdateWidthText();
    }

    void UpdateWidthText() {
        bodyWidthDisplay.text = (bridge.ScaleX * mannequinShoulderBreath).ToString("0.00");
    }

    void UpdateHeightText() {
        bodyHeightDisplay.text = (bridge.ScaleY * mannequinBodyHeight).ToString("0.00");
    }

    void OnToggleMannequinClick(bool enabled) {
        if (axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        axisBrain.axisMannequin.SetVisibility(enabled);
    }

    void OnShowVRCTrackerClick(bool enabled) {
        if (displays == null) return;
        foreach (var display in displays) {
            if (display == null) continue;
            var go = display.gameObject;
            if (go.activeSelf != enabled) {
                go.SetActive(enabled);
                if (enabled)
                    bridge.DataUpdated += display.DataUpdated;
                else
                    bridge.DataUpdated -= display.DataUpdated;
            }
        }
    }

    void UpdateHasTrackerToggle(bool value) {
        bridge.HasHead = value;
    }
}
