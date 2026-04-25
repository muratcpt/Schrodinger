using UnityEngine;

[CreateAssetMenu(fileName = "New Anchor Data", menuName = "Schrodinger/Anchor Data")]
public class AnchorData : ScriptableObject
{
    [Header("Çapa Bilgileri")]
    [Tooltip("Bu çapanın benzersiz kimliği (Örn: Anchor_LivingRoom)")]
    public string anchorID;
    
    [Tooltip("Bu çapa başlangıçta açık mı?")]
    public bool isUnlocked = false;

    // İleride kilit mekaniklerini eklediğimizde buraya kilit açma koşullarını da bağlayacağız.
}