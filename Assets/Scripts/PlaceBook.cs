using UnityEngine;
using System.Threading.Tasks;

public class PlaceBook : MonoBehaviour
{
    public static bool placing,isKeyboardOpen;
    public static bool createAnchor;
    string keyword = "";
    
    int layerMask = 1 << 8;
    AzureSpatialAnchorsScript AnchorManagerScript;
    
    void Start()
    {
        AnchorManagerScript = GameObject.Find("AzureSpatialAnchorManager").GetComponent<AzureSpatialAnchorsScript>();
        placing = true; 
        layerMask = ~layerMask;
        createAnchor = false;
        isKeyboardOpen = false;
    }

    public async void Selected()
    {
        placing = !placing;
        await ConfirmAnchor(GameManager.Instance.createCubeCounter.ToString());
        Debug.Log("clicked on select");
    }

    async Task<long> ConfirmAnchor(string keyword)
    {
        GameManager.Instance.createCubeCounter ++;
        if(GameManager.Instance.createCubeCounter < 9)
        {
            GameManager.Instance.CreateCube();
        }
        else if(GameManager.Instance.createCubeCounter == 9)
        {
            AnchorManagerScript.RemoveAllAnchorGameObjects();
        }
        await AnchorManagerScript.CreateAnchor(this.gameObject, keyword);
        return 1;
    }

    void Update()
    {
        // If the user is in placing mode,
        // update the placement to match the user's gaze.
        if (placing)
        {
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;
            RaycastHit hitInfo;

            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, layerMask))
            {
                // Move this object's parent object to
                // where the raycast hit the Spatial Mapping mesh.
                this.transform.position = hitInfo.point;

                // Rotate this object's parent object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
                this.transform.rotation = toQuat;
            }
        }
    }
}