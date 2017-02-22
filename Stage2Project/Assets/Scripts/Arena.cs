using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Arena : MonoBehaviour
{
    [SerializeField]
    private Camera Cam;

    public static float Width { get; private set; }
    public static float Height { get; private set; }

    void Update()
    {
#if UNITY_EDITOR 
        if (!Application.isPlaying)
        {
            Calculate();
        }
#endif

    }

    public void Calculate()
    {
        if (Cam != null)
        {
            Height = CameraUtils.FrustumHeightAtDistance(Cam.farClipPlane - 1.0f, Cam.fieldOfView);
            Width = Height * Cam.aspect;
            transform.localScale = new Vector3(Width * 0.1f, 1.0f, Height * 0.1f);
        }
    }

    public Vector2 GetRandomAvailableSpawnPoint()
    {
        Calculate();
        Vector2 point = new Vector2(0, 0);

        float x = Random.Range(-Width/2, Width/2);
        float z = Random.Range(-Height/2, Height/2);

        point.x = x;
        point.y = z;

        return point;
    }

    public List<GameObject> GetAllWalls()
    {
        List<GameObject> toret = new List<GameObject>();

        foreach (Transform child in transform.GetChild(0))
        {
            toret.Add(child.gameObject);
        }


        return toret;
    }
}
