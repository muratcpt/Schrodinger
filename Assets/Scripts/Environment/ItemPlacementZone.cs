using UnityEngine;
using UnityEngine.Events;

public class ItemPlacementZone : MonoBehaviour
{
    [Header("Yerleştirme Ayarları")]
    [Tooltip("Bu bölgeye konması gereken eşyanın ID'si")]
    public string requiredItemID;
    
    [Tooltip("Eşyanın tam olarak nereye oturacağı (örneğin kilit deliği veya raf). Boş bırakılırsa bu objenin kendi pozisyonuna oturur.")]
    public Transform placementPoint;

    [Tooltip("Bölgeye konulması gereken eşyanın yarı saydam (placeholder) modeli. Eşya konduğunda otomatik olarak gizlenir.")]
    public GameObject placeholderObject;

    [Tooltip("Bu bölgeye doğru eşya konulduğunda açılacak çapa (opsiyonel)")]
    public AnchorData targetAnchor;

    [Header("Olaylar (Event)")]
    public UnityEvent OnItemPlaced;

    private bool isSolved = false;

    public bool IsSolved()
    {
        return isSolved;
    }

    // GhostPickup scripti eşyayı buraya bırakmak istediğinde bunu çağıracak
    public bool TryPlaceItem(PickupableObject item)
    {
        if (isSolved) return false;

        if (item.itemID == requiredItemID)
        {
            isSolved = true;
            item.OnPlacedInZone(placementPoint != null ? placementPoint : transform);
            
            if (placeholderObject != null)
            {
                placeholderObject.SetActive(false); // Eşya konunca saydam modeli gizle
            }

            Debug.Log($"Doğru eşya ({item.itemID}) yerleştirildi!");
            
            if (targetAnchor != null)
            {
                targetAnchor.isUnlocked = true;
                Debug.Log($"Anchor Kilidi Açıldı: {targetAnchor.anchorID}");
            }
            
            OnItemPlaced?.Invoke();
            return true; // Eşya kabul edildi
        }

        Debug.Log("Yanlış eşya denendi.");
        return false; // Eşya bu bölgeye ait değil
    }

    public bool TryPlacePushable(PushableObject item)
    {
        if (isSolved) return false;

        if (!string.IsNullOrEmpty(item.itemID) && item.itemID == requiredItemID)
        {
            isSolved = true;
            item.OnPlacedInZone(placementPoint != null ? placementPoint : transform);
            
            if (placeholderObject != null)
            {
                placeholderObject.SetActive(false);
            }

            Debug.Log($"Doğru itilebilir eşya ({item.itemID}) yerleştirildi!");
            
            if (targetAnchor != null)
            {
                targetAnchor.isUnlocked = true;
                Debug.Log($"Anchor Kilidi Açıldı: {targetAnchor.anchorID}");
            }
            
            OnItemPlaced?.Invoke();
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f); // Mavi şeffaf küp
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}