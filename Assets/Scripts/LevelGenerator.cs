using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static Transform StartingPlatform;

    public GameObject PlatformPrefab;
    public GameObject MovingPlatformPrefab;
    public GameObject BreakablePlatformPrefab;
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
                platform = Instantiate(BreakablePlatformPrefab).transform;
            }
            else { 
                platform = Instantiate(PlatformPrefab).transform;
            }
            if (_previousPlatform != null)
            {
                if (_rand == null) Debug.Log("rand is null");
                platform.position = _previousPlatform.position + _previousPlatform.forward * _gapSize;
                int angle = _rand.Next(-35, 35);
                platform.Rotate(Vector3.up, angle);
                _previousPlatform.GetComponent<Platform>().Next = platform.GetComponent<Platform>();
            }
            else {
                //Set starting platform
                StartingPlatform = platform;
            }
            //Start high up and set goal near the water.
            platform.position = platform.position.withY((_platformCount - i) * _slope);

            Line.SetPosition(2 * i, platform.position);
            Vector3 midPoint = platform.position + platform.forward * _gapSize / 2;
            Line.SetPosition(2 * i + 1, midPoint);
            if (_dice.Next(0, 100) < 20) {
                GameObject blades = Instantiate(BladesPrefab, midPoint, transform.rotation);
            }

            //Prepare for next iteration
            _previousPlatform = platform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
