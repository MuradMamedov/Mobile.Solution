using System;
using Mobile.Solution.Infrastructure.Requests.NSI.Contracts;

namespace Mobile.Solution.Infrastructure.Requests.Reports
{
    public enum CargoPlanConditionScheme : byte
    {
        /// <summary>
        ///     Отчет по УК ГУ-12
        /// </summary>
        Gu12Rep = 0,

        /// <summary>
        ///     Отчет по жд суткам
        /// </summary>
        RwDayRep
    }

    public enum CargoPlanTypeScheme : byte
    {
        /// <summary>
        ///     Оперативный
        /// </summary>
        OperativePlan = 0,

        /// <summary>
        ///     Первоначальный
        /// </summary>
        InitialPlan
    }

    public enum ReportDirections : byte
    {
        /// <summary>
        ///     Отправление
        /// </summary>
        Departure = 0,

        /// <summary>
        ///     Назначение
        /// </summary>
        Arrival
    }

    public enum PeriodTypes : byte
    {
        /// <summary>
        ///     Месяц
        /// </summary>
        Month = 0,

        /// <summary>
        ///     Произвольный период
        /// </summary>
        AnyPeriod
    }

    public enum SourceType
    {
        EtranType,
        RzdType
    }

    public enum UnitTypes
    {
        Cars = 0,
        Weights = 1
    }

    public class PlanFactParameters
    {
        public ReportDirections? _reportDirection;

        public PlanFactParameters()
        {
        }

        public PlanFactParameters(PlanFactParameters from)
        {
            Date = from.Date;
            DateFrom = from.DateFrom;
            DateTo = from.DateTo;
            ToStation = from.ToStation;
            ToRwCode = from.ToRwCode;
            FromStation = from.FromStation;
            FromRwCode = from.FromRwCode;
            FreightGroup = from.FreightGroup;
            ConditionScheme = from.ConditionScheme;
            ReportDirection = from.ReportDirection;
            PlanType = from.PlanType;
            Recip = from.Recip;
            Sender = from.Sender;
            Payer = from.Payer;
            Owner = from.Owner;
            PeriodType = from.PeriodType;
            SourceType = from.SourceType;
            UnitType = from.UnitType;
        }

        public static string Extension { get; } = "pfaf";

        public DateTime Date { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public InfoOrgPassport Recip { get; set; }

        public InfoUniqueStation FromStation { get; set; }

        public short? FromRwCode { get; set; }

        public InfoOrgPassport Sender { get; set; }

        public InfoUniqueStation ToStation { get; set; }

        public short? ToRwCode { get; set; }

        public InfoSumFreight FreightGroup { get; set; }

        public CargoPlanConditionScheme ConditionScheme { get; set; }

        public CargoPlanTypeScheme PlanType { get; set; }

        public ReportDirections? ReportDirection
        {
            get
            {
                return _reportDirection.HasValue
                    ? _reportDirection
                    : (_reportDirection = ReportType);
            }
            set { _reportDirection = value; }
        }

        /// <summary>
        ///     Deprecated
        /// </summary>
        public ReportDirections ReportType { get; set; }

        public PeriodTypes PeriodType { get; set; }

        public SourceType SourceType { get; set; }

        public UnitTypes UnitType { get; set; }

        public InfoOrgPassport Payer { get; set; }

        public InfoOrgPassport Owner { get; set; }
    }
}