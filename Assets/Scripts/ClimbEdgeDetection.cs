using Assets.Scripts;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbEdgeDetection : XRBaseInteractable
{
    [SerializeField] private PlayerClimbState _playerClimbState;

    protected override void Awake()
    {
        base.Awake();
        FindClimbingProvider();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        TryAdd(args.interactorObject);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        TryRemove(args.interactorObject);
    }

    private void TryAdd(IXRInteractor interactor)
    {
        if (interactor.transform.TryGetComponent(out VelocityController controller))
        {
            _playerClimbState.AddController(controller);
        }
    }

    private void TryRemove(IXRInteractor interactor)
    {
        if (interactor.transform.TryGetComponent(out VelocityController controller))
        {
            _playerClimbState.RemoveController(controller);
        }
    }

    private void FindClimbingProvider()
    {
        if (!_playerClimbState)
        {
            _playerClimbState = FindObjectOfType<PlayerClimbState>();
        }
    }

    public override bool IsHoverableBy(IXRHoverInteractor interactor)
    {
        return base.IsHoverableBy(interactor) && interactor is XRDirectInteractor;
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        return base.IsSelectableBy(interactor) && interactor is XRDirectInteractor;
    }
}