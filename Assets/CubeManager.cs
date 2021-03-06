using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]

public class CubeManager : Singleton<CubeManager>
{
    [SerializeField]
    private Camera arCamera;

    private ARRaycastManager arRaycastManager = null;

    [SerializeField]
    public GameObject placedPrefab;

    public GameObject textPrefab;

    public GameObject imagePrefab;
    
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    private GameObject placedGameObject = null;
    
    public Text text;
    
    private List<GameObject> objectsCreated = new List<GameObject>();


    private ARAnchorManager arAnchorManager = null;

    public enum noteType{
        MODEL3D,
        TEXT,
        IMAGE
    }

    private noteType currentNoteType = noteType.MODEL3D;

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchPosition = touch.position;


                bool isOverUI = touchPosition.IsPointOverUIObject();

                return isOverUI ? false : true;
            }
        }

        touchPosition = default;

        return false;
    }

    public void RemovePlacements()
    {
        //Destroy(placedGameObject);
        placedGameObject.SetActive(false);
        for(int i = 0; i < objectsCreated.Count; ++i)
        {
            objectsCreated[i].SetActive(false);
        }
    }

    public void add3DModel()
    {
        currentNoteType = noteType.MODEL3D;
    }

    public void addText()
    {
        currentNoteType = noteType.TEXT;
    }

    public void addImage()
    {
        currentNoteType = noteType.IMAGE;
    }

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arAnchorManager = GetComponent<ARAnchorManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition) || EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit[] raycastHits = Physics.RaycastAll(ray);
        if (raycastHits.Length > 1)
        {
            return;
        }
            if (placedGameObject != null && arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            //    return;
            var hitPose = hits[0].pose;
            GameObject temp = null;
            if (currentNoteType == noteType.MODEL3D)
                temp = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            else if (currentNoteType == noteType.TEXT)
                temp = Instantiate(textPrefab, hitPose.position, hitPose.rotation);
            else if (currentNoteType == noteType.IMAGE)
                temp = Instantiate(imagePrefab, hitPose.position, hitPose.rotation);
            objectsCreated.Add(temp);
        }

        else if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            if (currentNoteType == noteType.MODEL3D)
                placedGameObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            else if (currentNoteType == noteType.TEXT)
                placedGameObject = Instantiate(textPrefab, hitPose.position, hitPose.rotation);
            else if(currentNoteType == noteType.IMAGE)
                placedGameObject = Instantiate(imagePrefab, hitPose.position, hitPose.rotation);

            var anchor = arAnchorManager.AddAnchor(new Pose(hitPose.position, hitPose.rotation));
            placedGameObject.transform.parent = anchor.transform;

            // this won't host the anchor just add a reference to be later host it
            ARCloudAnchorManager.Instance.QueueAnchor(anchor);
        }
    }
    /*private void CreateCube(Vector3 position)
    {
        lastObject = Instantiate(cubePrefab, position, Quaternion.identity);
        objectCreated.Add(lastObject);
    }

    private void DeleteCube(GameObject cubeObject)
    {
        //Destroy(cubeObject);
        GameObject.Destroy(cubeObject);
        Debug.Log("In cube delete, after destroy");
    }*/

    public void ReCreatePlacement(Transform transform)
    {
        //placedGameObject = Instantiate(placedPrefab, transform.position, transform.rotation);
        placedGameObject.transform.parent = transform;
        placedGameObject.transform.position = transform.position;
        placedGameObject.transform.rotation = transform.rotation;
        placedGameObject.SetActive(true);

        for(int i = 0; i < objectsCreated.Count; ++i)
        {
            objectsCreated[i].SetActive(true);
            //objectsCreated[i] = Instantiate(placedPrefab, transform.position, transform.rotation);
            //objectsCreated[i].transform.parent = transform;
        }
    }

}
