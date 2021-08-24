using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer {
    void StartGame();
    float GetProgress();
    string GetName();
}
