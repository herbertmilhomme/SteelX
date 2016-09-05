﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Photon.MonoBehaviour {
//	[SyncVar] public float TimeLeft;

	[SerializeField] GameObject PlayerPrefab;
	[SerializeField] GameObject Scoreboard;
	[SerializeField] GameObject PlayerStat;

	public Transform[] SpawnPoints;

	public float MaxTime = 10f;
	public int MaxKills = 2;
	public int CurrentMaxKills = 0;

	private bool showboard = false;

	private Dictionary<string, GameObject> playerScorePanels;
	public Dictionary<string, Score> playerScores;

	void Start() {
		GameObject player = PhotonNetwork.Instantiate (PlayerPrefab.name, SpawnPoints[0].position, SpawnPoints[0].rotation, 0);
		BuildMech mechBuilder = player.GetComponent<BuildMech>();
		Mech m = UserData.myData.Mech;
		mechBuilder.Build(m.Core, m.Arms, m.Legs, m.Head, m.Booster, m.Weapon1L, m.Weapon1R, m.Weapon2L, m.Weapon2R);
		playerScorePanels = new Dictionary<string, GameObject> ();
	}
		
	public void RegisterPlayer(string name) {
		if (playerScores == null) {
			playerScores = new Dictionary<string, Score>();
		}
		playerScores.Add (name, new Score ());

		GameObject ps = Instantiate (PlayerStat, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		ps.transform.FindChild ("Pilot Name").GetComponent<Text> ().text = name;
		ps.transform.FindChild ("Kills").GetComponent<Text> ().text = "0";
		ps.transform.FindChild ("Deaths").GetComponent<Text> ().text = "0";
		ps.transform.SetParent(Scoreboard.transform.FindChild ("Team1").transform);
		ps.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);
		playerScorePanels.Add (name, ps);
	}

	void Update() {
		Scoreboard.SetActive(Input.GetKey(KeyCode.Tab));
	}

//	void OnGUI() {
//		if (showboard || GameOver()) {
//			GUILayout.BeginArea(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2));
//			foreach (KeyValuePair<string, Score> entry in playerScores)
//			{
//				GUILayout.Label(entry.Key + ": kills = " + entry.Value.Kills + ", deaths = " + entry.Value.Deaths);
//			}
//			GUILayout.EndArea();
//		}
//	}

	public void RegisterKill (string shooter, string victim) {
		Debug.Log(shooter + " killed " + victim);
		Score newShooterScore = new Score ();
		newShooterScore.Kills = playerScores [shooter].Kills + 1;
		newShooterScore.Deaths = playerScores [shooter].Deaths;
		playerScores [shooter] = newShooterScore;

		Score newVictimScore = new Score ();
		newVictimScore.Kills = playerScores [victim].Kills;
		newVictimScore.Deaths = playerScores [victim].Deaths + 1;
		playerScores [victim] = newVictimScore;

		playerScorePanels [shooter].transform.FindChild ("Kills").GetComponent<Text> ().text = playerScores [shooter].Kills.ToString();
		playerScorePanels [victim].transform.FindChild ("Deaths").GetComponent<Text> ().text = playerScores [victim].Deaths.ToString();
		Debug.Log (shooter + " has " + playerScores [shooter].Kills + " kills.");
		Debug.Log (victim + " has " + playerScores [victim].Deaths + " deaths.");
	}

	public bool GameOver(){
		return CurrentMaxKills >= MaxKills;
	}
}