﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.ComponentModel;

public class HUDCustom : MonoBehaviour
{
	public NetworkManager manager;
	[SerializeField] public bool showGUI = true;
	[SerializeField] public int offsetX;
	[SerializeField] public int offsetY;
	[SerializeField] GameObject CreateRoomModal;
	[SerializeField] Button[] lobbyButtons;
	[SerializeField] Button[] createRoomButtons;

	// Runtime variable
	bool m_ShowServer;
	bool creatingRoom = false;

	int xpos;
	int ypos;
	const int spacing = 24;

	int roomHeight = 50;

	private Boolean showCreateRoom;

	[SerializeField] GameObject panel;
	[SerializeField] Transform content;

	GameObject[] rooms;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
		manager.StartMatchMaker();
		manager.matchMaker.ListMatches(0, 20, "", manager.OnMatchList);

		showCreateRoom = false;

		lobbyButtons [0].onClick.AddListener (toggleCreateRoom);

		lobbyButtons[2].onClick.AddListener(refresh);
		createRoomButtons [0].onClick.AddListener (toggleCreateRoom);
		createRoomButtons [1].onClick.AddListener (createRoom);
	}

	void Start() {
		if (manager.matches != null) {
			updateRooms ();
		}
	}
		
	void refresh() {
		manager.matchMaker.ListMatches (0, 20, "", manager.OnMatchList);
		updateRooms ();
	}

	void updateRooms() {
		Debug.Log ("Updating Rooms");
		if (rooms != null) {
			for (int i = 0; i < rooms.Length; i++) {
				Destroy (rooms [i]);
			}
		}
		rooms = new GameObject[manager.matches.Count];
		for (int i = 0; i < manager.matches.Count; i++) {
			GameObject roomPanel = Instantiate (panel);
			roomPanel.transform.SetParent(content);
			RectTransform rt = roomPanel.GetComponent<RectTransform> ();
			rt.localPosition = new Vector3(0, -1*roomHeight*i, 0);
			rt.localScale = new Vector3 (1, 1, 1);
			rooms [i] = roomPanel;
		}
		content.GetComponent<RectTransform> ().sizeDelta = new Vector2 (600, 50 * manager.matches.Count);
	}

	public void toggleCreateRoom() {
		showCreateRoom = !showCreateRoom;
		CreateRoomModal.SetActive (showCreateRoom);
		for (int i = 0; i < 3; i++) {
			lobbyButtons [i].interactable = !showCreateRoom;
		}
	}

	public void createRoom() {
		manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", manager.OnMatchCreate);
	}
//	void Update()
//	{
//		if (!showGUI)
//			return;
//
//		if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
//		{
//			if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
//			{
//				if (Input.GetKeyDown(KeyCode.S))
//				{
//					manager.StartServer();
//				}
//				if (Input.GetKeyDown(KeyCode.H))
//				{
//					manager.StartHost();
//				}
//			}
//			if (Input.GetKeyDown(KeyCode.C))
//			{
//				manager.StartClient();
//			}
//		}
//		if (NetworkServer.active && manager.IsClientConnected())
//		{
//			if (Input.GetKeyDown(KeyCode.X))
//			{
//				manager.StopHost();
//			}
//		}
//	}

	void OnGUI()
	{	
		if (manager.matchMaker == null) {
			manager.StartMatchMaker();
		}
		xpos = 10 + offsetX;
		ypos = 40 + offsetY;

		if (!showGUI)
			return;

		bool noConnection = (manager.client == null || manager.client.connection == null ||
			manager.client.connection.connectionId == -1);

		if (NetworkServer.active || manager.IsClientConnected()) {
			if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Back")) {
				manager.StopHost();
				creatingRoom = false;
			}
			ypos += spacing;
		}

		if (!NetworkServer.active && !manager.IsClientConnected() && noConnection) {
			ypos += 10;

			if (manager.matchInfo == null) {
				if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Create Room")) {
					creatingRoom = true;		
				}
				if (creatingRoom) {
					GUI.ModalWindow(0, new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2), createRoomModal, "Create Room");
				}
				ypos += spacing;

				if (GUI.Button (new Rect (xpos, ypos, 200, 20), "Refresh")) {
					manager.matchMaker.ListMatches(0, 20, "", manager.OnMatchList);
					Debug.Log ("Match Num: " + ((manager == null || manager.matches == null) ? 0 : manager.matches.Count));
				}
				ypos += spacing;

				if (manager.matches != null) {
					foreach (var match in manager.matches) {
						if (GUI.Button (new Rect (xpos, ypos, 200, 20), "Join Match:" + match.name)) {
							manager.matchName = match.name;
							manager.matchSize = (uint)match.currentSize;
							manager.matchMaker.JoinMatch (match.networkId, "", manager.OnMatchJoined);
						}
						ypos += spacing;
					}
				}
			}
		}
	}

	void createRoomModal(int id) {
		xpos = 10 + offsetX;
		ypos = 40 + offsetY;

		if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Create")) {
			manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", manager.OnMatchCreate);
		}

		xpos += 150;
		GUI.Label(new Rect(xpos, ypos, 100, 20), "Room Name:");
		manager.matchName = GUI.TextField(new Rect(xpos + 100, ypos, 100, 20), manager.matchName);

		xpos -= 150;
		ypos += spacing;
		if (GUI.Button(new Rect(xpos, ypos, 100, 20), "Cancel")) {
			creatingRoom = false;
		}
	}
}