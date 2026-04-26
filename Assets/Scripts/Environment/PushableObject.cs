using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PushableObject : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Objenin itilirken sağa sola devrilmemesi ve havaya zıplamaması için fiziksel sınırlandırmalar
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        
        // Ruhun rahatça itebilmesi için ideal bir ağırlık (maratonda hissiyatı buradan ayarlarsın)
        rb.mass = 2f; 
        
        // Unity 6 güncellemesi: 'drag' yerine 'linearDamping' kullanıyoruz
        rb.linearDamping = 2f; 
    }
}