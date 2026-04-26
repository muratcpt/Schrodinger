using UnityEngine;
using UnityEngine.InputSystem;

public class QuantumJumper : MonoBehaviour
{
    [Header("Kuantum Sıçrama Ayarları")]
    public Transform corpseTransform; 
    public Key jumpKey = Key.Space;
    
    [Header("Kuantum İzi")]
    [Tooltip("Sıçrama sonrası geride kalacak iz objesi (Prefab)")]
    public GameObject trailPrefab;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current[jumpKey].wasPressedThisFrame)
        {
            TryQuantumJump();
        }
    }

    private void TryQuantumJump()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f); 
        AnchorPoint closestValidAnchor = null;
        float minDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            AnchorPoint anchor = hitCollider.GetComponent<AnchorPoint>();
            if (anchor != null && anchor.anchorData != null)
            {
                float distance = Vector3.Distance(transform.position, anchor.transform.position);
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

        if (closestValidAnchor != null)
        {
            PerformJump(closestValidAnchor);
        }
    }

    private void PerformJump(AnchorPoint targetAnchor)
    {
        if (corpseTransform != null)
        {
            // 1. Sıçramadan ÖNCE eski konumda bir iz (Trail) oluştur
            if (trailPrefab != null)
            {
                Instantiate(trailPrefab, corpseTransform.position, Quaternion.identity);
            }

            // 2. Cesedi yeni çapa noktasına ışınla
            corpseTransform.position = targetAnchor.transform.position;
            corpseTransform.rotation = targetAnchor.transform.rotation;
            
            Debug.Log($"Kuantum Sıçraması Başarılı! Ceset '{targetAnchor.anchorData.anchorID}' noktasına çöktü.");
        }
    }
}