using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject[] platformPrefabs;  // Los prefabs de las plataformas
    public int[] platformCounts;          // Array que define cuántas plataformas generar de cada tipo
    public float minDistanceX = 3f;      // Distancia mínima entre plataformas en X
    public float maxDistanceX = 10f;     // Distancia máxima entre plataformas en X
    public float minHeight = 2f;         // Altura mínima de las plataformas
    public float maxHeight = 6f;         // Altura máxima de las plataformas
    public float mapWidth = 20f;         // Ancho del mapa (en unidades)
    public float mapHeight = 10f;        // Altura del mapa (en unidades)
    public LayerMask platformLayer;      // Layer donde están las plataformas para colisión

    private float lastPlatformX = 0f;    // La posición X de la última plataforma generada
    private float lastPlatformY = 0f;    // La posición Y de la última plataforma generada

    void Start()
    {
        GeneratePlatforms();
    }

    void GeneratePlatforms()
    {
        // La primera plataforma siempre se genera en (0, 0)
        lastPlatformX = 0f;  // Posición X de la primera plataforma
        lastPlatformY = 0f;  // Posición Y de la primera plataforma
        InstantiateFirstPlatform();

        // Generamos las plataformas adicionales
        for (int i = 0; i < platformPrefabs.Length; i++)
        {
            for (int j = 0; j < platformCounts[i]; j++)
            {
                GeneratePlatform(i, j);
            }
        }
    }

    // Método para instanciar la primera plataforma en (0, 0)
    void InstantiateFirstPlatform()
    {
        Vector3 platformPosition = new Vector3(lastPlatformX, lastPlatformY, 0);
        Instantiate(platformPrefabs[0], platformPosition, Quaternion.identity);
    }

    // Método para generar plataformas adicionales
    void GeneratePlatform(int prefabIndex, int platformIndex)
    {
        // Generar una posición X aleatoria, asegurándonos de que la distancia mínima se mantenga
        float randomX = lastPlatformX + Random.Range(minDistanceX, maxDistanceX);

        // Asegurarse de que la plataforma no salga fuera del límite del mapa
        randomX = Mathf.Clamp(randomX, -mapWidth / 2f, mapWidth / 2f);

        // Generar una posición Y aleatoria dentro del rango de alturas
        float randomY = lastPlatformY + Random.Range(minHeight, maxHeight);

        // Asegurarse de que la plataforma no salga fuera del límite del mapa en Y
        randomY = Mathf.Clamp(randomY, -mapHeight / 2f, mapHeight / 2f);

        // Verificar si la nueva plataforma solapa con otra existente
        Vector2 platformSize = platformPrefabs[prefabIndex].GetComponent<Collider2D>().bounds.size; // Obtener el tamaño del Collider2D de la plataforma
        Vector2 platformPosition = new Vector2(randomX, randomY);

        // Usamos Physics2D.OverlapBox para verificar si la nueva plataforma colisiona
        if (!Physics2D.OverlapBox(platformPosition, platformSize, 0f, platformLayer))
        {
            // Si no hay colisión, instanciamos la plataforma
            Instantiate(platformPrefabs[prefabIndex], platformPosition, Quaternion.identity);

            // Actualizar la posición X y Y de la última plataforma generada
            lastPlatformX = randomX;
            lastPlatformY = randomY;
        }
        else
        {
            // Si hay colisión, intentamos generar una nueva plataforma
            platformIndex--; // Repetir este ciclo sin avanzar, hasta encontrar una posición válida
        }
    }
}









