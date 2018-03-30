using UnityEngine;

using RPG.Weapons;

namespace RPG.Characters
{
    public class MagicMissileBehaviour : AbilityBehaviour
    {
        const float MISSILE_SPEED = 10f;

        public override void Use(GameObject target, bool useTargetPosition)
        {
            base.Use(target, useTargetPosition);
            CastMissile(target);
        }

        private void CastMissile(GameObject target)
        {
            gameObject.transform.LookAt(target.transform);

            Vector3 positionToHit = target.transform.position;
            Vector3 unitVectorToTarget = (positionToHit - gameObject.transform.position).normalized;

            Vector3 positionToSpawnFrom = gameObject.transform.position;
            positionToSpawnFrom.y += 1f; // do not spawn from feet ;-)
            positionToSpawnFrom.z += unitVectorToTarget.z; // move missile away from player in the direction player is facing x
            positionToSpawnFrom.x += unitVectorToTarget.x; // move missile away from player in the direction player is facing z

            GameObject newProjectile = Instantiate(((MagicMissileConfig)config).GetMissile(), positionToSpawnFrom, Quaternion.identity);
            newProjectile.name = "Magic Missile";
            newProjectile.transform.LookAt(target.transform);

            Projectile projectile = newProjectile.GetComponent<Projectile>();
            projectile.SetProjectileTarget(target);
            projectile.SetDamageCaused( ((MagicMissileConfig)config).GetMissileDamage());

            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToTarget * MISSILE_SPEED;
        }
    }
}
