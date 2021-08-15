using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    public Transform BeamOrigin;

    private Color[] _colors = { new Color(1, 0, 0, 0.3f), new Color(1, 1, 0, 0.3f), new Color(0, 1, 0, 0.3f) };

    private Renderer _renderer;
    private RaycastHit _hit;
    private Ray _ray;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _ray = new Ray(BeamOrigin.position, Vector3.down);
        //Check floor below player
        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity)) {
            if (_hit.collider.GetComponent<Platform>())
            {
                //Is Platform, turn yellow.
                ChangeColor(1);
                if (Vector3.Distance(_hit.point, _hit.collider.transform.position) < .2f)
                {
                    //Is in the middle, turn green.
                    ChangeColor(2);
                }

                SetBeamSize(_hit.point, BeamOrigin.position);
            }
            else
            {
                //Is not over platform, turn red.
                ChangeColor(0);
                SetBeamSize(_hit.point, BeamOrigin.position);
            }
        }
        else {
            //Is not over platform, turn red.
            ChangeColor(0);
        }
    }
    void SetBeamColor() { 
        
    }
    void ChangeColor(int colorIndex) {
        _renderer.material.SetColor("_Color", _colors[colorIndex]);
    }

    void SetBeamSize(Vector3 pos1, Vector3 pos2) {
        float height = Vector3.Distance(pos1, pos2);
        transform.localScale = new Vector3(0.2f, height, 0.2f);
        transform.localPosition = new Vector3(0, -height, 0);
    }
}
