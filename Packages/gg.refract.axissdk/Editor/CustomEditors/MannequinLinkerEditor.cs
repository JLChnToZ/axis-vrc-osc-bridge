using UnityEditor;
using UnityEngine;

namespace Axis.Elements.Linker._Editor
{
    [CustomEditor(typeof(CharacterToBrainLinker))]
    public class MannequinLinkerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CharacterToBrainLinker _target = (CharacterToBrainLinker)target;
            if(_target.linkedBrain != null)
            {
                
                //Debug.Log("List available mannequins");
            }
        }

    }
}

