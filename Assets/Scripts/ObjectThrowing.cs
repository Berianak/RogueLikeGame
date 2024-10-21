using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectThrowing : MonoBehaviour
{
    public bool canPickUp;
    public bool pickedUp;
    Collider2D item;
    GameObject pickedItem;
    bool trowing;
    Vector3 trowEnd;

    [SerializeField] Tilemap tileMap;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            if (pickedUp != true)
            {
                canPickUp = true;
                item = collision;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            canPickUp = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (canPickUp == true) // hide item from map
            {
                pickedUp = true;
                pickedItem = item.gameObject;
                pickedItem.SetActive(false);
                canPickUp = false;
            }

            else
            {
                if (pickedUp == true)  // show picked item & get trow position
                { 
                    Vector2 trowPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3Int tpos = tileMap.WorldToCell(trowPoint);
                    pickedItem.transform.position = this.transform.position;
                    trowEnd = tileMap.GetCellCenterWorld(tpos);
                    pickedItem.SetActive(true);
                    trowing = true;
                    pickedUp = false;
                }
            }
        }
        if (trowing == true)
        {
            var step = 10.0f * Time.deltaTime;
            pickedItem.transform.position = Vector3.MoveTowards(pickedItem.transform.position, trowEnd, step);
            //Vector3 collisionCheckPoint = (Vector3.Normalize(trowEnd - pickedItem.transform.position)) * 1f;
            //Debug.Log(LayerMask.GetMask("Walls"));
            //LayerMask layerMask = LayerMask.GetMask("Walls");
            //Debug.DrawRay(pickedItem.transform.position, trowEnd - pickedItem.transform.position, Color.red);
            //RaycastHit2D wallhit = Physics2D.Raycast(pickedItem.transform.position, trowEnd - pickedItem.transform.position, 0.5f, 1 << LayerMask.NameToLayer("Walls"));
            //if (Physics2D.OverlapPoint(pickedItem.transform.position, LayerMask.GetMask("Walls")))
            //Vector3 collisionCheckPoint = (Vector3.Normalize(trowEnd - pickedItem.transform.position)) * 1f;
            //Debug.DrawRay(pickedItem.transform.position, collisionCheckPoint, UnityEngine.Color.red);
            Collider2D currentCollider = Physics2D.OverlapPoint(pickedItem.transform.position, LayerMask.GetMask("Walls"));
            if (currentCollider != null)
            {
                Debug.Log("hit");
                Vector3Int dropPoint = tileMap.WorldToCell(pickedItem.transform.position);
                trowEnd = tileMap.GetCellCenterWorld(dropPoint);
            }
            //if (wallhit)
            //{
            //    Debug.Log("hit");
            //    Vector3Int dropPoint = tileMap.WorldToCell(pickedItem.transform.position);
            //    trowEnd = tileMap.GetCellCenterWorld(dropPoint);
            //}
            if (pickedItem.transform.position == trowEnd)
            { 
                trowing = false;
                pickedItem = null;
            }
        }
    }
}
