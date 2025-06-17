using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PointGain
{
    public int Points;
    public string Subtitle;

    public PointGain(int points, string subtitle)
    {
        Points = points;
        Subtitle = subtitle;
    }
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Customs")]
    public float AccelerationFactor;
    public float TurnFactor;
    public float DriftFactor;
    public float Deceleration;
    public float MaxSpeed;
    private float RotationAngle;
    [Header("Inputs")]
    public bool BlockInput;
    [Header("Color")]
    public ColorPallete Colors;
    [Header("Score System")]
    public GameObject ScoreGainPrefab;
    public List<ItemObject> ItemsPurchased = new List<ItemObject>();
    public List<PointGain> PointsGained = new List<PointGain>();
    public int Score;
    // Steering/Turn = direction.x || Acceleration = direction.y
    private Vector2 direction;
    public void GetMovement(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            direction = Vector2.zero;
        }
        if (context.performed)
        {
            if (BlockInput == false)
            {
                direction = context.ReadValue<Vector2>();
            }
        }
    }
    void Start()
    {
        RotationAngle = transform.rotation.eulerAngles.z;
        Color PlayerColor = Colors.Colors[UnityEngine.Random.Range(0, Colors.Colors.Length)];
        GetComponent<SpriteRenderer>().color = PlayerColor;
        float H = 0;
        float S = 0;
        float V = 0;
        Color.RGBToHSV(PlayerColor, out H, out S, out V);
        S = 0.5f;
        transform.Find("Cart").GetComponent<SpriteRenderer>().color = Color.HSVToRGB(H, S, V);
    }
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
                Vector2 Force = transform.up * direction.y * AccelerationFactor;
                GetComponent<Rigidbody2D>().AddForce(Force, ForceMode2D.Force);
            }

            float minSpeed = GetComponent<Rigidbody2D>().linearVelocity.magnitude / 8;
            minSpeed = Mathf.Clamp01(minSpeed);
            RotationAngle -= direction.x * TurnFactor * minSpeed;
            GetComponent<Rigidbody2D>().MoveRotation(RotationAngle);

            Vector2 forwardVel = transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().linearVelocity, transform.up);
            Vector2 rightVel = transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().linearVelocity, transform.right);
            GetComponent<Rigidbody2D>().linearVelocity = forwardVel + rightVel * DriftFactor;
        }
    }
    public void AddScore(int ScoreToAdd, string Subtitle)
    {
        GameObject scoregain = Instantiate(ScoreGainPrefab, GameObject.FindGameObjectWithTag("ScoreGains").transform);
        scoregain.transform.localScale = Vector3.one;

        scoregain.transform.Find("Points").GetComponent<TextMeshProUGUI>().text = ScoreToAdd + "<size=40>p+";
        scoregain.transform.Find("Subtitle").GetComponent<TextMeshProUGUI>().text = Subtitle;

        Score += ScoreToAdd;
        PointsGained.Add(new PointGain(ScoreToAdd, Subtitle));
        StartCoroutine(ScoreGainAnim(scoregain));
    }
    public IEnumerator ScoreGainAnim(GameObject score)
    {
        score.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        score.transform.DOScale(1, 0.5f);

        yield return new WaitForSeconds(3);

        score.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.OutExpo);

        yield return new WaitForSeconds(1);

        Destroy(score);
    }
}
