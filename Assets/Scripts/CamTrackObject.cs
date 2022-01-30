using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrackObject : MonoBehaviour
{
    // Start is called before the first frame update
    //Only used for testing bullet, basic script that tracks and object without having to be it's child

    public GameObject objectToTrack;

    private void Update()
    {
        if (objectToTrack)
        {
            Vector3 pos = objectToTrack.transform.localPosition;
            transform.localPosition = new Vector3(pos.x, pos.y, pos.z - 10);
        }
    }
}
