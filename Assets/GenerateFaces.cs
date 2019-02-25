using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateFaces : MonoBehaviour
{
    public Mesh mesh;
    public Material mat;
    public int resolution;
    public float height, radius;
    public bool showVertices;
    public bool showMesh;
    public bool showCircleVertices;
    public bool showSideVertices;
    public bool showVertexNumbers;

    Vector3[] circleVertexArray;
    Vector3[] sideVertexArray;
    int[] circleTriangles;
    int[] sideTriangles;
    public Vector2[] uvs;
    public Vector3[] totalVertices;
    public int[] totalTriangles;

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
        circleTriangles = new int[resolution * 6];//fix number overflow (initialization of int can be a negative number, as is now)
        sideTriangles = new int[resolution * 6];
        uvs = new Vector2[circleVertexArray.Length + sideVertexArray.Length];

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

        for (int triIndex = 0, vertIndex = 0; triIndex < circleTriangles.Length && vertIndex < circleVertexArray.Length; triIndex += 6, vertIndex++)
        {
            if (triIndex == (resolution - 1) * 6)//if we're at the last vertex of the circle
            {
                /*/build a triangle on the top circle from the last vertex on that circle
                circleTriangles[triIndex] = vertIndex;
                circleTriangles[triIndex + 1] = 2 * (1 + resolution) - 1;
                circleTriangles[triIndex + 2] = 0;

                //build a triangle on the bottom circle from the last vertex on that circle
                circleTriangles[triIndex + 3] = vertIndex + resolution;
                circleTriangles[triIndex + 4] = resolution;
                circleTriangles[triIndex + 5] = 2 * (1 + resolution) - 2;
                */
                /*
                //build quad on the side from the last vertices of the circles
                circleTriangles[triIndex + 6] = vertIndex;
                circleTriangles[triIndex + 7] = 0;
                circleTriangles[triIndex + 8] = resolution;
                circleTriangles[triIndex + 9] = vertIndex;
                circleTriangles[triIndex + 10] = resolution;
                circleTriangles[triIndex + 11] = vertIndex + resolution;
                */
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

                /*
                //build quad on the side
                circleTriangles[triIndex + 6] = vertIndex;
                circleTriangles[triIndex + 7] = vertIndex + 1;
                circleTriangles[triIndex + 8] = vertIndex + resolution + 1;
                circleTriangles[triIndex + 9] = vertIndex;
                circleTriangles[triIndex + 10] = vertIndex + resolution + 1;
                circleTriangles[triIndex + 11] = vertIndex + resolution;
                */
            }
        }


        for (int triIndex = 0, vertIndex = 0; triIndex < sideTriangles.Length && vertIndex < sideVertexArray.Length; triIndex += 6, vertIndex++)
        {
            if (triIndex == (resolution - 1) * 6)//if we're at the last vertex of the circle
            {
                //build quad on the side from the last vertices of the circles
                sideTriangles[triIndex] = vertIndex;
                sideTriangles[triIndex + 1] = 0;
                sideTriangles[triIndex + 2] = resolution;
                sideTriangles[triIndex + 3] = vertIndex;
                sideTriangles[triIndex + 4] = resolution;
                sideTriangles[triIndex + 5] = vertIndex + resolution;
            }
            else
            {
                //build quad on the side
                sideTriangles[triIndex] = vertIndex;
                sideTriangles[triIndex + 1] = vertIndex + 1;
                sideTriangles[triIndex + 2] = vertIndex + resolution + 1;
                sideTriangles[triIndex + 3] = vertIndex;
                sideTriangles[triIndex + 4] = vertIndex + resolution + 1;
                sideTriangles[triIndex + 5] = vertIndex + resolution;
            }
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

        totalVertices = new Vector3[circleVertexArray.Length + sideVertexArray.Length];
        totalTriangles = new int[circleTriangles.Length + sideTriangles.Length];
        for (int i = 0; i < circleVertexArray.Length; i++)
        {
            totalVertices[i] = circleVertexArray[i];
        }
        for (int i = 0; i < sideVertexArray.Length; i++)
        {
            totalVertices[i + circleVertexArray.Length] = sideVertexArray[i];
        }

        for (int i = 0; i < circleTriangles.Length; i++)
        {
            totalTriangles[i] = circleTriangles[i];
        }
        for (int i = 0; i < sideTriangles.Length; i++)
        {
            totalTriangles[i + circleTriangles.Length] = sideTriangles[i];
        }

        mesh.vertices = totalVertices;
        mesh.triangles = totalTriangles;

        mesh.RecalculateNormals();

        float uvPerResolution = 1 / resolution;

        //map uvs
        for (int i = 0; i < circleVertexArray.Length; i++)
        {
            uvs[i] = new Vector2(circleVertexArray[i].x, circleVertexArray[i].z).normalized;
        }
        for (int i = 0; i < sideVertexArray.Length; i += 2)
        {
            uvs[i + circleVertexArray.Length] = new Vector2(0, uvPerResolution * i);
            uvs[i + circleVertexArray.Length + 1] = new Vector2(1, uvPerResolution * i);
        }

        mesh.uv = uvs;

        GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public void Update()
    {
        if (showMesh)
        {
            RebuildMesh();
        }
        else
        {
            ClearMesh();
        }
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

    private void OnDrawGizmos()//the gizmo only displays the vertices of the local mesh variable and not 'GetComponent<MeshFilter>().mesh'
    {
        Gizmos.color = Color.yellow;
        if (showVertices)
        {
            if (showVertexNumbers)
            {
                for (int i = 0; i < totalVertices.Length; i++)
                {
                    Handles.Label(totalVertices[i], i.ToString());
                }
            }
            if (showCircleVertices)
            {
                for (int i = 0; i < circleVertexArray.Length; i++)
                {
                    Handles.Label(circleVertexArray[i], uvs[i].ToString());
                    Gizmos.DrawSphere(circleVertexArray[i], .1f);
                }
            }
            if (showSideVertices)
            {
                for (int i = 0; i < sideVertexArray.Length; i++)
                {
                    Handles.Label(sideVertexArray[i], uvs[i + circleVertexArray.Length].ToString());
                    Gizmos.DrawSphere(sideVertexArray[i], .1f);
                }
            }
        }
    }
}
