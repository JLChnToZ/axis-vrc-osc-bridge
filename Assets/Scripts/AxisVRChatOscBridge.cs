using System;
using System.Collections.Generic;
using UnityEngine;
using Axis.Elements;
using Axis.Elements.AnimatorLink;
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
    readonly Dictionary<HumanBodyBones, Quaternion> firstRotation = new Dictionary<HumanBodyBones, Quaternion>();
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
                if (mannequin != null && oscClient != null) {
                    firstRotation.Clear();
                    mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
                }
                return;
            }
        }
        if ((oscClient == null || forceEnable) && mannequin != null) {
            mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
            if (oscClient != null) {
                firstRotation.Clear();
                mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
            }
        }
    }

    void OnDataUpdated(BodyModelAnimatorLink animatorLink) {
        if (oscClient == null) return;
        int index = 1;
        var animator = animatorLink.Animator;
        var pose = animatorLink.tPoseLocalRotations;
        // Head (Reference Point)
        ReportTrackerUpdate(pose, animator, HumanBodyBones.Head, 0, "head");
        // Hip
        ReportTrackerUpdate(pose, animator, HumanBodyBones.Hips, 0, ref index);
        // Feet
        ReportTrackerUpdate(pose, animator, HumanBodyBones.LeftFoot, 0, ref index);
        ReportTrackerUpdate(pose, animator, HumanBodyBones.RightFoot, 0, ref index);
        // Chest
        ReportTrackerUpdate(pose, animator, HumanBodyBones.Chest, 0, ref index);
        // Elbows + Shoulders
        ReportTrackerUpdate(pose, animator, HumanBodyBones.LeftUpperArm, 0.5F, ref index);
        ReportTrackerUpdate(pose, animator, HumanBodyBones.RightUpperArm, 0.5F, ref index);
        // Knees
        ReportTrackerUpdate(pose, animator, HumanBodyBones.LeftUpperLeg, 1, ref index);
        ReportTrackerUpdate(pose, animator, HumanBodyBones.RightUpperLeg, 1, ref index);
    }

    void ReportTrackerUpdate(PoseStorage pose, Animator animator, HumanBodyBones boneName, float lerp, ref int index) {
        if (ReportTrackerUpdate(pose, animator, boneName, lerp, index.ToString())) index++;
    }

    bool ReportTrackerUpdate(PoseStorage pose, Animator animator, HumanBodyBones boneName, float lerp, string key) {
        if (boneName >= HumanBodyBones.LastBone) return false;
        var nodeTransform = animator.GetBoneTransform(boneName);
        if (nodeTransform == null) return false;
        var position = nodeTransform.position;
        // Get relative rotation only
        var rotation = nodeTransform.rotation;
        if (firstRotation.TryGetValue(boneName, out var originRotation))
            rotation *= Quaternion.Inverse(originRotation);
        else {
            firstRotation[boneName] = rotation;
            rotation = Quaternion.identity;
        }
        // Offset the bone reference point to middle if applicable
        if (lerp > 0 && nextBones.TryGetValue(boneName, out var nextBoneName)) {
            var nextBone = animator.GetBoneTransform(nextBoneName);
            if (nextBone != null) position = Vector3.Lerp(position, nextBone.position, lerp);
        }
        oscClient.Send($"/tracking/trackers/{key}/position", position * scale);
        oscClient.Send($"/tracking/trackers/{key}/rotation", rotation.eulerAngles);
        return true;
    }

    void IDisposable.Dispose() => Disconnect();
}
