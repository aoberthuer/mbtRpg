using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowSmoothed : MonoBehaviour {

    [SerializeField] GameObject player;

    private Vector3 offset;

    public float smoothTime = 0.5f; // Approximately the time it will take to reach the target. A smaller value will reach the target faster.
    private Vector3 velocity = Vector3.zero;


    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {

        Vector3 newTransformPosition = player.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newTransformPosition, ref velocity, smoothTime);
    }
}
