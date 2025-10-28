using UnityEngine;

[CreateAssetMenu(menuName = "NewTool")]
public class PlayerToolSO : ScriptableObject
{
    public PlayerTool PlayerTool;
    public GameObject InteractableToolObject;
    public GameObject CarriedToolObject;
}
