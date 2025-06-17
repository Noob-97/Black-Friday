using UnityEngine;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using JetBrains.Annotations;
using DG.Tweening;
using System.Collections;




#if UNITY_EDITOR
using UnityEditor;
#endif
public class AIBrain : MonoBehaviour
{
    [Header("Movement Module")]
    public float AccelerationFactor;
    public float TurnFactor;
    public float DriftFactor;
    public float Deceleration;
    public float MaxSpeed;
    private float RotationAngle;
    [Header("Node Module")]
    public NodeMap NodeMap;
    private Vector3 targetNodePos;
    private Node currentNode;
    [Header("Car Detection Module")]
    public float DetectionRadius;
    [Header("Colors")]
    public ColorPallete Colors;
    [Header("Inputs")]
    public bool BlockInput;
    // Steering/Turn = direction.x || Acceleration = direction.y
    private Vector2 direction;

    void Start()
    {
        RotationAngle = transform.rotation.eulerAngles.z;
        Color PlayerColor = Colors.Colors[Random.Range(0, Colors.Colors.Length)];
        GetComponent<SpriteRenderer>().color = PlayerColor;
        float H = 0;
        float S = 0;
        float V = 0;
        Color.RGBToHSV(PlayerColor, out H, out S, out V);
        S = 0.5f;
        transform.Find("Cart").GetComponent<SpriteRenderer>().color = Color.HSVToRGB(H, S, V);
    }

    // Movement Module (PlayerMovement)

    private void FixedUpdate()
    {
        if (BlockInput == false)
        {
            if (direction.y == 0)
            {
                GetComponent<Rigidbody2D>().linearDamping = Mathf.Lerp(GetComponent<Rigidbody2D>().linearDamping, Deceleration, Time.fixedDeltaTime * Deceleration);
            }
            else
            {
                GetComponent<Rigidbody2D>().linearDamping = 0;
            }

            bool ApplyForce = true;
            float velSpeed = Vector2.Dot(transform.up, GetComponent<Rigidbody2D>().linearVelocity);
            if (velSpeed > MaxSpeed && direction.y > 0)
            {
                ApplyForce = false;
            }
            if (velSpeed < -MaxSpeed * 0.5f && direction.y < 0)
            {
                ApplyForce = false;
            }
            if (GetComponent<Rigidbody2D>().linearVelocity.sqrMagnitude > MaxSpeed * MaxSpeed && direction.y > 0)
            {
                ApplyForce = false;
            }

            if (ApplyForce)
            {
                Vector2 Force = AccelerationFactor * direction.y * transform.up;
                GetComponent<Rigidbody2D>().AddForce(Force, ForceMode2D.Force);
            }

            float minSpeed = GetComponent<Rigidbody2D>().linearVelocity.magnitude / 8;
            minSpeed = Mathf.Clamp01(minSpeed);
            RotationAngle -= direction.x * TurnFactor * minSpeed;
            GetComponent<Rigidbody2D>().MoveRotation(RotationAngle);

            Vector2 forwardVel = transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().linearVelocity, transform.up);
            Vector2 rightVel = transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().linearVelocity, transform.right);
            GetComponent<Rigidbody2D>().linearVelocity = forwardVel + rightVel * DriftFactor;
            
            // Node Module

            if (currentNode == null)
            {
                currentNode = NodeMap.Nodes.OrderBy(t => Vector3.Distance(transform.position, t.Position)).FirstOrDefault();
            }

            targetNodePos = currentNode.Position;
            float distanceLeft = (targetNodePos - transform.position).magnitude;
            if (distanceLeft < NodeMap.ReachingRadius)
            {
                MaxSpeed = currentNode.MaxSpeed;
                currentNode = NodeMap.Nodes[currentNode.NextNodes[Random.Range(0, currentNode.NextNodes.Length)]];
            }

            // Move To Node

            Vector3 VectorToTarget = targetNodePos - transform.position;
            VectorToTarget.Normalize();

            AvoidCars(VectorToTarget, out VectorToTarget);

            float AngleToTarget = Vector2.SignedAngle(transform.up, VectorToTarget);
            AngleToTarget *= -1;

            float TurnAmount = AngleToTarget / 45;
            TurnAmount = Mathf.Clamp(TurnAmount, -1, 1);
            direction.x = TurnAmount;
            if (Vector2.Dot(transform.up, GetComponent<Rigidbody2D>().linearVelocity) > MaxSpeed)
            {
                direction.y = 0;
            }
            else
            {
                direction.y = 1.05f - Mathf.Abs(direction.x) / 1;
            }
        }
    }

    public bool DetectCars(out Vector3 position, out Vector3 OtherCarRightVector)
    {
        // Car Detection Module

        position = Vector3.zero;
        OtherCarRightVector = Vector3.zero;

        RaycastHit2D hit = Physics2D.BoxCast(transform.position + transform.up * 0.5f, new Vector2(DetectionRadius, 1), 0, transform.up, 12, 1 << LayerMask.NameToLayer("Car"));

        if (hit.collider != null && hit.collider.name != gameObject.name)
        {
            position = hit.collider.transform.position;
            OtherCarRightVector = hit.collider.transform.right;
            return true;
        }

        return false;
    }
    public void AvoidCars(Vector3 VectorToTarget, out Vector3 NewVector)
    {
        if (DetectCars(out Vector3 position, out Vector3 otherCarRightVector))
        {
            Vector2 avoidanceVector = Vector2.zero;
            avoidanceVector = Vector2.Reflect((position - transform.position).normalized, otherCarRightVector);

            NewVector = avoidanceVector;
            NewVector.Normalize();

            return;
        }

        NewVector = VectorToTarget;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(CollideAnimation());
    }

    public IEnumerator CollideAnimation()
    {
        BlockInput = true;
        gameObject.transform.DOMove(gameObject.transform.Find("BumpRef").transform.position, 1).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(1);
        BlockInput = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (NodeMap != null)
        {
            foreach (Node node in NodeMap.Nodes)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(node.Position, NodeMap.ReachingRadius);
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.blue;
                style.fontSize = 18;
                style.fontStyle = FontStyle.Bold;
                Handles.Label(node.Position + new Vector3(-0.225f, 1.25f), NodeMap.Nodes.IndexOf(node).ToString(), style);
                for (int i = 0; i < node.NextNodes.Length; i++)
                {
                    Debug.DrawLine(node.Position, NodeMap.Nodes[node.NextNodes[i]].Position, Color.blue);
                }
            }
        }
        //if (CarDetected)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.55f) + transform.up * 0.5f, DetectionRadius);
        //}
        //else
        //{
        //    Gizmos.color = Color.black;
        //    Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.55f) + transform.up * 0.5f, DetectionRadius);
        //}
    }
    #endif
}
