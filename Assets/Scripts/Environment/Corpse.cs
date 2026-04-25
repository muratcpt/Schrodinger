using UnityEngine;

public class Corpse : MonoBehaviour
{
    [Header("Ceset / Tabut Ayarları")]
    [Tooltip("NPC tarafından cesedin bulunup bulunmadığı durumu")]
    public bool isDiscovered = false;

    // Maratonun ilerleyen aşamalarında Kuantum Sıçraması (Quantum Jump) yapıldığında,
    // ceset fiziksel olarak taşınmayacak, bu objenin transform.position'ı 
    // yeni çapa noktasına (Anchor Point) ışınlanacak.

    void Start()
    {
        // Maratonun Faz 1 (Gri Kutu) aşamasında tabutumuz başlangıç noktasında sabit duruyor.
    }
}