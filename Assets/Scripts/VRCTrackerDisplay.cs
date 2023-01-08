using UnityEngine;

public class VRCTrackerDisplay : MonoBehaviour {
    [SerializeField] string key;
    public void DataUpdated(string key, Vector3 position, Quaternion rotation) {
        if (!string.Equals(key, this.key, System.StringComparison.Ordinal)) return;
        transform.SetPositionAndRotation(position, rotation);
    }
}
