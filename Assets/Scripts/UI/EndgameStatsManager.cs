using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class EndgameStatsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI houseDamageTextObj, RepairCostTextObj;
    private bool statsShown = false;
    

    public void GetGameoverStats(int houseHealth)
    {
        if (!statsShown)
        {
            // House damage
            var houseDamage = 100 - houseHealth;
            houseDamageTextObj.text = $"Total damage to the house is {houseDamage}%";
            Debug.Log("Damage counted");

            // Repair Cost
            System.Random random = new System.Random();

            int houseCost = random.Next(100000, 200000); // Random price of a cottage in Finland
            int repairCost = houseCost * houseDamage / 100;

            RepairCostTextObj.text = $"Repair cost is {repairCost}€";
            Debug.Log("Price set");
            statsShown = true;
        }
    }
}
