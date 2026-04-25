using UnityEngine;
using UnityEngine.InputSystem; // Eski Input yerine Yeni Input System kütüphanesini ekledik

[RequireComponent(typeof(Rigidbody))]
public class GhostController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 12f;

    private Rigidbody rb;
    private Vector3 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Ruh fiziksel olarak objeleri itebilsin ama kendi devrilmesin
        rb.constraints = RigidbodyConstraints.FreezeRotation; 
        
        // Ruhun süzülme hissiyatı için yerçekimini kapatıyoruz
        rb.useGravity = false; 
    }

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // Yeni Input Sistemi ile klavyeyi doğrudan okuyoruz (Maraton için en hızlı, en risksiz yöntem)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        }

        // Y ekseninde hareket yok, sadece X ve Z ekseninde (zemin üzerinde)
        movement = new Vector3(moveX, 0.0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        MoveGhost();
        RotateGhost();
    }

    private void MoveGhost()
    {
        if (movement.magnitude >= 0.1f)
        {
            // Rigidbody ile hareket
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void RotateGhost()
    {
        if (movement != Vector3.zero)
        {
            // Karakterin hareket ettiği yöne doğru yumuşakça dönmesi
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.rotation = Quaternion.Lerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}