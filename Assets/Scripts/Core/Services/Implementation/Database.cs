using UnityEngine;

namespace DOTSTemplate
{
    [CreateAssetMenu(menuName = "Database/Database")]
    public class Database : ScriptableObject, IDatabaseService
    {
        [SerializeField]
        private LevelDefinition[] levels;

        public LevelDefinition[] Levels => levels;
    }
}