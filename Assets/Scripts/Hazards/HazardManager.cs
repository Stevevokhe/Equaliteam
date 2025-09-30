using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class HazardManager : MonoBehaviour
{
    public List<HazardBase> Hazards;



    private void Start()
    {
        HazardBase[] sceneHazards = FindObjectsByType<HazardBase>(FindObjectsSortMode.None);
        Hazards = new List<HazardBase>(sceneHazards);
    }
}
