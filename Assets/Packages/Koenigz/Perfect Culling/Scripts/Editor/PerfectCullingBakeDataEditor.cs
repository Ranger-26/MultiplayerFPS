// Perfect Culling (C) 2021 Patrick König
//

#if UNITY_EDITOR
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace Koenigz.PerfectCulling
{
    [CustomEditor(typeof(PerfectCullingBakeData), true)]
    public class PerfectCullingBakeDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PerfectCullingBakeData data = target as PerfectCullingBakeData;
            
            if (!data.bakeCompleted)
            {
                EditorGUILayout.HelpBox("This bake was not completed and might not function correctly.", MessageType.Error);
            }
            
            GUILayout.Label("Bake information", EditorStyles.boldLabel);
            
            data.DrawInspectorGUI();

            if (!string.IsNullOrEmpty(data.strBakeDate))
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Bake date");
                    EditorGUILayout.LabelField(System.DateTime.ParseExact(data.strBakeDate, "o", CultureInfo.InvariantCulture).ToLocalTime().ToString());
                EditorGUILayout.EndHorizontal();
            }

            if (data.bakeDurationMilliseconds > 0)
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Bake duration");
                    EditorGUILayout.LabelField(PerfectCullingUtil.FormatSeconds(data.bakeDurationMilliseconds * 0.001f));
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            
            if (PerfectCullingEditorUtil.TryGetAssetBakeSize(target as PerfectCullingBakeData, out float bakeSize))
            {
                GUILayout.Label($"Bake size: {bakeSize} mb(s)");
            }
        }
    }
}
#endif