using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UniVRM10;
using UniVRM10.Migration;
using UniGLTF;
using UniGLTF.Extensions.VRMC_vrm;
using VRMShaders;
using Axis.Elements;
using Axis.Elements.AnimatorLink;
using SimpleFileBrowser;

public class AxisVRMHandler : MonoBehaviour {
    [SerializeField] AxisBrain axisBrain;
    [SerializeField] Text vrmInfoText;
    AxisMannequin mannequin;
    HumanPoseHandler mannequinPoseHandler, vrmPoseHandler;
    Vrm10Instance vrmInstance;
    HumanPose pose;

    public AxisBrain AxisBrain {
        get => axisBrain;
        set {
            axisBrain = value ? value : AxisBrain.FetchBrainOnScene();
            UpdateMannequin();
        }
    }

    void Awake() {
        if (axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
    }

    void OnEnable() => UpdateMannequin();

    void OnDisable() => UpdateMannequin();

    void OnDestroy() {
        if (mannequinPoseHandler != null) mannequinPoseHandler.Dispose();
        UnloadModel();
    }

    void UpdateMannequin(bool forceEnable = false) {
        if (forceEnable && axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        if (axisBrain != null) {
            var newMannequin = axisBrain.axisMannequin;
            if (newMannequin != mannequin) {
                if (mannequin != null || !enabled) {
                    if (mannequin != null)
                        mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
                    if (mannequinPoseHandler != null) mannequinPoseHandler.Dispose();
                    mannequinPoseHandler = null;
                }
                mannequin = newMannequin;
                if (mannequin != null && enabled)
                    mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
                return;
            }
        }
        if (!enabled || forceEnable) {
            if (mannequin != null) {
                mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
                mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
            }
            if (!enabled) {
                if (mannequinPoseHandler != null) mannequinPoseHandler.Dispose();
                mannequinPoseHandler = null;
            }
        }
    }

    void OnDataUpdated(BodyModelAnimatorLink animatorLink) {
        if (vrmPoseHandler == null) return;
        if (mannequinPoseHandler == null) {
            var animator = animatorLink.Animator;
            mannequinPoseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
        }
        // Copy pose from mannequin to VRM avatar
        mannequinPoseHandler.GetHumanPose(ref pose);
        vrmPoseHandler.SetHumanPose(ref pose);
    }

    void OnVRMMetaData(Texture2D thumbnail, Meta newMeta, Vrm0Meta oldMeta) {
        if (vrmInfoText == null) return;
        if (newMeta != null) 
            vrmInfoText.text = $"{newMeta.Name} V{newMeta.Version}\n{newMeta.CreditNotation}";
        else if (oldMeta != null)
            vrmInfoText.text = $"{oldMeta.title} V{oldMeta.version}\n{oldMeta.author} {oldMeta.contactInformation}";
        else
            vrmInfoText.text = "";
    }

    public void LoadModelFromDialog() {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("VRM", ".vrm"));
        FileBrowser.SetDefaultFilter("VRM");
        FileBrowser.ShowLoadDialog(
            paths => {
                if (paths.Length > 0) LoadModel(paths[0]).Forget();
            },
            () => {},
            FileBrowser.PickMode.Files,
            title: "Load VRM Avatar",
            loadButtonText: "Open"
        );
    }

    public async UniTask LoadModel(string path, CancellationToken cancelToken = default) {
        UnloadModel();
        vrmInstance = await Vrm10.LoadPathAsync(path,
            awaitCaller: new RuntimeOnlyAwaitCaller(),
            materialGenerator: new BuiltInVrm10MaterialDescriptorGenerator(),
            vrmMetaInformationCallback: OnVRMMetaData,
            ct: cancelToken
        );
        vrmInstance.UpdateType = Vrm10Instance.UpdateTypes.None;
        var gltfInstance = vrmInstance.GetComponent<RuntimeGltfInstance>();
        gltfInstance.EnableUpdateWhenOffscreen();
        vrmPoseHandler = new HumanPoseHandler(vrmInstance.Humanoid.CreateAvatar(), vrmInstance.transform);
    }

    public void UnloadModel() {
        if (vrmInfoText != null) vrmInfoText.text = "";
        if (vrmInstance != null) {
            Destroy(vrmInstance.gameObject);
            vrmInstance = null;
        }
        if (vrmPoseHandler != null) {
            vrmPoseHandler.Dispose();
            vrmPoseHandler = null;
        }
    }
}
