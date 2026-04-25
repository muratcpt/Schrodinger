using UnityEngine;
using UnityEngine.InputSystem;

public class QuantumJumper : MonoBehaviour
{
    [Header("Kuantum Sıçrama Ayarları")]
    [Tooltip("Sağ ekranda duran gerçek ceset/tabut objesi")]
    public Transform corpseTransform; 
    
    [Tooltip("Sıçrama için basılacak tuş (Varsayılan: Space)")]
    public Key jumpKey = Key.Space;

    void Update()
    {
        // Yeni Input Sistemi ile sıçrama tuşunu kontrol et
        if (Keyboard.current != null && Keyboard.current[jumpKey].wasPressedThisFrame)
        {
            TryQuantumJump();
        }
    }

    private void TryQuantumJump()
    {
        // Sahnede ruhun etrafındaki AnchorPoint'leri bul (Performans için OverlapSphere kullanıyoruz)
        // Şimdilik sadece yakınlık ve kilit durumunu kontrol edeceğiz.
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f); // 5 birimlik arama yarıçapı
        
        AnchorPoint closestValidAnchor = null;
        float minDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            AnchorPoint anchor = hitCollider.GetComponent<AnchorPoint>();
            
            if (anchor != null && anchor.anchorData != null)
            {
                // Çapa ile ruh arasındaki mesafeyi ölç
                float distance = Vector3.Distance(transform.position, anchor.transform.position);
                
                // Eğer mesafe etkileşim yarıçapı içindeyse ve çapa AÇIKSA
                if (distance <= anchor.interactionRadius && anchor.anchorData.isUnlocked)
                {
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestValidAnchor = anchor;
                    }
                }
            }
        }

        // Eğer uygun bir çapa bulduysak, sıçramayı gerçekleştir!
        if (closestValidAnchor != null)
        {
            PerformJump(closestValidAnchor);
        }
        else
        {
            Debug.Log("Kuantum Sıçraması Başarısız: Yakında açık bir çapa noktası bulunamadı.");
            // İleride buraya düşük perdeli, hata belirten bir "bzzzt" sesi eklenecek.
        }
    }

    private void PerformJump(AnchorPoint targetAnchor)
    {
        if (corpseTransform != null)
        {
            // Cesedi çapa noktasının pozisyonuna ışınla
            corpseTransform.position = targetAnchor.transform.position;
            
            // İsteğe bağlı: Cesedin yönünü de çapa ile aynı yap
            corpseTransform.rotation = targetAnchor.transform.rotation;
            
            Debug.Log($"Kuantum Sıçraması Başarılı! Ceset '{targetAnchor.anchorData.anchorID}' noktasına çöktü.");
            
            // İleride buraya partikül efekti, iz bırakma ve imza niteliğindeki "çınlama" sesi eklenecek.
        }
        else
        {
            Debug.LogError("QuantumJumper: Ceset (Corpse) objesi atanmamış!");
        }
    }
}