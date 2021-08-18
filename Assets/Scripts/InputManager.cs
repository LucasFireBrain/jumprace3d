using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Call functions from scripts that need Input
/// Add more Functions to IInputHandler Interface depending on the game complexity:
/// Like OnTap(touch), OnPinch(touch1, touch2), OnHold(touch)... etc
/// </summary>
public class InputManager : MonoBehaviour
{
    public static List<ITouchHandler> TouchHandlers = new List<ITouchHandler>();

    private float _tapTresh = 0.16f;
    private float _tapTimer = 0;

    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0) {
            if (Input.touches[0].phase == TouchPhase.Began) {
                _tapTimer = Time.time;
            }
            if (Input.touches[0].phase == TouchPhase.Ended) {
                //Call OnTap()
                if (Time.time - _tapTimer < _tapTresh) { 
                    foreach (ITouchHandler touchHandler in TouchHandlers.ToArray()) {
                        touchHandler.OnTap(Input.touches[0]);
                    }
                }
            }

            //Call OnTouch()
            foreach (ITouchHandler touchHandler in TouchHandlers) {
                touchHandler.OnTouch(Input.touches[0]);
            }
        }
    }
}
