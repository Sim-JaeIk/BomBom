using UnityEngine;

// Rigidbody2D 컴포넌트를 필수로 요구합니다.
[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    // Rigidbody2D 컴포넌트를 저장하기 위한 변수입니다.
    private Rigidbody2D rb;

    // 현재 이동 방향을 나타내는 벡터입니다.
    private Vector2 direction = Vector2.down;

    // 이동 속도를 설정합니다.
    public float speed = 5f;

    [Header("Input")]
    // 이동 입력 키를 설정합니다.
    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputRight = KeyCode.D;

    [Header("Sprites")]
    // 각 방향별 스프라이트 렌더러를 설정합니다.
    public AnimatedSpriteRenderer spriteRendererUp;
    public AnimatedSpriteRenderer spriteRendererDown;
    public AnimatedSpriteRenderer spriteRendererLeft;
    public AnimatedSpriteRenderer spriteRendererRight;
    public AnimatedSpriteRenderer spriteRendererDeath;

    // 현재 활성화된 스프라이트 렌더러를 저장합니다.
    private AnimatedSpriteRenderer activeSpriteRenderer;

    // 컴포넌트가 깨어날 때 호출됩니다.
    private void Awake()
    {
        // Rigidbody2D 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody2D>();
        // 기본적으로 아래쪽 스프라이트를 활성화합니다.
        activeSpriteRenderer = spriteRendererDown;
    }

    // 매 프레임마다 호출됩니다.
    private void Update()
    {
        // 입력 키에 따라 방향과 스프라이트를 설정합니다.
        if (Input.GetKey(inputUp))
        {
            SetDirection(Vector2.up, spriteRendererUp);
        }
        else if (Input.GetKey(inputDown))
        {
            SetDirection(Vector2.down, spriteRendererDown);
        }
        else if (Input.GetKey(inputLeft))
        {
            SetDirection(Vector2.left, spriteRendererLeft);
        }
        else if (Input.GetKey(inputRight))
        {
            SetDirection(Vector2.right, spriteRendererRight);
        }
        else
        {
            SetDirection(Vector2.zero, activeSpriteRenderer);
        }
    }

    // 고정된 시간 간격으로 물리 연산을 처리합니다.
    private void FixedUpdate()
    {
        // 현재 위치를 가져옵니다.
        Vector2 position = rb.position;
        // 이동할 벡터를 계산합니다.
        Vector2 translation = speed * Time.fixedDeltaTime * direction;
        // 새로운 위치로 이동합니다.
        rb.MovePosition(position + translation);
    }

    // 새로운 방향과 스프라이트를 설정합니다.
    private void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
    {
        direction = newDirection;

        // 각 방향의 스프라이트를 활성화 또는 비활성화합니다.
        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;

        // 현재 활성화된 스프라이트 렌더러를 업데이트합니다.
        activeSpriteRenderer = spriteRenderer;
        // 방향이 없는 경우, 스프라이트를 대기 상태로 설정합니다.
        activeSpriteRenderer.idle = direction == Vector2.zero;
    }

    // 다른 2D Collider와 충돌할 때 호출됩니다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 객체가 "Explosion" 레이어에 있는 경우 사망 시퀀스를 실행합니다.
        if (other.gameObject.layer == LayerMask.NameToLayer("Explosion"))
        {
            DeathSequence();
        }
    }

    // 사망 시퀀스를 처리합니다.
    private void DeathSequence()
    {
        // 이 스크립트를 비활성화합니다.
        enabled = false;
        // BombController 스크립트를 비활성화합니다.
        GetComponent<BombController>().enabled = false;

        // 모든 방향의 스프라이트를 비활성화하고 사망 스프라이트를 활성화합니다.
        spriteRendererUp.enabled = false;
        spriteRendererDown.enabled = false;
        spriteRendererLeft.enabled = false;
        spriteRendererRight.enabled = false;
        spriteRendererDeath.enabled = true;

        // 1.25초 후에 OnDeathSequenceEnded를 호출합니다.
        Invoke(nameof(OnDeathSequenceEnded), 1.25f);
    }

    // 사망 시퀀스가 끝날 때 호출됩니다.
    private void OnDeathSequenceEnded()
    {
        // 게임 객체를 비활성화합니다.
        gameObject.SetActive(false);
        // 게임 매니저에서 승리 상태를 확인합니다.
        GameManager.Instance.CheckWinState();
    }
}
