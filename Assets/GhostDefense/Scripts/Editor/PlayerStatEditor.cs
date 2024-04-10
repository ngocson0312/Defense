using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UDEV.GhostDefense.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PlayerStat), editorForChildClasses: true)]
    public class PlayerStatEditor : ActorStatEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Level Up"))
            {
                m_target.LevelUpCore();
            }
        }
    }
}
