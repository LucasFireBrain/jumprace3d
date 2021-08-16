using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    public static Transform StartingPlatform;

    public GameObject PlatformPrefab;
    public GameObject MovingPlatformPrefab;
    public GameObject BreakablePlatformPrefab;
    public GameObject PlatformLabelPrefab;
    public GameObject BladesPrefab;
    public GameObject GoalPrefab;
    public LineRenderer Line;

    //Generation Parameters
    private int _platformCount = 20;
    private float _gapSize = 6f;
    private float _zigzag = 0.2f;
    private float _slope = 1.5f;
    private System.Random _rand;
    private System.Random _dice;
    public int[] LevelSeeds = { 13, 309, 22, 4, 8, 40 };
    private int _seed;

    private Transform _previousPlatform;

    // Start is called before the first frame update
    void Start() {
        //Load Level and Seed
        Debug.Log(GameController.Main.CurrentLevel);
        if (GameController.Main.CurrentLevel >= LevelSeeds.Length) {
            _seed = UnityEngine.Random.Range(1, 1000); //Get Random Seed
        }
        else {
            _seed = LevelSeeds[GameController.Main.CurrentLevel];
        }

        //Set Platform Count
        _platformCount = 10 + (5 * GameController.Main.CurrentLevel);

        _rand = new System.Random(_seed);
        _dice = new System.Random(_seed);

        GenerateLevel();
    }


    void GenerateLevel() {

        Line.positionCount = _platformCount * 2 + 1;    // plus 1 is the Goal Platform

        for (int i = 0; i < _platformCount; i++) {
            Transform platform;

            if (i != 0 && _dice.Next(0, 100) < 30) {   //30% chance of moving platform
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
            if (_previousPlatform != null) {
                //Curve Path
                platform.position = _previousPlatform.position + _previousPlatform.forward * _gapSize;
                int angle = _rand.Next(5, 45);
                platform.eulerAngles = _previousPlatform.eulerAngles + Vector3.up * angle;
                _previousPlatform.GetComponent<Platform>().Next = platform.GetComponent<Platform>();

                //Line renderer
                Line.SetPosition(2 * i - 2, _previousPlatform.position);
                Vector3 midPoint = (platform.position.withY((_platformCount - i) * _slope) + _previousPlatform.position) / 2;
                Line.SetPosition(2 * i - 1, midPoint);

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

            //Set corresponding progress based on index
            platform.GetComponent<Platform>().Progress = (float)i / (float)_platformCount;

            //Start high up and set goal near the water.
            platform.position = platform.position.withY((_platformCount - i) * _slope);

            //Platform Label
            GameObject label = Instantiate(PlatformLabelPrefab, platform);
            label.GetComponentInChildren<TextMesh>().text = (_platformCount - i).ToString();

            //Prepare for next iteration
            _previousPlatform = platform;


        }
        // Create Goal Platform
        {
            GameObject goal = Instantiate(GoalPrefab);
            goal.transform.rotation = _previousPlatform.rotation;
            goal.transform.position = (_previousPlatform.position + _previousPlatform.forward * _gapSize).withY(0);
            Vector3 midPoint = (goal.transform.position + _previousPlatform.position) / 2;
            Line.SetPosition(Line.positionCount - 3, _previousPlatform.position);
            Line.SetPosition(Line.positionCount - 2, midPoint);
            Line.SetPosition(Line.positionCount - 1, goal.transform.position);
        }


    }
    // Update is called once per frame
    void Update() {

    }
}
