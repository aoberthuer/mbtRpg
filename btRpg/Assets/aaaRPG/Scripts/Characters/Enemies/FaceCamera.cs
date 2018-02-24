using UnityEngine;

// Add a UI Socket transform to your enemy
// Attack this script to the socket
// Link to a canvas prefab that contains NPC UI

namespace RPG.Characters
{
    public class FaceCamera : MonoBehaviour
    {
        private Camera cameraToLookAt;

        void Start()
        {
            cameraToLookAt = Camera.main;
        }

        void LateUpdate()
        {
            transform.LookAt(cameraToLookAt.transform);
        }
    }
}