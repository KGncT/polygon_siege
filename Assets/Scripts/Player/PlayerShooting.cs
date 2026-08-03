using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private OnScreenJoystick aimJoystick;
    [SerializeField] private PlayerProjectile projectilePrefab;
    [SerializeField] private GameObject muzzleEffectPrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Transform playerBody; // rotate edilecek karakter
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float rotateSpeed = 720f;
    [SerializeField] private int prewarmCount = 20;

    private ParticleSystem muzzleEffect;
    private GameObject muzzleEffectObj;
    private ObjectPool<PlayerProjectile> projectilePool;
    private Coroutine fireRoutine;
    private bool isFiring = false;
    private Animator animator;

    private void Awake()
    {
        projectilePool = new ObjectPool<PlayerProjectile>(projectilePrefab, prewarmCount);
        animator = GetComponent<Animator>();
        muzzleEffectObj = Instantiate(muzzleEffectPrefab, shootingPoint.position, shootingPoint.rotation, shootingPoint);
        muzzleEffect = muzzleEffectObj.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (aimJoystick.IsPressed && aimJoystick.Direction != Vector2.zero)
        {
            RotateTowardsAim(aimJoystick.Direction);

            animator.SetBool("isShooting", true);

            if (!isFiring)
                StartFiring();
        }
        else
        {
            animator.SetBool("isShooting", false);
            if (isFiring)
                StopFiring();
        }
    }

    public Vector3 AimDirection => new Vector3(aimJoystick.Direction.x, 0f, aimJoystick.Direction.y);

    private void RotateTowardsAim(Vector2 dir)
    {
        Vector3 aimDir = new Vector3(dir.x, 0f, dir.y);
        Quaternion targetRot = Quaternion.LookRotation(aimDir);
        playerBody.rotation = Quaternion.RotateTowards(
            playerBody.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }

    private void StartFiring()
    {
        isFiring = true;
        fireRoutine = StartCoroutine(FireLoop());
    }

    private void StopFiring()
    {
        isFiring = false;
        if (fireRoutine != null)
            StopCoroutine(fireRoutine);
        
        if(muzzleEffect.isPlaying)
                muzzleEffect.Stop();
    }

    private IEnumerator FireLoop()
    {
        while (isFiring)
        {
            SpawnProjectile();
            if(!muzzleEffect.isPlaying)
                muzzleEffect.Play();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void SpawnProjectile()
    {
        PlayerProjectile proj = projectilePool.Get(shootingPoint.position, shootingPoint.rotation);
        proj.Init(ReturnProjectile);
    }

    private void ReturnProjectile(PlayerProjectile proj)
    {
        projectilePool.Return(proj);
    }

    public void AssignShootingPoint(GameObject obj)
    {
        shootingPoint = obj.transform.Find("ShootingPoint");
    }
}
