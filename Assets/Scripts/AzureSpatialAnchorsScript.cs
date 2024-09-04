using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors;
using UnityEngine;

public class AzureSpatialAnchorsScript : MonoBehaviour
{
    public GameObject bookPrefab;
    public SpatialAnchorManager _spatialAnchorManager = null;
    public AudioClip audioClip;
    public AudioSource audioSource;

    List<GameObject> _foundOrCreatedAnchorGameObjects = new List<GameObject>();
    string[] anchorIdsToLocate;
    List<String> anchorIdsToDelete = new List<String>();
    string[] anchorsAfterSplit;
    public Transform cam;

    ClientScript clientScript;
    
    void Start()
    {
        clientScript = GetComponent<ClientScript>();
        cam = Camera.main.transform;

        _spatialAnchorManager = GetComponent<SpatialAnchorManager>();
        _spatialAnchorManager.LogDebug += (sender, args) => Debug.Log($"ASA - Debug: {args.Message}");
        _spatialAnchorManager.Error += (sender, args) => Debug.LogError($"ASA - Error: {args.ErrorMessage}");
        _spatialAnchorManager.AnchorLocated += SpatialAnchorManager_AnchorLocated;

        var sessionTask = _spatialAnchorManager.StartSessionAsync();
    }

    void Update()
    {
        gameObject.transform.position = cam.position + new Vector3(0.0415f,0.09f,0.213f);
        gameObject.transform.rotation = cam.rotation;
    }

    public async Task StartSession()
    {
        if(!_spatialAnchorManager.IsSessionStarted)
        {
            await _spatialAnchorManager.StartSessionAsync();
        }
    }

    public async Task CreateAnchor(GameObject anchorObject, string keyword)
    {
        long anchorNumber = 1;

        if(!_spatialAnchorManager.IsSessionStarted)
        {
            await _spatialAnchorManager.StartSessionAsync();
        }
        
        CloudNativeAnchor cloudNativeAnchor = anchorObject.AddComponent<CloudNativeAnchor>();
        cloudNativeAnchor.NativeToCloud();
        try{
            CloudSpatialAnchor cloudSpatialAnchor = cloudNativeAnchor.CloudAnchor;
            cloudSpatialAnchor.Expiration = DateTimeOffset.Now.AddDays(3);
            try
            {
               cloudSpatialAnchor.AppProperties[@"keyword"] = keyword;
            }catch(Exception e)
            {
            }

            while (!_spatialAnchorManager.IsReadyForCreate)
            {
                float createProgress = _spatialAnchorManager.SessionStatus.RecommendedForCreateProgress;
            }

            try
            {
                await _spatialAnchorManager.CreateAnchorAsync(cloudSpatialAnchor);

                bool saveSucceeded = cloudSpatialAnchor != null;
                if (!saveSucceeded)
                {
                    Debug.LogError("ASA - Failed to save, but no exception was thrown.");
                    return;
                }

                _foundOrCreatedAnchorGameObjects.Add(anchorObject);
                //anchorObject.GetComponent<MeshRenderer>().material.color = Color.green;
                audioSource.PlayOneShot(audioClip,0.5f);
                anchorNumber  = (await clientScript.StoreAnchorKey(cloudSpatialAnchor.Identifier, "Singleplayer"));
                long result = await clientScript.PostContent(keyword);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }catch(Exception e) {
        }

    }

    public /*async*/ void RemoveAllAnchorGameObjects()
    {
        foreach (var anchorGameObject in _foundOrCreatedAnchorGameObjects)
        {
            try{
                Destroy(anchorGameObject);
            }catch(Exception e){
                Debug.Log(e.ToString());
            }
        }
        _foundOrCreatedAnchorGameObjects.Clear();

        // foreach (var i in anchorIdsToDelete)
        // {
        //     CloudSpatialAnchor anchorToDelete = await _spatialAnchorManager.Session.GetAnchorPropertiesAsync(i);
        //     await _spatialAnchorManager.Session.DeleteAnchorAsync(anchorToDelete);
        // }
    }

    public void LocateAnchor()
    {   
        AnchorLocateCriteria anchorLocateCriteria = new AnchorLocateCriteria();
        anchorLocateCriteria.Identifiers = anchorIdsToLocate;      

        try
        {
            _spatialAnchorManager.Session.CreateWatcher(anchorLocateCriteria);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void SpatialAnchorManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        if (args.Status == LocateAnchorStatus.Located)
        {
            //Creating and adjusting GameObjects have to run on the main thread. We are using the UnityDispatcher to make sure this happens.
            UnityDispatcher.InvokeOnAppThread(() =>
            {
                string keyword = "";
                // Read out Cloud Anchor values
                CloudSpatialAnchor cloudSpatialAnchor = args.Anchor;
                try{
                    keyword = cloudSpatialAnchor.AppProperties[@"keyword"];
                }catch(Exception e) {

                }
                //Create GameObject
                GameObject anchorGameObject;
                try{
                    anchorGameObject = Instantiate(bookPrefab);
                    anchorGameObject.GetComponent<ContentFinder>().keywordID = keyword;
                
                    // Link to Cloud Anchor
                    anchorGameObject.AddComponent<CloudNativeAnchor>().CloudToNative(cloudSpatialAnchor);
                    _foundOrCreatedAnchorGameObjects.Add(anchorGameObject);

                }catch(Exception e ){
                }
            });
        }
    }

    #pragma warning disable CS1998
    public async Task ConfigureAnchorsToFind(string mode)
    #pragma warning restore CS1998
    {
        string anchorkeys = await clientScript.RetrieveAllAnchorKeys(mode);
        //string[] anchorIdsToLocate;

        anchorIdsToLocate = anchorkeys.Split(',');
        for(int i = 0; i < anchorIdsToLocate.Length; i++)
        {
            anchorIdsToLocate[i] = anchorIdsToLocate[i].Replace("[", "");
            anchorIdsToLocate[i] = anchorIdsToLocate[i].Replace("]","");
            anchorIdsToLocate[i] = anchorIdsToLocate[i].Replace("\"","");
        }
        return;
    }
}