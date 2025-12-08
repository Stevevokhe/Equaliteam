using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;

public class EndgameStatsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI caseIDTextObj, propertyValueTextObj, houseDamageTextObj, RepairCostTextObj, primaryHazardTextObj;
    [SerializeField] private GameObject restartButton, BackToMenuButton;
    private bool statsShown = false;
    private bool skipRequested;
    [SerializeField] private float typingSpeed = 0.1f;

    private int caseID, houseCost, houseDamage, repairCost;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) 
        {
            SkipTypingAnimation();
        }
    }
    public void GetGameoverStats(int houseHealth)
    {
        if (!statsShown)
        {
            System.Random random = new System.Random();

            // Generate Case ID
            caseID = UnityEngine.Random.Range(0, 10000000);

            // Generate Property Value
            houseCost = random.Next(100000, 200000); // Random price of a cottage in Finland

            // Generate House damage
            houseDamage = 100 - houseHealth;

            // Generate Repair Cost
            repairCost = houseCost * houseDamage / 100;

            StartCoroutine(FillAllFieldsInOrder());
            statsShown = true;
        }
    }

    string AssessDamageSeverity(int damage)
        {
            if (damage == 0)
            {
                return "No damage to report";
            }
            else if (damage <= 20)
            {
                return "Minor Insident";
            }
            else if (damage <= 50)
            {
                return "Moderate Damage";
            }
            else if (damage <= 80)
            {
                return "Major Damage";
            }
            else return "Catastrophic event";
        }

    IEnumerator TypeTextCoroutine(string fullText, TMPro.TextMeshProUGUI textField, float delay)
        {
            textField.text = "";

            foreach (char c in fullText)
            {
                textField.text += c;
                yield return new WaitForSecondsRealtime(delay);
            }
        }

    IEnumerator FillAllFieldsInOrder()
        {
            // ID
            yield return StartCoroutine(TypeTextCoroutine(
               caseID.ToString("D8"),
               caseIDTextObj,
               typingSpeed));

            // Property Value
            yield return StartCoroutine(TypeTextCoroutine(
                $"€{houseCost}",
                propertyValueTextObj,
                typingSpeed));

            // House Damage
            yield return StartCoroutine(TypeTextCoroutine(
                $"{houseDamage}% ({AssessDamageSeverity(houseDamage)})",
                houseDamageTextObj,
                typingSpeed));

            // Repair Cost
            yield return StartCoroutine(TypeTextCoroutine(
                $"€{repairCost}",
                RepairCostTextObj,
                typingSpeed));

            StartCoroutine(ShowButtons());
        }

    void RevealScoresNow()
    {
        // ID
        caseIDTextObj.text = caseID.ToString("D8");

        // Property Value
        propertyValueTextObj.text = $"€{houseCost}";

        // House Damage
        houseDamageTextObj.text = $"{houseDamage}% ({AssessDamageSeverity(houseDamage)})";

        // Repair Cost
        RepairCostTextObj.text = $"€{repairCost}";

        ShowButtons();
    }

    IEnumerator ShowButtons()
        {
            yield return new WaitForSecondsRealtime(.3f);
            restartButton.SetActive(true);

            yield return new WaitForSecondsRealtime(.3f);
            BackToMenuButton.SetActive(true);
        }

    void SkipTypingAnimation()
    {
        if(skipRequested) return;

        skipRequested = true;

        StopAllCoroutines();

        RevealScoresNow();
    }
}
