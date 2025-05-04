using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapPlatformManager : MonoBehaviour
{
    [Header("Tilemap Config")]
    public Tilemap tilemap;
    public TileBase groundTile;
    public TileBase mainTile;

    [Header("Platform Templates")]
    public Vector2Int[] platform3x1 = { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
    public Vector2Int[] platform6x1 = { /* ... */ };
    public Vector2Int[] platform9x1 = { /* ... */ };

    [Header("Platform Counts")]
    public int count3x1 = 3;
    public int count6x1 = 2;
    public int count9x1 = 1;

    [Header("Spawn Area")]
    public Vector2Int spawnAreaMin = new Vector2Int(-20, -5);
    public Vector2Int spawnAreaMax = new Vector2Int(20, 5);

    [Header("Vertical Spacing")]
    [SerializeField] private int minVerticalSpacing = 3; // Mínimo espacio entre plataformas en Y

    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
    private List<int> usedYLevels = new List<int>(); // Para controlar niveles Y ocupados

    void Start()
    {
        GeneratePlatforms();
    }

    void GeneratePlatforms()
    {
        // Plataforma principal (opcional)
        // PlacePlatform(mainTile, platform9x1, Vector2Int.zero);

        // Generar plataformas con spacing vertical
        SpawnPlatforms(platform3x1, count3x1);
        SpawnPlatforms(platform6x1, count6x1);
        SpawnPlatforms(platform9x1, count9x1);
    }

    void SpawnPlatforms(Vector2Int[] platformTemplate, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int origin = GetValidPosition(platformTemplate);
            PlacePlatform(groundTile, platformTemplate, origin);
        }
    }

    Vector2Int GetValidPosition(Vector2Int[] platformTemplate)
    {
        int attempts = 0;
        while (attempts < 100)
        {
            // 1. Elegir un Y que respete el spacing vertical
            int yPos = GetValidYLevel();

            // 2. Generar X aleatoria
            int xPos = Random.Range(spawnAreaMin.x, spawnAreaMax.x - GetPlatformWidth(platformTemplate));

            Vector2Int origin = new Vector2Int(xPos, yPos);

            if (IsAreaClear(origin, platformTemplate))
            {
                usedYLevels.Add(yPos); // Registrar nivel Y usado
                return origin;
            }
            attempts++;
        }
        Debug.LogWarning("No se encontró posición válida.");
        return Vector2Int.zero;
    }

    int GetValidYLevel()
    {
        int yPos;
        bool validY;
        int attempts = 0;

        do
        {
            validY = true;
            yPos = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

            // Verificar spacing con otros niveles Y
            foreach (int usedY in usedYLevels)
            {
                if (Mathf.Abs(yPos - usedY) < minVerticalSpacing)
                {
                    validY = false;
                    break;
                }
            }
            attempts++;
        } while (!validY && attempts < 50);

        return validY ? yPos : 0; // Fallback
    }

    bool IsAreaClear(Vector2Int origin, Vector2Int[] platformTemplate)
    {
        foreach (Vector2Int offset in platformTemplate)
        {
            Vector2Int tilePos = origin + offset;
            if (occupiedPositions.Contains(tilePos) ||
                tilePos.x < spawnAreaMin.x || tilePos.x > spawnAreaMax.x ||
                tilePos.y < spawnAreaMin.y || tilePos.y > spawnAreaMax.y)
            {
                return false;
            }
        }
        return true;
    }

    int GetPlatformWidth(Vector2Int[] platformTemplate)
    {
        int maxX = 0;
        foreach (Vector2Int pos in platformTemplate)
        {
            if (pos.x > maxX) maxX = pos.x;
        }
        return maxX + 1; // Ancho en celdas
    }

    void PlacePlatform(TileBase tile, Vector2Int[] platformTemplate, Vector2Int origin)
    {
        foreach (Vector2Int offset in platformTemplate)
        {
            Vector3Int tilePos = new Vector3Int(origin.x + offset.x, origin.y + offset.y, 0);
            tilemap.SetTile(tilePos, tile);
            occupiedPositions.Add(new Vector2Int(tilePos.x, tilePos.y));
        }
    }
}
