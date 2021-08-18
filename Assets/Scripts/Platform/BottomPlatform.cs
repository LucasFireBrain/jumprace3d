using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomPlatform : Platform {

    private bool _isHide;

    // Update is called once per frame
    void Update() {
        if (_isHide) {
            transform.Translate(Vector3.down * Time.deltaTime * 2);
            if (transform.position.y < -4) {
                GameObject.Destroy(this.gameObject);
            }
        }
    }

    //Hide
    IEnumerator HideRoutine() {
        yield return new WaitForSeconds(1);
    }

    public override void Bounce() {
        base.Bounce();
        _isHide = true;
    }
}
