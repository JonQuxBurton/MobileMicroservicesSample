namespace MobileTelecomsNetwork.EventHandlers.Domain
{
    public class ActivateResult
    {
        internal readonly bool isError;

        public bool IsAccepted { get; internal set; }
        public string Reason { get; internal set; }
    }
}