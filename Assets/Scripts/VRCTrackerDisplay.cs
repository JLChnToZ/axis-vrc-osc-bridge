using UnityEngine;

public class VRCTrackerDisplay : MonoBehaviour {
    [SerializeField] int channel;
    new Renderer renderer;
    MaterialPropertyBlock propertyBlock;
    static int lastIndicatorId;

    void Awake() {
        if (lastIndicatorId == 0) lastIndicatorId = Shader.PropertyToID("_LastUpdateTime");
        renderer = GetComponentInChildren<Renderer>(true);
        propertyBlock = new MaterialPropertyBlock();
    }

    public void DataUpdated(int key, Vector3 position) {
        if (key != channel) return;
        transform.position = position;
        Flash();
    }

    public void DataUpdated(int key, Quaternion rotation) {
        if (key != channel) return;
        transform.rotation = rotation;
        Flash();
    }

    void Flash() {
        if (renderer != null) {
            propertyBlock.SetFloat(lastIndicatorId, Time.timeSinceLevelLoad);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
