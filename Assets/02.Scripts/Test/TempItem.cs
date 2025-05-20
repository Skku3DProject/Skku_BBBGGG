using System;
using UnityEngine;
using UnityEngine.UI;

public class TempItem : MonoBehaviour
{
    public Sprite ItemImage;

    private void Start()
    {
        ItemImage = GetComponent<Image>().sprite;
    }
}
