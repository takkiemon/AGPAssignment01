using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFaces : MonoBehaviour {

    public Mesh mesh;
    public int resolution;
    public float height, radius;
    Vector3[] vertexArray;

    void Start()
    {
        RebuildMesh();
    }

    public void RebuildMesh()
    {
        mesh = new Mesh();

        if (resolution < 3)
        {
            resolution = 3;
        }
        if (resolution > 10)
        {
            resolution = 10;
        }

        vertexArray = new Vector3[2 * (1 + resolution)];
        int[] triangles = new int[resolution * 4 * 3];//fix number overflow (initialization of int can be a negative number, as is now)

        vertexArray[0] = new Vector3(0, 0, 0);
        vertexArray[1] = new Vector3(0, height, 0);

        for(int i = 0; i < resolution; i++)
        {
            Debug.Log("about to create Vertex[" + i + "]: Vector3(" + Mathf.Cos(i) * resolution + ", 0, " + Mathf.Sin(i) * resolution + ").");
            vertexArray[i] = new Vector3(Mathf.Cos(i) * resolution, 0, Mathf.Sin(i) * resolution);//new vector3(cosine(resolution), sine(resolution))
        }

        /*
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
        */

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
