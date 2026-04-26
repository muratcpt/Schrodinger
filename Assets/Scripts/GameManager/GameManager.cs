using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oyun durumu yöneticisi. Game Over'ı tetikler ve sahne geçişlerini kontrol eder.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referanslar")]
    [Tooltip("Oyun sırasında izlenecek ceset (isDiscovered=true olunca oyun biter)")]
    public Corpse corpse;

    [Tooltip("Game Over panel - oyun bitince aktive olur")]
    public GameObject gameOverPanel;

    [Header("Game Over Ayarları")]
    [Tooltip("Game Over panelinin fade-in süresi (saniye)")]
    public float gameOverFadeDuration = 1.5f;

    [Tooltip("Oyun bittiğinde zaman duraklatılsın mı")]
    public bool pauseTimeOnGameOver = true;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        // Ceset keşfedildiyse oyun bitti
        if (corpse != null && corpse.isDiscovered)
        {
            TriggerGameOver();
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.LogWarning("OYUN BİTTİ - DALGA FONKSİYONU ÇÖKTÜ");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            StartCoroutine(FadeInPanel(gameOverPanel));
        }
    }

    IEnumerator FadeInPanel(GameObject panel)
    {
        var canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        float t = 0f;

        // Realtime kullan ki Time.timeScale=0'da bile çalışsın
        while (t < gameOverFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / gameOverFadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        if (pauseTimeOnGameOver) Time.timeScale = 0f;
    }

    // === BUTON FONKSİYONLARI (UI'dan çağrılır) ===

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("EDA");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
