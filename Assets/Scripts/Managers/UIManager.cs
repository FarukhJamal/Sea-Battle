using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject counterPanel;
    public TMP_Text counterText;
    public GameObject playerPlacementPanel;
    public GameObject aiPlacementPanel;
    public GameObject gameplayPanel;

    [Header("Buttons")]
    public Button randomButton;
    public Button saveButton;

    public event Action OnRandomClicked;
    public event Action OnSaveClicked;

    private void Awake()
    {
        if (randomButton != null) randomButton.onClick.AddListener(() => OnRandomClicked?.Invoke());
        if (saveButton != null) saveButton.onClick.AddListener(() => OnSaveClicked?.Invoke());
    }

    public void ShowCounter(Action onComplete = null)
    {
        StartCoroutine(CounterRoutine(onComplete));
    }

    private IEnumerator CounterRoutine(Action onComplete)
    {
        if (counterPanel != null) counterPanel.SetActive(true);
        if (playerPlacementPanel != null) playerPlacementPanel.SetActive(false);
        if (aiPlacementPanel != null) aiPlacementPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);

        int[] values = { 1, 2, 3 };
        foreach (var v in values)
        {
            if (counterText != null) counterText.text = v.ToString();
            yield return new WaitForSeconds(0.6f);
        }

        if (counterPanel != null) counterPanel.SetActive(false);
        onComplete?.Invoke();
    }

    public void ShowPlayerPlacementPanel()
    {
        playerPlacementPanel.SetActive(true);
        aiPlacementPanel.SetActive(false);
        gameplayPanel.SetActive(false);
    }

    public void ShowAIPlacementPanel()
    {
        playerPlacementPanel.SetActive(false);
        aiPlacementPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }

    public void ShowGameplayPanel()
    {
        playerPlacementPanel.SetActive(false);
        aiPlacementPanel.SetActive(false);
        gameplayPanel.SetActive(true);
    }
}