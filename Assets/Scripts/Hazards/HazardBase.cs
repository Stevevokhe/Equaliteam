using UnityEngine;

public class HazardBase : MonoBehaviour
{
    public string hazardName;
    public int hazardPhase;
    public PlayerTool requiredTool;

    public int GetHazardPhase()
    {
        return hazardPhase;
    }

    public bool CheckToolRequirement(PlayerTool heldTool)
    {
        return heldTool == requiredTool;
    }

    public void ProgressHazardPhase()
    {
        hazardPhase++;

        if (hazardPhase >= 3)
        {
            hazardPhase = 3;
        }
    }

    public void ResetHazardPhase()
    {
        hazardPhase = 1;
    }
}
