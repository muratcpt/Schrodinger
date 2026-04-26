using UnityEngine;
using UnityEngine.InputSystem;

public class GhostPickup : MonoBehaviour
{
    [Header("Alma Ayarları")]
    [Tooltip("Eşyaların ele gelip duracağı Transform noktası (Ghost'un önünde bir boş obje oluşturup buraya sürükleyin)")]
    public Transform holdPoint;
    public float pickupRadius = 2f;
    public Key interactKey = Key.E;

    [Header("Yerleştirme (Snap) Ayarları")]
    public float placementSnapRadius = 3f;
    public float snapSpeed = 10f;
    [Tooltip("Sadece karakterin önüne doğru bu açıda hedefler kitlenir")]
    public float placementFOV = 90f;
    [Tooltip("İtilen obje bu mesafeden fazla hedefe girerse otomatik kilitlenir")]
    public float pushableSnapRadius = 1.5f;

    private PickupableObject currentlyHoldingItem = null;
    private ItemPlacementZone targetedZone = null;

    private PushableObject currentlyGrabbingObject = null;
    private FixedJoint grabJoint = null;
    private GhostController ghostController = null;

    private void Start()
    {
        ghostController = GetComponent<GhostController>();
    }

    private void Update()
    {
        if (currentlyHoldingItem != null)
        {
            HandlePlacementPreview();
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current[interactKey].wasPressedThisFrame)
            {
                if (currentlyHoldingItem == null && currentlyGrabbingObject == null)
                {
                    if (!TryPickupItem())
                    {
                        TryGrabObject();
                    }
                }
                else if (currentlyHoldingItem != null)
                {
                    TryDropOrPlaceItem();
                }
            }

            if (Keyboard.current[interactKey].wasReleasedThisFrame)
            {
                if (currentlyGrabbingObject != null)
                {
                    ReleaseObject();
                }
            }
        }

        if (currentlyGrabbingObject != null)
        {
            CheckPushableSnap();
        }
    }

    private void CheckPushableSnap()
    {
        Collider[] hitColliders = Physics.OverlapSphere(currentlyGrabbingObject.transform.position, pushableSnapRadius);

        foreach (var hitCollider in hitColliders)
        {
            ItemPlacementZone zone = hitCollider.GetComponent<ItemPlacementZone>();
            // Null olmayan, çözülmemiş ve id'si eşleşen bir zone ise
            if (zone != null && !zone.IsSolved() && !string.IsNullOrEmpty(currentlyGrabbingObject.itemID) && zone.requiredItemID == currentlyGrabbingObject.itemID)
            {
                bool placed = zone.TryPlacePushable(currentlyGrabbingObject);
                if (placed)
                {
                    // Başarıyla yerleşti, otomatik olarak elimizden bırak
                    ReleaseObject();
                    break;
                }
            }
        }
    }

    private void HandlePlacementPreview()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, placementSnapRadius);
        ItemPlacementZone closestZone = null;
        float minDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            ItemPlacementZone zone = hitCollider.GetComponent<ItemPlacementZone>();
            // Sadece çözülmemiş ve elimizdeki eşyayı isteyen bölgelere odaklan
            if (zone != null && !zone.IsSolved() && zone.requiredItemID == currentlyHoldingItem.itemID)
            {
                // Karakterin baktığı yön ile bölge arasındaki açıyı hesapla
                Vector3 directionToZone = (zone.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToZone);

                // Eğer bölge karakterin görüş açısı (FOV) içindeyse kilitlen
                if (angle <= placementFOV / 2f)
                {
                    float distance = Vector3.Distance(transform.position, zone.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestZone = zone;
                    }
                }
            }
        }

        targetedZone = closestZone;

        if (targetedZone != null)
        {
            // Hedefe doğru havada süzülerek hizalan
            Transform targetPoint = targetedZone.placementPoint != null ? targetedZone.placementPoint : targetedZone.transform;
            currentlyHoldingItem.transform.position = Vector3.Lerp(currentlyHoldingItem.transform.position, targetPoint.position, Time.deltaTime * snapSpeed);
            currentlyHoldingItem.transform.rotation = Quaternion.Lerp(currentlyHoldingItem.transform.rotation, targetPoint.rotation, Time.deltaTime * snapSpeed);
        }
        else if (holdPoint != null)
        {
            // Hedef yoksa tekrar ele (HoldPoint'e) dön
            currentlyHoldingItem.transform.position = Vector3.Lerp(currentlyHoldingItem.transform.position, holdPoint.position, Time.deltaTime * snapSpeed);
            currentlyHoldingItem.transform.rotation = Quaternion.Lerp(currentlyHoldingItem.transform.rotation, holdPoint.rotation, Time.deltaTime * snapSpeed);
        }
    }

    private bool TryPickupItem()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);
        PickupableObject closestItem = null;
        float minDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            PickupableObject item = hitCollider.GetComponent<PickupableObject>();
            if (item != null)
            {
                float distance = Vector3.Distance(transform.position, item.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }
            }
        }

        if (closestItem != null)
        {
            currentlyHoldingItem = closestItem;
            currentlyHoldingItem.OnPickedUp(holdPoint);
            Debug.Log($"Eşya Alındı: {currentlyHoldingItem.itemID}");
            return true;
        }
        
        return false;
    }

    private void TryDropOrPlaceItem()
    {
        if (targetedZone != null)
        {
            bool placed = targetedZone.TryPlaceItem(currentlyHoldingItem);
            if (placed)
            {
                // Başarıyla yerleştirildi, elimizden çıkardık
                currentlyHoldingItem = null;
                targetedZone = null;
                return;
            }
        }

        // Zone yoksa veya yerleşemediyse yere bırak
        currentlyHoldingItem.OnDropped();
        Debug.Log($"Eşya Bırakıldı: {currentlyHoldingItem.itemID}");
        currentlyHoldingItem = null;
        targetedZone = null;
    }

    private void TryGrabObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);
        PushableObject closestObj = null;
        float minDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            PushableObject obj = hitCollider.GetComponent<PushableObject>();
            if (obj != null)
            {
                float distance = Vector3.Distance(transform.position, obj.transform.position);
                if (distance < minDistance)
                {
                    // Objeye dönük olup olmadığımızı kontrol edelim ki arkamızdaki dolabı çekmeyelim
                    Vector3 directionToObj = (obj.transform.position - transform.position).normalized;
                    float angle = Vector3.Angle(transform.forward, directionToObj);
                    
                    if (angle <= placementFOV / 2f)
                    {
                        minDistance = distance;
                        closestObj = obj;
                    }
                }
            }
        }

        if (closestObj != null)
        {
            currentlyGrabbingObject = closestObj;
            grabJoint = gameObject.AddComponent<FixedJoint>();
            grabJoint.connectedBody = closestObj.GetComponent<Rigidbody>();
            
            if (ghostController != null)
            {
                ghostController.isGrabbing = true;
            }

            Debug.Log($"Obje tutuldu: {closestObj.gameObject.name}");
        }
    }

    private void ReleaseObject()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
        }
        currentlyGrabbingObject = null;
        
        if (ghostController != null)
        {
            ghostController.isGrabbing = false;
        }

        Debug.Log("Obje bırakıldı");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
