namespace Mobile.Solution.Infrastructure.Requests.NSI.Contracts
{
    public class InfoAgreement : INsiItem
    {
        #region Properties

        public string RequestParameter => "InfoAgreement";

        public string DisplayName => Name;

        public int Id { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        #endregion //Properties
    }
}