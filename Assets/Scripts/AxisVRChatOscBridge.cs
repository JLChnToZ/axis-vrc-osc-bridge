using System;
using System.Collections.Generic;
using UnityEngine;
using Axis.Elements;
using Axis.Elements.AnimatorLink;
using Axis.Enumerations;
using Axis.DataProcessing;
using OscCore;

public class AxisVRChatOscBridge : IDisposable {
    static readonly Dictionary<HumanBodyBones, HumanBodyBones> nextBones = new Dictionary<HumanBodyBones, HumanBodyBones> {
        [HumanBodyBones.LeftShoulder] = HumanBodyBones.LeftUpperArm,
        [HumanBodyBones.LeftUpperArm] = HumanBodyBones.LeftLowerArm,
        [HumanBodyBones.LeftLowerArm] = HumanBodyBones.LeftHand,
        [HumanBodyBones.LeftUpperLeg] = HumanBodyBones.LeftLowerLeg,
        [HumanBodyBones.LeftLowerLeg] = HumanBodyBones.LeftFoot,
        [HumanBodyBones.LeftFoot] = HumanBodyBones.LeftToes,
        [HumanBodyBones.RightShoulder] = HumanBodyBones.RightUpperArm,
        [HumanBodyBones.RightUpperArm] = HumanBodyBones.RightLowerArm,
        [HumanBodyBones.RightLowerArm] = HumanBodyBones.RightHand,
        [HumanBodyBones.RightUpperLeg] = HumanBodyBones.RightLowerLeg,
        [HumanBodyBones.RightLowerLeg] = HumanBodyBones.RightFoot,
        [HumanBodyBones.RightFoot] = HumanBodyBones.RightToes,
    };

    AxisBrain axisBrain;
    float scale = 1F;
    OscClient oscClient;
    AxisMannequin mannequin;

    public AxisBrain AxisBrain {
        get => axisBrain;
        set {
            axisBrain = value ? value : AxisBrain.FetchBrainOnScene();
            UpdateMannequin();
        }
    }

    public float Scale {
        get => scale;
        set {
            if (scale == value) return;
            scale = value;
            if (mannequin != null && oscClient != null)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public void Connect(string ipAddr = "127.0.0.1", int port = 9000) {
        oscClient = new OscClient(ipAddr, port);
        UpdateMannequin(true);
    }

    public void Disconnect() {
        oscClient = null;
        UpdateMannequin();
    }

    void UpdateMannequin(bool forceEnable = false) {
        if (forceEnable && axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        if (axisBrain != null) {
            var newMannequin = axisBrain.axisMannequin;
            if (newMannequin != mannequin) {
                if (mannequin != null || oscClient == null)
                    mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
                mannequin = newMannequin;
                if (oscClient != null)
                    mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
                return;
            }
        }
        if ((oscClient == null || forceEnable) && mannequin != null) {
            mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
            if (oscClient != null)
                mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
        }
    }

    void OnDataUpdated(BodyModelAnimatorLink animatorLink) {
        if (oscClient == null) return;
        int index = 1;
        var animator = animatorLink.Animator;
        ReportTrackerUpdate(animator, HumanBodyBones.Head, "head");
        ReportTrackerUpdate(animator, HumanBodyBones.Hips, ref index);
        ReportTrackerUpdate(animator, HumanBodyBones.LeftFoot, ref index);
        ReportTrackerUpdate(animator, HumanBodyBones.RightFoot, ref index);
        ReportTrackerUpdate(animator, HumanBodyBones.Chest, ref index);
        ReportTrackerUpdate(animator, HumanBodyBones.LeftUpperArm, ref index);
        ReportTrackerUpdate(animator, HumanBodyBones.RightUpperArm, ref index);
        ReportTrackerUpdate(animator, HumanBodyBones.LeftUpperLeg, ref index);
        ReportTrackerUpdate(animator, HumanBodyBones.RightUpperLeg, ref index);
    }

    void ReportTrackerUpdate(Animator animator, HumanBodyBones boneName, ref int index) {
        if (ReportTrackerUpdate(animator, boneName, index.ToString())) index++;
    }

    bool ReportTrackerUpdate(Animator animator, HumanBodyBones boneName, string key) {
        if (boneName >= HumanBodyBones.LastBone) return false;
        var nodeTransform = animator.GetBoneTransform(boneName);
        if (nodeTransform == null) return false;
        var position = nodeTransform.position;
        var rotation = Quaternion.identity;
        // Offset the bone reference point to middle if applicable
        if (nextBones.TryGetValue(boneName, out var nextBoneName)) {
            var nextBone = animator.GetBoneTransform(nextBoneName);
            if (nextBone != null) {
                var nextPos = nextBone.position;
                rotation = Quaternion.LookRotation(nextPos - position, nodeTransform.up);
                position = (position + nextPos) / 2;
            }
        } else
            rotation = nodeTransform.rotation;
        oscClient.Send($"/tracking/trackers/{key}/position", position * scale);
        oscClient.Send($"/tracking/trackers/{key}/rotation", rotation.eulerAngles);
        return true;
    }

    void IDisposable.Dispose() => Disconnect();
}
