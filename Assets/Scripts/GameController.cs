using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController Main;

    public UIController UIController;

    public List<IPlayer> Players = new List<IPlayer>();

    public bool IsStarted;
    public bool IsTapToContinue;
    public bool IsLevelCleared;
    public bool IsGameOver;

    public int CurrentLevel;

    private bool _isShowingInstructions;


    // Start is called before the first frame update
    void Start() {
        if (Main == null) {
            Main = this;
        }
        else {
            GameObject.Destroy(this.gameObject);
            return;
        }

        //Player Prefs
        if (PlayerPrefs.HasKey("CurrentLevel")) {
            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
        }
        else {
            PlayerPrefs.SetInt("CurrentLevel", 0);
        }

        //UI
        UIController.SetLevelText(CurrentLevel);
    }

#if UNITY_EDITOR
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (_isShowingInstructions) HideInstructions();
            if (!IsStarted) StartGame();
            if (IsTapToContinue) Restart();
        }
    }
#endif

    void StartGame() {
        IsStarted = true;
        foreach (IPlayer player in Players) player.StartGame();
        UIController.StartPanel.SetActive(false);   //Hide "Tap to Start"
        UIController.InstructionFade(true);         //Show "Hold to move forward"
        _isShowingInstructions = true;
    }

    void HideInstructions() {
        _isShowingInstructions = false;
        UIController.InstructionFade(false);    //Hide "Hold to Move Forward"
    }

    void Restart() {
        SceneManager.LoadScene(0);

        //Reset static variables
        AiPlayer.AiIndex = 0;
    }

    public void GameOver(bool isCompleted) {
        UpdatePlayerRank();
        UIController.ShowGameOverPanel(isCompleted);
        StartCoroutine(SetTapToContinueRoutine());
    }

    IEnumerator SetTapToContinueRoutine() {
        yield return new WaitForSeconds(4f);
        IsTapToContinue = true;
    }

    public void ClearLevel() {
        IsLevelCleared = true;
        PlayerPrefs.SetInt("CurrentLevel", ++CurrentLevel);
    }

    public void UpdatePlayerRank() {
        Players = Players.OrderByDescending(p => p.GetProgress()).ToList();
        UIController.UpdateRankText();
    }
}
