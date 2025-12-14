using TMPro;
using System.Globalization;
using UnityEngine;
using System.Collections;

public class EndgameStatsManager : MonoBehaviour
{

    [SerializeField] private float typingSpeed = 0.1f;
    [SerializeField] private string[] additionalCharges, discounts;
    [HideInInspector][SerializeField]
    private TextMeshProUGUI caseIDTextObj,
        propertyValueTextObj, houseDamageTextObj, repairCostTextObj,
        fireDamageTextObj, waterDamageTextObj, electricalDamageTextObj, structuralDamageTextObj, cleanUpTextObj,
        extraCharge1TextObj, extraCharge2TextObj,
        discount1TextObj, discount2TextObj,
        adjustedTotalTextObj;

    [HideInInspector][SerializeField] private GameObject approveStumpSprite, backToMenuButton;
    private bool statsShown, skipRequested = false;

    private int caseID;
    private float  houseCost, houseDamage, repairCost,
        fireDamage, waterDamage, electricalDamage, structuralDamage, cleanUp;
    private string extraCharge1, extraCharge2, discount1, discount2;

    private static readonly CultureInfo fiCulture = new CultureInfo("fi-FI");

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
            // Generate Case ID
            caseID = Random.Range(0, 100000000);


            // ============= SUMMARY =============
            // Generate Property Value
            houseCost = Random.Range(200000, 300000); // Random price of a cottage in Finland

            // Generate House damage
            houseDamage = 100 - houseHealth;

            // Generate Repair Cost
            if (houseDamage <= 0)
            {
                repairCost = 0;
            }
            else
            {
                float damageFactor = houseDamage / 100f;

                float minPct = 0.05f;   // 5%
                float maxPct = 0.35f;   // 35%

                float repairPct = Mathf.Lerp(minPct, maxPct, damageFactor);

                repairCost = houseCost * repairPct;
            }

            // ============= DAMAGE BREAKDOWN =============
            // Generate breakdown values
            fireDamage = repairCost * 0.28f;
            waterDamage = repairCost * 0.26f;
            electricalDamage = repairCost * 0.09f;
            structuralDamage = repairCost * 0.22f;
            cleanUp = repairCost * 0.15f;

            if (repairCost > 0)
            {
                // ============= ADDITIONAL CHARGES =============
                extraCharge1 = ChooseRandomString(additionalCharges);
                extraCharge2 = ChooseRandomString(additionalCharges, extraCharge1);

                // ============= DISCOUNTS =============
                discount1 = ChooseRandomString(discounts);
                discount2 = ChooseRandomString(discounts, discount1);
            }
            else
            {
                // No charges when no damage
                extraCharge1 = "";
                extraCharge2 = "";

                discount1 = "";
                discount2 = "";
            }



            StartCoroutine(FillAllFieldsInOrder());
            statsShown = true;
        }
    }

    string ChooseRandomString(string[] array)
    {
        if (array == null || array.Length == 0)
            return string.Empty;

        int index = Random.Range(0, array.Length);
        return array[index];
    }

    string ChooseRandomString(string[] array, string firstString)
    {
        if (array == null || array.Length == 0)
            return string.Empty;

        string s;
        do
        {
            s = array[Random.Range(0, array.Length)];
        } while (s == firstString);

        return s;
    }

    string AssessDamageSeverity(int damage)
        {
            if (damage == 0) // No damage
            {
                return "No damage to report";
            }
            else if (damage <= 20) // 1 - 20
            {
                return "Minor Damage";
            }
            else if (damage <= 50) // 21 - 50
            {
                return "Moderate Damage";
            }
            else if (damage <= 80) // 51 - 80
            {
                return "Major Damage";
            }
            else return "Catastrophic event"; // 81 - 100
        }

    IEnumerator TypeTextCoroutine(string fullText, TextMeshProUGUI textField, float delay)
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

        // ============= SUMMARY =============
        // Property Value
        yield return StartCoroutine(TypeTextCoroutine(
                "€" + houseCost.ToString("N0", fiCulture),
                propertyValueTextObj,
                typingSpeed));

            // House Damage
            yield return StartCoroutine(TypeTextCoroutine(
                $"{houseDamage}% ({AssessDamageSeverity(((int)houseDamage))})",
                houseDamageTextObj,
                typingSpeed));

            // Repair Cost
            yield return StartCoroutine(TypeTextCoroutine(
                "€" + repairCost.ToString("N0", fiCulture),
                repairCostTextObj,
                typingSpeed));

        // ============= DAMAGE BREAKDOWN =============
        yield return StartCoroutine(TypeTextCoroutine(
            "€" + fireDamage.ToString("N0", fiCulture),
            fireDamageTextObj, typingSpeed));

        yield return StartCoroutine(TypeTextCoroutine(
            "€" + waterDamage.ToString("N0", fiCulture),
            waterDamageTextObj, typingSpeed));

        yield return StartCoroutine(TypeTextCoroutine(
            "€" + electricalDamage.ToString("N0", fiCulture),
            electricalDamageTextObj, typingSpeed));

        yield return StartCoroutine(TypeTextCoroutine(
            "€" + structuralDamage.ToString("N0", fiCulture),
            structuralDamageTextObj, typingSpeed));

        yield return StartCoroutine(TypeTextCoroutine(
            "€" + cleanUp.ToString("N0", fiCulture),
            cleanUpTextObj, typingSpeed));


        // ============= ADDITIONAL CHARGES =============
        if (repairCost > 0)
        {
            yield return StartCoroutine(TypeTextCoroutine(extraCharge1, extraCharge1TextObj, typingSpeed));
            yield return StartCoroutine(TypeTextCoroutine(extraCharge2, extraCharge2TextObj, typingSpeed));
        }
        else
        {
            extraCharge1TextObj.text = "";
            extraCharge2TextObj.text = "";
        }


        // ============= DISCOUNTS =============
        if (repairCost > 0)
        {
            yield return StartCoroutine(TypeTextCoroutine(discount1, discount1TextObj, typingSpeed));
            yield return StartCoroutine(TypeTextCoroutine(discount2, discount2TextObj, typingSpeed));
        }
        else
        {
            discount1TextObj.text = "";
            discount2TextObj.text = "";
        }

        StartCoroutine(ShowButtons());
        }

    void RevealScoresNow()
    {
        // ID
        caseIDTextObj.text = caseID.ToString("D8");

        // ============= SUMMARY =============
        // Property Value
        propertyValueTextObj.text = "€" + houseCost.ToString("N0", fiCulture);

        // House Damage
        houseDamageTextObj.text = $"{houseDamage}% ({AssessDamageSeverity(((int)houseDamage))})";

        // Repair Cost
        repairCostTextObj.text = "€" + repairCost.ToString("N0", fiCulture);

        // ============= DAMAGE BREAKDOWN =============
        fireDamageTextObj.text = "€" + fireDamage.ToString("N0", fiCulture);
        waterDamageTextObj.text = "€" + waterDamage.ToString("N0", fiCulture);
        electricalDamageTextObj.text = "€" + electricalDamage.ToString("N0", fiCulture);
        structuralDamageTextObj.text = "€" + structuralDamage.ToString("N0", fiCulture);
        cleanUpTextObj.text = "€" + cleanUp.ToString("N0", fiCulture);

        // ============= ADDITIONAL CHARGES =============
        if (repairCost > 0)
        {
            extraCharge1TextObj.text = extraCharge1;
            extraCharge2TextObj.text = extraCharge2;
        }
        else
        {
            discount1TextObj.text = "";
            discount2TextObj.text = "";
        }

        // ============= DISCOUNTS =============
        if (repairCost > 0)
        {
            discount1TextObj.text = discount1;
            discount2TextObj.text = discount2;
        }
        else
        {
            discount1TextObj.text = "";
            discount2TextObj.text = "";
        }

        StartCoroutine(ShowButtons());
    }

    IEnumerator ShowButtons()
        {
            yield return new WaitForSecondsRealtime(.5f);
            approveStumpSprite.SetActive(true);

            yield return new WaitForSecondsRealtime(.5f);
            backToMenuButton.SetActive(true);
        }

    void SkipTypingAnimation()
    {
        if(skipRequested) return;

        skipRequested = true;

        StopAllCoroutines();

        RevealScoresNow();
    }
}
