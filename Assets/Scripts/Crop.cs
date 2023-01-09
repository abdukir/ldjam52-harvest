using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Crop : MonoBehaviour
{
	public enum CropLevel
	{
		Level1, Level2, Level3
	}
	public CropLevel cropLevel;

	public int score;

	public List<QTA> QTAS= new List<QTA>(); // list of quick time actions


	private void Start()
	{
		GenerateQTAs();
	}

	[ContextMenu("testGen")]
	private void GenerateQTAs()
	{
		int QTAamount = 0;
		switch (cropLevel)
		{
			case CropLevel.Level1:
				QTAamount = 3;
				break;
			case CropLevel.Level2:
				QTAamount = 5;
				break;
			case CropLevel.Level3:
				QTAamount = 7;
				break;

		}
		for (int i = 0; i < QTAamount; i++)
		{
			switch (cropLevel)
			{
				case CropLevel.Level1:
					QTAS.Add((QTA)Random.Range(0, 2));
					break;
				case CropLevel.Level2:
					QTAS.Add((QTA)Random.Range(0, 3));
					break;
				case CropLevel.Level3:
					QTAS.Add((QTA)Random.Range(0, 4));
					break;
			}
			
		}
		
	}
}
