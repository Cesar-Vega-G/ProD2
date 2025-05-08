using UnityEngine;

public class TokenSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject tokenPrefab;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private Vector2 spawnArea = new Vector2(8f, 0.5f);
    [SerializeField] private int minValue = 1;
    [SerializeField] private int maxValue = 100;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnToken), 0f, spawnRate);
    }

    private void SpawnToken()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(-spawnArea.x, spawnArea.x),
            spawnArea.y
        );
        
        GameObject tokenObj = Instantiate(tokenPrefab, spawnPos, Quaternion.identity);
        Token token = tokenObj.GetComponent<Token>();
        token.Initialize(Random.Range(minValue, maxValue + 1));
    }

    // Debug: Dibuja el área de spawn en el editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(0, spawnArea.y, 0), new Vector3(spawnArea.x * 2, 0.1f, 0));
    }
}
