using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;

namespace RPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AICharacterControl))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class PlayerMovement : MonoBehaviour
    {

        private ThirdPersonCharacter thirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object
        private CameraRaycaster cameraRaycaster = null;

        private AICharacterControl aiCharacterControl = null;

        private Vector3 clickPoint;

        private bool isInDirectMode = false;

        private GameObject walkTarget = null;

        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();

            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;

            thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
            aiCharacterControl = GetComponent<AICharacterControl>();

            walkTarget = new GameObject("Walk target");
        }

        private void OnMouseOverEnemy(Enemy enemy)
        {
            if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(1))
            {
                aiCharacterControl.SetTarget(enemy.transform);
            }
        }

        private void OnMouseOverWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                walkTarget.transform.position = destination;
                aiCharacterControl.SetTarget(walkTarget.transform);
            }
        }

        private void ProcessDirectMovement()
        {
            // read inputs
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 movement = v * cameraForward + h * Camera.main.transform.right;

            thirdPersonCharacter.Move(movement, false, false);
        }
    }
}

