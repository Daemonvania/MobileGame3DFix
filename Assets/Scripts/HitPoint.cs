using System.Collections;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    private float minX;
    private float maxX;
    
    [SerializeField] private GameObject visualObject;
    [SerializeField] public Transform brickStart;
    [SerializeField] public Transform brickEnd;
    
    [SerializeField] private float minHoverSpeed = 1f;
    [SerializeField] private float maxHoverSpeed = 3f;
    private float hoverSpeed = 1.5f;
    
    private float hoverTime;
    
    
    private void OnEnable()
    {
        Hand.OnBrickHit += OnHit;
    }
    
    private void OnDisable()
    {
        Hand.OnBrickHit -= OnHit;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minX = brickStart.position.x;
        maxX = brickEnd.position.x;
        MoveHitPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPlaying) return;
        HandleHovering();
    }
    
    void HandleHovering()
    {
        hoverTime += Time.deltaTime * hoverSpeed;

        // Calculate the X position using a sine wave
        float xPosition = Mathf.Lerp(minX, maxX, (Mathf.Sin(hoverTime) + 1f) / 2f);

        // Update the position of the Hand object
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
    }

    void OnHit()
    {
        StartCoroutine(WaitToMove());
        visualObject.SetActive(false);
    }

    private IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(1f);
        MoveHitPoint();
    }
    
    public void MoveHitPoint()
    {
        visualObject.SetActive(true);
        hoverSpeed = Random.Range(minHoverSpeed, maxHoverSpeed);
        // Random phase shift for the sine wave
        float randomPhase = Random.Range(0f, Mathf.PI * 2);
        hoverTime = randomPhase;

        // Calculate new X position from the shifted sine wave
        float xPosition = Mathf.Lerp(minX, maxX, (Mathf.Sin(hoverTime) + 1f) / 2f);

        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
    }
}
