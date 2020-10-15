using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    IEnumerator LoadAsync(string sceneName, GameObject loadingBar)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //float progress = operation.progress;
            if(loadingBar != null)
            {
                //visual laoding bar
                loadingBar.SetActive(true);
                loadingBar.GetComponent<Slider>().value = progress;
                var p = loadingBar.transform.Find("Progress");
                if(p != null)
                {
                    //if(operation.progress <0.8) 
                    p.GetComponent<TextMeshProUGUI>().text = progress * 100 + " %";
                    //else p.GetComponent<TextMeshProUGUI>().text = progress * 100 + " %: LoadingNavMesh";

                }
            }
            Debug.Log(progress);

            yield return null;
        }
    }

    public void LoadSampleScene(GameObject loadingBar)
    {
        StartCoroutine(LoadAsync("SampleScene", loadingBar));
    }

    public void LoadMapGeneratorScene()
    {
        SceneManager.LoadSceneAsync("MapGeneratorScene");
    }

    public void LoadMainMenuScene()
    {
        var s = SceneManager.LoadSceneAsync("MainMenu");
    }

    public void LoadCustomScene(GameObject loadingBar)
    {
        StartCoroutine(LoadAsync("CustomScene", loadingBar));
    }

    private void move(string scene)
    {
        //var mapGenscript = GetComponent<MapGeneratorGlobalScripts>();
        var terrain = GameObject.Find("GeneratedWater");
        if (terrain != null)
        {
            Scene newscene = SceneManager.GetSceneByName(scene);
            SceneManager.MoveGameObjectToScene(terrain, newscene);
        }
    }
}
