using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFaces : MonoBehaviour {

    public Mesh mesh;
    public int vertexGridXSize, vertexGridYSize;
    Vector3[] vertexArray;

    void Start()
    {
        RebuildMesh();
    }

    public void RebuildMesh()
    {
        mesh = new Mesh();

        vertexArray = new Vector3[vertexGridXSize * vertexGridYSize];
        int[] triangles = new int[(vertexGridXSize - 1) * (vertexGridYSize - 1) * 6];//fix number overflow (initialization of int can be a negative number, as is now)

        for (int y = 0; y < vertexGridYSize; y++)
        {
            for (int x = 0; x < vertexGridXSize; x++)
            {
                vertexArray[y * vertexGridXSize + x] = new Vector3(x, y, 0);

            }
        }

        for (int triIterator = 0, vertIterator = 0; vertIterator < (vertexGridXSize * (vertexGridYSize - 1) - 1); vertIterator++)
        {
            Debug.Log("t: " + triIterator + ", v: " + vertIterator + ". ");
            if ((vertIterator + 1) % vertexGridXSize != 0)
            {
                triangles[triIterator] = vertIterator + 1;
                triangles[triIterator + 1] = vertIterator + vertexGridXSize;
                triangles[triIterator + 2] = vertIterator;
                triangles[triIterator + 3] = vertIterator + 1;
                triangles[triIterator + 4] = vertIterator + vertexGridXSize + 1;
                triangles[triIterator + 5] = vertIterator + vertexGridXSize;
                triIterator += 6;
            }
        }

        mesh.vertices = vertexArray;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        mesh = GetComponent<MeshFilter>().mesh;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Gizmos.DrawSphere(mesh.vertices[i], .2f);
        }
    }
}
