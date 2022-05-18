using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace DOTSTemplate.Procedural
{
    public struct NativeMeshBuilder<TVertex> : IDisposable where TVertex : unmanaged
    {
        private static FixedList4096Bytes<VertexAttributeDescriptor> staticAttributes;
        
        public NativeList<TVertex> Vertices;
        public NativeList<uint> Indices;

        private uint indexOffset;
        
        public int VertexCount => Vertices.Length;
        public int IndexCount => Indices.Length;
        
        static NativeMeshBuilder()
        {
            var type = typeof(TVertex);
            var attrs = new List<VertexAttributeDescriptor>();
            foreach (var fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public |
                                                     BindingFlags.NonPublic))
            {
                var c = fieldInfo.GetCustomAttribute<VertexAttributeAttribute>();
                if (c == null)
                {
                    throw new ArgumentException(
                        $"Field \"{fieldInfo.Name}\" of \"{type.FullName}\" must define VertexAttributeAttribute");
                }

                attrs.Add(c.ToDescriptor());
            }
            
            staticAttributes = new FixedList4096Bytes<VertexAttributeDescriptor>();
            for (var index = 0; index < attrs.Count; index++)
            {
                var vertexAttributeDescriptor = attrs[index];
                staticAttributes.Add(vertexAttributeDescriptor);
            }
        }
        
        public NativeMeshBuilder(Allocator allocator)
        {
            Vertices = new NativeList<TVertex>(allocator);
            Indices = new NativeList<uint>(allocator);
            indexOffset = 0;
        }
        
        public void AddVertex(TVertex vertex)
        {
            Vertices.Add(vertex);
        }
        
        public void AddIndex(int i)
        {
            var index = Indices.Length;
            Indices.Resize(Indices.Length + 3, NativeArrayOptions.UninitializedMemory);
            Indices[index] = (uint)(indexOffset + i);
        }
        
        public void AddTriangleIndices(int i0, int i1, int i2)
        {
            var index = Indices.Length;
            Indices.Resize(Indices.Length + 3, NativeArrayOptions.UninitializedMemory);
            Indices[index] = (uint)(indexOffset + i0);
            Indices[index + 1] = (uint)(indexOffset + i1);
            Indices[index + 2] = (uint)(indexOffset + i2);
        }
        
        public void AddQuadIndices(int i0, int i1, int i2, int i3)
        {
            var index = Indices.Length;
            Indices.Resize(Indices.Length + 6, NativeArrayOptions.UninitializedMemory);
            Indices[index] = (uint)(indexOffset + i0);
            Indices[index + 1] = (uint)(indexOffset + i1);
            Indices[index + 2] = (uint)(indexOffset + i2);
            Indices[index + 3] = (uint)(indexOffset + i0);
            Indices[index + 4] = (uint)(indexOffset + i2);
            Indices[index + 5] = (uint)(indexOffset + i3);
        }
        
        public void EndPart()
        {
            indexOffset = (uint)Vertices.Length;
        }

        public void ToMeshData(ref Mesh.MeshData meshData)
        {
            using (var attributeArray = staticAttributes.ToNativeArray(Allocator.Temp))
            {
                meshData.SetVertexBufferParams(Vertices.Length, attributeArray);
            }

            var vertexData = meshData.GetVertexData<TVertex>();
            vertexData.CopyFrom(Vertices);
            
            meshData.SetIndexBufferParams(Indices.Length, IndexFormat.UInt32);
            var indexData = meshData.GetIndexData<uint>();
            indexData.CopyFrom(Indices);
            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0,  new SubMeshDescriptor(0, Indices.Length, MeshTopology.Triangles), MeshUpdateFlags.DontNotifyMeshUsers 
                                                                                                      | MeshUpdateFlags.DontRecalculateBounds 
                                                                                                      | MeshUpdateFlags.DontValidateIndices);
        }
        
        public void Dispose()
        {
            Vertices.Dispose();
            Indices.Dispose();
        }
    }
}