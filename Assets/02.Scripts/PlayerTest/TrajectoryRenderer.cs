using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    public Transform shootPoint;
    public Vector3 initialVelocity;
    public int resolution = 30;               // 그릴 점 개수
    public float timeStep = 0.1f;             // 점 간 시간 간격
    private LineRenderer lineRenderer;
    private Vector3 gravity;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        gravity = Physics.gravity;
    }

    public void DrawTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            points[i] = CalculatePosition(startPosition, initialVelocity, t);
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(points);
    }

    Vector3 CalculatePosition(Vector3 startPos, Vector3 initVel, float t)
    {
        // 포물선 공식: p(t) = p0 + v0*t + 0.5*g*t^2
        return startPos + initVel * t + 0.5f * gravity * t * t;
    }

    public void ClearTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}
