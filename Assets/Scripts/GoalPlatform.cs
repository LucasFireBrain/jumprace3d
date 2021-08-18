using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPlatform : Platform
{
    public GameObject Confetti;

    public override void Bounce()
    {
        //Confetti Particles
        Confetti.SetActive(true);

        //Clear Level
        GameController.Main.ClearLevel();
        GameController.Main.UIController.SetProgress(1);

        //Show Ranking

    }
}
