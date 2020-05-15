using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTable : MonoBehaviour
{
    /// <summary>
    /// tab to place gameobject that will define the spawn point of the table asset
    /// </summary>
    public GameObject tabToPlaceObject;

    /// <summary>
    /// final table asset / board the game is played on
    /// </summary>
    public GameObject tableAsset;


    private GameObject instantiatedTabToPlace;
    private GameObject instantiatedTable;
   
    /// <summary>
    ///  this will spawn the tab to place object for initially placing the table
    /// </summary>
    public void CreateTabToPlaceTable()
    {
        if (instantiatedTable == null)
        {
            instantiatedTabToPlace = Object.Instantiate(tabToPlaceObject) as GameObject;
        }
        else
        {
            Debug.Log("You can't instantiate the tab to place object anymore once the table has already been placed - move the existing table if you want to change its location");
        }
    }

    /// <summary>
    /// creates the final table asset / game board
    /// </summary>
    public void CreateTable()
    {
        instantiatedTable = Object.Instantiate(tableAsset) as GameObject;
        instantiatedTable.transform.position = instantiatedTabToPlace.transform.position;

        Object.Destroy(instantiatedTabToPlace);
    }
}
