using System;

namespace Mobile.Solution.Infrastructure.Requests.NSI.Contracts
{
    public class InfoCarInAgreement : INsiItem
    {
        #region Properties

        public string RequestParameter => "InfoCarInAgreement";

        public string DisplayName => Number.ToString();

        public int Number { get; set; }

        /// <summary>
        ///     Тип нумерации.
        /// </summary>
        public short? NumerationId { get; set; }

        /// <summary>
        ///     ID по НСИ – InfoAgreement.
        /// </summary>
        public int? AgreementId { get; set; }

        /// <summary>
        ///     Дата включения/исключения
        /// </summary>
        public DateTime? DateInclude { get; set; }

        /// <summary>
        ///     Пользователь включивший вагон
        /// </summary>
        public string User { get; set; }

        /// <summary>
        ///     Дата включения/исключения
        /// </summary>
        public DateTime? DateExclude { get; set; }

        #endregion //Properties
    }
}