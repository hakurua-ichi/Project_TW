using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform lever;
    public RectTransform joystickBackground;
    public float leverRange = 50f;

    private Vector2 inputDirection = Vector2.zero;
    private bool isInputActive = false;

    public Vector2 InputDirection => inputDirection;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
        isInputActive = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        pos = Vector2.ClampMagnitude(pos, leverRange);
        lever.anchoredPosition = pos;

        inputDirection = pos / leverRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
        isInputActive = false;
    }

    public bool IsMoving()
    {
        return isInputActive && inputDirection.magnitude > 0.1f;
    }

    public float GetHorizontal()
    {
        return inputDirection.x;
    }

    public float GetVertical()
    {
        return inputDirection.y;
    }
}
