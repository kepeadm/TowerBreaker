using UnityEngine;
using UnityEngine.EventSystems;

public class InGameButtonHandler : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType { Move, Defense, Attack }

    [SerializeField] private ButtonType buttonType;
    [SerializeField] private PlayerCombat playerCombat;

    public void OnPointerDown(PointerEventData e)
    {
        switch (buttonType)
        {
            case ButtonType.Move:    playerCombat.OnMoveButton();        break;
            case ButtonType.Defense: playerCombat.OnDefenseButton();     break;
            case ButtonType.Attack:  playerCombat.OnAttackButtonDown();  break;
        }
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (buttonType == ButtonType.Attack)
            playerCombat.OnAttackButtonUp();
    }
}