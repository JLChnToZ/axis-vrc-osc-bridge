using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HumanoidRelativeRotationHelper : MonoBehaviour {
    Quaternion[] originRotation;
    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
        originRotation = new Quaternion[(int)HumanBodyBones.LastBone];
        for (var i = 0; i < (int)HumanBodyBones.LastBone; i++) {
            var boneTransform = animator.GetBoneTransform((HumanBodyBones)i);
            if (boneTransform != null) originRotation[i] = Quaternion.Inverse(boneTransform.rotation);
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
