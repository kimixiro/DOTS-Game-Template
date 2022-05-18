using System;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DOTSTemplate
{
    [Serializable]
    public struct PointWithRadius
    {
        public float3 Position;
        public float Radius;

        public PointWithRadius(float3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
        
        public float GetRadiusOrDefault(float defaultRadius)
        {
            return Radius > 0 ? Radius : defaultRadius;
        }
    }
    
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(PointWithRadius))]
    class PointWithRadiusDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var radiusPropertyWidth = Mathf.Min(50, position.width * 0.25f);
            const float spacing = 15;
            

            position.width = position.width - (spacing + radiusPropertyWidth);
            EditorGUI.PropertyField(
                position, 
                property.FindPropertyRelative(nameof(PointWithRadius.Position)), 
                label);
            
            
            position.x += position.width + spacing;
            position.width = radiusPropertyWidth;
            EditorGUI.PropertyField(
                position, 
                property.FindPropertyRelative(nameof(PointWithRadius.Radius)), 
                GUIContent.none);
        }
    }
    
#endif
}