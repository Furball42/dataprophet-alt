using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour {
	public SpriteRenderer bookImage;
	public Image fadeImage;
	public AudioSource narrationSource;
	public float narrationSpan;
	public float zoomSpan;
	public Camera mainCamera;
	public float FOVStart;
	public float FOVEnd;
	public float fadeSpeed;

	int zoomDirection = 1;
	bool fadingToBlack = false;
	bool fadingToClear = false;
	bool narration = false;
	bool zooming = false;

	float narrationTimer = 0.0f;
	float zoomTimer = 0.0f;

	AsyncOperation asyncLoad;

	void Awake() {
		fadeImage.rectTransform.localScale = new Vector2 (Screen.width, Screen.height);
		fadeImage.enabled = false;
	}

	// Use this for initialization
	void Start () {
		LoadScene ();
	}

	// Update is called once per frame
	void Update () {	

		if (narration) {
			narrationTimer += Time.deltaTime;
			MonitorNarration ();
		}

		if (zooming) {
			zoomTimer += Time.deltaTime;
			Zoom ();
		}

		if (fadingToBlack) {
			FadeToBlack ();
			Debug.Log (fadeImage.color.a);
			if (fadeImage.color.a >= 0.95f && asyncLoad.progress >= 0.9f) {
				TransitionScene ();
				fadingToBlack = false;
			}
		}

		if (fadingToClear) {
			FadeToClear ();
			if (fadeImage.color.a <= 0.05f) {
				fadeImage.enabled = false;
				fadingToClear = false;
			}
		}
	}

	void LoadScene() {

		if (narrationSpan == 0.0f)
			narrationSpan = SceneList.listScenes [SceneList.currentListIndex].audioLength;

		if (!SceneList.listScenes [SceneList.currentListIndex].isZoomed) {
			string spriteName = SceneList.listScenes [SceneList.currentListIndex].slug + "book";
			Sprite newSprite = Resources.Load<Sprite> ("Sprites/" + spriteName);
			bookImage.sprite = newSprite;
			PlayNarration ();
		} else {
			string spriteName = SceneList.listScenes [SceneList.currentListIndex].slug + "book";
			Sprite newSprite = Resources.Load<Sprite> ("Sprites/" + spriteName);
			bookImage.sprite = newSprite;
			mainCamera.fieldOfView = FOVEnd;
			fadeImage.color = Color.black;
			fadeImage.enabled = true;
			fadingToClear = true;
			ZoomOut ();
		}
	}

	//NARRATION
	void PlayNarration() {
		string clipName = SceneList.listScenes [SceneList.currentListIndex].slug + "audio";
		AudioClip clip = Resources.Load<AudioClip> ("Audio/" + clipName);
		narrationSource.PlayOneShot (clip);
		narration = true;
	}

	void StopNarration() {
		narrationSource.Stop ();
		narrationTimer = 0.0f;
		narration = false;
	}

	void MonitorNarration() {
		if (narrationTimer >= narrationSpan) {
			StopNarration ();
			//start zoom
			//start preload of 3d scene
			ZoomIn();
			Load3DScene();
		}
	}

	//ZOOM
	void ZoomIn() {
		zoomDirection = 1;
		zooming = true;
	}

	void ZoomOut() {
		zoomDirection = -1;
		zooming = true;
	}

	void Zoom() {
		if (zoomTimer < zoomSpan) {
			if (zoomDirection == 1) {
				mainCamera.fieldOfView = Mathf.Lerp (FOVStart, FOVEnd, zoomTimer / zoomSpan);
			} else {
				mainCamera.fieldOfView = Mathf.Lerp (FOVEnd, FOVStart, zoomTimer / zoomSpan);
			}
		} else {

			zooming = false;
			zoomTimer = 0.0f;
			if (zoomDirection == 1) {
				SceneList.listScenes [SceneList.currentListIndex].isZoomed = true;
				if (asyncLoad.progress >= 0.9f) {
					//TransitionScene ();
					fadeImage.enabled = true;
					fadeImage.color = Color.clear;
					fadingToBlack = true;
				}
			} else {
				SceneList.listScenes [SceneList.currentListIndex].isZoomed = false;
				LoadNextPage ();
			}
		}
	}

	//FADING
	void FadeToBlack() {
		fadeImage.color = Color.Lerp (fadeImage.color, Color.black, fadeSpeed * Time.deltaTime);
	}

	void FadeToClear() {
		fadeImage.color = Color.Lerp (fadeImage.color, Color.clear, fadeSpeed * Time.deltaTime);
	}

	//LOADING SCENES
	void Load3DScene() {
		string scene3Dname = SceneList.listScenes [SceneList.currentListIndex].slug + "3d";
		Debug.Log (scene3Dname);
		StartCoroutine (AsyncLoad3DScene (scene3Dname));
	}

	IEnumerator AsyncLoad3DScene(string _sceneName) {	
		asyncLoad = SceneManager.LoadSceneAsync (_sceneName, LoadSceneMode.Single);
		asyncLoad.allowSceneActivation = false;
		yield return asyncLoad;
	}

	void TransitionScene() {
		asyncLoad.allowSceneActivation = true;
	}

	void LoadNextPage() {
		SceneList.currentListIndex++;
		if (SceneList.currentListIndex < SceneList.listScenes.Count)
			LoadScene ();
	}
}
