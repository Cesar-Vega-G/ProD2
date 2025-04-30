using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;         // El Tilemap donde se colocarán los tiles
    public Tile platformTile;       // El tile que representa las plataformas
    public int mapWidth = 20;       // Ancho del mapa
    public int mapHeight = 10;      // Alto del mapa
    public int platformCount = 5;   // Cantidad de plataformas a generar

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Limpiar el Tilemap antes de generar el mapa
        tilemap.ClearAllTiles();

        // Generar plataformas aleatorias
        for (int i = 0; i < platformCount; i++)
        {
            GenerateRandomPlatform();
        }
    }

    void GenerateRandomPlatform()
    {
        // Generar una posición aleatoria dentro de los límites del mapa
        int xPosition = Random.Range(0, mapWidth);
        int yPosition = Random.Range(0, mapHeight);

        // Convertir las coordenadas de mundo a celdas del Tilemap (ajustar al grid)
        Vector3Int cellPosition = tilemap.WorldToCell(new Vector3(xPosition, yPosition, 0));

        // Crear una plataforma en la posición generada (alineada a la cuadrícula)
        for (int x = cellPosition.x; x < cellPosition.x + 3; x++) // Plataforma de 3 tiles de ancho
        {
            if (x >= mapWidth) break;  // Evitar que la plataforma se salga del mapa
            tilemap.SetTile(new Vector3Int(x, cellPosition.y, 0), platformTile);
        }
    }
}



