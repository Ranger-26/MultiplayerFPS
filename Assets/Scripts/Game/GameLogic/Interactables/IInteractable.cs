namespace Game.GameLogic.Interactables
{
    public interface IInteractable
    {
        void ServerInteract();

        void ClientInteract();

        void Highlight();

        void UnHighlight();
    }
}