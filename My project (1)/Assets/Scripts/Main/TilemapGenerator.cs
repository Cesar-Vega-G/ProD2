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
    public Vector2Int spawnAreaMin = new Vector2Int(-20, 1); // Comenzamos en Y=1 para la primera capa
    public Vector2Int spawnAreaMax = new Vector2Int(20, 10);

    [Header("Layer Settings")]
    [SerializeField] private int platformsPerLayer = 3;  // Cuántas plataformas por capa antes de subir
    private int currentLayerY; // Nivel Y actual de la capa
    [SerializeField] private int minHorizontalSpacing = 1;
    private int platformsInCurrentLayer = 0; // Contador por capa

    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

    void Start()
    {
        GeneratePlatforms();
    }

    void GeneratePlatforms()
    {

        // Plataforma inicial en (-3, 0)
        PlacePlatform(mainTile, platform9x1, new Vector2Int(-3, 0));


        // Inicializar la primera capa en Y = spawnAreaMin.y
        currentLayerY = spawnAreaMin.y;
        platformsInCurrentLayer = 0;

        // Genera las plataformas en capas
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

            // Contamos plataformas en la capa actual
            platformsInCurrentLayer++;

            if (platformsInCurrentLayer >= platformsPerLayer)
            {

                currentLayerY += 3; // Sube 3 unidades en Y (a la siguiente capa)

                platformsInCurrentLayer = 0; // Reinicio del contador para la nueva capa
            }
        }
    }

    Vector2Int GetValidPosition(Vector2Int[] platformTemplate)
    {
        int attempts = 0;
        while (attempts < 100)
        {
            // Generamos X aleatorio dentro del rango horizontal
            int xPos = Random.Range(spawnAreaMin.x, spawnAreaMax.x - GetPlatformWidth(platformTemplate) + 1);

            // Y es fijo
            int yPos = currentLayerY;

            Vector2Int origin = new Vector2Int(xPos, yPos);

            if (IsAreaClear(origin, platformTemplate))
            {
                return origin;
            }
            attempts++;
        }
        Debug.LogWarning($"No se encontró posición válida en la capa Y={currentLayerY}, se usará (0,{currentLayerY}).");
        return new Vector2Int(0, currentLayerY); // fallback seguro
    }



    bool IsAreaClear(Vector2Int origin, Vector2Int[] platformTemplate)
    {
        foreach (Vector2Int offset in platformTemplate)
        {
            Vector2Int tilePos = origin + offset;

            // Revisamos la propia celda
            if (occupiedPositions.Contains(tilePos) ||
                tilePos.x < spawnAreaMin.x || tilePos.x > spawnAreaMax.x ||
                tilePos.y < spawnAreaMin.y || tilePos.y > spawnAreaMax.y)
            {
                return false;
            }

            // Revisamos celdas adyacentes horizontales, según minHorizontalSpacing
            for (int i = -minHorizontalSpacing; i <= minHorizontalSpacing; i++)
            {
                if (i == 0) continue; // Saltamos la celda actual
                Vector2Int checkPos = new Vector2Int(tilePos.x + i, tilePos.y);
                if (occupiedPositions.Contains(checkPos))
                {
                    return false;
                }
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

