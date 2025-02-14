using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneChanger : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName = "Def_LvL1";
    public GameObject fadeObject;
    public float fadeDuration = 1.5f;

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(PlayFadeAndChangeScene());
    }

    private System.Collections.IEnumerator PlayFadeAndChangeScene()
    {
        if (fadeObject != null)
        {
            fadeObject.SetActive(true);
            yield return new WaitForSeconds(fadeDuration);
        }

        SceneManager.LoadScene(nextSceneName);
    }
}