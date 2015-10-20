﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager {

	private bool first = true;
	public void StartupHost(){
		SetPort ();
		NetworkManager.singleton.StartHost ();
	}

	public void JoinGame(){
		SetIPAddress ();
		SetPort ();
		NetworkManager.singleton.StartClient ();
	}

	void SetPort(){
		NetworkManager.singleton.networkPort = 7777;
	}

	void SetIPAddress(){
		string ipAddress = GameObject.Find ("InputFieldIPAddress").transform.FindChild ("Text").GetComponent<Text> ().text;
		NetworkManager.singleton.networkAddress = ipAddress;
	}

	void OnLevelWasLoaded(int level){
		if (level == 1 && !first)  {
//			Debug.Log ("lvl 1");
			StartCoroutine(SetupMenuSceneButtons());
		} else if (level == 2) {
//			Debug.Log ("lvl 2");
			SetupOtherSceneButtons ();
		}
		first = false;
	}

	IEnumerator SetupMenuSceneButtons(){
		yield return new WaitForSeconds (0.2f);
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonStartHost").GetComponent<Button> ().onClick.AddListener(StartupHost);

		GameObject.Find ("ButtonJoinGame").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonJoinGame").GetComponent<Button> ().onClick.AddListener(JoinGame);
	}

	void SetupOtherSceneButtons(){
		GameObject.Find ("ButtonDisconnect").GetComponent<Button> ().onClick.RemoveAllListeners ();
		GameObject.Find ("ButtonDisconnect").GetComponent<Button> ().onClick.AddListener(NetworkManager.singleton.StopHost);
	}
}