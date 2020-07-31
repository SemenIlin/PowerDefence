using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTower : Tower
{
    [SerializeField, Range(0.5f, 2f)]
    private float _shootPerSeconds = 1f;

    [SerializeField]
    private Transform _mortar;

    [SerializeField, Range(0.5f, 3f)]
    private float _shellBlastRadius = 1;

    [SerializeField, Range(1f, 100f)]
    private float _damage = 10f;

    private float _launchSpeed;
    private float _launchProgress;

    private const float GRAVITY_CONST = 9.81f;

    public override TowerType Type => TowerType.Mortar;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        float x = _targetingRange + 0.251f;
        float y = -_mortar.position.y;
        _launchSpeed = Mathf.Sqrt(GRAVITY_CONST * (y + Mathf.Sqrt(x * x + y * y)));
    }

    public override void GameUpdate()
    {
        _launchProgress += Time.deltaTime * _shootPerSeconds;
        while(_launchProgress >= 1f)
        {
            if(IsAcquireTarget(out TargetPoint target))
            {
                Launch(target);

                _launchProgress -= 1f;
            }
            else
            {
                _launchProgress = 0.9999f;
            }
        }
    }

    private void Launch(TargetPoint target)
    {
        Vector3 launchPoint = _mortar.position;
        Vector3 targetPoint = target.Position;
        targetPoint.y = 0f;

        Vector2 dir;
        dir.x = targetPoint.x - launchPoint.x;
        dir.y = targetPoint.z - launchPoint.z;

        float x = dir.magnitude;
        float y = -launchPoint.y;
        dir /= x;

        float g = GRAVITY_CONST;
        float s = _launchSpeed;
        float s2 = s * s;

        float r = s2 * s2 - g * (g * x * x + 2f * y * s2);
        float tanTheta = (s2 + Mathf.Sqrt(r)) / (g * x);
        float cosTheta = Mathf.Cos(Mathf.Atan(tanTheta));
        float sinTheta = cosTheta * tanTheta;

        _mortar.localRotation = Quaternion.LookRotation(new Vector3(dir.x, tanTheta, dir.y));

        Game.SpawnShell().Initiallize(launchPoint, targetPoint,
            new Vector3(s * cosTheta * dir.x, s * sinTheta, s * cosTheta * dir.y), _shellBlastRadius, _damage);
    }
}
