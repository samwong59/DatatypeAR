using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class PointCloudInfo : MonoBehaviour
{
    public ARPointCloudManager pointCloudManager;
    public List<ARPoint> points = new List<ARPoint>();
    private void OnEnable()
    {
        pointCloudManager.pointCloudsChanged += PointCloudManager_pointCloudsChanged;
    }
    private void PointCloudManager_pointCloudsChanged(ARPointCloudChangedEventArgs obj)
    {
        foreach (var pointCloud in obj.added)
        {
            foreach (var pos in pointCloud.positions)
            {
                ARPoint newPoint = new ARPoint(pos);
                points.Add(newPoint);
                Debug.Log("New point added at" + pos);
            }
        }
    }
}
public class ARPoint
{
    public Vector3 position;
    public ARPoint(Vector3 pos)
    {
        position = pos;
    }
}
