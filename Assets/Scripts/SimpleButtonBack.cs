using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleButtonBack : MonoBehaviour {

	AsyncOperation ao;
	bool loading = false;

	void Update() {
		if (loading) {
			if (ao.progress >= 0.9f) {
				ao.allowSceneActivation = true;
			}
		}
	}

	public void ReturnTo2D() {
		loading = true;
		StartCoroutine(Load2D ());
	}

	IEnumerator Load2D() {
		ao = SceneManager.LoadSceneAsync ("scene_main");
		ao.allowSceneActivation = false;
		yield return ao;
	}
}
