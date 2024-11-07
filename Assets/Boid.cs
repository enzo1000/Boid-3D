using UnityEngine;

public class Boid : MonoBehaviour
{
    //Parametres specifiques aux boids
    BoidSettings settings;

    // Les etats de nos boids (afficher les gizmo si possible)
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    private Vector3 velocity;

    // Les parametres a update
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;  //Defini le nombre de boid visible (selon les settings)
    private Vector3 acceleration;

    // Cached
    private Material material;
    private Transform cachedTransform;
    private Transform target;   //Defini lors de l'initialisation

    void Awake()
    {
        material = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedTransform = transform;
    }
    
    //Fonction appele lors de l'initialisation de nos Boid
    public void Initialize(BoidSettings settings, Transform target)
    {
        //On recupere la position de notre cible ainsi que nos settings de Boid
        this.target = target;
        this.settings = settings;

        //On update nos etats
        position = cachedTransform.position;
        forward = cachedTransform.forward;

        //On change la velocite en fonction de nos settings (moyenne de vitesseMin + vitesseMax)
        float startSpeed = (settings.vitesseMin + settings.vitesseMax) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour(Color col)
    {
        if (material != null)
        {
            material.color = col;
        }
    }

    //Fonction appele dans la methode Update de BoidManager
    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero;

        //Si notre Boid a une cible
        if (target != null)
        {
            //Vecteur target -> boid
            Vector3 offsetToTarget = (target.position - position);
            //Defini l'acceleration du Boid
            acceleration = SteerTowards(offsetToTarget) * settings.poidCible;
        }

        if (numPerceivedFlockmates != 0)
        {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask))
        {
            return true;
        }
        else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                return dir;
            }
        }

        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }

}