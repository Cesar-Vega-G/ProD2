using UnityEngine;

public class TokenSpawner : MonoBehaviour
{
    public GameObject tokenPrefab;
    public float tiempoEntreTokens = 2f;
    public float rangoX = 8f;
    public float alturaSpawn = 6f;

    private float tiempoActual;

    void Update()
    {
        tiempoActual += Time.deltaTime;

        if (tiempoActual >= tiempoEntreTokens)
        {
            GenerarToken();
            tiempoActual = 0f;
        }
    }

    void GenerarToken()
    {
        float x = Random.Range(-rangoX, rangoX);
        Vector3 posicion = new Vector3(x, alturaSpawn, 0);

        GameObject nuevoToken = Instantiate(tokenPrefab, posicion, Quaternion.identity);

        int numero = Random.Range(1, 100); // o el rango que quieras
        Token scriptToken = nuevoToken.GetComponent<Token>();
        scriptToken.AsignarNumero(numero);
    }
}
