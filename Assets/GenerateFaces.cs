using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFaces : MonoBehaviour {

    public bool showVertices;
    public Mesh mesh;
    public Material mat;
    public int circleSubdivisions;
    public float height, radius;
    Vector3[] vertexArray;
    int trisPerVerts = 12;
    int[] triangles;
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

        circleSubdivisions = Mathf.Clamp(circleSubdivisions, 1, 400);

        vertexArray = new Vector3[2 * (1 + circleSubdivisions)];
        triangles = new int[circleSubdivisions * trisPerVerts];//fix number overflow (initialization of int can be a negative number, as is now)
        uvs = new Vector2[vertexArray.Length];

        vertexArray[2 * (1 + circleSubdivisions) - 1] = new Vector3(0, .5f * height, 0);
        vertexArray[2 * (1 + circleSubdivisions) - 2] = new Vector3(0, -.5f * height, 0);

        for (int i = 0; i < circleSubdivisions; i++)
        {
            //Debug.Log("About to create vertex(" + Mathf.Cos(i / resolution * 2 * Mathf.PI) * radius + "(Mathf.Cos(" + i + " / " + resolution + " * 2 * Mathf.PI) * " + radius + "), 0, " + Mathf.Sin(i / resolution * 2 * Mathf.PI) * radius + "(Mathf.Sin(" + i + " / " + resolution + " * 2 * Mathf.PI) * " + radius + ")).");
            float cornerAngle = (float)i / circleSubdivisions * 2 * Mathf.PI;
            vertexArray[i] = new Vector3(Mathf.Cos(cornerAngle) * radius, .5f * height, Mathf.Sin(cornerAngle) * radius);
            vertexArray[i + circleSubdivisions] = new Vector3(Mathf.Cos(cornerAngle) * radius, -.5f * height, Mathf.Sin(cornerAngle) * radius);
        }

        //Debug.Log("triangleCount: " + triangles.Length + ", vertexCount: " + vertexArray.Length + ", resolution: " + resolution + ".");

        for (int triIndex = 0, vertIndex = 0; triIndex < triangles.Length && vertIndex < vertexArray.Length; triIndex += trisPerVerts, vertIndex++)
        {
            if (triIndex == (circleSubdivisions - 1) * trisPerVerts)//if we're at the last vertex of the circle
            {
                //build a triangle on the top circle from the last vertex on that circle
                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = 2 * (1 + circleSubdivisions) - 1;
                triangles[triIndex + 2] = 0;

                //build a triangle on the bottom circle from the last vertex on that circle
                triangles[triIndex + 3] = vertIndex + circleSubdivisions;
                triangles[triIndex + 4] = circleSubdivisions;
                triangles[triIndex + 5] = 2 * (1 + circleSubdivisions) - 2;

                //build quad on the side from the last vertices of the circles
                triangles[triIndex + 6] = vertIndex;
                triangles[triIndex + 7] = 0;
                triangles[triIndex + 8] = circleSubdivisions;
                triangles[triIndex + 9] = vertIndex;
                triangles[triIndex + 10] = circleSubdivisions;
                triangles[triIndex + 11] = vertIndex + circleSubdivisions;
            }
            else
            {
                //build a triangle on the top circle
                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = 2 * (1 + circleSubdivisions) - 1;
                triangles[triIndex + 2] = vertIndex + 1;

                //build a triangle on the bottom circle
                triangles[triIndex + 3] = vertIndex + circleSubdivisions;
                triangles[triIndex + 4] = vertIndex + circleSubdivisions + 1;
                triangles[triIndex + 5] = 2 * (1 + circleSubdivisions) - 2;

                //build quad on the side
                triangles[triIndex + 6] = vertIndex;
                triangles[triIndex + 7] = vertIndex + 1;
                triangles[triIndex + 8] = vertIndex + circleSubdivisions + 1;
                triangles[triIndex + 9] = vertIndex;
                triangles[triIndex + 10] = vertIndex + circleSubdivisions + 1;
                triangles[triIndex + 11] = vertIndex + circleSubdivisions;
            }
        }

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertexArray[i].x, vertexArray[i].z);
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

        mesh.vertices = vertexArray;
        mesh.uv = uvs;
        mesh.triangles = triangles;

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
