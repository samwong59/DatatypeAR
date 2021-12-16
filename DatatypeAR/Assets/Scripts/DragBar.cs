using UnityEngine;

public class DragBar : MonoBehaviour
{

    private Vector3 mOffset;
    private float mZCoord;
    private GameObject[] chests;

    private void Start()
    {
        chests = GameObject.FindGameObjectsWithTag("Chest");
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Store offset - gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - getMouseWorldPos();
    }

    private Vector3 getMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = getMouseWorldPos() + mOffset;
    }

    private void OnMouseUp()
    {
        foreach (GameObject chest in chests)
        {
            if (Vector3.Distance(gameObject.transform.position, chest.transform.position) < 3)
            {
                gameObject.transform.position = new Vector3(0, 4, (float)-5.5);
            }
        }
    }

}
