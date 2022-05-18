using System.Collections.Generic;
using UnityEngine;

namespace DOTSTemplate
{
    public interface IPrefabProvider
    {
        void DeclarePrefabs(List<GameObject> referencedPrefabs);
        
        void PreparePrefabs(GameObjectConversionSystem conversionSystem);
    }
}