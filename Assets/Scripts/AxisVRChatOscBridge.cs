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
    AxisBrain axisBrain;
    float scaleX = 1F, scaleY = 1F;
    OscClient oscClient;
    AxisMannequin mannequin;
    HumanoidRelativeRotationHelper rotationHelper;
    Animator animator;

    public event Action<string, Vector3, Quaternion> DataUpdated;

    public AxisBrain AxisBrain {
        get => axisBrain;
        set {
            axisBrain = value ? value : AxisBrain.FetchBrainOnScene();
            UpdateMannequin();
        }
    }

    public float Scale {
        get => (scaleX + scaleY) / 2F;
        set {
            scaleX = scaleY = value;
            if (mannequin != null && oscClient != null)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public float ScaleX {
        get => scaleX;
        set {
            if (scaleX == value) return;
            scaleX = value;
            if (mannequin != null && oscClient != null)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public float ScaleY {
        get => scaleY;
        set {
            if (scaleY == value) return;
            scaleY = value;
            if (mannequin != null && oscClient != null)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public void Connect(string ipAddr = "127.0.0.1", int port = 9000) {
        oscClient = new OscClient(ipAddr, port);
        DataUpdated += ReportToOsc;
        UpdateMannequin(true);
    }

    public void Disconnect() {
        oscClient = null;
        DataUpdated -= ReportToOsc;
        UpdateMannequin();
    }

    void UpdateMannequin(bool forceEnable = false) {
        if (forceEnable && axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        if (axisBrain != null) {
            var newMannequin = axisBrain.axisMannequin;
            if (newMannequin != mannequin) {
                if (mannequin != null || oscClient == null) {
                    if (mannequin != null)
                        mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
                    rotationHelper = null;
                    animator = null;
                }
                mannequin = newMannequin;
                if (mannequin != null && oscClient != null)
                    mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
                return;
            }
        }
        if (oscClient == null || forceEnable) {
            if (mannequin != null) {
                mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
                if (oscClient != null)
                    mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
            }
            if (oscClient == null) {
                rotationHelper = null;
                animator = null;
            }
        }
    }

    void OnDataUpdated(BodyModelAnimatorLink animatorLink) {
        if (oscClient == null) return;
        if (animator == null) animator = animatorLink.Animator;
        if (rotationHelper == null) rotationHelper = animatorLink.GetComponent<HumanoidRelativeRotationHelper>();
        int index = 1;
        // Head (Reference Point)
        ReportTrackerUpdate(HumanBodyBones.Head, 0, "head");
        // Hip
        ReportTrackerUpdate(HumanBodyBones.Hips, 0, ref index);
        // Feet
        ReportTrackerUpdate(HumanBodyBones.LeftFoot, 0, ref index);
        ReportTrackerUpdate(HumanBodyBones.RightFoot, 0, ref index);
        // Chest
        ReportTrackerUpdate(HumanBodyBones.Chest, 0, ref index);
        // Elbows + Shoulders
        ReportTrackerUpdate(HumanBodyBones.LeftUpperArm, 0.5F, ref index);
        ReportTrackerUpdate(HumanBodyBones.RightUpperArm, 0.5F, ref index);
        // Knees
        ReportTrackerUpdate(HumanBodyBones.LeftUpperLeg, 1, ref index);
        ReportTrackerUpdate(HumanBodyBones.RightUpperLeg, 1, ref index);
    }

    void ReportTrackerUpdate(HumanBodyBones boneName, float lerp, ref int index) {
        if (ReportTrackerUpdate(boneName, lerp, index.ToString())) index++;
    }

    bool ReportTrackerUpdate(HumanBodyBones boneName, float lerp, string key) {
        if (boneName >= HumanBodyBones.LastBone) return false;
        var nodeTransform = animator.GetBoneTransform(boneName);
        if (nodeTransform == null) return false;
        var position = nodeTransform.position;
        // Get relative rotation only
        var rotation = nodeTransform.rotation;
        if (rotation != null) rotation = rotationHelper.GetRotation(rotation, boneName);
        // Offset the bone reference point to middle if applicable
        if (lerp > 0 && nextBones.TryGetValue(boneName, out var nextBoneName)) {
            var nextBone = animator.GetBoneTransform(nextBoneName);
            if (nextBone != null) position = Vector3.Lerp(position, nextBone.position, lerp);
        }
        position.x *= scaleX;
        position.y *= scaleY;
        position.z *= scaleX;
        DataUpdated?.Invoke(key, position, rotation);
        return true;
    }

    void ReportToOsc(string key, Vector3 position, Quaternion rotation) {
        oscClient.Send($"/tracking/trackers/{key}/position", position);
        oscClient.Send($"/tracking/trackers/{key}/rotation", rotation.eulerAngles);
    }

    void IDisposable.Dispose() => Disconnect();
}
