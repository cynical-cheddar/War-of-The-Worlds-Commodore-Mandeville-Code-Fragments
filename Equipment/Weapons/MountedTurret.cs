using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountedTurret : Weapon
{
        [Header("Rotations")]

        [Tooltip("Transform of the turret's azimuthal rotations.")]
        [SerializeField] Transform turretBase = null;

        [Tooltip("Transform of the turret's elevation rotations. ")]
        [SerializeField] Transform barrels = null;

        [Header("Elevation")]
        [Tooltip("Speed at which the turret's guns elevate up and down.")]
        public float ElevationSpeed = 30f;

        [Tooltip("Highest upwards elevation the turret's barrels can aim.")]
        public float MaxElevation = 60f;

        [Tooltip("Lowest downwards elevation the turret's barrels can aim.")]
        public float MaxDepression = 5f;

        [Header("Traverse")]

        [Tooltip("Speed at which the turret can rotate left/right.")]
        public float TraverseSpeed = 60f;

        float normalTraverseSpeed = 60f;
        [Tooltip("When true, the turret can only rotate horizontally with the given limits.")]
        [SerializeField] bool hasLimitedTraverse = false;
        [Range(0, 179)] public float LeftLimit = 120f;
        [Range(0, 179)] public float RightLimit = 120f;

        [Header("Behavior")]

        [Tooltip("When idle, the turret does not aim at anything and simply points forwards.")]
        public bool IsIdle = false;

        [Tooltip("Position the turret will aim at when not idle. Set this to whatever you want" +
            "the turret to actively aim at.")]
        public Vector3 AimPosition = Vector3.zero;

        [Tooltip("When the turret is within this many degrees of the target, it is considered aimed.")]
        [SerializeField] private float aimedThreshold = 5f;
        float limitedTraverseAngle = 0f;

        [Header("Debug")]
        public bool DrawDebugRay = true;
        public bool DrawDebugArcs = false;

        float angleToTarget = 0f;
        float elevation = 0f;

        bool hasBarrels = false;

        bool isAimed = false;
        bool isBaseAtRest = false;
        bool isBarrelAtRest = false;

        /// <summary>
        /// True when the turret cannot rotate freely in the horizontal axis.
        /// </summary>
        public bool HasLimitedTraverse { get { return hasLimitedTraverse; } }

        /// <summary>
        /// True when the turret is idle and at its resting position.
        /// </summary>
        public bool IsTurretAtRest { get { return isBarrelAtRest && isBaseAtRest; } }

        /// <summary>
        /// True when the turret is aimed at the given <see cref="AimPosition"/>. When the turret
        /// is idle, this is never true.
        /// </summary>
        public bool IsAimed { get { return isAimed; } }

        /// <summary>
        /// Angle in degress to the given <see cref="AimPosition"/>. When the turret is idle,
        /// the angle reports 999.
        /// </summary>
        public float AngleToTarget { get { return IsIdle ? 999f : angleToTarget; } }


 
        new void Awake()
        {
            base.Awake();
            normalTraverseSpeed = TraverseSpeed;
            hasBarrels = barrels != null;
            if (turretBase == null)
                Debug.LogError(name + ": TurretAim requires an assigned TurretBase!");
        }

        void Update()
        {
            if(selected && Time.timeScale < 0.02)TraverseSpeed = 200f;
            else TraverseSpeed = normalTraverseSpeed;
            if(!selected) TraverseSpeed = normalTraverseSpeed;
            if(!selected && autoControlled){
                // follow AI behaviour dictated by aimpoint;
                    RotateBaseToFaceTarget(AimPosition, false);

                    if (hasBarrels)
                        RotateBarrelsToFaceTarget(AimPosition, false);

                    // Turret is considered "aimed" when it's pointed at the target.
                    angleToTarget = GetTurretAngleToTarget(AimPosition);

                    // Turret is considered "aimed" when it's pointed at the target.
                    isAimed = angleToTarget < aimedThreshold;

                    isBarrelAtRest = false;
                    isBaseAtRest = false;
            }
            
            else if(!selected && !autoControlled){
                
                if (IsIdle)
                {
                    if (!IsTurretAtRest)
                        RotateTurretToIdle();
                    isAimed = false;
                }
                else
                {
                    /*
                    RotateBaseToFaceTarget(AimPosition);

                    if (hasBarrels)
                        RotateBarrelsToFaceTarget(AimPosition);

                    // Turret is considered "aimed" when it's pointed at the target.
                    angleToTarget = GetTurretAngleToTarget(AimPosition);

                    // Turret is considered "aimed" when it's pointed at the target.
                    isAimed = angleToTarget < aimedThreshold;

                    isBarrelAtRest = false;
                    isBaseAtRest = false;*/
                }
            }
        }

        void FixedUpdate()
        {
            if(selected){
                if (IsIdle)
                {
                    if (!IsTurretAtRest)
                        RotateTurretToIdle();
                    isAimed = false;
                }
                else
                {
                    //TraverseSpeed = 9999f;
                    RotateBaseToFaceTarget(AimPosition, false);

                    if (hasBarrels)
                        RotateBarrelsToFaceTarget(AimPosition, false);

                    // Turret is considered "aimed" when it's pointed at the target.
                    angleToTarget = GetTurretAngleToTarget(AimPosition);

                    // Turret is considered "aimed" when it's pointed at the target.
                    isAimed = angleToTarget < aimedThreshold;

                    isBarrelAtRest = false;
                    isBaseAtRest = false;
                }
            }
        }

        float GetTurretAngleToTarget(Vector3 targetPosition)
        {
            float angle = 999f;

            if (hasBarrels)
            {
                angle = Vector3.Angle(targetPosition - barrels.position, barrels.forward);
            }
            else
            {
                Vector3 flattenedTarget = Vector3.ProjectOnPlane(
                    targetPosition - turretBase.position,
                    turretBase.up);

                angle = Vector3.Angle(
                    flattenedTarget - turretBase.position,
                    turretBase.forward);
            }

            return angle;
        }

        void RotateTurretToIdle()
        {
            // Rotate the base to its default position.
            if (hasLimitedTraverse)
            {
                limitedTraverseAngle = Mathf.MoveTowards(
                    limitedTraverseAngle, 0f,
                    TraverseSpeed * Time.deltaTime);

                if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                    turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
                else
                    isBaseAtRest = true;
            }
            else
            {
                turretBase.rotation = Quaternion.RotateTowards(
                    turretBase.rotation,
                    transform.rotation,
                    TraverseSpeed * Time.deltaTime);

                isBaseAtRest = Mathf.Abs(turretBase.localEulerAngles.y) < Mathf.Epsilon;
            }

            if (hasBarrels)
            {
                elevation = Mathf.MoveTowards(elevation, 0f, ElevationSpeed * Time.deltaTime);
                if (Mathf.Abs(elevation) > Mathf.Epsilon)
                    barrels.localEulerAngles = Vector3.right * -elevation;
                else
                    isBarrelAtRest = true;
            }
            else // Barrels automatically at rest if there are no barrels.
                isBarrelAtRest = true;
        }

        void RotateBarrelsToFaceTarget(Vector3 targetPosition, bool instant)
        {
            Vector3 localTargetPos = turretBase.InverseTransformDirection(targetPosition - barrels.position);
            Vector3 flattenedVecForBarrels = Vector3.ProjectOnPlane(localTargetPos, Vector3.up);

            float targetElevation = Vector3.Angle(flattenedVecForBarrels, localTargetPos);
            targetElevation *= Mathf.Sign(localTargetPos.y);

            targetElevation = Mathf.Clamp(targetElevation, -MaxDepression, MaxElevation);
            if(!selected)elevation = Mathf.MoveTowards(elevation, targetElevation, ElevationSpeed * Time.deltaTime);
            if(selected)elevation = Mathf.MoveTowards(elevation, targetElevation, ElevationSpeed * Time.fixedUnscaledDeltaTime * 10000);
            
            

            if (Mathf.Abs(elevation) > Mathf.Epsilon)
                barrels.localEulerAngles = Vector3.right * -elevation;

#if UNITY_EDITOR
            if (DrawDebugRay)
                Debug.DrawRay(barrels.position, barrels.forward * localTargetPos.magnitude, Color.red);
#endif
        }

        void RotateBaseToFaceTarget(Vector3 targetPosition, bool instant)
        {
            Vector3 turretUp = transform.up;

            Vector3 vecToTarget = targetPosition - turretBase.position;
            Vector3 flattenedVecForBase = Vector3.ProjectOnPlane(vecToTarget, turretUp);
            if(instant && !hasLimitedTraverse)turretBase.rotation = Quaternion.LookRotation(flattenedVecForBase, turretUp);
            if (hasLimitedTraverse)
            {
                Vector3 turretForward = transform.forward;
                float targetTraverse = Vector3.SignedAngle(turretForward, flattenedVecForBase, turretUp);

                targetTraverse = Mathf.Clamp(targetTraverse, -LeftLimit, RightLimit);
                if(!selected)limitedTraverseAngle = Mathf.MoveTowards(limitedTraverseAngle,targetTraverse, TraverseSpeed * Time.deltaTime);
                if(selected)limitedTraverseAngle = Mathf.MoveTowards(limitedTraverseAngle,targetTraverse, TraverseSpeed * Time.fixedUnscaledDeltaTime);
                if(instant)turretBase.rotation = Quaternion.LookRotation(flattenedVecForBase, turretUp);
                if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                    turretBase.localEulerAngles = Vector3.up * limitedTraverseAngle;
            }
            else
            {
                    if(!selected){
                        turretBase.rotation = Quaternion.RotateTowards(
                        Quaternion.LookRotation(turretBase.forward, turretUp),
                        Quaternion.LookRotation(flattenedVecForBase, turretUp),
                        TraverseSpeed * Time.deltaTime);
                    }
                    else{
                        if(!instant)turretBase.rotation = Quaternion.RotateTowards(Quaternion.LookRotation(turretBase.forward, turretUp), Quaternion.LookRotation(flattenedVecForBase, turretUp),TraverseSpeed * Time.fixedUnscaledDeltaTime);
                        if(instant)turretBase.rotation = Quaternion.LookRotation(flattenedVecForBase, turretUp);
                     //   turretBase.rotation = Quaternion.LookRotation(flattenedVecForBase, turretUp);
                       // turretBase.rotation = Quaternion.l
                    }
            }
        }


        public override void setLookDir(Vector3 pos, bool instant){
            AimPosition = pos;
        //  targetRotation = direction;
            RotateBaseToFaceTarget(pos, instant);
            RotateBarrelsToFaceTarget(pos, instant);
        }

#if UNITY_EDITOR
        // This should probably go in an Editor script, but dealing with Editor scripts
        // is a pain in the butt so I'd rather not.
        void OnDrawGizmosSelected()
        {
            if (!DrawDebugArcs)
                return;

            if (turretBase != null)
            {
                const float kArcSize = 10f;
                Color colorTraverse = new Color(1f, .5f, .5f, .1f);
                Color colorElevation = new Color(.5f, 1f, .5f, .1f);
                Color colorDepression = new Color(.5f, .5f, 1f, .1f);

                Transform arcRoot = barrels != null ? barrels : turretBase;

                // Red traverse arc
                UnityEditor.Handles.color = colorTraverse;
                if (hasLimitedTraverse)
                {
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, turretBase.up,
                        transform.forward, RightLimit,
                        kArcSize);
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, turretBase.up,
                        transform.forward, -LeftLimit,
                        kArcSize);
                }
                else
                {
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, turretBase.up,
                        transform.forward, 360f,
                        kArcSize);
                }

                if (barrels != null)
                {
                    // Green elevation arc
                    UnityEditor.Handles.color = colorElevation;
                    UnityEditor.Handles.DrawSolidArc(
                        barrels.position, barrels.right,
                        turretBase.forward, -MaxElevation,
                        kArcSize);

                    // Blue depression arc
                    UnityEditor.Handles.color = colorDepression;
                    UnityEditor.Handles.DrawSolidArc(
                        barrels.position, barrels.right,
                        turretBase.forward, MaxDepression,
                        kArcSize);
                }
            }
        }
#endif
}


