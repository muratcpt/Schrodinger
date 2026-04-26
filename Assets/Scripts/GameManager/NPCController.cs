using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public enum NPCState { Patrol, Investigating }
    
    [Header("Durum")]
    public NPCState currentState = NPCState.Patrol;

    [Header("Devriye Ayarları")]
    [Tooltip("NPC'nin sırayla gezeceği noktalar (Sahnede boş GameObject'ler oluşturup buraya at)")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Hız Ayarları")]
    public float patrolSpeed = 2.5f;
    public float investigateSpeed = 4.5f;
    public float investigateDuration = 3f;

    [Header("Animasyon")]
    [Tooltip("Animator (opsiyonel) - varsa Speed parametresi otomatik set edilir")]
    public Animator animator;

    private NavMeshAgent agent;
    private float investigateTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        
        // Animator otomatik bul
        if (animator == null) animator = GetComponentInChildren<Animator>();
        
        GoToNextWaypoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case NPCState.Patrol:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    GoToNextWaypoint();
                }
                break;

            case NPCState.Investigating:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    investigateTimer += Time.deltaTime;
                    if (investigateTimer >= investigateDuration)
                    {
                        currentState = NPCState.Patrol;
                        agent.speed = patrolSpeed;
                        GoToNextWaypoint();
                    }
                }
                break;
        }

        // Animator Speed parametresi
        if (animator != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }
    }

    private void GoToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        
        agent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    private void OnTriggerEnter(Collider other)
    {
        QuantumTrail trail = other.GetComponent<QuantumTrail>();
        if (trail != null)
        {
            currentState = NPCState.Investigating;
            agent.speed = investigateSpeed;
            agent.destination = trail.transform.position;
            investigateTimer = 0f;
            
            Debug.Log("NPC Şüpheli Bir İz Buldu! Araştırıyor...");
            Destroy(trail.gameObject);
        }

        Corpse foundCorpse = other.GetComponent<Corpse>();
        if (foundCorpse != null)
        {
            foundCorpse.isDiscovered = true;
            agent.isStopped = true;
            Debug.LogWarning("OYUN BİTTİ! DALGA FONKSİYONU ÇÖKTÜ! NPC CESEDİ BULDU!");
        }
    }
}
