using UnityEngine.XR.ARFoundation;
using UnityEngine;

public class PointCloudInfo : MonoBehaviour
{
    private ARPointCloud pointCloud;

    public UnityEngine.UI.Text Log;

    private void OnEnable()
    {
        pointCloud = GetComponent<ARPointCloud>();
        pointCloud.updated += OnPointCloudChanged;
    }

    private void OnDisable()
    {
        pointCloud.updated -= OnPointCloudChanged;
    }

    private void OnPointCloudChanged(ARPointCloudUpdatedEventArgs eventArgs)
    {
        if (!pointCloud.positions.HasValue ||
            !pointCloud.identifiers.HasValue ||
            !pointCloud.confidenceValues.HasValue)
            return;

        var positions = pointCloud.positions.Value;
        var identifiers = pointCloud.identifiers.Value;
        var confidence = pointCloud.confidenceValues.Value;

        if (positions.Length == 0) return;

        var logText = "Number of points: " + positions.Length + "\nPoint info: x = "
            + positions[0].x + ", y = " + positions[0].y + ", z = " + positions[0].z
            + ",\n Identifier = " + identifiers[0] + ", Confidence = " + confidence[0];

        if (Log)
        {
            Log.text = logText;
        }
        else
        {
            Debug.Log(logText);
        }
    }

}
