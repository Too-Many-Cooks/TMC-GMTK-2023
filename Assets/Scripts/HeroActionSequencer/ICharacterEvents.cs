using UnityEngine.Events;

public interface ICharacterEvents
{
    public UnityEvent<float> OnMove { get; }
    public UnityEvent<float> OnMoveOver { get; }
    public UnityEvent<bool> OnSprint { get; }
    public UnityEvent OnJump { get; }
}