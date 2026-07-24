using UnityEngine;
using UnityEngine.EventSystems;

// Hazır joystick UI (Background + Handle Image) üzerine eklenir.
// Touch/drag bilgisini EventSystem üzerinden alır, yön vektörünü ve basılı olma durumunu dışarı verir.
[RequireComponent(typeof(RectTransform))]
public class OnScreenJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 100f;
    [SerializeField] private float deadzone = 0.15f;

    private Vector2 rawInput = Vector2.zero;

    public Vector2 Direction { get; private set; } = Vector2.zero;
    public bool IsPressed { get; private set; } = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            float x = localPoint.x / background.sizeDelta.x;
            float y = localPoint.y / background.sizeDelta.y;

            rawInput = new Vector2(x * 2f, y * 2f);
            rawInput = rawInput.magnitude > 1f ? rawInput.normalized : rawInput;

            handle.anchoredPosition = rawInput * handleRange;

            Direction = rawInput.magnitude < deadzone ? Vector2.zero : rawInput;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        rawInput = Vector2.zero;
        Direction = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}