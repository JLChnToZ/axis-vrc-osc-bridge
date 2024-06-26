using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HumanoidRelativeRotationHelper : MonoBehaviour {
    Quaternion[] originRotation;
    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
        originRotation = new Quaternion[HumanTrait.BoneCount];
        for (int i = 0; i < originRotation.Length; i++) {
            var boneTransform = animator.GetBoneTransform((HumanBodyBones)i);
            originRotation[i] = boneTransform != null ? Quaternion.Inverse(boneTransform.rotation) : Quaternion.identity;
        }
    }

    public Quaternion GetRotation(HumanBodyBones bone) {
        var boneTransform = animator.GetBoneTransform(bone);
        if (boneTransform == null) return Quaternion.identity;
        return GetRotation(boneTransform.rotation, bone);
    }

    public Quaternion GetRotation(Quaternion rotation, HumanBodyBones bone) {
        return rotation * originRotation[(int)bone];
    }
}
