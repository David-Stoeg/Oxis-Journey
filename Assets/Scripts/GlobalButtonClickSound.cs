using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalButtonClickSound : MonoBehaviour
{
    public static GlobalButtonClickSound Instance;

    [Header("Sound")]
    public AudioSource sfxSource;
    public AudioClip clickClip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Optional")]
    public bool dontDestroyOnLoad = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        // Hook current scene + future scenes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        HookAllButtonsInScene();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HookAllButtonsInScene();
    }

    void HookAllButtonsInScene()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // include inactive
        foreach (Button b in buttons)
        {
            if (b == null) continue;

            // Avoid adding multiple listeners if scene reloads
            b.onClick.RemoveListener(PlayClick);
            b.onClick.AddListener(PlayClick);
        }
    }

    public void PlayClick()
    {
        if (clickClip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clickClip, volume);
    }
}
