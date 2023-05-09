using UnityEngine.EventSystems;

public interface ISwitchActivatable :IPointerEnterHandler, IPointerExitHandler
{
    void Activate();
    void Deactivate();
    public void HighlightInteractable(float height);
}
