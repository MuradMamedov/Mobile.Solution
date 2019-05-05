using System;

namespace Mobile.Solution.Infrastructure.Requests.NSI.Contracts
{
    public class InfoSumFreight : INsiItem
    {
        /// <summary>
        ///     Номер транзакции
        /// </summary>
        public long TransId { get; set; }

        /// <summary>
        ///     Порядковый номер
        /// </summary>
        public short? Number { get; set; }

        /// <summary>
        ///     Дата появления записи в таблице
        /// </summary>
        public DateTime? RecDateNew { get; set; }

        /// <summary>
        ///     Дата ввода в действие записи
        /// </summary>
        public DateTime? RecDateBegin { get; set; }

        /// <summary>
        ///     Дата вывода записи из действия
        /// </summary>
        public DateTime? RecDateEnd { get; set; }

        /// <summary>
        ///     Наименование номенклатурных групп
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Буквенное обозначение групп (шифр)
        /// </summary>
        public string CharName { get; set; }

        /// <summary>
        ///     Минимальная норма загрузки данного груза на вагон
        /// </summary>
        public short MinimalLoadRate { get; set; }

        /// <summary>
        ///     Максимальная норма загрузки данного груза на вагон
        /// </summary>
        public short MaximalLoadRate { get; set; }

        /// <summary>
        ///     Признак фиктивных групп (для фиктивных групп (40,41,42) должен быть 1, для всех остальных - null)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Признак необходимости указания веса груза в п.д.
        /// </summary>
        public string NeedWeight { get; set; }

        public string RequestParameter => "InfoSumFreight";

        public string DisplayName => Name;

        public override string ToString()
        {
            return DisplayName ?? "";
        }
    }
}