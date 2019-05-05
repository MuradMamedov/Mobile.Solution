using System;
using System.Collections.Generic;
using System.Linq;
using Mobile.Solution.Infrastructure.Requests.Reports;
using Mobile.Solution.Infrastructure.Requests.Reports.Contracts;

namespace Mobile.Solution.UI.Helpers
{
    public abstract class Creator
    {
        private List<Creator> _allTabs;

        private List<Creator> _excludes = new List<Creator>();
        Func<CargoPlanFact, bool> _filter;
        Func<RegCard, bool> _rcFilter;
        private List<Creator> _tabs;

        protected Creator(ReportDirections direction, params Creator[] excludeTypes)
        {
            Direction = direction;
            _excludes = new List<Creator>(excludeTypes);
        }

        protected List<Creator> AllTabs
        {
            get
            {
                if (_allTabs == null)
                {
                    _allTabs = new List<Creator>
                    {
                        CreateChild<RailwayCreator>(ReverseDirection),
                        CreateChild<RailwayCreator>(Direction),
                        CreateChild<ContractorCreator>(ReverseDirection),
                        CreateChild<ContractorCreator>(Direction),
                        CreateChild<StationCreator>(ReverseDirection),
                        CreateChild<StationCreator>(Direction),
                        CreateChild<PayerCreator>(Direction),
                        CreateChild<OwnerCreator>(Direction),
                        CreateChild<FreightCreator>(Direction)
                    };
                    foreach (var exc in _excludes)
                    {
                        _allTabs.Remove(_allTabs.FirstOrDefault(t => t.Header == exc.Header));
                    }
                }
                return _allTabs;
            }
        }

        public List<Creator> Tabs
        {
            get
            {
                if (_tabs == null)
                {
                    AllTabs.Remove(AllTabs.FirstOrDefault(t => t.Header == Header));
                    var parent = Parent;
                    while (parent != null)
                    {
                        AllTabs.Remove(AllTabs.FirstOrDefault(t => t.GetType() == parent.GetType() && t.Direction == parent.Direction));
                        parent = parent.Parent;
                    }
                    _tabs = new List<Creator>(AllTabs);
                    _tabs.Add(CreateChild<RegCardCreator>(Direction));
                }
                return _tabs;
            }
        }

        public ReportDirections Direction { get; }

        public ReportDirections ReverseDirection
        {
            get
            {
                return Direction == ReportDirections.Arrival ? ReportDirections.Departure : ReportDirections.Arrival;
            }
        }

        public string SelectedItem { get; set; }

        public int Level { get; private set; } = 0;

        public string Header { get; protected set; }

        public Func<CargoPlanFact, bool> Filter
        {
            get
            {
                var f1 = Parent?.Filter == null ? (v => true) : Parent.Filter;
                var f2 = _filter == null ? (v => true) : _filter;
                return v => f1(v) && f2(v);
            }

            protected set { _filter = value; }
        }

        public Func<RegCard, bool> RcFilter
        {
            get
            {
                var f1 = Parent?.Filter == null ? (v => true) : Parent.RcFilter;
                var f2 = _rcFilter == null ? (v => true) : _rcFilter;
                return v => f1(v) && f2(v);
            }

            protected set { _rcFilter = value; }
        }

        public Func<CargoPlanFact, string> Group { get; protected set; }

        public Creator Parent { get; private set; }

        protected Creator CreateChild<T>(ReportDirections direction)
            where T : Creator
        {
            var child = (T) Activator.CreateInstance(typeof(T), direction, _excludes.ToArray());
            child.Level = Level + 1;
            child.Parent = this;

            return child;
        }

        public Creator Clone()
        {
            var clone = (Creator) Activator.CreateInstance(GetType(), Direction, _excludes.ToArray());
            clone.Level = Level;
            clone.SelectedItem = SelectedItem;
            if (Parent != null)
                clone.Parent = Parent.Clone();
            return clone;
        }
    }
}