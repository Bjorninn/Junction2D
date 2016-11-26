using System;
using UnityEngine;
using System.Collections;

public class FlashLight : MonoBehaviour
{
    private const float JumpWobble = 6.06f;
    private const float MaxWobble = 2.7f;
    private const float MaxIntensity = 8.0f;
    private const float DrainRate = 8.0f;

    public static FlashLight Instance { private set; get; }

    public float MaxCharge;
    public Light Beam;
    public float CurrentCharge { private set; get; }

    private bool _isOn;
    private float _wobbleSpeed;
    private bool _isWobbling;
    private float _wobbleEps;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        CurrentCharge = MaxCharge;
        _isOn = Beam.enabled;

        _wobbleSpeed = 0.34f;
        _wobbleEps = _wobbleSpeed + 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleFlashlight();
        }

        if (_isOn)
        {
            var player = gameObject.GetComponentInParent<CharacterMovement>();

            if (player.isDead)
            {
                ToggleFlashlight();
            }

            // Get player velocity
            var velocity = player.isRunning ? 1 : 0;
            var jump = player.isJumping;

            // If player V > X then wobble the light
            if (velocity > 0)
            {
                _isWobbling = true;

                Vector3 pos = transform.localPosition;

                if (Math.Abs(pos.y) >= MaxWobble)
                {
                    _wobbleSpeed *= -1;
                }

                pos.y += _wobbleSpeed;
                transform.localPosition = pos;
            }
            else if (_isWobbling)
            {
                Vector3 pos = transform.localPosition;

                _wobbleSpeed = Math.Abs(_wobbleSpeed);

                if (Math.Abs(pos.y) > _wobbleEps)
                {
                    if (pos.y > _wobbleSpeed)
                    {
                        pos.y -= _wobbleSpeed;
                        transform.localPosition = pos;
                    }
                    else if (pos.y < _wobbleSpeed)
                    {
                        pos.y += _wobbleSpeed;
                        transform.localPosition = pos;
                    }
                }
                else
                {
                    _isWobbling = false;
                }
            }

            // If player is jumping then do Z
            if (jump && !_isWobbling)
            {
                _isWobbling = true;
                var pos = transform.localPosition;
                pos.y = JumpWobble;
                transform.localPosition = pos;
            }
        }
    }

    public void AddCharge(float charge)
    {
        CurrentCharge += charge;

        if (CurrentCharge > MaxCharge)
        {
            CurrentCharge = MaxCharge;
        }
    }

    private IEnumerator Drain()
    {
        while (_isOn)
        {
            CurrentCharge -= DrainRate;

            if (CurrentCharge <= 0)
            {
                CurrentCharge = 0;
                ToggleFlashlight();
            }

            float charge = ((CurrentCharge / MaxCharge) * (MaxIntensity - 1.0f)) + 1.0f;
            Beam.intensity = charge;

            yield return new WaitForSeconds(1.0f);
        }
    }

    public void ToggleFlashlight()
    {
        _isOn = !_isOn;

        Beam.enabled = _isOn;

        if (CurrentCharge <= 0 && _isOn)
        {
            // If the player tries to turn the light on but it has no charge
            // then just turn it off again.
            ToggleFlashlight();
        }
        else if (_isOn)
        {
            StartCoroutine(Drain());
        }
    }
}