using UnityEngine.Events;

public interface ICharacterEvents
{
    public UnityEvent<float> OnMove { get; }
    public UnityEvent OnJump { get; }
}