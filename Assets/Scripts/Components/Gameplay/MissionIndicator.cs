using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    [RequireComponent(typeof(ParticleSystem))]
    public class MissionIndicator : MonoBehaviour, IConvertGameObjectToEntity
    {
        private ParticleSystem particles;

        void Awake()
        {
            particles = GetComponent<ParticleSystem>();
            SetState(false);
        }
        
        public void SetState(bool state)
        {
            if (state)
            {
                particles.gameObject.SetActive(true);
                particles.Play(true);
            }
            else
                particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            conversionSystem.CreateAdditionalEntity(GetComponent<ParticleSystem>());
            conversionSystem.CreateAdditionalEntity(this);
        }
    }
}