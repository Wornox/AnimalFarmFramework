using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.Networking;

[RequireComponent(typeof(Button))]
public class LoadHeightmap : MonoBehaviour, IPointerDownHandler
{
    //public RawImage output;
    public GameObject terrain;
    //public GameObject SceneScripts;


#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", "png, jpg", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Awake()
    {
        //terrain = TerrainController.generatedTerrainData;
    }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Choose a heightmap", "", extensions, false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutine(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                Texture2D hMapIn = DownloadHandlerTexture.GetContent(uwr);
                terrain.GetComponent<TerrainGenerator>().GenerateHeightMap(hMapIn);

            }
        }
    }
}