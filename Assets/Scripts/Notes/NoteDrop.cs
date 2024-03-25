using UnityEngine;

public class NoteDrop : MonoBehaviour
{
    public float time;
    public int noteSortOrder;
    public bool isFake = false;
}

public class NoteLongDrop : NoteDrop
{
    public float LastFor = 1f;
}