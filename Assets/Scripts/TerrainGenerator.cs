using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//looked at this tutorial to try and get color to work https://www.youtube.com/watch?v=lNyZ9K71Vhc

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 10;
    public int zSize = 10;

    public Gradient heightMap;

    public float minTerrainHeight;
    public float maxTerrainHeight;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
        
    }

    private void Update()
    {
        UpdateMesh();
    }

    IEnumerator CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z* .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);
                

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

                i++;
            }
        }


        triangles = new int[xSize * zSize * 6];
        int verts = 0;
        int tris = 0;


        for (int z = 0; z <= zSize; z++)
        {

            for (int x = 0; x < xSize; x++)
            {

                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + xSize + 1;
                triangles[tris + 2] = verts + 1;
                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + xSize + 1;
                triangles[tris + 5] = verts + xSize + 2;

                verts++;
                tris += 6;

                yield return new WaitForSeconds(0);
            }
            verts++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = heightMap.Evaluate(height);
                i++;
                //colors[i] = Color.Lerp(Color.red, Color.green, vertices[i].y);


            }
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        
        for (int i = 0; i < vertices.Length; i++)
        {
           // Gizmos.DrawSphere(vertices[i], .1f);
        }
    }


}
