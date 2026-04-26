using UnityEngine;

public enum MovementAxis
{
    All,
    X_Only,
    Z_Only
}

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [Tooltip("Hangi eksende hareket edebileceğini seçin.")]
    public MovementAxis allowedAxis = MovementAxis.All;

    [Header("Yerleştirme Ayarları")]
    [Tooltip("Eğer bu objenin belirli bir yere yerleşmesi gerekiyorsa, o bölgeyle eşleşecek bir ID yazın.")]
    public string itemID;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Objenin itilirken sağa sola devrilmemesi ve havaya zıplamaması için fiziksel sınırlandırmalar
        RigidbodyConstraints constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        
        if (allowedAxis == MovementAxis.X_Only)
        {
            constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        else if (allowedAxis == MovementAxis.Z_Only)
        {
            constraints |= RigidbodyConstraints.FreezePositionX;
        }

        rb.constraints = constraints;
        
        // Ruhun rahatça itebilmesi için ideal bir ağırlık
        rb.mass = 2f; 
        
        // Unity 6 güncellemesi: 'drag' yerine 'linearDamping' kullanıyoruz
        rb.linearDamping = 2f; 
    }

    public void OnPlacedInZone(Transform zoneTransform)
    {
        // Obje hedef bölgeye kilitlendiğinde fiziğini dondur
        rb.isKinematic = true;
        
        // Pozisyonu ve rotasyonu hedefe eşitle
        transform.position = zoneTransform.position;
        transform.rotation = zoneTransform.rotation;
    }
}