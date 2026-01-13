using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InfoScreenTyper : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textBox;                 // Drag your TMP text here
    public GameObject nextButtonObject;      // Optional: drag your Next button (for hiding/showing)
    
    [Header("Text Pages")]
    [TextArea(3, 10)]
    public string[] pages;

    [Header("Typing")]
    public float lettersPerSecond = 60f;     // “fast animal crossing”
    public bool playOnStart = true;

    [Header("Scene")]
    public string nextSceneName;             // Set in Inspector (or leave empty to use build index)

    private int _pageIndex = 0;
    private Coroutine _typingRoutine;
    private bool _isTyping = false;

    void Start()
    {
        if (textBox == null)
        {
            Debug.LogError("InfoScreenTyper: textBox is not assigned.");
            enabled = false;
            return;
        }

        if (pages == null || pages.Length == 0)
        {
            Debug.LogWarning("InfoScreenTyper: No pages assigned.");
            textBox.text = "";
            return;
        }

        if (playOnStart)
            ShowPage(0);
    }

    public void OnNextPressed()
    {
        // If currently typing → finish instantly
        if (_isTyping)
        {
            FinishTypingInstantly();
            return;
        }

        // If finished typing → go to next page or next scene
        _pageIndex++;

        if (_pageIndex < pages.Length)
        {
            ShowPage(_pageIndex);
        }
        else
        {
            LoadNextScene();
        }
    }

    void ShowPage(int index)
    {
        if (_typingRoutine != null)
            StopCoroutine(_typingRoutine);

        //textBox.text = "";
        // Text sofort setzen, aber unsichtbar machen (maxVisibleCharacters = 0)
        textBox.text = pages[index];
        textBox.maxVisibleCharacters = 0;
        _typingRoutine = StartCoroutine(TypeText(pages[index]));
    }

    IEnumerator TypeText(string fullText)
    {
        _isTyping = true;

        if (nextButtonObject != null)
            nextButtonObject.SetActive(true);

        // Wichtig: TMP muss einmal updaten, um zu wissen, wie viele "echte" Zeichen (ohne Tags) es gibt
        textBox.ForceMeshUpdate();

        int totalVisibleCharacters = textBox.textInfo.characterCount; // Zählt Tags NICHT mit
        int counter = 0;

        float delay = 1f / Mathf.Max(1f, lettersPerSecond);

        //for (int i = 0; i < fullText.Length; i++)
        //{
        //    textBox.text += fullText[i];
        //    yield return new WaitForSecondsRealtime(delay); // ✅ ignores Time.timeScale
        //}

        while (counter <= totalVisibleCharacters)
        {
            textBox.maxVisibleCharacters = counter;
            counter++;

            yield return new WaitForSecondsRealtime(delay); // ✅ ignores Time.timeScale
        }

        _isTyping = false;
    }

    void FinishTypingInstantly()
    {
        if (_typingRoutine != null)
            StopCoroutine(_typingRoutine);

        textBox.text = pages[_pageIndex];
        textBox.maxVisibleCharacters = 99999; // Alles sichtbar machen
        _isTyping = false;
    }

    void LoadNextScene()
    {
        // Option A: load by name (recommended)
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // Option B: load next build index
        int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(current + 1);
    }
}
