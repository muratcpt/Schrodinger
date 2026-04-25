using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Kameralar")]
    [Tooltip("Sadece GhostLayer ve Shared gösterir")]
    public Camera quantumCamera; 
    [Tooltip("Sadece RealLayer ve Shared gösterir")]
    public Camera realWorldCamera; 

    [Header("Takip Ayarları")]
    [Tooltip("Kameraların takip edeceği Ruh (Ghost) karakteri")]
    public Transform target; 
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 10f, -10f);

    void Start()
    {
        SetupSplitScreen();
    }

    private void SetupSplitScreen()
    {
        if (quantumCamera != null && realWorldCamera != null)
        {
            // Ekranı dikey olarak ikiye bölüyoruz
            quantumCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
            realWorldCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        }
        else
        {
            Debug.LogError("SplitScreenManager: Kameralar atanmamış!");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Kameralar birlikte hareket eder, böylece sağ ekranda (gerçek dünyada) olan değişiklikler de oyuncu tarafından görülebilir.
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}