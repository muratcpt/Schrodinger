using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GhostController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 12f;

    private Rigidbody rb;
    private Vector3 movement;
    
    [Header("Animasyon")]
    [Tooltip("Karakterin üzerindeki Animator bileşeni (Otomatik bulunur)")]
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
        // Ruh devrilmesin diye rotasyonu kilitliyoruz (X ve Z ekseninde devrilmez)
        rb.constraints = RigidbodyConstraints.FreezeRotation; 
        
        // İSKELET YÜRÜDÜĞÜ İÇİN YERÇEKİMİNİ AÇIYORUZ!
        rb.useGravity = true; 
    }

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        }

        movement = new Vector3(moveX, 0.0f, moveZ).normalized;

        if (animator != null)
        {
            animator.SetFloat("Speed", movement.magnitude);
        }
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
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void RotateGhost()
    {
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.rotation = Quaternion.Lerp(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}