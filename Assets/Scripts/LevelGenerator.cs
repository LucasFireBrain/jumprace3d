using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static Transform StartingPlatform;

    public GameObject PlatformPrefab;
    public GameObject MovingPlatformPrefab;
    public GameObject BreakablePlatformPrefab;
    public GameObject PlatformLabelPrefab;
    public GameObject BladesPrefab;
    public GameObject GoalPrefab;
    public LineRenderer Line;

    private int _platformCount = 20;
    private float _gapSize = 4f;
    private float _zigzag = 0.2f;
    private float _slope = 3f;
    private System.Random _rand;
    private System.Random _dice;
    private int _seed = 43;

    private Transform _previousPlatform;

    // Start is called before the first frame update
    void Start()
    {
        //use seed if not 0
        if (_seed == 0) { 
            _rand = new System.Random();
            _dice = new System.Random();
        }
        else { 
            _rand = new System.Random(_seed);
            _dice = new System.Random(_seed);
        }
        GenerateLevel();
    }


    void GenerateLevel() {

        Line.positionCount = _platformCount * 2;

        for (int i = 0; i < _platformCount; i++)
        {
            Transform platform;

            if (i != 0 && _dice.Next(0, 100) < 30)
            {   //30% chance of moving platform
                platform = Instantiate(MovingPlatformPrefab).transform;
            }
            else if (i != 0 && _dice.Next(0, 100) < 30) {
                //breakable platform
                platform = Instantiate(BreakablePlatformPrefab).transform;
            }
            else {
                //regular platform
                platform = Instantiate(PlatformPrefab).transform;
            }
            if (_previousPlatform != null)
            {
                //Curve Path
                platform.position = _previousPlatform.position + _previousPlatform.forward * _gapSize;
                int angle = _rand.Next(5, 45);
                platform.eulerAngles = _previousPlatform.eulerAngles + Vector3.up * angle;
                _previousPlatform.GetComponent<Platform>().Next = platform.GetComponent<Platform>();

                //Line renderer
                Line.SetPosition(2 * i - 1, _previousPlatform.position);
                Vector3 midPoint = (platform.position.withY((_platformCount - i) * _slope) + _previousPlatform.position)/2;
                Line.SetPosition(2 * i, midPoint);

                //Create Blades with 20% chance
                if (_dice.Next(0, 100) < 20) {
                    GameObject blades = Instantiate(BladesPrefab);
                    blades.transform.position = midPoint;
                    blades.transform.forward = platform.forward;
                }
            }
            else {
                //Set starting platform
                StartingPlatform = platform;
            }
            //Start high up and set goal near the water.
            platform.position = platform.position.withY((_platformCount - i) * _slope);

            //Platform Label
            GameObject label = Instantiate(PlatformLabelPrefab, platform);
            label.GetComponentInChildren<TextMesh>().text = (_platformCount - i).ToString();

            //Prepare for next iteration
            _previousPlatform = platform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
