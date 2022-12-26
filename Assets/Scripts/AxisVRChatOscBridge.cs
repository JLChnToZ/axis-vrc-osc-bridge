using System;
using UnityEngine;
using Axis.Elements;
using Axis.Elements.AnimatorLink;
using Axis.Enumerations;
using Axis.DataProcessing;
using OscCore;

public class AxisVRChatOscBridge : IDisposable {
    [SerializeField] AxisBrain axisBrain;
    [SerializeField] float scale = 1F;
    OscClient oscClient;
    AxisMannequin mannequin;

    public float Scale {
        get => scale;
        set {
            if (scale == value) return;
            scale = value;
            if (mannequin != null)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public void Connect(string ipAddr = "127.0.0.1", int port = 9000) {
        oscClient = new OscClient(ipAddr, port);
        if (axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        mannequin = axisBrain.axisMannequin;
        if (mannequin != null)
            mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
    }

    public void Disconnect() {
        oscClient = null;
        if (mannequin != null)
            mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
    }

    void OnDataUpdated(BodyModelAnimatorLink animatorLink) {
        if (oscClient == null) return;
        int index = 1;
        var animator = animatorLink.Animator;
        ReportTrackerUpdate(animator, NodeBinding.Head, "head");
        ReportTrackerUpdate(animator, NodeBinding.Hips, ref index);
        ReportTrackerUpdate(animator, NodeBinding.LeftFoot, ref index);
        ReportTrackerUpdate(animator, NodeBinding.RightFoot, ref index);
        ReportTrackerUpdate(animator, NodeBinding.Chest, ref index);
        ReportTrackerUpdate(animator, NodeBinding.LeftUpperArm, ref index);
        ReportTrackerUpdate(animator, NodeBinding.RightUpperArm, ref index);
        ReportTrackerUpdate(animator, NodeBinding.LeftCalf, ref index);
        ReportTrackerUpdate(animator, NodeBinding.RightCalf, ref index);
    }

    void ReportTrackerUpdate(Animator animator, NodeBinding binding, ref int index) {
        if (ReportTrackerUpdate(animator, binding, index.ToString())) index++;
    }

    bool ReportTrackerUpdate(Animator animator, NodeBinding binding, string key) {
        var boneName = AxisDataUtility.ConvertAxisLimbToHumanBone(binding);
        if (boneName == HumanBodyBones.LastBone) return false;
        var nodeTransform = animator.GetBoneTransform(boneName);
        if (nodeTransform == null) return false;
        oscClient.Send($"/tracking/trackers/{key}/position", nodeTransform.position * scale);
        oscClient.Send($"/tracking/trackers/{key}/rotation", nodeTransform.eulerAngles);
        return true;
    }

    void IDisposable.Dispose() => Disconnect();
}
