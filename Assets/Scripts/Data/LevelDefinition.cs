using UnityEngine;

namespace DOTSTemplate
{
    [CreateAssetMenu(menuName = "Database/Level")]
    public class LevelDefinition : ScriptableObject
    {
        public string Name;
        public string Scene;
    }
}