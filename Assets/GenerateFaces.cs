using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFaces : MonoBehaviour
{

    public bool showVertices;
    public Mesh mesh;
    public Material mat;
    public int resolution;
    public float height, radius;
    Vector3[] circleVertexArray;
    Vector3[] sideVertexArray;
    int trisPerVerts = 12;
    int[] circleTriangles;
    int[] sideTriangles;
    Vector2[] uvs;
    public bool showMesh;

    void Start()
    {
        showVertices = true;
        showMesh = true;
        //RebuildMesh();
    }

    public void RebuildMesh()
    {
        showMesh = true;
        mesh = new Mesh();

        resolution = Mathf.Clamp(resolution, 1, 400);

        circleVertexArray = new Vector3[2 * (1 + resolution)];
        sideVertexArray = new Vector3[2 * resolution];
        circleTriangles = new int[resolution * trisPerVerts];//fix number overflow (initialization of int can be a negative number, as is now)
        sideTriangles = new int[resolution * 6];
        uvs = new Vector2[circleVertexArray.Length];

        circleVertexArray[2 * (1 + resolution) - 1] = new Vector3(0, .5f * height, 0);
        circleVertexArray[2 * (1 + resolution) - 2] = new Vector3(0, -.5f * height, 0);

        for (int i = 0; i < resolution; i++)
        {
            //Debug.Log("About to create vertex(" + Mathf.Cos(i / resolution * 2 * Mathf.PI) * radius + "(Mathf.Cos(" + i + " / " + resolution + " * 2 * Mathf.PI) * " + radius + "), 0, " + Mathf.Sin(i / resolution * 2 * Mathf.PI) * radius + "(Mathf.Sin(" + i + " / " + resolution + " * 2 * Mathf.PI) * " + radius + ")).");
            float cornerAngle = (float)i / resolution * 2 * Mathf.PI;
            circleVertexArray[i] = new Vector3(Mathf.Cos(cornerAngle) * radius, .5f * height, Mathf.Sin(cornerAngle) * radius);
            sideVertexArray[i] = circleVertexArray[i];
            circleVertexArray[i + resolution] = new Vector3(Mathf.Cos(cornerAngle) * radius, -.5f * height, Mathf.Sin(cornerAngle) * radius);
            sideVertexArray[i + resolution] = circleVertexArray[i + resolution];
        }

        //Debug.Log("triangleCount: " + triangles.Length + ", vertexCount: " + vertexArray.Length + ", resolution: " + resolution + ".");

        for (int triIndex = 0, vertIndex = 0; triIndex < circleTriangles.Length && vertIndex < circleVertexArray.Length; triIndex += trisPerVerts, vertIndex++)
        {
            if (triIndex == (resolution - 1) * trisPerVerts)//if we're at the last vertex of the circle
            {
                //build a triangle on the top circle from the last vertex on that circle
                circleTriangles[triIndex] = vertIndex;
                circleTriangles[triIndex + 1] = 2 * (1 + resolution) - 1;
                circleTriangles[triIndex + 2] = 0;

                //build a triangle on the bottom circle from the last vertex on that circle
                circleTriangles[triIndex + 3] = vertIndex + resolution;
                circleTriangles[triIndex + 4] = resolution;
                circleTriangles[triIndex + 5] = 2 * (1 + resolution) - 2;

                //build quad on the side from the last vertices of the circles
                circleTriangles[triIndex + 6] = vertIndex;
                circleTriangles[triIndex + 7] = 0;
                circleTriangles[triIndex + 8] = resolution;
                circleTriangles[triIndex + 9] = vertIndex;
                circleTriangles[triIndex + 10] = resolution;
                circleTriangles[triIndex + 11] = vertIndex + resolution;
            }
            else
            {
                //build a triangle on the top circle
                circleTriangles[triIndex] = vertIndex;
                circleTriangles[triIndex + 1] = 2 * (1 + resolution) - 1;
                circleTriangles[triIndex + 2] = vertIndex + 1;

                //build a triangle on the bottom circle
                circleTriangles[triIndex + 3] = vertIndex + resolution;
                circleTriangles[triIndex + 4] = vertIndex + resolution + 1;
                circleTriangles[triIndex + 5] = 2 * (1 + resolution) - 2;

                //build quad on the side
                circleTriangles[triIndex + 6] = vertIndex;
                circleTriangles[triIndex + 7] = vertIndex + 1;
                circleTriangles[triIndex + 8] = vertIndex + resolution + 1;
                circleTriangles[triIndex + 9] = vertIndex;
                circleTriangles[triIndex + 10] = vertIndex + resolution + 1;
                circleTriangles[triIndex + 11] = vertIndex + resolution;
            }
        }

        //map uvs
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(circleVertexArray[i].x, circleVertexArray[i].z);
        }

        //uvs[x + (gridX * y)] = new Vector2((float)x / (gridX - 1), (float)y / (gridY - 1));

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

        mesh.vertices = circleVertexArray;
        mesh.uv = uvs;
        mesh.triangles = circleTriangles;

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public void Update()
    {
        RebuildMesh();
    }

    public void ClearMesh()
    {
        showMesh = false;
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
        if (showVertices)
        {
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                Gizmos.DrawSphere(mesh.vertices[i], .1f);
            }
        }
    }
}
