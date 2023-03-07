using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HermitianSpline", menuName = "Splines/HermitianSpline", order = 1)]
public class HermitianSpline : SplineDescriptor
{
    Matrix4x4 M = new Matrix4x4(new Vector4( 2f,-2f, 1f, 1f),
                                new Vector4(-3f, 3f,-2f,-1f),
                                new Vector4( 0f, 0f, 1f, 0f),
                                new Vector4( 1f, 0f, 0f, 0f));

    public override bool IsPointAKnot(int PointID) => PointID % 2 == 0;


    public override void GetT(float u, int inputCount, out float t, out int startingPoint)
    {
        int knotCount = inputCount / 2;
        float knotQuantity = u * (knotCount - 1);
        int startingKnot = Mathf.FloorToInt(knotQuantity);

        startingPoint = startingKnot * 2;

        t = knotQuantity - startingKnot;
    }

    public override Vector3 EvaluateFromPolynomial(float u, List<Vector3> inputPoints)
    {
        GetT(u, inputPoints.Count, out float t, out int startingPoint);

        Vector3 PointA = inputPoints[startingPoint + 0];
        Vector3 DerivA = inputPoints[startingPoint + 1] - PointA;

        Vector3 PointB = inputPoints[startingPoint + 2];
        Vector3 DerivB = inputPoints[startingPoint + 3] - PointB;

        float tSqr = t * t;
        float tCube = tSqr * t;

        return (2f * tCube - 3f * tSqr + 1f) * PointA + (-2f * tCube + 3f * tSqr) * PointB + (tCube - 2f * tSqr + t) * DerivA + (tCube - tSqr) * DerivB;
    }

    public override Vector3 EvaluateFromMatrix(float u, List<Vector3> inputPoints)
    {
        GetT(u, inputPoints.Count, out float t, out int startingPoint);

        List<Vector3> intervallePoints = inputPoints.GetRange(startingPoint, 4);

        return GetGeometryMatrix(intervallePoints) * M * GetTimeVector(t);
    }

    public Vector4 GetTimeVector(float time)
    {
        float timeSqr = time * time;
        float timeCube = timeSqr * time;
        return new Vector4(timeCube, timeSqr, time, 1f);
    }

    public Matrix4x4 GetGeometryMatrix(List<Vector3> inputPoints)
    {
        Vector3 PointA = inputPoints[0];
        Vector3 DerivA = inputPoints[1] - PointA;

        Vector3 PointB = inputPoints[2];
        Vector3 DerivB = inputPoints[3] - PointB;

        return new Matrix4x4(new Vector4(PointA.x, PointA.y, PointA.z, 0f), 
                             new Vector4(PointB.x, PointB.y, PointB.z, 0f),
                             new Vector4(DerivA.x, DerivA.y, DerivA.z, 0f),
                             new Vector4(DerivB.x, DerivB.y, DerivB.z, 1f));
    }
}
