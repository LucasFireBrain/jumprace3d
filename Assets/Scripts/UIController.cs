using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject GameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowGameOverPanel() {
        StartCoroutine(ShowGameOverPanelRoutine());
    }
    IEnumerator ShowGameOverPanelRoutine() {
        yield return new WaitForSeconds(0.5f);
        GameOverPanel.SetActive(true);
    }
}
