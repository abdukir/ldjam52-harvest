using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

	public PlayerController player;
	
	
	private void Awake()
	{
		Instance = this;
	}


}
