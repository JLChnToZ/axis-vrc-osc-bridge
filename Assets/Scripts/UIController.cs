using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Axis.Elements;
using Axis.Events;
using Axis.Enumerations;

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
    [SerializeField] Button calibrateTrackers;
    [SerializeField] Button zeroTrackers;
    [SerializeField] Toggle hipsModeToggle;
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
    string defaultIp;
    int defaultPort;

    AxisVRChatOscBridge bridge;

    void Awake() {
        bridge = new AxisVRChatOscBridge();
        defaultIp = ipInput.text;
        int.TryParse(portInput.text, out defaultPort);
        connectVRCButton.onClick.AddListener(OnConnectClick);
        disconnectVRCButton.onClick.AddListener(OnDisconnectClick);
        showMannequinButton.onValueChanged.AddListener(OnToggleMannequinClick);
        showVRChatTrackers.onValueChanged.AddListener(OnShowVRCTrackerClick);
        bodyHeightSlider.onValueChanged.AddListener(OnBodyHeightChanged);
        hasTrackerToggles[0].onValueChanged.AddListener(OnFirstTrackerToggleClick);
        for (int i = 0; i < hasTrackerToggles.Length; i++) {
            var t = hasTrackerToggles[i];
            int index = i;
            t.onValueChanged.AddListener(isOn => bridge.SetChannelEnabled(index, isOn));
        }
        manualSyncHeadButton.onClick.AddListener(ZeroOrientation);
        calibrateTrackers.onClick.AddListener(CalibrateAxisTrackers);
        zeroTrackers.onClick.AddListener(ZeroAxisTrackers);
        hipsModeToggle.onValueChanged.AddListener(OnToggleHipsModeClick);
        UpdateWidthText();
        UpdateHeightText();
        RestoreValues().Forget();
    }

    async UniTaskVoid RestoreValues() {
        await UniTask.Yield();
        ipInput.PersistentMemorize("ip");
        portInput.PersistentMemorize("port");
        bodyHeightSlider.PersistentMemorize("bodyHeight");
        hasTrackerToggles.PersistentMemorize("trackers");
        hipsModeToggle.PersistentMemorize("hipsMode");
    }

    void OnDestroy() => bridge.Disconnect();

    void OnConnectClick() {
        try {
            if (string.IsNullOrWhiteSpace(ipInput.text)) ipInput.text = defaultIp;
            if (string.IsNullOrWhiteSpace(portInput.text)) portInput.text = defaultPort.ToString();
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

    void OnFirstTrackerToggleClick(bool enabled) {
        if (manualSyncHeadButton == null) return;
        manualSyncHeadButton.interactable = !enabled;
    }

    void ZeroOrientation() => ZeroOrientationAsync(manualSyncHeadText).Forget();

    void CalibrateAxisTrackers() => CalibrateAxisTrackersAsync(resetTrackersText).Forget();

    void ZeroAxisTrackers() => ZeroAxisTrackersAsync(zeroTrackersText).Forget();

    void OnToggleHipsModeClick(bool enabled) {
        if (axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        axisBrain.hipProvider = enabled ? HipProvider.Node : HipProvider.Hub;
    }

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

    async UniTask CalibrateAxisTrackersAsync(TMP_Text display = null) {
        if (isResetTrackers) return;
        isResetTrackers = true;
        try {
            await CountDown(3, display);
            AxisEvents.OnCalibration?.Invoke();
        } finally {
            isResetTrackers = false;
        }
    }

    async UniTask ZeroAxisTrackersAsync(TMP_Text display = null) {
        if (isZeroTrackers) return;
        isZeroTrackers = true;
        try {
            await CountDown(3, display);
            AxisEvents.OnZeroAll?.Invoke();
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
