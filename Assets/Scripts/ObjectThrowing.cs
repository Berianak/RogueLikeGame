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
    Vector3 trowFinish;

    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile wallTile;

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
                    trowFinish = tileMap.GetCellCenterWorld(tpos);
                    pickedItem.SetActive(true);
                    trowing = true;
                    pickedUp = false;
                }
            }
        }
        if (trowing == true)
        {
            var step = 1.0f * Time.deltaTime;
            pickedItem.transform.position = Vector3.MoveTowards(pickedItem.transform.position, trowFinish, step);
            Vector3 collisionCheckPoint = (Vector3.Normalize(trowFinish - pickedItem.transform.position)) * 1f;
            Debug.DrawRay(pickedItem.transform.position, collisionCheckPoint, UnityEngine.Color.red);
            var wallPos = tileMap.WorldToCell(collisionCheckPoint);
            var checkedTile = tileMap.GetTile(wallPos);
            if (checkedTile == wallTile)
            {
                Debug.Log("wall!");
            }
            //Debug.Log(LayerMask.GetMask("Walls"));
            //LayerMask layerMask = LayerMask.GetMask("Walls");
            //Debug.DrawRay(pickedItem.transform.position, trowFinish - pickedItem.transform.position, Color.red);
            //RaycastHit2D wallhit = Physics2D.Raycast(pickedItem.transform.position, trowFinish - pickedItem.transform.position, 0.5f, 1 << LayerMask.NameToLayer("Walls"));
            //if (Physics2D.OverlapPoint(pickedItem.transform.position, LayerMask.GetMask("Walls")))
            //Vector3 collisionCheckPoint = (Vector3.Normalize(trowFinish - pickedItem.transform.position)) * 1f;
            //Debug.DrawRay(pickedItem.transform.position, collisionCheckPoint, UnityEngine.Color.red);
            //Collider2D currentCollider = Physics2D.OverlapPoint(pickedItem.transform.position, LayerMask.GetMask("Walls"));
            //if (currentCollider != null)
            //{
            //    Debug.Log("hit");
            //    Vector3Int dropPoint = tileMap.WorldToCell(pickedItem.transform.position);
            //    trowFinish = tileMap.GetCellCenterWorld(dropPoint);
            //}
            //if (wallhit)
            //{
            //    Debug.Log("hit");
            //    Vector3Int dropPoint = tileMap.WorldToCell(pickedItem.transform.position);
            //    trowFinish = tileMap.GetCellCenterWorld(dropPoint);
            //}
            //if (pickedItem.transform.position == trowFinish)
            //{ 
            //    trowing = false;
            //    pickedItem = null;
            //}
        }
    }
}
