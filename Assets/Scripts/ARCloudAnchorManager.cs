using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class UnityEventResolver : UnityEvent<Transform> { }

public class ARCloudAnchorManager : Singleton<ARCloudAnchorManager>
{
    [SerializeField]
    private Camera arCamera = null;

    [SerializeField]
    private float resolveAnchorPassedTimeout = 10.0f;

    private ARAnchorManager arAnchorManager = null;

    private ARAnchor pendingHostAnchor = null;

    private ARCloudAnchor cloudAnchor = null;

    private string anchorToResolve;

    private bool anchorUpdateInProgress = false;

    private bool anchorResolveInProgress = false;

    private float safeToResolvePassed = 0;

    public Text infoText;

    private UnityEventResolver resolver = null;
    private void Awake()
    {
        resolver = new UnityEventResolver();
        resolver.AddListener((t) => CubeManager.Instance.ReCreatePlacement(t));
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position,
            arCamera.transform.rotation);
    }

#region Anchor Cycle

    public void QueueAnchor(ARAnchor arAnchor)
    {
        pendingHostAnchor = arAnchor;
    }

    public void HostAnchor()
    {
        textManager.current.write("HostAnchor call in progress");

        //recomended up to 30 sec of scanning
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        
        textManager.current.write("Feature map quality is :" + quality.ToString());
        textManager.current.write("Pending anchor :" + pendingHostAnchor.ToString());



        cloudAnchor = arAnchorManager.HostCloudAnchor(pendingHostAnchor, 1);

        

        if(cloudAnchor == null)
        {
            textManager.current.write("Unable to host cloud anchor");

        }
        else
        {
            anchorUpdateInProgress = true;
        }
    }

    public void Resolve()
    {
        textManager.current.write("Resolve call in progress");
        cloudAnchor = arAnchorManager.ResolveCloudAnchorId(anchorToResolve);

        if (cloudAnchor == null)
        {
            textManager.current.write("Unable to resolve cloud anchor : " + anchorToResolve.ToString());

        }
        else
        {
            anchorResolveInProgress = true;
        }
    }

    private void CheckHostingProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;

        if(cloudAnchorState == CloudAnchorState.Success)
        {
            anchorUpdateInProgress = false;

            anchorToResolve = cloudAnchor.cloudAnchorId;

            textManager.current.write("Anchor Hosted successfully!");
        }
        else if(cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            textManager.current.write("Error while hosting cloud anchor : " + cloudAnchorState.ToString());
            anchorUpdateInProgress = false;
        }
    }

    private void CheckResolveProgress()
    {
        CloudAnchorState cloudAnchorState = cloudAnchor.cloudAnchorState;
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            textManager.current.write("Cloud anchor id : " + cloudAnchor.cloudAnchorId + " resolved!");
            anchorResolveInProgress = false;
            resolver.Invoke(cloudAnchor.transform);
            //resolver?.Invoke(cloudAnchor.transform);////        LOOK HERE TOMOROWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW

        }
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            textManager.current.write("Error while resolving cloud anchor : " + cloudAnchorState.ToString());
            anchorResolveInProgress = false;
        }
    }

#endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //check for host result
        if (anchorUpdateInProgress)
        {
            CheckHostingProgress();
            return;
        }

        //checking for resolve result
        if(anchorResolveInProgress && safeToResolvePassed <= 0)
        {
            safeToResolvePassed = resolveAnchorPassedTimeout;

            if (!string.IsNullOrEmpty(anchorToResolve))
            {
                textManager.current.write("Resolving Anchor id : " + anchorToResolve.ToString());
                CheckResolveProgress();
            }
        }
        else
        {
            safeToResolvePassed -= Time.deltaTime * 1.0f;
        }

    }
}
