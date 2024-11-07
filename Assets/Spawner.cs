using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //Type enumere pour le Gizmo
    public enum GizmoType { Never, SelectedOnly, Always }

    public Boid prefab; //Le prefab d'un Boid
    public float spawnRadius = 10.0f;
    public int spawnCount = 20;
    public Color colour;
    public GizmoType showSpawnRegion;   //Affiche selon le type enumerer certaines informations des Boid

    private void Awake()
    {
        //
        for (int i = 0; i < spawnCount; i++)
        {
            //La position est aleatoire dans le rayon defini
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;

            Boid boid = Instantiate(prefab);
            boid.transform.position = pos;
            //Initialise avec une direction aleatoire
            boid.transform.forward = Random.insideUnitSphere;
            boid.SetColour(colour);
        }
    }

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    private void DrawGizmos()
    {
        Gizmos.color = new Color(colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }
}
