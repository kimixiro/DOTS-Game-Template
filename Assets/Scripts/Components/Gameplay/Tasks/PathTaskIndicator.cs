using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSTemplate
{
    [RequireComponent(typeof(LineRenderer))]
    public class PathTaskIndicator : MonoBehaviour, IConvertGameObjectToEntity
    {
        private static readonly int progressProperty = Shader.PropertyToID("Progress");
        private static readonly int lengthProperty = Shader.PropertyToID("Length");

        public PathTaskAuthoring PathTask;
        
        public float Complexity;
        public CurvedPathProvider PathProvider;
        
        private LineRenderer pathRenderer;
        private MaterialPropertyBlock materialPropertyBlock;
        
        void Awake()
        {
            pathRenderer = GetComponent<LineRenderer>();
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        public void SetEnabled(bool state)
        {
            pathRenderer.enabled = state;
        }

        public void SetProgress(float progress, float length)
        {
            materialPropertyBlock.SetFloat(progressProperty, progress);
            materialPropertyBlock.SetFloat(lengthProperty, length);
            pathRenderer.SetPropertyBlock(materialPropertyBlock);
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                typeof(AttachedTask),
                typeof(Disabled)
            ));

            var pathTaskEntity = conversionSystem.GetPrimaryEntity(PathTask);
            dstManager.SetComponentData(entity, new AttachedTask {Entity = pathTaskEntity});
            dstManager.Link(pathTaskEntity, entity);
            
            var transformMatrix = (float4x4)transform.localToWorldMatrix;
            using var points = 
                PathProvider.GetPath(ref transformMatrix, Complexity);
            
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points.AsArray().Reinterpret<Vector3>());
            
            conversionSystem.CreateAdditionalEntity(lineRenderer);
            conversionSystem.CreateAdditionalEntity(this);
        }
    }
}