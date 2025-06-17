using UnityEngine;

public class Minimap : MonoBehaviour
{
    [Header("Refs")]
    public RectTransform ref1Canvas;
    public RectTransform ref2Canvas;
    public Transform ref1World;
    public Transform ref2World;

    [Header("Player")]
    public RectTransform PlayerCanvas;
    public Transform PlayerWorld;

    private float minimapRatio;

    private void Awake()
    {
        CalculateMapRatio();
    }

    private void Update()
    {
        PlayerCanvas.anchoredPosition = ref1Canvas.anchoredPosition + new Vector2((PlayerWorld.position.x - ref1World.position.x) * minimapRatio,
                                         (PlayerWorld.position.y - ref1World.position.y) * minimapRatio);
    }


    public void CalculateMapRatio()
    {
        //distance world ignoring Z axis
        Vector3 distanceWorldVector = ref1World.position - ref2World.position;
        distanceWorldVector.z = 0f;
        float distanceWorld = distanceWorldVector.magnitude;


        //distance minimap
        float distanceMinimap = Mathf.Sqrt(
                                Mathf.Pow((ref1Canvas.anchoredPosition.x - ref2Canvas.anchoredPosition.x), 2) +
                                Mathf.Pow((ref1Canvas.anchoredPosition.y - ref2Canvas.anchoredPosition.y), 2));


        minimapRatio = distanceMinimap / distanceWorld;
    }
}
