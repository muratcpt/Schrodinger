using UnityEngine;

public class LockTrigger : MonoBehaviour
{
    [Header("Kilit Ayarları")]
    [Tooltip("Bu bulmaca çözüldüğünde HANGİ çapanın kilidi açılacak?")]
    public AnchorData targetAnchorData;
    
    [Tooltip("Bu kilidi açmak için spesifik bir obje mi gerekiyor? (Boş bırakırsan herhangi bir PushableObject açar)")]
    public PushableObject requiredObject;

    private bool isUnlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        // Eğer kilit zaten açıldıysa tekrar işlem yapma
        if (isUnlocked) return;

        // Çarpışan objede PushableObject scripti var mı diye kontrol et
        PushableObject pushedObj = other.GetComponent<PushableObject>();
        
        if (pushedObj != null)
        {
            // Eğer özel bir obje (örn: Kırmızı Kitaplık) istediysek ve bu o değilse, kabul etme
            if (requiredObject != null && pushedObj != requiredObject) return;

            UnlockAnchor();
        }
    }

    private void UnlockAnchor()
    {
        isUnlocked = true;
        
        if (targetAnchorData != null)
        {
            // Çapanın kilidini aç!
            targetAnchorData.isUnlocked = true;
            Debug.Log($"KİLİT ÇÖZÜLDÜ! Artık '{targetAnchorData.anchorID}' noktasına kuantum sıçraması yapabilirsin.");
            
            // Maratonun Polish aşamasında buraya "Click" veya tatlı bir kilit açılma sesi ekleyeceğiz.
        }
    }

    // Editörde hedef bölgesini şeffaf yeşil bir kutu olarak görmek için
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f); // Yarı saydam yeşil
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}