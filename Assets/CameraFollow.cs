using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float smoothing = 5f;
	Vector3 offset;

	public static CameraFollow instance;

    void Awake()
    {
		Debug.Log("contact");
		instance = this;
    }
    // Use this for initialization
    void Start()
	{
		offset = transform.localPosition;
		Debug.Log("my offset: " + offset);
	}

	public void TellCameraToFollowMe(Transform me)
    {
		target = me;
		offset = transform.position - target.position;
		Debug.Log(offset);
		Debug.Log(target);
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (target == null) { return; }
		Vector3 targetCamPos = target.position + offset;
		transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}