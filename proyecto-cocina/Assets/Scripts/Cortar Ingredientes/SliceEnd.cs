using UnityEngine;

public class SliceEnd : MonoBehaviour
{
    public SlicePoint slicePoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<KnifeController>() == null)
            return;

        slicePoint.TerminarCorte();
    }
}