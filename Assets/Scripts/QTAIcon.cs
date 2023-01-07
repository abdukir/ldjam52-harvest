using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTAIcon : MonoBehaviour
{
    private Image icon;

    [SerializeField] private Sprite[] icons;
    public QTA qtaType;
    // Start is called before the first frame update
    void Start()
    {
        icon = GetComponent<Image>();
    }

    [ContextMenu("UpdateIcon")]
    public void UpdateIcon()
    {
		icon.sprite = icons[(int)qtaType];
	}
}
