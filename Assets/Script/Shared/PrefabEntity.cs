using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DOTSTemplate
{
    [Serializable]
    public class PrefabEntity
    {
        [SerializeField]
        internal GameObject prefab;
        [NonSerialized]
        private Entity entity;

        public PrefabEntity(GameObject prefab)
        {
            this.prefab = prefab;
        }

        public void DeclarePrefab(List<GameObject> referencedPrefabs)
        {
            entity = Entity.Null;
            if (prefab != null)
                referencedPrefabs.Add(prefab);
        }
        
        public void PreparePrefab(GameObjectConversionSystem conversionSystem)
        {
            if (prefab == null) return;
            entity = conversionSystem.GetPrimaryEntity(prefab);
        }
        
        public static implicit operator Entity(PrefabEntity prefabEntity)
        {
            return prefabEntity.entity;
        }
    }
    
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(PrefabEntity))]
    class PrefabEntityDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property.FindPropertyRelative(nameof(PrefabEntity.prefab)), label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(PrefabEntity.prefab)), label);
        }
    }
    
#endif
}