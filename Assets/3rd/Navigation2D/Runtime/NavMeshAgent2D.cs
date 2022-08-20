using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Navigation2d.Runtime
{
    public class NavMeshAgentArrivedEvent: UnityEvent{}
    public class NavMeshAgent2D : MonoBehaviour
    {
        [Header("Steering")] public float speed = 3.5f;
        public float angularSpeed = 120;
        public float acceleration = 8;
        public float stoppingDistance = 0;
        public bool autoBraking = true; // false is too weird, true by default.

        [Header("Obstacle Avoidance")] public float radius = 0.5f;
        public ObstacleAvoidanceType quality = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        public int priority = 50;

        [Header("Pathfinding")] public bool autoRepath = true;

        public NavMeshAgent agent;

        private new Rigidbody2D rigidbody2D;
        private new Collider2D collider2D;

        public NavMeshAgentArrivedEvent ArrivedEvent { get; private set; }

        private bool _isArrived;

        private void Awake()
        {
            ArrivedEvent = new NavMeshAgentArrivedEvent();
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Navigation2d_agent";
            go.transform.position = NavMeshUtils2D.ProjectTo3D(transform.position); // todo height 0.5 again?
            agent = go.AddComponent<NavMeshAgent>();
            
            rigidbody2D = GetComponent<Rigidbody2D>();
            collider2D = GetComponent<Collider2D>();
        
            Destroy(agent.GetComponent<Collider>());
            Destroy(agent.GetComponent<MeshRenderer>());
        }

        private bool IsStuck()
        {
            // stuck detection: get max distance first (best with collider)
            float maxDist = 2; // default if no collider
            if (collider2D != null)
            {
                var bounds = collider2D.bounds;
                maxDist = Mathf.Max(bounds.extents.x, bounds.extents.y) * 2;
            }

            // stuck detection: reset if distance > max distance
            var dist = Vector2.Distance(transform.position, NavMeshUtils2D.ProjectTo2D(agent.transform.position));
            return dist > maxDist;
        }

        private void Update()
        {
            agent.speed = speed;
            agent.radius = radius;
            agent.autoRepath = autoRepath;
            agent.autoBraking = autoBraking;
            agent.angularSpeed = angularSpeed;
            agent.acceleration = acceleration;
            agent.avoidancePriority = priority;
            agent.obstacleAvoidanceType = quality;
            agent.stoppingDistance = stoppingDistance;

            if (rigidbody2D == null || rigidbody2D.isKinematic)
                transform.position = NavMeshUtils2D.ProjectTo2D(agent.transform.position);
            
            agent.transform.rotation = Quaternion.Euler(NavMeshUtils2D.RotationTo3D(transform.eulerAngles));
            
            if (IsStuck())
            {
                agent.ResetPath();
                agent.transform.position = NavMeshUtils2D.ProjectTo3D(transform.position);
                Debug.Log("stopped agent because of collision in 2D plane");
            }

            if (!_isArrived)
            {
                var dist = Vector2.Distance(NavMeshUtils2D.ProjectTo2D(agent.transform.position), destination);
                if (dist < 0.01f)
                {
                    ArrivedEvent.Invoke();
                    _isArrived = true;
                }
            }
        }

        private void FixedUpdate()
        {
            // copy position: transform in Update, rigidbody in FixedUpdate
            if (rigidbody2D != null && !rigidbody2D.isKinematic)
                rigidbody2D.MovePosition(NavMeshUtils2D.ProjectTo2D(agent.transform.position));
        }

        private void OnDestroy()
        {
            if (agent != null) Destroy(agent.gameObject);
        }

        private void OnEnable()
        {
            if (agent != null) agent.enabled = true;
        }

        private void OnDisable()
        {
            if (agent != null) agent.enabled = false;
        }

        // // draw radius gizmo (gizmos.matrix for correct rotation)
        // void OnDrawGizmosSelected()
        // {
        //     Gizmos.color = Color.green;
        //     Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.localScale);
        //     Gizmos.DrawWireSphere(Vector3.zero, radius);
        // }

        // NavMeshAgent proxies ////////////////////////////////////////////////////
        public bool CalculatePath(Vector2 targetPosition, NavMeshPath2D path)
        {
            var temp = new NavMeshPath();
            if (agent.CalculatePath(NavMeshUtils2D.ProjectTo3D(targetPosition), temp))
            {
                // convert 3D to 2D
                path.corners = temp.corners.Select(NavMeshUtils2D.ProjectTo2D).ToArray();
                path.status = temp.status;
                return true;
            }

            return false;
        }

        public Vector3 destination
        {
            get { return NavMeshUtils2D.ProjectTo2D(agent.destination); }
            set
            {
                _isArrived = false;
                agent.destination = NavMeshUtils2D.ProjectTo3D(value);
            }
        }

        public bool hasPath
        {
            get { return agent.hasPath; }
        }

        public bool isOnNavMesh
        {
            get { return agent.isOnNavMesh; }
        }

        public bool isPathStale
        {
            get { return agent.isPathStale; }
        }

        public bool isStopped
        {
            get { return agent.isStopped; }
            set { agent.isStopped = value; }
        }

        public NavMeshPath2D path
        {
            get
            {
                return new NavMeshPath2D()
                {
                    corners = agent.path.corners.Select(NavMeshUtils2D.ProjectTo2D).ToArray(),
                    status = agent.path.status
                };
            }
        }

        public bool pathPending
        {
            get { return agent.pathPending; }
        }

        public NavMeshPathStatus pathStatus
        {
            get { return agent.pathStatus; }
        }

        public float remainingDistance
        {
            get { return agent.remainingDistance; }
        }

        public void ResetPath()
        {
            agent.ResetPath();
        }

        public void SetDestination(Vector3 v)
        {
            destination = v;
        }

        // we set transform.position to agent.position in each Update, but if we
        // need the 100% 'true position right now' then we can also use this one:
        // (this was important for uMMORPG 2D)
        public Vector2 truePosition
        {
            get { return NavMeshUtils2D.ProjectTo2D(agent.transform.position); }
        }

        public Vector2 velocity
        {
            get { return NavMeshUtils2D.ProjectTo2D(agent.velocity); }
            set { agent.velocity = NavMeshUtils2D.ProjectTo3D(value); }
        }

        public void Warp(Vector2 v)
        {
            // try to warp, set this agent's position immediately if it worked, so
            // that Update doesn't cause issues when trying to move the rigidbody to
            // a far away position etc.
            if (agent.Warp(NavMeshUtils2D.ProjectTo3D(v)))
                transform.position = v;
        }

        public void SetPos(Vector3 pos)
        {
            agent.transform.position = NavMeshUtils2D.ProjectTo3D(pos);
        }
    }
}