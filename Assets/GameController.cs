using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Main;

    public UIController UIController;

    public bool IsGameOver;
    public bool IsTapToContinue;
    public bool IsLevelCleared;

    // Start is called before the first frame update
    void Start()
    {
        if (Main == null) {
            Main = this;
        }
        else {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTapToContinue) {
            if (Input.GetMouseButtonDown(0)) {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void GameOver() {
        UIController.ShowGameOverPanel();
        StartCoroutine(SetTapToContinueRoutine());
    }

    IEnumerator SetTapToContinueRoutine() {
        yield return new WaitForSeconds(1f);
        IsTapToContinue = true;
    }
}
