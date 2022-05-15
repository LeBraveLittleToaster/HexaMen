using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResizeCollector : MonoBehaviour
{
    [SerializeField] private TMP_InputField columnInputFieldRef;
    [SerializeField] private TMP_InputField rowInputFieldRef;
    [SerializeField] private HexaGridScript gridRef;

    public void Resize()
    {
        gridRef.ResizeGrid(
            int.Parse(columnInputFieldRef.text),
            int.Parse(rowInputFieldRef.text)
        );
    }
}