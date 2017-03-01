using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SceneList {
	public static List<SceneProperties> listScenes = new List<SceneProperties>();
	public static int currentListIndex;

	static SceneList()
	{
		currentListIndex = 0;

		for(int i = 0; i <= 4; i++) {
			SceneProperties sp = new SceneProperties ();
			int sceneNumber = i + 1;

			sp.sceneName = "scene" + sceneNumber;
			sp.sceneIndex = i;
			sp.isZoomed = false;
			sp.slug = "scene" + sceneNumber + "_";
			sp.audioLength = 2.0f;

			listScenes.Add (sp);
		}
	}				
}

public class SceneProperties
{
	public string sceneName {get; set;}
	public int sceneIndex { get; set; }
	public bool isZoomed { get; set; }
	public string slug { get; set; }
	public float audioLength { get; set; }
}
