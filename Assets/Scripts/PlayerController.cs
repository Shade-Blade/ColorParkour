using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public const float DEATH_BARRIER = -25f;
    
    public bool grounded;
    public bool jumping;
    public bool canJump;
    public float jumpTime;
    public bool jumpStart;

    public float minJumpVel;
    //public Vector3 minJumpForce;
    public Vector3 jumpForce;

    public GameObject character;
    public GameObject rpoint;
    public float rspeed;
    public float wspeed;
    //public float mspeed;
    Vector2 dir;

    private float lerpCoeff;
    public float groundLerpCoeff;
    public float airLerpCoeff;

    Vector3 warpPoint;
    Quaternion warpRotation;

    public float b_warpInvulnerability;
    float warpInvulnerability;

    Rigidbody rb;

    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;


    private bool seeCyan = false;
    private bool seeGreen = false;
    private bool seeMagenta = false;
    private bool onYellow = false;
    private bool onBlue = false;
    public float cyanVel;
    public float b_powerCooldown = 0.1f;
    private float powerCooldown;

    public enum Power
    {
        None,
        Lift,
        Dash,
        Hover,
        Immune,
        Stick,
        Invert,
        Teleport
    }

    public Power power;
    public bool canUse;
    public Vector3 LiftPower;
    public Vector3 DashPower;
    public float b_hoverDuration;
    private float hoverDuration;
    private float hoverY;
    public float stickStrength;
    public float stickMoveStrength;
    private bool isStuck;
    private bool tryStick;
    private int stickCooldown = 2;
    private Vector3 stickDir;
    public bool invert;

    private void Awake()
    {
        instance = this;

        //character = gameObject;
        rb = GetComponent<Rigidbody>();

        rb.sleepThreshold = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (MenuScript.instance.pause)
        {
            return;
        }

        MoveUpdate();
        PowerUpdate();

        if (warpInvulnerability > 0)
        {
            warpInvulnerability -= Time.deltaTime;
        }

        RaycastHit hit;
        seeCyan = false;
        seeGreen = false;
        seeMagenta = false;

        //update warp point if you are grounded and the platform is directly below
        //Prevent warp points being on the side of platforms
        if (grounded)
        {
            if (Physics.Raycast(rpoint.transform.position, Vector3.down, out hit, 10))
            {
                if (hit.collider.CompareTag("WhitePlatform") && !invert)
                {
                    warpPoint = transform.position;
                    warpRotation = transform.rotation;
                }
            }
        }

        if (Physics.Raycast(rpoint.transform.position, rpoint.transform.forward, out hit, 7.5f))
        {
            if (((hit.collider.gameObject.CompareTag("RedPlatform") && !invert) || (hit.collider.gameObject.CompareTag("CyanPlatform") && invert)) && power != Power.Immune)
            {
                TryWarp();
            }
        }

        if (Physics.Raycast(rpoint.transform.position, rpoint.transform.forward, out hit, 25))
        {
            if (((hit.collider.gameObject.CompareTag("CyanPlatform") && !invert) || (hit.collider.gameObject.CompareTag("RedPlatform") && invert)) && power != Power.Immune)
            {
                seeCyan = true;
            }
            if (((hit.collider.gameObject.CompareTag("GreenPlatform") && !invert) || (hit.collider.gameObject.CompareTag("MagentaPlatform") && invert)) && power != Power.Immune)
            {
                seeGreen = true;
            }
            if (((hit.collider.gameObject.CompareTag("MagentaPlatform") && !invert) || (hit.collider.gameObject.CompareTag("GreenPlatform") && invert)) && power != Power.Immune)
            {
                seeMagenta = true;
            }
        }

        if (transform.position[1] < DEATH_BARRIER)
        {
            ForceWarp();
        }
    }
    void MoveUpdate()
    {
        // mouse position change
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        //make this less jittery
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        // incrementally add to the camera look
        mouseLook += smoothV;

        rpoint.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);

        if (Input.GetButtonDown("Jump") && !jumping)
        {
            jumpStart = true;
        }
    }
    void PowerUpdate()
    {
        if (powerCooldown > 0)
        {
            powerCooldown -= Time.deltaTime;
        }

        //Debug.Log(canUse + " " + powerCooldown);

        //All moves recharge when you are grounded.
        if (!canUse && powerCooldown <= 0)
        {
            canUse = canJump;
        }
        if (onBlue)
        {
            canUse = false;
        }

        if (Input.GetButtonDown("UsePower") && canUse)
        {
            switch (power)
            {
                case Power.Lift:
                    rb.AddForce(LiftPower, ForceMode.Impulse);
                    transform.position = transform.position + Vector3.up * 0.05f;
                    powerCooldown = b_powerCooldown;
                    canUse = false;
                    break;
                case Power.Dash:
                    rb.AddForce(DashPower[0] * transform.forward + DashPower[1] * Vector3.up, ForceMode.Impulse);
                    transform.position = transform.position + Vector3.up * 0.05f;
                    powerCooldown = b_powerCooldown;
                    canUse = false;
                    break;
                case Power.Hover:
                    hoverDuration = b_hoverDuration;
                    hoverY = transform.position[1] + 0.025f;
                    powerCooldown = b_powerCooldown;
                    canUse = false;
                    break;
                case Power.Invert:
                    invert = !invert;
                    break;
                case Power.Stick:
                    tryStick = true;
                    break;
                case Power.Teleport:
                    RaycastHit hit;
                    if (Physics.Raycast(rpoint.transform.position, rpoint.transform.forward, out hit, 25))
                    {
                        if (hit.collider.gameObject.CompareTag("BlackPlatform"))
                        {
                            transform.position = hit.point;
                            powerCooldown = b_powerCooldown;
                            canUse = false;
                        }
                    }
                    break;
            }
        }
    }
    void ChangePower(Power p)
    {
        power = p;
        invert = false;
        isStuck = false;
        hoverDuration = 0;
    }

    public void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (tryStick)
            {
                isStuck = true;
                tryStick = false;
                //Debug.Log("yes");
            }
            if (isStuck)
            {
                stickDir = -contact.normal.normalized;
                stickCooldown = 2;
                //Debug.Log("foosh");
            }

            //Debug.Log(contact.normal.normalized);
            if (contact.normal.normalized[1] > 0.75)
            {
                //Debug.Log("yes");
                canJump = true;
                grounded = true;

                if ((collision.collider.CompareTag("BlackPlatform") && !invert) || (collision.collider.CompareTag("WhitePlatform") && invert))
                {
                    ForceWarp();
                }
                if ((collision.collider.CompareTag("YellowPlatform") && !invert) || (collision.collider.CompareTag("BluePlatform") && invert))
                {
                    onYellow = true;
                }
                if ((collision.collider.CompareTag("BluePlatform") && !invert) || (collision.collider.CompareTag("YellowPlatform") && invert))
                {
                    onBlue = true;
                }
            }
        }
    }
    public virtual void FixedUpdate()
    {
        if (stickCooldown > 0)
        {
            stickCooldown--;
        } else
        {
            isStuck = false;
        }


        if (hoverDuration > 0)
        {
            hoverDuration -= Time.fixedDeltaTime;
            transform.position = Vector3.up * hoverY + transform.position[0] * Vector3.right + transform.position[2] * Vector3.forward;
        } else
        {
            hoverDuration = 0;
        }

        if (hoverDuration > 0 || isStuck || seeMagenta)
        {
            rb.useGravity = false;
        } else
        {
            rb.useGravity = true;
        }

        // Input.GetAxis() is used to get the user's input
        float speed = Input.GetButton("Run") ? rspeed : wspeed;
        dir = Vector2.right * Input.GetAxis("Horizontal") * speed * Time.deltaTime + Vector2.up * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        Vector3 dir3 = dir[0] * transform.right + dir[1] * transform.forward;

        //rb.velocity = rb.velocity[1] * Vector3.up + dir[0] * transform.right + dir[1] * transform.forward;       
        lerpCoeff = grounded ? groundLerpCoeff : airLerpCoeff;

        if (seeMagenta)
        {
            rb.velocity = Vector3.zero;
        } else if (seeCyan)
        {
            rb.velocity = Vector3.Lerp(rb.velocity,rpoint.transform.forward * Time.fixedDeltaTime * cyanVel, 1 - Mathf.Pow((1 - lerpCoeff), Time.fixedDeltaTime));
        } else if (seeGreen)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, rpoint.transform.forward * -1 * Time.fixedDeltaTime * cyanVel, 1 - Mathf.Pow((1 - lerpCoeff), Time.fixedDeltaTime));
        } else if (isStuck)
        {
            Vector3 dir4 = stickMoveStrength * (dir[0] * rpoint.transform.right + dir[1] * rpoint.transform.forward);
            rb.velocity = Vector3.Lerp(rb.velocity, dir4, 1 - Mathf.Pow((1 - lerpCoeff), Time.fixedDeltaTime));
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, dir3 + (rb.velocity[1] * Vector3.up), 1 - Mathf.Pow((1 - lerpCoeff), Time.fixedDeltaTime));
        }

        //Add an additional force to make sure you stick to the surface
        if (isStuck)
        {
            rb.AddForce(stickDir * stickStrength);
        }

        //rb.WakeUp();

        if (onYellow)
        {
            canJump = false;
        }

        onYellow = false;
        onBlue = false;
        grounded = false;

        if (jumpStart && canJump)
        {
            jumping = true;
            rb.velocity = rb.velocity[0] * Vector3.right + rb.velocity[2] * Vector3.forward + minJumpVel * Vector3.up;
        }
        if (Input.GetButton("Jump") && jumping && jumpTime < 0.15)
        {
            jumpTime += Time.fixedDeltaTime;
            rb.AddForce(jumpForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
        else
        {
            jumpTime = 0;
            jumping = false;
        }
        jumpStart = false;

        canJump = false;
    }

    public void ForceWarp()
    {
        warpInvulnerability = b_warpInvulnerability;
        transform.position = warpPoint;
        transform.rotation = warpRotation;
        rb.velocity = Vector3.zero;
        hoverY = transform.position[1];
    } //hard warp (black blocks, void)
    public void TryWarp()
    {
        if (warpInvulnerability <= 0)
        {
            ForceWarp();
        }
    } //soft warp (looking at red blocks)
}
