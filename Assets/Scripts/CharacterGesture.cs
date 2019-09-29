using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using PDollarGestureRecognizer;

public class CharacterGesture : MonoBehaviour
{

    public Transform gestureOnScreenPrefab;
    public Camera lineCamera;
    private List<Gesture> trainingSet = new List<Gesture>();
    private GameObject LocalPlayer;
    private List<Point> points = new List<Point>();
    private int strokeId = -1;

    private Vector3 virtualKeyPosition = Vector2.zero;
    private Rect drawArea;

    private RuntimePlatform platform;
    private int vertexCount = 0;

    private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
    private LineRenderer currentGestureLineRenderer;

    //GUI
    private string message;
    private bool recognized;
    private bool isDrawing = false;

    void Start()
    {

        platform = Application.platform;
        drawArea = new Rect(Screen.width / 5, 0, Screen.width - Screen.width * 2 / 5, Screen.height);
        LocalPlayer = gameObject;
        //Load pre-made gestures
        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureTemplates/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

        //Load user custom gestures
        //		string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        //		foreach (string filePath in filePaths)
        //			trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
    }

    void Update()
    {
        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

        if (drawArea.Contains(virtualKeyPosition))
        {

            if (Input.GetMouseButtonDown(0))
            {
                isDrawing = true;
                if (recognized)
                {
                    recognized = false;
                    strokeId = -1;
                    points.Clear();
                }

                ++strokeId;

                Transform tmpGesture = Instantiate(gestureOnScreenPrefab, new Vector3(0, 100, 0), Quaternion.Euler(0, 0, 0)) as Transform;
                currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                gestureLinesRenderer.Add(currentGestureLineRenderer);

                vertexCount = 0;
                points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));
            }

            if (Input.GetMouseButton(0) && isDrawing)
            {
                points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                currentGestureLineRenderer.SetVertexCount(++vertexCount);
                currentGestureLineRenderer.SetPosition(vertexCount - 1, lineCamera.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
            }
        }
    }
    public void clear()
    {
        foreach (LineRenderer lineRenderer in gestureLinesRenderer)
        {

            lineRenderer.SetVertexCount(0);
            Destroy(lineRenderer.gameObject);
        }
        gestureLinesRenderer.Clear();
    }
    void OnGUI()
    {
        GUI.backgroundColor = Color.clear;
        GUI.Box(drawArea, "");

        GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);
        try
        {
            if (Input.GetMouseButtonUp(0) && isDrawing)
            {

                recognized = true;
                isDrawing = false;
                Gesture candidate = new Gesture(points.ToArray());
                Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
                Debug.Log(gestureResult.GestureClass);
                if (gestureResult.Score > 0.9f && gestureResult.GestureClass != null)
                {
                    LocalPlayer.GetComponent<CharacterManager>().attack(Int32.Parse(gestureResult.GestureClass));
                }
                message = gestureResult.GestureClass + " " + gestureResult.Score;

                clear();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Not affect game play: " + e);
        }
    }
}
