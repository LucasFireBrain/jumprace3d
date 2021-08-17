using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomPlatform : Platform {

    // Update is called once per frame
    void Update() {

    }

    //Hide
    IEnumerator HideRoutine() {
        yield return new WaitForSeconds(1);
    }
}
