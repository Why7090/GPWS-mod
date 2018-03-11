using Jundroo.SimplePlanes.ModTools;
using Jundroo.SimplePlanes.ModTools.Parts;
using System.Linq;
using UnityEngine;

public class GPWSCoreBehaviour : PartModifierBehaviour
{
    private static ServiceProvider sp = ServiceProvider.Instance;

    private bool _enabled = true;
    private IPlayerAircraftControls controls;
    private GPWSCore modifier;
    private bool inDesigner;
    private AudioSource audioSource;

    private static AudioClip obstacleClip;
    private static AudioClip pullupClip;
    private static AudioClip angleClip;

    private const float m2ft = 3.2808f;
    private const float m2kn = 1.9438f;
    private float vSpeed = 0;
    private float alt = 0;
    private float angle = 0;
    private float airSpeed = 0;
    private float? obstacle = null;

    static GPWSCoreBehaviour()
    {
        obstacleClip = sp.ResourceLoader.LoadAsset<AudioClip>("Assets/Sounds/terrainTerrain.ogg");
        pullupClip = sp.ResourceLoader.LoadAsset<AudioClip>("Assets/Sounds/pullup.ogg");
        angleClip = sp.ResourceLoader.LoadAsset<AudioClip>("Assets/Sounds/bank angle.ogg");
    }

    private void Awake()
    {
        inDesigner = sp.GameState.IsInDesigner;
    }

    private void Start()
    {
        if (inDesigner) return;
        modifier = (GPWSCore)PartModifier;
        controls = sp.PlayerAircraft.Controls;
        audioSource = Camera.main.gameObject.AddComponent<AudioSource>();

        alt = GroundAltitude();
        angle = BankAngle();
    }

    private void Update()
    {
        if (inDesigner) return;
        _enabled = controls.GetActivationGroupState(modifier.activationGroup);
    }

    private void FixedUpdate()
    {
        if (inDesigner || !_enabled) return;

        alt = GroundAltitude();
        angle = BankAngle();
        vSpeed = VerticalSpeed();
        airSpeed = AirSpeed();
        obstacle = ObstacleDistance(modifier.obstacleRangeSeconds);
        print("alt: " + alt + "; angle: " + angle + "; vSpeed:" + vSpeed);

        if (airSpeed > 30 && obstacle != null)
        {
            print("Terrain!");
            PlaySound(obstacleClip);
        }
        else if (modifier.isVerticleSpeedWarningEnabled && ((vSpeed < -50) || (alt <= 600 && vSpeed < -30)))
        {
            print("Pull Up!");
            PlaySound(pullupClip);
        }
        else if (modifier.isAngleWarningEnabled && (alt < 5000 && angle > 30))
        {
            print("Bank Angle!");
            PlaySound(angleClip);
        }
        else if (modifier.isStallWarningEnabled && (alt > 10 && airSpeed < 150))
        {
            print("Stall!");
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(clip);
            print(clip.name);
        }
    }

    private float GroundAltitude()
    {
        var parts = sp.PlayerAircraft.Parts;
        var lowest = parts.OrderBy((go) => (go.transform.position.y)).First();
        var pos = lowest.transform.position;
        RaycastHit hit;
        if (!Physics.Raycast(new Ray(pos, -Vector3.up), out hit))
        {
            return Mathf.Infinity;
        }
        for (int i = 0; i < 10 && hit.collider.bounds.size.x < 10; i++)
        {
            pos = hit.point;
            pos.y -= 0.1f;
            if (!Physics.Raycast(new Ray(pos, -Vector3.up), out hit))
            {
                return Mathf.Infinity;
            }
        }
        return hit.distance * m2ft;
    }

    private float BankAngle()
    {
        var rot = sp.PlayerAircraft.MainCockpitRotation;
        var x = rot.x;
        x = x < 180 ? x : (360 - x);
        var z = rot.z;
        z = z < 180 ? z : (360 - z);
        return Mathf.Max(x, z);
    }

    private float VerticalSpeed()
    {
        return sp.PlayerAircraft.Velocity.y * m2ft;
    }

    private float AirSpeed()
    {
        return sp.PlayerAircraft.Airspeed * m2kn;
    }

    private float? ObstacleDistance(float time = 15)
    {
        var velocity = sp.PlayerAircraft.Velocity;
        var cockpitPos = sp.PlayerAircraft.MainCockpitPosition;
        var parts = sp.PlayerAircraft.Parts;
        var lowest = parts.OrderBy((go) => (Mathf.Abs(go.transform.position.z - cockpitPos.z))).First();
        var pos = velocity.normalized * (lowest.transform.position.z - cockpitPos.z) + cockpitPos;
        RaycastHit hit;
        if (!Physics.Raycast(new Ray(pos, velocity), out hit, velocity.magnitude * time))
        {
            return null;
        }
        for (int i = 0; i < 10 && hit.collider.bounds.size.x < 10; i++)
        {
            pos = hit.point;
            pos.y -= 0.1f;
            if (!Physics.Raycast(new Ray(pos, velocity), out hit, velocity.magnitude * time))
            {
                return null;
            }
        }
        return hit.distance * m2ft;
    }
}
