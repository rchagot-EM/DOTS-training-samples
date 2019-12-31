using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

public class ClothSimEcsAuthoring : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
	    Mesh originalMesh = meshFilter.sharedMesh;
	    Mesh mesh = meshFilter.sharedMesh = Instantiate(originalMesh);

	    // Create entity prefab from the game object hierarchy once
	    EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
	    GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
	    Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, settings);

            DynamicBuffer<BarElement> bars = entityManager.AddBuffer<BarElement>(entity);
            bars.ResizeUninitialized(mesh.vertices.Length);
            DynamicBuffer<BarLengthElement> barLengths = entityManager.AddBuffer<BarLengthElement>(entity);
            barLengths.ResizeUninitialized(mesh.vertices.Length);
            DynamicBuffer<VertexStateCurrentElement> vertexStateCurrentElement = entityManager.AddBuffer<VertexStateCurrentElement>(entity);
            DynamicBuffer<VertexStateOldElement> vertexStateOldElement = entityManager.AddBuffer<VertexStateOldElement>(entity);

            vertexStateCurrentElement = entityManager.GetBuffer<VertexStateCurrentElement>(entity);
	    vertexStateCurrentElement.ResizeUninitialized(mesh.vertices.Length);

            vertexStateOldElement = entityManager.GetBuffer<VertexStateOldElement>(entity);
	    vertexStateOldElement.ResizeUninitialized(mesh.vertices.Length);
	    for (int i=0,n=mesh.vertices.Length; i<n; ++i)
	    {
		vertexStateCurrentElement[i] = mesh.vertices[i];
		vertexStateOldElement[i] = mesh.vertices[i];
	    }

	    ClothSimEcsSystem.AddSharedComponents(entity, originalMesh, entityManager);
	    meshFilter.sharedMesh = originalMesh;
	    UnityEngine.Object.Destroy(gameObject);
        }
    }

#if FALSE
    // jiv fixme
    // - try out the subscene conversion workflow when it is more robust (or at least more obvious how to use robustly).
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
	    Mesh mesh = meshFilter.mesh = Instantiate(prefabMesh);

            DynamicBuffer<BarElement> bars = dstManager.AddBuffer<BarElement>(entity);
            bars.ResizeUninitialized(mesh.vertices.Length);
            DynamicBuffer<BarLengthElement> barLengths = dstManager.AddBuffer<BarLengthElement>(entity);
            barLengths.ResizeUninitialized(mesh.vertices.Length);
            DynamicBuffer<VertexStateCurrentElement> vertexStateCurrentElement = dstManager.AddBuffer<VertexStateCurrentElement>(entity);
            DynamicBuffer<VertexStateOldElement> vertexStateOldElement = dstManager.AddBuffer<VertexStateOldElement>(entity);

            vertexStateCurrentElement = dstManager.GetBuffer<VertexStateCurrentElement>(entity);
	    vertexStateCurrentElement.ResizeUninitialized(mesh.vertices.Length);

            vertexStateOldElement = dstManager.GetBuffer<VertexStateOldElement>(entity);
	    vertexStateOldElement.ResizeUninitialized(mesh.vertices.Length);
	    for (int i=0,n=mesh.vertices.Length; i<n; ++i)
	    {
		vertexStateCurrentElement[i] = mesh.vertices[i];
		vertexStateOldElement[i] = mesh.vertices[i];
	    }

	    ClothSimEcsSystem.AddSharedComponents(entity, prefabMesh, dstManager);
        }
    }
#endif
};