﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour {

	//camera
	public Camera camera;

	//UI Elements
	public Text loadText;
	public Button worldRepButton;
	public Button nextSceneButton;
	private Text worldRepText;
	private Text nextSceneText;
	public Slider heuristicSlider;
	public Text ManhattanLabel;
	public Text EuclideanLabel;

	//Script References
	public GameObject gameMap;
	private AStarSearch aStar;
	private WaypointCreation wpc;

	//bools for toggling buttons
	private bool isTile;
	private bool isManhattan;

	//new map to be loaded
	public string nextSceneName;

	// Use this for initialization
	void Start () {
		loadText.text = "Loading, Please Wait...";
		worldRepButton.gameObject.SetActive (false);
		nextSceneButton.gameObject.SetActive (false);
		heuristicSlider.gameObject.SetActive (false);
		ManhattanLabel.gameObject.SetActive (false);
		EuclideanLabel.gameObject.SetActive (false);
		worldRepText = worldRepButton.gameObject.GetComponentInChildren<Text> ();
		nextSceneText = nextSceneButton.gameObject.GetComponentInChildren<Text> ();
		worldRepButton.onClick.AddListener (toggleWorld);
		nextSceneButton.onClick.AddListener (loadNextScene);

		aStar = gameMap.GetComponent<AStarSearch> ();
		wpc = gameMap.GetComponent<WaypointCreation> ();

		isTile = true;
		isManhattan = false;
	}

	public void activateButtons(){
		//Once everything is loaded, sets UI elements on
		loadText.text = "";
		worldRepButton.gameObject.SetActive (true);
		worldRepText.text = "Tile View";
		nextSceneButton.gameObject.SetActive (true);
		nextSceneText.text = "Switch Maps";
		heuristicSlider.gameObject.SetActive (true);
		ManhattanLabel.gameObject.SetActive (true);
		EuclideanLabel.gameObject.SetActive (true);
	}

	void toggleWorld(){
		if (isTile) {
			worldRepText.text = "Waypoint View";
			camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z + wpc.mapOffset);
			isTile = false;
		} else {
			worldRepText.text = "Tile View";
			camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z - wpc.mapOffset);
			isTile = true;
		}
	}

	void loadNextScene (){
		SceneManager.LoadScene (nextSceneName);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
