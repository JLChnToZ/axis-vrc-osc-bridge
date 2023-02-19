using System;
using System.Collections.Generic;
using UnityEngine;
using Axis.Elements;
using Axis.Elements.AnimatorLink;
using OscCore;

public class AxisVRChatOscBridge : IDisposable {
    public const string HEAD = "head";
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
    static readonly string[] oscChannelNames = new[] { HEAD, "1", "2", "3", "4", "5", "6", "7", "8" };
    AxisBrain axisBrain;
    bool activated;
    float scaleX = 1F, scaleY = 1F;
    OscClient oscClient;
    AxisMannequin mannequin;
    HumanoidRelativeRotationHelper rotationHelper;
    Animator animator;
    bool hasHead;
    Quaternion headRotation;
    Action<int, Vector3> positionUpdated;
    Action<int, Quaternion> rotationUpdated;
    readonly bool[] channelEnabled = new bool[] {
        false,
        true, true, true, true,
        true, true, true, true,
    };

    public event Action<int, Vector3> PositionUpdated {
        add {
            positionUpdated += value;
            if (!activated && positionUpdated != null) {
                activated = true;
                UpdateMannequin(true);
            }
        }
        remove {
            positionUpdated -= value;
            if (activated && positionUpdated == null && rotationUpdated == null) {
                activated = false;
                UpdateMannequin();
            }
        }
    }

    public event Action<int, Quaternion> RotationUpdated {
        add {
            rotationUpdated += value;
            if (!activated && rotationUpdated != null) {
                activated = true;
                UpdateMannequin(true);
            }
        }
        remove {
            rotationUpdated -= value;
            if (activated && positionUpdated == null && rotationUpdated == null) {
                activated = false;
                UpdateMannequin();
            }
        }
    }

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
            if (mannequin != null && activated)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public float ScaleX {
        get => scaleX;
        set {
            if (scaleX == value) return;
            scaleX = value;
            if (mannequin != null && activated)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public float ScaleY {
        get => scaleY;
        set {
            if (scaleY == value) return;
            scaleY = value;
            if (mannequin != null && activated)
                OnDataUpdated(mannequin.bodyModelAnimatorLink);
        }
    }

    public bool HasHead {
        get => hasHead;
        set {
            hasHead = value;
            if (hasHead && oscClient != null) SyncHeadRotation();
        }
    }

    public void Connect(string ipAddr = "127.0.0.1", int port = 9000) {
        oscClient = new OscClient(ipAddr, port);
        PositionUpdated += ReportToOsc;
        RotationUpdated += ReportToOsc;
    }

    public void Disconnect() {
        oscClient = null;
        PositionUpdated -= ReportToOsc;
        RotationUpdated -= ReportToOsc;
    }

    public bool IsChannelEnabled(int key) => channelEnabled[key];

    public void SetChannelEnabled(int key, bool enabled)  {
        if (channelEnabled[key] != enabled) {
            channelEnabled[key] = enabled;
            if (enabled && key == 0 && oscClient != null) SyncHeadRotation();
        }
    }

    void UpdateMannequin(bool forceEnable = false) {
        if (forceEnable && axisBrain == null) axisBrain = AxisBrain.FetchBrainOnScene();
        if (axisBrain != null) {
            var newMannequin = axisBrain.axisMannequin;
            if (newMannequin != mannequin) {
                if (mannequin != null || !activated) {
                    if (mannequin != null)
                        mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
                    rotationHelper = null;
                    animator = null;
                }
                mannequin = newMannequin;
                if (mannequin != null && activated)
                    mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
                return;
            }
        }
        if (!activated || forceEnable) {
            if (mannequin != null) {
                mannequin.onBodyModelAnimatorLinkUpdated -= OnDataUpdated;
                mannequin.onBodyModelAnimatorLinkUpdated += OnDataUpdated;
            }
            if (!activated) {
                rotationHelper = null;
                animator = null;
            }
        }
    }

    void OnDataUpdated(BodyModelAnimatorLink animatorLink) {
        if (positionUpdated == null || rotationUpdated == null) return;
        if (animator == null) animator = animatorLink.Animator;
        if (rotationHelper == null) rotationHelper = animatorLink.GetComponent<HumanoidRelativeRotationHelper>();
        // Head (Reference Point)
        ReportTrackerUpdate(HumanBodyBones.Head, 0, 0);
        // Hip
        ReportTrackerUpdate(HumanBodyBones.Hips, 0, 1);
        // Feet
        ReportTrackerUpdate(HumanBodyBones.LeftFoot, 0, 2);
        ReportTrackerUpdate(HumanBodyBones.RightFoot, 0, 3);
        // Chest
        ReportTrackerUpdate(HumanBodyBones.UpperChest, 0, 4);
        // Elbows + Shoulders
        ReportTrackerUpdate(HumanBodyBones.LeftUpperArm, 0.5F, 5);
        ReportTrackerUpdate(HumanBodyBones.RightUpperArm, 0.5F, 6);
        // Knees
        ReportTrackerUpdate(HumanBodyBones.LeftUpperLeg, 1, 7);
        ReportTrackerUpdate(HumanBodyBones.RightUpperLeg, 1, 8);
    }

    void ReportTrackerUpdate(HumanBodyBones boneName, float lerp, int key) {
        if (boneName >= HumanBodyBones.LastBone) return;
        var nodeTransform = animator.GetBoneTransform(boneName);
        if (nodeTransform == null) return;
        var position = nodeTransform.position;
        // Get relative rotation only
        var rotation = nodeTransform.rotation;
        if (rotationHelper != null) rotation = rotationHelper.GetRotation(rotation, boneName);
        // Offset the bone reference point to middle if applicable
        if (lerp > 0 && nextBones.TryGetValue(boneName, out var nextBoneName)) {
            var nextBone = animator.GetBoneTransform(nextBoneName);
            if (nextBone != null) position = Vector3.Lerp(position, nextBone.position, lerp);
        }
        position.x *= scaleX;
        position.y *= scaleY;
        position.z *= scaleX;
        if (key == 0) headRotation = rotation;
        if (key == 0 || channelEnabled[key]) positionUpdated?.Invoke(key, position);
        if (channelEnabled[key]) rotationUpdated?.Invoke(key, rotation);
    }

    void ReportToOsc(int index, Vector3 position) => oscClient.Send($"/tracking/trackers/{oscChannelNames[index]}/position", position);
    void ReportToOsc(int index, Quaternion rotation) => oscClient.Send($"/tracking/trackers/{oscChannelNames[index]}/rotation", rotation.eulerAngles);

    public void SyncHeadRotation() => oscClient?.Send($"/tracking/trackers/{HEAD}/rotation", headRotation.eulerAngles);

    void IDisposable.Dispose() => Disconnect();
}
