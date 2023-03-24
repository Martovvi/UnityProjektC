using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask grappleableSurface;
    public LineRenderer lr;
    public Animator animator;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grappleContactPoint;
    private SpringJoint joint;

    //new rope animation
    private Spring spring;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    private Vector3 currentGrapplePosition;
    public AnimationCurve affectCurve;

    [Header("Cooldown")]
    public float grappleCooldown;
    private float grappleCooldownTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;
    private AudioSource audioSrc;
    public AudioClip GrappleExtend;
    public AudioClip GrappleRetract;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void Start()
    {
        pm = GetComponent<PlayerMovement>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (Input.GetKeyUp(grappleKey)) StopGrapple();

        if (grappleCooldownTimer > 0)
            grappleCooldownTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        
        
        //new
        //If not grappling, don't draw rope and reset everything
        if (!grappling)
        {
            currentGrapplePosition = gunTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }

        //old nested within new
        //lr.SetPosition(0, gunTip.position);
        //old end

        if (lr.positionCount == 0)
        {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }

        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = grappleContactPoint;
        var gunTipPosition = gunTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++)
        {
            var delta = i / (float)quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
        //new end
    }

    private void StartGrapple()
    {
        if (grappleCooldownTimer > 0) return;
        audioSrc.PlayOneShot(GrappleExtend);

        grappling = true;

        animator.SetTrigger("OnGrappleStart");

        //pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappleableSurface))
        {
            grappleContactPoint = hit.point;

            //Swinging implementation
            joint = pm.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grappleContactPoint;

            float distanceFromPoint = Vector3.Distance(pm.transform.position, grappleContactPoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = 0;//distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grappleContactPoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        lr.enabled = true;
        //lr.SetPosition(1, grappleContactPoint);
    }

    private void ExecuteGrapple()
    {

        //audioSrc.loop = true;
        audioSrc.PlayOneShot(GrappleRetract);

        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grappleContactPointRelativeYPos = grappleContactPoint.y - lowestPoint.y;
        float highestPointOnArc = grappleContactPointRelativeYPos + overshootYAxis;

        if (grappleContactPointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grappleContactPoint, highestPointOnArc);

        //Invoke(nameof(StopGrapple), 1f);

    }

    public void StopGrapple()
    {

        animator.SetTrigger("OnGrappleStop");

        pm.freeze = false;

        grappling = false;

        grappleCooldownTimer = grappleCooldown;

        lr.enabled = false;
        Destroy(joint);
    }


}
