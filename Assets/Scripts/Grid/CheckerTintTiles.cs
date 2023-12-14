using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerTintTiles : MonoBehaviour
{
    public int rows = 8;
    public int columns = 8;
    public float tileSize = 1.0f;
    public float transparency = 0.5f; // Adjust this value for transparency
    public Transform checkeredboardTint; // Reference to the CheckeredboardTint object -9.8,-19.5, -5.5

    void Start()
    {
        GenerateCheckerboard();
    }

    void GenerateCheckerboard()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component not found on the GameObject.");
            return;
        }

        Material material = new Material(Shader.Find("Standard"));
        material.SetFloat("_Mode", 2); // Set rendering mode to Fade
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject tile = new GameObject("Tile");
                tile.transform.parent = transform;

                MeshFilter meshFilter = tile.AddComponent<MeshFilter>();
                MeshRenderer tileRenderer = tile.AddComponent<MeshRenderer>();

                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(col * tileSize, 0, row * tileSize) + checkeredboardTint.position;
                vertices[1] = new Vector3((col + 1) * tileSize, 0, row * tileSize) + checkeredboardTint.position;
                vertices[2] = new Vector3(col * tileSize, 0, (row + 1) * tileSize) + checkeredboardTint.position;
                vertices[3] = new Vector3((col + 1) * tileSize, 0, (row + 1) * tileSize) + checkeredboardTint.position;

                mesh.vertices = vertices;

                int[] triangles = { 0, 2, 1, 1, 2, 3 };
                mesh.triangles = triangles;

                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(0, 0);
                uv[1] = new Vector2(1, 0);
                uv[2] = new Vector2(0, 1);
                uv[3] = new Vector2(1, 1);

                mesh.uv = uv;

                meshFilter.mesh = mesh;

                // Set the material based on whether the sum of row and column is even or odd
                tileRenderer.material = material;
                Color tileColor = ((row + col) % 2 == 0) ? Color.white : Color.black;
                tileColor.a = transparency; // Set the alpha value for transparency
                tileRenderer.material.color = tileColor;

                tile.transform.parent = transform;
            }
        }
    }





    /*public GameObject gridManager; // Reference to your existing grid manager
    public GameObject checkerboardTilePrefab;
    public int gridSize = 6;
    public float tileSize = 1.0f;
    public Color evenTileColor = Color.white;
    public Color oddTileColor = new Color(0.9f, 0.9f, 0.9f); // Subtle tint color

    void Start()
    {
        GenerateCheckerboardGrid();
    }

    void GenerateCheckerboardGrid()
    {
        Vector3 gridManagerPosition = gridManager.transform.position;

        GameObject checkerboardGrid = new GameObject("CheckerboardGrid");
        checkerboardGrid.transform.parent = gridManager.transform;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                GameObject tile = Instantiate(checkerboardTilePrefab);
                tile.transform.parent = checkerboardGrid.transform;

                float xPos = x * tileSize;
                float zPos = z * tileSize;

                // Ensure the correct positioning relative to the gridManager
                tile.transform.position = new Vector3(gridManagerPosition.x + xPos, gridManagerPosition.y, gridManagerPosition.z + zPos);

                SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();

                // Set the material color based on whether the tile is even or odd
                if ((x + z) % 2 == 0)
                {
                    spriteRenderer.color = evenTileColor;
                }
                else
                {
                    spriteRenderer.color = oddTileColor;
                }
            }
        }
    }*/
}