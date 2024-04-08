using UnityEngine;

public class VRCTrackerDisplay : MonoBehaviour {
    [SerializeField] int channel;
    [SerializeField] int materialIndex;
    [SerializeField] Color connectedColor, disconnectedColor;
    [SerializeField] float notUpdatedTimeout = 1;
    float updatingTime;
    new Renderer renderer;
    MaterialPropertyBlock propertyBlock;
    static int colorId;

    void Awake() {
        if (colorId == 0) colorId = Shader.PropertyToID("_Color");
        renderer = GetComponentInChildren<Renderer>(true);
        propertyBlock = new MaterialPropertyBlock();
        updatingTime = notUpdatedTimeout;
    }

    void Update() {
        updatingTime += Time.deltaTime;
        if (renderer != null) {
            propertyBlock.SetColor(colorId, updatingTime > notUpdatedTimeout ? disconnectedColor : connectedColor);
            renderer.SetPropertyBlock(propertyBlock, materialIndex);
            renderer.enabled = updatingTime <= notUpdatedTimeout;
        }
    }

    public void DataUpdated(int key, Vector3 position) {
        if (key != channel) return;
        transform.position = position;
        updatingTime = 0;
    }

    public void DataUpdated(int key, Quaternion rotation) {
        if (key != channel) return;
        transform.rotation = rotation;
        updatingTime = 0;
    }
}
