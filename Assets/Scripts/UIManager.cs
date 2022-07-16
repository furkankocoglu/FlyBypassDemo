using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Scrollbar finishScrollbar;
    [SerializeField]
    GameObject waitingPanel, readyPanel, finishText, ScrollbarPanel, nextLevelPanel, levelFailedPanel;
    [SerializeField]
    Text levelText, scoreText;
    private void Start()
    {
        levelText.text = "Lv." + LevelManager.instance.level.ToString();
        Time.timeScale = 1;
    }
    void UpdateFinishScrollbar(float value)
    {
        finishScrollbar.value = value;
    }
    void HideWaitingPanel()
    {
        waitingPanel.SetActive(false);
        StartCoroutine(ShowReadyPanel());
    }
    IEnumerator ShowReadyPanel()
    {
        readyPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        readyPanel.SetActive(false);
        EventManager.Fire_onStartRun();
    }
    void OnFinishLevel()
    {
        ScrollbarPanel.SetActive(false);
        finishText.SetActive(true);
    }
    void OnLandAndDance()
    {
        finishText.SetActive(false);
        nextLevelPanel.SetActive(true);
    }
    void OnLevelFailed()
    {
        ScrollbarPanel.SetActive(false);
        levelFailedPanel.SetActive(true);
        Time.timeScale = 0;
    }
    void ChangeScoreText(string text)
    {
        scoreText.text = "Score:" + text;
    }
    public void NextLevelButton()
    {
        LevelManager.instance.NextLevel();
    }
    public void LevelFailedButton()
    {
        LevelManager.instance.LevelFailed();
    }
    private void OnEnable()
    {
        EventManager.onCharacterMove += UpdateFinishScrollbar;
        EventManager.onOpponentReady += HideWaitingPanel;
        EventManager.onFinishLevel += OnFinishLevel;
        EventManager.onLandAndDance += OnLandAndDance;
        EventManager.onlevelFailed += OnLevelFailed;
        EventManager.onChangeScore += ChangeScoreText;
    }
    private void OnDisable()
    {
        EventManager.onCharacterMove -= UpdateFinishScrollbar;
        EventManager.onOpponentReady -= HideWaitingPanel;
        EventManager.onFinishLevel -= OnFinishLevel;
        EventManager.onLandAndDance -= OnLandAndDance;
        EventManager.onlevelFailed -= OnLevelFailed;
        EventManager.onChangeScore -= ChangeScoreText;
    }
}
