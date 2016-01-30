using System;
using BEPUphysics;
using BEPUphysics.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xi
{
    /// <summary>
    /// Provides helper methods for XNA model operations.
    /// </summary>
    public static class ModelHelper
    {
        /// <summary>
        /// Get the vertices and indices from a model's given meshes.
        /// </summary>
        public static void GetVerticesAndIndices(
            this Model model,
            out Vector3[] vertices,
            out int[] indices)
        {
            XiHelper.ArgumentNullCheck(model);
            TriangleMesh.GetVerticesAndIndicesFromModel(model, out vertices, out indices);
        }

        /// <summary>
        /// Get the vertices and indices from a model's given meshes.
        /// </summary>
        public static void GetVerticesAndIndices(
            this Model model,
            out StaticTriangleGroup.StaticTriangleGroupVertex[] vertices,
            out int[] indices)
        {
            XiHelper.ArgumentNullCheck(model);
            Vector3[] vectorArray;
            model.GetVerticesAndIndices(out vectorArray, out indices);
            vertices = new StaticTriangleGroup.StaticTriangleGroupVertex[vectorArray.Length];
            for (int i = 0; i < vectorArray.Length; i++)
                vertices[i] = new StaticTriangleGroup.StaticTriangleGroupVertex(vectorArray[i]);
        }

        /// <summary>
        /// Generate the current bounding box of a model.
        /// </summary>
        public static BoundingBox GenerateBoundingBox(this ModelMeshPart part, ModelMesh parentMesh)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);
            int stride = part.VertexStride;
            int vertexCount = part.NumVertices;
            byte[] vertexData = new byte[stride * vertexCount]; // MEMORYCHURN
            parentMesh.VertexBuffer.GetData(vertexData);
            for (int index = 0; index < vertexData.Length; index += stride)
            {
                float x = BitConverter.ToSingle(vertexData, index);
                float y = BitConverter.ToSingle(vertexData, index + 4);
                float z = BitConverter.ToSingle(vertexData, index + 8);
                if (x < min.X) min.X = x;
                if (x > max.X) max.X = x;
                if (y < min.Y) min.Y = y;
                if (y > max.Y) max.Y = y;
                if (z < min.Z) min.Z = z;
                if (z > max.Z) max.Z = z;
            }
            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Generate the current bounding box of a model.
        /// </summary>
        public static BoundingBox GenerateBoundingBox(this Model model)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);
            Matrix[] bonesAbsolute = new Matrix[model.Bones.Count]; // MEMORYCHURN
            model.CopyAbsoluteBoneTransformsTo(bonesAbsolute);
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix boneAbsolute = bonesAbsolute[mesh.ParentBone.Index];
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    int stride = part.VertexStride;
                    int vertexCount = part.NumVertices;
                    byte[] vertexData = new byte[stride * vertexCount]; // MEMORYCHURN
                    mesh.VertexBuffer.GetData(vertexData);
                    for (int index = 0; index < vertexData.Length; index += stride)
                    {
                        float x = BitConverter.ToSingle(vertexData, index);
                        float y = BitConverter.ToSingle(vertexData, index + 4);
                        float z = BitConverter.ToSingle(vertexData, index + 8);
                        Vector3 vertex = new Vector3(x, y, z);
                        Vector3 vertexWorld = Vector3.Transform(vertex, boneAbsolute);
                        if (vertexWorld.X < min.X) min.X = vertexWorld.X;
                        if (vertexWorld.X > max.X) max.X = vertexWorld.X;
                        if (vertexWorld.Y < min.Y) min.Y = vertexWorld.Y;
                        if (vertexWorld.Y > max.Y) max.Y = vertexWorld.Y;
                        if (vertexWorld.Z < min.Z) min.Z = vertexWorld.Z;
                        if (vertexWorld.Z > max.Z) max.Z = vertexWorld.Z;
                    }
                }
            }
            return new BoundingBox(min, max);
        }
    }
}
