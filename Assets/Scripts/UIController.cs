using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Axis.Elements;
using Axis.Events;

public class UIController : MonoBehaviour {
    [SerializeField] AxisBrain axisBrain;
    [SerializeField] Button connectVRCButton;
    [SerializeField] Button disconnectVRCButton;
    [SerializeField] TMP_Text bodyHeightDisplay;
    [SerializeField] TMP_Text bodyWidthDisplay;
    [SerializeField] Toggle showMannequinButton;
    [SerializeField] Toggle showVRChatTrackers;
    [SerializeField] Toggle[] hasTrackerToggles;
    [SerializeField] Button manualSyncHeadButton;
    [SerializeField] Button resetTrackers;
    [SerializeField] Button zeroTrackers;
    [SerializeField] TMP_Text manualSyncHeadText;
    [SerializeField] TMP_Text resetTrackersText;
    [SerializeField] TMP_Text zeroTrackersText;
    [SerializeField] Slider bodyHeightSlider;
    [SerializeField] Slider bodyWidthSlider;
    [SerializeField] TMP_InputField ipInput;
    [SerializeField] TMP_InputField portInput;
    [SerializeField] VRCTrackerDisplay[] displays;
    [SerializeField, Range(0, 80)] float mannequinShoulderBreath = 42;
    [SerializeField, Range(0, 200)] float mannequinBodyHeight = 180;
    [SerializeField] AudioSource beep;
    bool isZeroOrientation, isResetTrackers, isZeroTrackers;

    AxisVRChatOscBridge bridge;

    void Awake() {
        bridge = new AxisVRChatOscBridge();
        connectVRCButton.onClick.AddListener(OnConnectClick);
        disconnectVRCButton.onClick.AddListener(OnDisconnectClick);
        showMannequinButton.onValueChanged.AddListener(OnToggleMannequinClick);
        showVRChatTrackers.onValueChanged.AddListener(OnShowVRCTrackerClick);
        bodyHeightSlider.onValueChanged.AddListener(OnBodyHeightChanged);
        foreach (var hasTrackerToggle in hasTrackerToggles)
            hasTrackerToggle.onValueChanged.AddListener(UpdateHasTrackerToggle);
        manualSyncHeadButton.onClick.AddListener(ZeroOrientation);
        resetTrackers.onClick.AddListener(ResetAxisTrackers);
        zeroTrackers.onClick.AddListener(ZeroAxisTrackers);
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
        bridge.Scale = height;
        UpdateHeightText();
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
                if (enabled) {
                    bridge.PositionUpdated += display.DataUpdated;
                    bridge.RotationUpdated += display.DataUpdated;
                } else {
                    bridge.PositionUpdated -= display.DataUpdated;
                    bridge.RotationUpdated -= display.DataUpdated;
                }
            }
        }
    }

    void UpdateHasTrackerToggle(bool value) {
        manualSyncHeadButton.interactable = !hasTrackerToggles[0].isOn;
        for (int i = 0; i < hasTrackerToggles.Length; i++)
            bridge.SetChannelEnabled(i, hasTrackerToggles[i].isOn);
    }

    void ZeroOrientation() => ZeroOrientationAsync(manualSyncHeadText).Forget();

    void ResetAxisTrackers() => ResetAxisTrackersAsync(resetTrackersText).Forget();

    void ZeroAxisTrackers() => ZeroAxisTrackersAsync(zeroTrackersText).Forget();

    async UniTask ZeroOrientationAsync(TMP_Text display = null) {
        if (isZeroOrientation) return;
        isZeroOrientation = true;
        try {
            await CountDown(3, display);
            bridge.SyncHeadRotation();
        } finally {
            isZeroOrientation = false;
        }
    }

    async UniTask ResetAxisTrackersAsync(TMP_Text display = null) {
        if (isResetTrackers) return;
        isResetTrackers = true;
        try {
            await CountDown(3, display);
            AxisEvents.OnReboot?.Invoke();
        } finally {
            isResetTrackers = false;
        }
    }

    async UniTask ZeroAxisTrackersAsync(TMP_Text display = null) {
        if (isZeroTrackers) return;
        isZeroTrackers = true;
        try {
            await CountDown(3, display);
            if (AxisEvents.OnZero == null) return;
            for (int i = 0; i <= (int)Axis.Enumerations.NodeBinding.FreeNode; i++)
                AxisEvents.OnZero(i);

        } finally {
            isZeroTrackers = false;
        }
    }

    async UniTask CountDown(float seconds, TMP_Text display = null) {
        string originalText = display != null ? display.text : "";
        while (seconds >= 1) {
            if (display != null) display.text = seconds.ToString("0");
            beep.Play();
            await UniTask.Delay(1000);
            seconds--;
        }
        if (display != null) display.text = "0";
        await UniTask.Delay(Mathf.FloorToInt(seconds * 1000));
        if (display != null) display.text = originalText;
    }
}
