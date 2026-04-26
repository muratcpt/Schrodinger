using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupableObject : MonoBehaviour
{
    [Header("Eşya Ayarları")]
    [Tooltip("Bu eşyanın bulmacalar için kullanılacak benzersiz ID'si (Örn: Anahtar_Oda1)")]
    public string itemID;
    
    private Rigidbody rb;
    private Collider[] colliders;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponents<Collider>();
    }

    public void OnPickedUp(Transform holdPoint)
    {
        // Fiziği devre dışı bırak
        rb.isKinematic = true;
        rb.useGravity = false;
        
        // Ghost'un HoldPoint objesinin altına koy ve pozisyonunu sıfırla
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Çarpışmaları kapat (Karakterin içinden geçip itmemesi için)
        SetCollidersEnabled(false);
    }

    public void OnDropped()
    {
        // Parent'ı temizle
        transform.SetParent(null);
        
        // Fiziği tekrar aktif et
        rb.isKinematic = false;
        rb.useGravity = true;
        
        // Çarpışmaları aç
        SetCollidersEnabled(true);
    }

    public void OnPlacedInZone(Transform placementPoint)
    {
        // Zone içine oturtuldu
        transform.SetParent(placementPoint);
        transform.position = placementPoint.position;
        transform.rotation = placementPoint.rotation;

        rb.isKinematic = true;
        rb.useGravity = false;
        SetCollidersEnabled(false);
    }

    private void SetCollidersEnabled(bool isEnabled)
    {
        if (colliders == null) return;
        foreach (var col in colliders)
        {
            if (col != null) col.enabled = isEnabled;
        }
    }
}
