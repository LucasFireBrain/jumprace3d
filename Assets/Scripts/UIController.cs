using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public GameObject StartPanel;
    public GameObject InstructionPanel;
    public GameObject GameOverPanel;
    public Animator PerfectPowerup;
    public Animator GoodPowerup;
    public Animator LongJumpPowerup;

    public Text GameOverTitle;

    public Image ProgressBar;
    public Text CurrentLevel;
    public Text NextLevel;

    //Ranking
    public List<Text> RankNames;
    public List<Text> RankPlaces;
    public List<Image> RankBgImages;
    public List<Text> GameOverRanks;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void SetProgress(float progress) {
        ProgressBar.fillAmount = progress;
    }
    public void SetLevelText(int currentLevel) {
        CurrentLevel.text = currentLevel.ToString();
        NextLevel.text = (currentLevel + 1).ToString();
    }

    public void InstructionFade(bool isFadeIn) {
        Animator animator = InstructionPanel.GetComponent<Animator>();
        if (isFadeIn) {
            animator.Play("FadeIn");    //same animation, but states have 1 and -1 speed
        }
        else {
            animator.Play("FadeOut");
        }

    }

    public void ShowGameOverPanel(bool isCompleted) {
        if (isCompleted) GameOverTitle.text = "Level Completed";
        StartCoroutine(ShowGameOverPanelRoutine());
    }

    IEnumerator ShowGameOverPanelRoutine() {
        yield return new WaitForSeconds(2f);
        UpdateGameOverRankText();
        GameOverPanel.SetActive(true);
    }

    public void UpdateRankText() {
        for (int i = 0; i < GameController.Main.Players.Count; i++) {
            IPlayer player = GameController.Main.Players[i];

            if (i < RankNames.Count) {
                if (player is Player) {
                    RankNames[i].text = "You";
                    RankBgImages[i].color = new Color(0, 0, 0, 0.7f);
                }
                else {
                    RankNames[i].text = player.GetName();
                    RankBgImages[i].GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
                }
                RankPlaces[i].text = (i + 1).ToString();
            }

            //Always show player
            if (player is Player) {
                if (i > 2) {
                    RankNames[2].text = "You";
                    RankPlaces[2].text = (i+1).ToString();
                    RankBgImages[2].color = new Color(0, 0, 0, 0.7f);
                }
            }
        }
    }

    public void UpdateGameOverRankText() {
        for (int i = 0; i < GameController.Main.Players.Count; i++) {
            IPlayer player = GameController.Main.Players[i];

            if (i < GameOverRanks.Count) {
                GameOverRanks[i].text = player.GetName();
            }
            if (player is Player) {
                GameOverRanks[i].text = "You";
                if (i != 0) GameOverRanks[i].transform.parent.GetComponent<Image>().color = new Color(0.88f, 0.36f, 0.13f);
            }
        }
    }

    public void ShowPowerup(int index) {
        switch (index) {
            case 0:
                PerfectPowerup.Play("FadeIn");
                break;
            case 1:
                GoodPowerup.Play("FadeIn");
                break;
            case 2:
                LongJumpPowerup.Play("FadeIn");
                break;
            default:
                break;
        }
    }
}
