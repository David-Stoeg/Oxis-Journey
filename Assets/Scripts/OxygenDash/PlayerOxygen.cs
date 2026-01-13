using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float verticalLimit = 4f;

    [Header("Mouse Follow")]
    public float dragSpeed = 2f;        // slower drag follow
    public float dragDeadZone = 0.1f;

    [Header("Sprites / Animation")]
    public SpriteRenderer spriteRenderer;   // drag your SpriteRenderer (or leave empty to auto-find)
    public Sprite idleSprite;

    public Sprite[] upFrames;               // sliced sprites from "drag up" sheet
    public Sprite[] downFrames;             // sliced sprites from "drag down" sheet

    public float animFps = 12f;

    [Header("Damage Look")]
    public Sprite hurtSprite;
    public float hurtDuration = 0.25f;

    [Header("Sprite Hold")]
    [Tooltip("Wie lange das 'Up'/'Down' Sprite nach dem Aufhören der Bewegung gehalten wird (Sekunden)")]
    public float holdSpriteDuration = 1f;

    private Rigidbody2D rb;

    // Shield
    private bool isShielded = false;
    private float invincibilityTimer = 0f;

    // input state
    private int _keyboardVertical = 0;
    private bool _mouseHeld = false;
    private float _mouseTargetY = 0f;

    // animation state
    private enum AnimState { Idle, Up, Down, Hurt }
    private AnimState _state = AnimState.Idle;

    private Sprite[] _currentFrames;
    private int _frameIndex = 0;
    private float _frameTimer = 0f;

    private float _hurtTimer = 0f;

    // last movement direction from physics step (-1,0,+1)
    private int _moveDir = 0;

    private bool _prevMouseHeld = false;

    // --- added for hold-timer behaviour ---
    private int _prevMoveDir = 0;
    private float _holdTimer = 0f;
    private AnimState _holdState = AnimState.Idle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // good defaults if you forgot
        if (idleSprite == null && spriteRenderer != null)
            idleSprite = spriteRenderer.sprite;
    }

    void Update()
    {
        // ------- keyboard -------
        _keyboardVertical = 0;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) _keyboardVertical = 1;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) _keyboardVertical = -1;
        }

        // ------- mouse -------
        _mouseHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
        if (_mouseHeld && Camera.main != null)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _mouseTargetY = mouseWorld.y;
        }

        // Rising edge: Maustaste wurde gerade gedrückt -> setze Animation sofort basierend auf Ziel
        if (_mouseHeld && !_prevMouseHeld)
        {
            float deltaToTarget = _mouseTargetY - transform.position.y;
            if (deltaToTarget > 0.01f && upFrames != null && upFrames.Length > 0)
                SetAnim(AnimState.Up, upFrames);
            else if (deltaToTarget < -0.01f && downFrames != null && downFrames.Length > 0)
                SetAnim(AnimState.Down, downFrames);
            else
                SetAnim(AnimState.Idle, null);
        }

        _prevMouseHeld = _mouseHeld;

        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;

        // ------- hurt timer + animation update -------
        if (_hurtTimer > 0f) _hurtTimer -= Time.deltaTime;

        UpdateSpriteAnimation(Time.deltaTime);
    }

    void FixedUpdate()
    {
        float y = rb.position.y;
        float newY = y;

        if (_mouseHeld)
        {
            float targetY = Mathf.Clamp(_mouseTargetY, -verticalLimit, verticalLimit);
            float delta = targetY - y;

            if (Mathf.Abs(delta) > dragDeadZone)
                newY = Mathf.MoveTowards(y, targetY, dragSpeed * Time.fixedDeltaTime);
        }
        else
        {
            newY = y + _keyboardVertical * moveSpeed * Time.fixedDeltaTime;
            newY = Mathf.Clamp(newY, -verticalLimit, verticalLimit);
        }

        float dy = newY - y;
        if (Mathf.Abs(dy) < 0.0001f) _moveDir = 0;
        else _moveDir = (dy > 0f) ? 1 : -1;

        // wenn Bewegung von non-zero -> zero wechselt, starte Hold-Timer für das vorherige AnimState
        if (_prevMoveDir != 0 && _moveDir == 0)
        {
            _holdTimer = holdSpriteDuration;
            _holdState = (_prevMoveDir > 0) ? AnimState.Up : AnimState.Down;
        }

        _prevMoveDir = _moveDir;

        rb.MovePosition(new Vector2(rb.position.x, newY));
    }

    private void UpdateSpriteAnimation(float dt)
    {
        if (spriteRenderer == null) return;

        // update hold timer
        if (_holdTimer > 0f)
            _holdTimer -= dt;

        // Hurt overrides everything
        if (_hurtTimer > 0f && hurtSprite != null)
        {
            if (_state != AnimState.Hurt)
            {
                _state = AnimState.Hurt;
                spriteRenderer.sprite = hurtSprite;
            }
            return;
        }

        // choose state based on movement
        if (_moveDir > 0 && upFrames != null && upFrames.Length > 0)
            SetAnim(AnimState.Up, upFrames);
        else if (_moveDir < 0 && downFrames != null && downFrames.Length > 0)
            SetAnim(AnimState.Down, downFrames);
        else
        {// no active movement: if hold-timer läuft, keep last up/down sprite
            if (_holdTimer > 0f && (_holdState == AnimState.Up || _holdState == AnimState.Down))
            {
                if (_holdState == AnimState.Up && upFrames != null && upFrames.Length > 0)
                {
                    SetAnim(AnimState.Up, upFrames);
                    AdvanceFrames(dt); // show anim frames while holding
                    return;
                }
                else if (_holdState == AnimState.Down && downFrames != null && downFrames.Length > 0)
                {
                    SetAnim(AnimState.Down, downFrames);
                    AdvanceFrames(dt); // show anim frames while holding
                    return;
                }
            }
            _state = AnimState.Idle;
            _currentFrames = null;
            _frameIndex = 0;
            _frameTimer = 0f;
            if (idleSprite != null) spriteRenderer.sprite = idleSprite;
            return;
        }

        //// advance flipbook frames
        //if (_currentFrames == null || _currentFrames.Length == 0) return;

        //float frameTime = 1f / Mathf.Max(1f, animFps);
        //_frameTimer += dt;

        //while (_frameTimer >= frameTime)
        //{
        //    _frameTimer -= frameTime;
        //    _frameIndex = (_frameIndex + 1) % _currentFrames.Length;
        //    spriteRenderer.sprite = _currentFrames[_frameIndex];
        //}
    }

    // Separate small helper to advance frames when SetAnim already ensured _currentFrames is correct.
    private void AdvanceFrames(float dt)
    {
        if (_currentFrames == null || _currentFrames.Length == 0) return;

        float frameTime = 1f / Mathf.Max(1f, animFps);
        _frameTimer += dt;

        while (_frameTimer >= frameTime)
        {
            _frameTimer -= frameTime;
            _frameIndex = (_frameIndex + 1) % _currentFrames.Length;
            spriteRenderer.sprite = _currentFrames[_frameIndex];
        }
    }

    private void SetAnim(AnimState state, Sprite[] frames)
    {
        if (_state == state && _currentFrames == frames) return;

        _state = state;
        _currentFrames = frames;
        _frameIndex = 0;
        _frameTimer = 0f;

        if (spriteRenderer != null && _currentFrames != null && _currentFrames.Length > 0)
            spriteRenderer.sprite = _currentFrames[0];
    }

    private void TriggerHurt()
    {
        _hurtTimer = hurtDuration;
        // sprite gets set in UpdateSpriteAnimation
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (isShielded)
            {
                isShielded = false;
                invincibilityTimer = 1f;
            }
            else if (invincibilityTimer <= 0f)
            {
                OxygenDashGameManager.Instance.LoseLife(); // ✅ flashes red now
                TriggerHurt();                              // ✅ hurt sprite
                invincibilityTimer = 1f;
            }

            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Boost"))
        {
            ActivateShield();
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Finish"))
        {
            OxygenDashGameManager.Instance.Victory();
            return;
        }
    }

    void ActivateShield()
    {
        isShielded = true;
        invincibilityTimer = 0f;
    }

    public bool IsShielded() => isShielded;
}
