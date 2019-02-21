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
        //RebuildMesh();
    }

    public void RebuildMesh()
    {
        mesh = new Mesh();

        resolution = Mathf.Clamp(resolution, 3, 400);

        vertexArray = new Vector3[2 * (1 + resolution)];
        int[] triangles = new int[resolution * 4 * 3];//fix number overflow (initialization of int can be a negative number, as is now)

        vertexArray[2 * (1 + resolution) - 1] = new Vector3(0, .5f * height, 0);
        vertexArray[2 * (1 + resolution) - 2] = new Vector3(0, -.5f * height, 0);

        for (int i = 0; i < resolution; i++)
        {
            //Debug.Log("About to create vertex(" + Mathf.Cos(i / resolution * 2 * Mathf.PI) * radius + "(Mathf.Cos(" + i + " / " + resolution + " * 2 * Mathf.PI) * " + radius + "), 0, " + Mathf.Sin(i / resolution * 2 * Mathf.PI) * radius + "(Mathf.Sin(" + i + " / " + resolution + " * 2 * Mathf.PI) * " + radius + ")).");
            float cornerAngle = (float)i / resolution * 2 * Mathf.PI;
            vertexArray[i] = new Vector3(Mathf.Cos(cornerAngle) * radius, .5f * height, Mathf.Sin(cornerAngle) * radius);
            vertexArray[i + resolution] = new Vector3(Mathf.Cos(cornerAngle) * radius, -.5f * height, Mathf.Sin(cornerAngle) * radius);
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

    public void ClearMesh()
    {
        mesh = new Mesh
        {
            vertices = new Vector3[0],
            triangles = new int[0]
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDrawGizmos()//the ggizmo only displays the vertices of the local mesh variable and not 'GetComponent<MeshFilter>().mesh'
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Gizmos.DrawSphere(mesh.vertices[i], .2f);
        }
    }
}
