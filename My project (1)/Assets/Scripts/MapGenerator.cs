using UnityEngine;
using System.Collections.Generic;

public class PlatformGenerator : MonoBehaviour
{
    [Header("Tipos de Plataformas")]
    public List<PlatformType> platformTypes; // Configura en el Inspector.

    [Header("Configuraci�n de Spawn")]
    public Vector2 spawnAreaMin = new Vector2(-10, -5);
    public Vector2 spawnAreaMax = new Vector2(10, 5);
    public float minDistanceBetweenPlatforms = 3f; // Distancia m�nima entre plataformas.

    void Start()
    {
        GeneratePlatforms();
    }

    void GeneratePlatforms()
    {
        // 1. Generar la plataforma PRINCIPAL en (0, 0)
        GenerateMainPlatform();

        // 2. Generar el resto de plataformas aleatorias
        foreach (PlatformType type in platformTypes)
        {
            for (int i = 0; i < type.quantity; i++)
            {
                Vector2 randomPosition = GetValidRandomPosition();
                GeneratePlatform(type.prefab, randomPosition);
            }
        }
    }

    void GenerateMainPlatform()
    {
        // Busca el tipo marcado como "isMainPlatform" y lo genera en (0, 0).
        foreach (PlatformType type in platformTypes)
        {
            if (type.isMainPlatform)
            {
                GeneratePlatform(type.prefab, Vector2.zero);
                break;
            }
        }
    }

    void GeneratePlatform(GameObject prefab, Vector2 position)
    {
        Instantiate(prefab, position, Quaternion.identity);
    }

    Vector2 GetValidRandomPosition()
    {
        Vector2 randomPosition;
        bool positionIsValid;
        int attempts = 0;
        int maxAttempts = 100;

        do
        {
            // Genera una posici�n aleatoria dentro del �rea.
            randomPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            // Verifica que no est� demasiado cerca de otras plataformas.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(randomPosition, minDistanceBetweenPlatforms);
            positionIsValid = colliders.Length == 0;

            attempts++;
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("No se encontr� posici�n v�lida despu�s de " + maxAttempts + " intentos.");
                break;
            }
        } while (!positionIsValid);

        return randomPosition;
    }
}









