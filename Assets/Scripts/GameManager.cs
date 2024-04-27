using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private MazeGenerator _mazeGenerator;
    public States state;

    [Header("GameObjects")]
    [SerializeField] private GameObject EndPoint;
    [SerializeField] private GameObject Seeker;
    [NonSerialized] public GameObject lastSeeker;
    [NonSerialized]public List<Seeker> seekers = new List<Seeker>();
    private GameObject mainSeeker;
    private GameObject EndP;

    [Header("State Materials")] 
    [SerializeField] private Material mainSeekerMaterial;
    [SerializeField] private Material disabled;
    [SerializeField] public Material stuck;
    [SerializeField] public Material Finish;

    [Header("ShowPath")] 
    public List<Vector3> pathSeekers = new List<Vector3>(); 

    [FormerlySerializedAs("sphere")]

    public Vector3 pointA;
    public Vector3 pointB;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _mazeGenerator = GetComponent<MazeGenerator>();
        StartState(States.GenerateMaze);
    }

    public void StartState(States newState)
    {
        switch (newState)
        {
            case States.GenerateMaze:
                state = States.GenerateMaze;
                GetComponent<MazeGenerator>().StartMaze();
                break;
            case States.SetTwoRandomPoints:
                state = States.SetTwoRandomPoints;
                SetPoints();
                break;
            case States.FindPathBetweenPoints:
                state = States.FindPathBetweenPoints;
                InstantiatePathFinders();
                break;
            case States.Finish:
                state = States.Finish;
                DisableSeekers();
                break;
            default:
                Debug.LogWarning("Tragic accident");
                break;
        }
    }
    private void DisableSeekers()
    {
        foreach (var seeker in seekers)
        {
            seeker.enabled = false;
            seeker.GetComponent<MeshRenderer>().material = disabled;
            
        }

        EndP.GetComponent<MeshRenderer>().material = Finish;

    }
    
 
    
    
    private void InstantiatePathFinders()
    {
        mainSeeker = Instantiate(Seeker, pointA, Quaternion.identity);
        mainSeeker.GetComponent<Seeker>().FillInfo(Seeker,true,null);
        mainSeeker.GetComponent<MeshRenderer>().material = mainSeekerMaterial;
        
       EndP = Instantiate(EndPoint, pointB, Quaternion.identity);
    }

    private void SetPoints()
    {
        Vector3[] corners = _mazeGenerator.Corners;
        float yPos = _mazeGenerator.FixedYPos;
        pointA = new Vector3((int)Random.Range(corners[0].x, corners[1].x), yPos, (int)Random.Range(corners[0].z, corners[1].z));
        pointB = new Vector3((int)Random.Range(corners[0].x, corners[1].x), yPos, (int)Random.Range(corners[0].z, corners[1].z));

        pointA.z += 0.55f; 
        pointB.z += 0.55f;
        
        StartState(States.FindPathBetweenPoints);
    }
}
public enum States
{
    GenerateMaze,
    SetTwoRandomPoints,
    FindPathBetweenPoints,
    Finish,
    
}
