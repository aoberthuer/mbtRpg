using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

using RPG.Characters;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour
    {
        [SerializeField] Texture2D cursorWalk = null;
        [SerializeField] Texture2D cursorTarget = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        const int WALKABLE_LAYER = 8;
        const int ENEMY_LAYER = 13;


        float maxRaycastDepth = 250f; // Hard coded value
        Rect screenRectOnStart = new Rect(0, 0, Screen.width, Screen.height); // might need to resize in Update() as rect is now static
       

        // Setup delegates for broadcasting mouse events to other classes
        public delegate void OnMouseOverEnemy(EnemyAI enemy); // declare new delegate type
        public event OnMouseOverEnemy onMouseOverEnemy; // instantiate an observer set

        public delegate void OnMouseOverWalkable(Vector3 destination); // declare new delegate type
        public event OnMouseOverWalkable onMouseOverWalkable; // instantiate an observer set


        void Update()
        {
            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // implement UI interaction here
                return; // then stop looking for other objects
            }
            else
            {
                PerformRaycasts();    
            }
        }

        private void PerformRaycasts()
        {
            if (screenRectOnStart.Contains(Input.mousePosition))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Order matters!
                if (RaycastForEnemy(ray))
                {
                    return;
                }

                if (RaycastForWalkable(ray))
                {
                    return;
                }
            }
        }

        // If enemy is blocked by child object (or rather child object's collider) like e.g. audio trigger, this one needs to be added to 
        // raycast ignore layer. Alternatively the other method RaycastForEnemyAo() may be used.
        private bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask enemyLayerMask = 1 << ENEMY_LAYER;

            Physics.Raycast(ray, out hitInfo, maxRaycastDepth, enemyLayerMask);
            if(hitInfo.collider == null)
            {
                return false;
            }

            GameObject gameObjectHit = hitInfo.collider.gameObject;
            EnemyAI enemyHit = gameObjectHit.GetComponent<EnemyAI>();

            if (enemyHit != null)
            {
                Cursor.SetCursor(cursorTarget, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemyHit);
                return true;
            }

            return false;
        }


        // Recognize enemies by their enemy script component, even if blocked by something else 
        // private bool RaycastForEnemyAo(Ray ray)
        // {
        //    Enemy enemyHit = null;

        //    RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);
        //    foreach (RaycastHit element in raycastHits)
        //    {
        //        GameObject gameObjectHit = element.collider.gameObject;
        //        enemyHit = gameObjectHit.GetComponent<Enemy>();

        //        if(enemyHit != null)
        //        {
        //            break;
        //        }
        //    }

        //    if (enemyHit != null)
        //    {
        //        Cursor.SetCursor(cursorTarget, cursorHotspot, CursorMode.Auto);
        //        onMouseOverEnemy(enemyHit);
        //        return true;
        //    }

        //    return false;
        // }


        // Recognize walkable by layer
        private bool RaycastForWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask walkableLayerMask = 1 << WALKABLE_LAYER;
            bool walkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, walkableLayerMask);

            if(walkableHit)
            {
                Cursor.SetCursor(cursorWalk, cursorHotspot, CursorMode.Auto);
                onMouseOverWalkable(hitInfo.point);
            }

            return walkableHit;
        }
    }
}