using UnityEngine;

public class QuantumTrail : MonoBehaviour
{
    [Tooltip("İzin sahnede ne kadar süre kalacağı")]
    public float lifetime = 4f;

    void Start()
    {
        // Maraton kurtarıcısı: Obje doğduktan 'lifetime' saniye sonra kendini otomatik yok eder
        Destroy(gameObject, lifetime);
    }
    
    // Editörde görünmez izi yeşil bir küre olarak belirtelim
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.4f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}