using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    [Header("Çapa Ayarları")]
    [Tooltip("Bu fiziksel noktanın bağlı olduğu veri dosyası")]
    public AnchorData anchorData;

    [Tooltip("Ruh bu çapaya sıçrayabilmek için ne kadar yaklaşmalı?")]
    public float interactionRadius = 3f;

    // Sahne görünümünde çapaları kolay seçebilmek için bir küre çizelim
    private void OnDrawGizmos()
    {
        Gizmos.color = anchorData != null && anchorData.isUnlocked ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}