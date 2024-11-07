using UnityEngine;

[CreateAssetMenu]
public class BoidSettings : ScriptableObject
{
    // Paramettres pour nos Boids
    public float vitesseMin = 2;
    public float vitesseMax = 5;
    public float rayonDeVue = 2.5f;
    public float rayonDeRepulsion = 1;   //Avoidance Radius
    public float maxSteerForce = 3;
    //Pas de alignementRadius ?

    public float poidAlignement = 1;
    public float poidCohesion = 1;
    public float poidSepare = 1;

    public float poidCible = 1;

    //Pour la gestion de collision
    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;
}