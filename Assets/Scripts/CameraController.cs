using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Transform rabbitTransform;
    Vector3 movePos;

    float width = 9, height = 5;
    bool isTouching;
    [SerializeField] float xMin, xMax, yMin, yMax;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rabbitTransform = GameController.instance.rabbit.transform;
        GameController.SetCameraCollider(GameController.instance.transform.Find("RoomCollider").GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movePos.Set(rabbitTransform.position.x, rabbitTransform.position.y, cam.transform.position.z);
        movePos.x = movePos.x < xMin ? xMin : movePos.x;
        movePos.y = movePos.y < yMin ? yMin : movePos.y;
        movePos.x = movePos.x > xMax ? xMax : movePos.x;
        movePos.y = movePos.y > yMax ? yMax : movePos.y;
        
        // cam.transform.position = new Vector3(
        //     Mathf.Lerp(cam.transform.position.x, movePos.x, 0.1f),
        //     Mathf.Lerp(cam.transform.position.y, movePos.y, 0.1f),
        //     cam.transform.position.z);
        cam.transform.position = Vector3.Lerp(cam.transform.position, movePos, 0.1f);
        //cam.transform.position = Vector3.MoveTowards(cam.transform.position, new Vector3(movePos.x, movePos.y, cam.transform.position.z), 0.1f);
        //cam.transform.position = movePos;
        transform.Find("/forest").transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Find("/forest").transform.rotation = transform.rotation;

        isTouching = false;
        GameController.AfterCamareUpdate();
    }

    public void SetCollider(Collider2D collider)
    {
        // xMin = collider.bounds.center.x - collider.bounds.size.x / 2 + width;
        // yMin = collider.bounds.center.y - collider.bounds.size.y / 2 + height;
        // xMax = collider.bounds.center.x + collider.bounds.size.x / 2 - width;
        // yMax = collider.bounds.center.y + collider.bounds.size.y / 2 - height;
        Vector2 direction = width * GetComponent<MoveController>().moveDirection + height * GetComponent<MoveController>().jumpDirection;
        //Debug.Log(direction);
        xMin = collider.bounds.center.x - collider.bounds.size.x / 2 + Mathf.Abs(direction.x);
        yMin = collider.bounds.center.y - collider.bounds.size.y / 2 + Mathf.Abs(direction.y);
        xMax = collider.bounds.center.x + collider.bounds.size.x / 2 - Mathf.Abs(direction.x);
        yMax = collider.bounds.center.y + collider.bounds.size.y / 2 - Mathf.Abs(direction.y);
    }
}
