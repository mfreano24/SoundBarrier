using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SightlineVisualizer : MonoBehaviour
{
    //public LineRenderer left;
    //public LineRenderer right;
    public float FOV = 30f;
    public LayerMask lm;
    public Renderer domeRenderer;
    public Light robotLight;

    Color currentColor;

    //SL's FoV cone tutorial.
    public float meshResolution;
    public float viewRadius;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    MeshCollider mc;
    public Material meshMaterial;
    public Transform childTransform;
    float meshY;

    AirBob robotBody;

    Vector3 currentGroundPosition;

    public Color CurrentColor
    {
        get
        {
            return currentColor;
        }
        set
        {
            StartCoroutine(FadeColor(value));   
        }
    }

    private void Awake()
    {
        currentGroundPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        robotBody = transform.parent.GetComponent<AirBob>();
    }

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "FOVMesh";
        viewMeshFilter.mesh = viewMesh;
        mc = GetComponent<MeshCollider>();

        mc.sharedMesh = viewMesh;
        mc.convex = true;
        mc.isTrigger = true; //this is so we dont run through it

        mc.enabled = false; //this collider just sucks, it is also unnecessary
        //meshMaterial = new Material(meshMaterial); //make a copy for each enemy
        meshMaterial.color = new Color(1, 1, 1, 0.45f); //dynamic mesh color is not working out very well
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(currentGroundPosition, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(domeRenderer.transform.position, 0.5f);
    }

    void LateUpdate()
    {
        
        RaycastHit hit;
        if(Physics.Raycast(domeRenderer.transform.position, Vector3.down, out hit, Mathf.Infinity, lm))
        {
            robotBody.defaultY = hit.point.y + 0.5f;
            currentGroundPosition = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            meshY = hit.point.y + 0.1f; //set the mesh's position to the ground below the robot regardless of height.
        }
        else
        {
            meshY = transform.position.y;
        }
        //Debug.Log("meshY = " + meshY);
        transform.position = new Vector3(childTransform.position.x, meshY, childTransform.position.z);
        transform.eulerAngles = childTransform.eulerAngles;
        
        DrawFieldOfView();
    }

    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(FOV * 2 * meshResolution);
        float stepAngleSize = (FOV * 2) / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for(int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - FOV + stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + directionFromAngle(angle, true) * viewRadius, Color.yellow);
            ViewCastInfo newViewCast = ViewCast(angle);
            if(i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.ptA != Vector3.zero)
                    {
                        viewPoints.Add(edge.ptA);
                    }
                    if(edge.ptB != Vector3.zero)
                    {
                        viewPoints.Add(edge.ptB);
                    }
                }
            }
            viewPoints.Add(newViewCast.pt);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++) 
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;

            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        

    }

    EdgeInfo FindEdge(ViewCastInfo minViewcast, ViewCastInfo maxViewcast)
    {
        float minAngle = minViewcast.angle;
        float maxAngle = maxViewcast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        for(int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewcast.dist - newViewCast.dist) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewcast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.pt;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.pt;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);

    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = directionFromAngle(globalAngle, true);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, lm))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    

    public Vector3 directionFromAngle(float angleInDegrees, bool global)
    {
        if (!global)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    IEnumerator FadeColor(Color newColor)
    {
        for(int i = 0; i < 20; i++)
        {
            domeRenderer.material.color = Color.Lerp(currentColor, newColor, 1);
            robotLight.color = Color.Lerp(currentColor, newColor, 1);
            yield return new WaitForSeconds(0.025f);
        }
        
        currentColor = newColor;
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 pt;
        public float dist;
        public float angle;
        public ViewCastInfo(bool _hit, Vector3 _pt, float _dist, float _angle)
        {
            hit = _hit;
            pt = _pt;
            dist = _dist;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 ptA;
        public Vector3 ptB;

        public EdgeInfo(Vector3 _a, Vector3 _b)
        {
            ptA = _a;
            ptB = _b;
        }
    }


}
