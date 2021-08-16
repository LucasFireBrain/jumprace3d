using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject InstructionPanel;
    public GameObject GameOverPanel;

    public Image ProgressBar;
    public Text CurrentLevel;
    public Text NextLevel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (isFadeIn)
        {
            animator.Play("FadeIn");    //same animation, but states have 1 and -1 speed
        }
        else { 
            animator.Play("FadeOut");
        }
        
    }

    public void ShowGameOverPanel() {
        StartCoroutine(ShowGameOverPanelRoutine());
    }
    IEnumerator ShowGameOverPanelRoutine() {
        yield return new WaitForSeconds(0.5f);
        GameOverPanel.SetActive(true);
    }
}
