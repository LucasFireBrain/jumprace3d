using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour, ITouchHandler {
    public static GameController Main;

    public UIController UIController;

    public List<IPlayer> Players = new List<IPlayer>();

    public bool IsGameOver;
    public bool IsTapToContinue;
    public bool IsLevelCleared;

    public int CurrentLevel;


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

        //Add to Input Manager
        InputManager.TouchHandlers.Add(this);
    }

    public void OnTouch(Touch touch) {}

    public void OnTap(Touch touch) {
        if (IsTapToContinue) {
            Restart();
        }
    }

    void Restart() {
        InputManager.TouchHandlers.Clear();

        //Reset static variables
        SceneManager.LoadScene(0);
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
