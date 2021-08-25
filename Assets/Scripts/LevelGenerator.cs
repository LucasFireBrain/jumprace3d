using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dreamteck.Splines;

public class LevelGenerator : MonoBehaviour {
    
    public static Transform StartingPlatform;

    public GameObject PlatformPrefab;
    public GameObject MovingPlatformPrefab;
    public GameObject BreakablePlatformPrefab;
    public GameObject BottomPlatformPrefab;
    public GameObject PlatformLabelPrefab;
    public GameObject BladesPrefab;
    public GameObject GoalPrefab;
    public GameObject Spline;
    public Material SplineMaterial;

    private Transform _previousPlatform;
    private Bounds _bounds; //Used to add Bottom platforms below the generated stage

    //Generation Parameters
    private float _gapSize = 6f;    //Distance between each platform
    private float _slope = 1.5f;    //Height factor
    private int _zigzag = 10;   //Chance of direction change

    private int _platformCount;     //Controlled by current level
    
    private int[] _levelSeeds = { 13, 309, 22, 4, 8, 40 };    //Make the first levels always the same
    private int _seed;

    private bool _isCurvedLeft;

    private System.Random _rand;
    private System.Random _dice;

    // Start is called before the first frame update
    void Start() {
        //Load Level and Seed
        if (GameController.Main.CurrentLevel < _levelSeeds.Length) {
            _seed = _levelSeeds[GameController.Main.CurrentLevel];   //Get seed from array
        }
        else {
            _seed = UnityEngine.Random.Range(1, 1000); //Get Random Seed
        }

        _rand = new System.Random(_seed);
        _dice = new System.Random(_seed);
        //Set Platform Count

        _platformCount = 10 + (5 * GameController.Main.CurrentLevel);

        GenerateLevel();

    }


    void GenerateLevel() {

        SplineComputer splineComputer = Spline.AddComponent<SplineComputer>();
        SplinePoint[] points = new SplinePoint[_platformCount];

        for (int i = 0; i < _platformCount; i++) {
            Transform platform;

            if (i != 0 && _dice.Next(0, 100) < 30) {   //30% chance of moving platform
                platform = Instantiate(MovingPlatformPrefab).transform;
            }
            else if (i != 0 && _dice.Next(0, 100) < 30) {   //30% chance after that for breakable platform
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
                int angle = _rand.Next(5, 90);

                //Switch Direction
                if (_dice.Next(0, 100) < _zigzag) {
                    _isCurvedLeft = !_isCurvedLeft;
                }
                if (_isCurvedLeft) angle = -angle;  //make angle negative to turn left

                platform.eulerAngles = _previousPlatform.eulerAngles + Vector3.up * angle;
                _previousPlatform.GetComponent<Platform>().Next = platform.GetComponent<Platform>();

                Vector3 midPoint = (platform.position.withY((_platformCount - i) * _slope) + _previousPlatform.position) / 2;
        
                //Create Blades with 20% chance
                if (_dice.Next(0, 100) < 20) {
                    GameObject blades = Instantiate(BladesPrefab);
                    blades.transform.position = midPoint;
                    blades.transform.forward = _previousPlatform.forward;
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
            label.GetComponentInChildren<TextMeshPro>().text = (_platformCount - i).ToString();

            //Prepare for next iteration
            _previousPlatform = platform;

            //Spline 
            points[i] = new SplinePoint();
            points[i].position = _previousPlatform.position;
            points[i].normal = Vector3.up;
            points[i].size = 0.15f;
            points[i].color = Color.white;

            //Add to bounds
            _bounds.Encapsulate(platform.GetComponentInChildren<Renderer>().bounds);

        }
        //Create Bottom Platforms (on water)
        int xCount = (int)_bounds.size.x / 5;
        int zCount = (int)_bounds.size.z / 5;
        Vector3 origin = _bounds.min;
        for (int x = 0; x < xCount; x++) {
            for (int z = 0; z < zCount; z++) {
                GameObject bottomPlatform = Instantiate(BottomPlatformPrefab);
                bottomPlatform.transform.position = origin + new Vector3(x * 7, 0, z * 7) + Vector3.zero.RandomOffset(2,0,2);
            }
        }

        // Create Goal Platform
        {
            GameObject goal = Instantiate(GoalPrefab);
            goal.transform.rotation = _previousPlatform.rotation;
            goal.transform.position = (_previousPlatform.position + _previousPlatform.forward * _gapSize).withY(0);
            Vector3 midPoint = (goal.transform.position + _previousPlatform.position) / 2;

        }

        //Set Spline Points
        splineComputer.SetPoints(points);
        //Add spline renderer
        Spline.AddComponent<SplineRenderer>().spline = splineComputer;
        Spline.GetComponent<MeshRenderer>().material = SplineMaterial;
        GameObject.Destroy(splineComputer, 0.5f);
    }


    // Update is called once per frame
    void Update() {

    }
}
