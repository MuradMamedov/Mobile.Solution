namespace Mobile.Solution.Infrastructure.Requests.NSI.Contracts
{
    public interface INsiItem
    {
        string RequestParameter { get; }

        string DisplayName { get; }
    }
}