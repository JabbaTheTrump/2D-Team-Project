using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform originPoint;
    private Mesh mesh;
    [SerializeField] private float fov = 90f;
    [SerializeField] int rayCount = 75;
    [SerializeField] float viewDistance = 10f;
    private float startingAngle;


    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        PlayerSpawnController.OnLocalPlayerSpawn += SetPlayer;
    }

    void SetPlayer(GameObject playerObject)
    {
        originPoint = playerObject.transform;
        //localPlayerObject.GetComponent<PlayerController>().OnPlayerDeath += _ => PlayerDied();
        //FindObjectOfType<VirtualCameraHandler>().OnFollowTargetChanged += ChangeOriginPoint;
    }

    void PlayerDied()
    {
        if (fov != 360)
        {
            GetComponent<MeshRenderer>().enabled = false;
            enabled = false;
        }

        rayCount = 300;
        viewDistance = 10;
    }
  
    void ChangeOriginPoint(Transform point)
    {
        originPoint = point;
    }

    private void FixedUpdate()
    {
        if (originPoint == null) return;
        SetAimDirection(originPoint.eulerAngles.z + 90); //Adding 90 degrees because the "forward" of the player is actually the up axis
    }

    private void LateUpdate()
    {
        if (originPoint == null) return;

        transform.position = originPoint.position;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(originPoint.position, GetVectorFromAngle(angle), viewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                vertex = GetVectorFromAngle(angle) * viewDistance;
                vertex.z = transform.position.z;
            }
            else
            {
                Vector3 hitPoint = raycastHit2D.point;
                vertex = hitPoint - transform.position;
                vertex.z = transform.position.z;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;

            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void SetAimDirection(float angle)
    {
        startingAngle = angle + 0.5f * fov;
    }

    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }


    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
