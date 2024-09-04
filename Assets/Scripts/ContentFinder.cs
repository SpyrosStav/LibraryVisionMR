using UnityEngine;
using TMPro;
using System;

public class ContentFinder : MonoBehaviour
{
    public string keywordID;
    string content;
    MeshRenderer[] childrenMesh;
    public TextMeshProUGUI contentText;
    public GameObject slatePrefab;
    ClientScript script;
    // Start is called before the first frame update
    void Start()
    {
        content = "";
        script = GameObject.Find("AzureSpatialAnchorManager").GetComponent<ClientScript>();
    }

    public async void Selected()
    {
        Debug.Log(keywordID);
        content = await script.RetrieveContent(keywordID);   
        Debug.Log(content);
        slatePrefab.gameObject.SetActive(true);
        contentText.text = content;
        childrenMesh = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        GameManager.Instance.gameCounter++;
        foreach(MeshRenderer child in childrenMesh)
        {
            child.enabled = false;
        }
    }

    void Update()
    {
        try
        {
            contentText.text = content;
        }catch(Exception e)
        {
            //Debug.Log(e.ToString());
        }
    }
}
