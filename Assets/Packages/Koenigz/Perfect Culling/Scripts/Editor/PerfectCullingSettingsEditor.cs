// Perfect Culling (C) 2021 Patrick König
//

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Koenigz.PerfectCulling
{
    [CustomEditor(typeof(PerfectCullingSettings))]
    public class PerfectCullingSettingsEditor : Editor
    {
        static readonly string PerfectCullingDisplayVersion = "1.1.7";
        
#pragma warning disable 414
        private static readonly int PerfectCullingVersionCode = 1170;
#pragma warning restore 414

        SerializedObject so; // PerfectCullingSettings SO
        
        SerializedProperty useUnityForRendering;
        SerializedProperty useUnityForRenderingCpuCompute;
        
        SerializedProperty renderTransparency;
        SerializedProperty bakeCameraResolution;
        SerializedProperty bakeAverageSamplingSpeedMs;
        
        private void OnEnable()
        {
            PerfectCullingSettings targetSettings = (PerfectCullingSettings)target;
            
            so = new SerializedObject(targetSettings);

            useUnityForRendering = so.FindProperty("useUnityForRendering");
            useUnityForRenderingCpuCompute = so.FindProperty("useUnityForRenderingCpuCompute");
            
            renderTransparency = so.FindProperty("renderTransparency");
            bakeCameraResolution = so.FindProperty("bakeCameraResolution");
            bakeAverageSamplingSpeedMs = so.FindProperty("bakeAverageSamplingSpeedMs");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox($"\n*** Perfect Culling ***\n\n Version: {PerfectCullingDisplayVersion}\n\n Support: info@koenigz.com\n", MessageType.Info);
            
            so.Update();
            {
                PerfectCullingSettings targetSettings = (PerfectCullingSettings)target;
                
                EditorGUILayout.PropertyField(useUnityForRendering, new GUIContent( "Use Unity For Rendering" ) );
                
                if (useUnityForRendering.boolValue)
                {
                    ++EditorGUI.indentLevel;
                    
                    EditorGUILayout.PropertyField(useUnityForRenderingCpuCompute, new GUIContent( "No Compute Shader support (computes on CPU)" ) );
                    
                    if (useUnityForRenderingCpuCompute.boolValue)
                    {
                        EditorGUILayout.HelpBox("\nMake sure to drop your Bake Camera Resolution or your bakes will take very very long.\nGood starting value: 32\nYou might ramp it up further but start small because it is getting slower and slower as you scale this up!\n", MessageType.Warning);
                    }

                    --EditorGUI.indentLevel;
                }
                else
                {
                    EditorGUILayout.HelpBox("Native renderer is being deprecated and will eventually be removed.\n\nReason: Unity Editor performance was and is still seeing major improvements and the need for a faster native renderer fades.", MessageType.Warning);    
                }
                
                bakeCameraResolution.intValue = Mathf.Clamp(Mathf.ClosestPowerOfTwo(bakeCameraResolution.intValue), 16, 2048);
                
                EditorGUILayout.PropertyField(renderTransparency, new GUIContent( "Render Transparency" ) );

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(bakeCameraResolution, new GUIContent("Bake Camera Resolution"));

                    if (GUILayout.Button("<", GUILayout.Width(25)))
                    {
                        int prevPowerOfTwo = 16;
                        int currentPowerOfTWo = Mathf.ClosestPowerOfTwo(bakeCameraResolution.intValue);
                        
                        while (true)
                        {
                            int newPowerOfTwo = Mathf.NextPowerOfTwo(prevPowerOfTwo + 1);
                            
                            if (newPowerOfTwo >= currentPowerOfTWo)
                            {
                                break;
                            }

                            prevPowerOfTwo = newPowerOfTwo;
                        }

                        bakeCameraResolution.intValue  = prevPowerOfTwo;
                    }
                    
                    
                    if (GUILayout.Button(">", GUILayout.Width(25)))
                    {
                        bakeCameraResolution.intValue = Mathf.NextPowerOfTwo(bakeCameraResolution.intValue + 1);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                
                EditorGUILayout.PropertyField(bakeAverageSamplingSpeedMs, new GUIContent( "Bake Average Sampling Speed" ) );
            }
            so.ApplyModifiedProperties();
        }
    }
}
#endif