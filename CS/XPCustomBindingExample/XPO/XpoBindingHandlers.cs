using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace XPCustomBindingExample.XPO {
    public class XpoBindingHandlers {
        private Session Session;
        private XPClassInfo ClassInfo;

        public XpoBindingHandlers(Session session, XPClassInfo classInfo) {
            Session = session;
            ClassInfo = classInfo;
        }

        public void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e) {
            CriteriaOperator filter = CriteriaOperator.Parse(e.FilterExpression);
            CriteriaOperator expression = CriteriaOperator.Parse("count");
            e.DataRowCount = (int)Session.Evaluate(ClassInfo, expression, filter);
        }

        public void GetData(GridViewCustomBindingGetDataArgs e) {
            XPCollection data = new XPCollection(Session, ClassInfo);
            data.Criteria = CriteriaOperator.Parse(e.FilterExpression) & GetGroupFilter(e.GroupInfoList);
            data.Sorting = GetSorting(e.State.SortedColumns, e.StartDataRowIndex != 0 || e.DataRowCount != 0);
            data.SkipReturnedObjects = e.StartDataRowIndex;
            data.TopReturnedObjects = e.DataRowCount;
            e.Data = data;
        }

        public void GetSummaryValues(GridViewCustomBindingGetSummaryValuesArgs e) {
            XPView data = new XPView(Session, ClassInfo);
            data.Criteria = CriteriaOperator.Parse(e.FilterExpression) & GetGroupFilter(e.GroupInfoList);
            foreach(GridViewSummaryItemState summaryItem in e.SummaryItems) {
                Aggregate aggregateType = GetAggregateType(summaryItem.SummaryType);
                CriteriaOperator aggregatedExpression = new OperandProperty(summaryItem.FieldName);
                CriteriaOperator property = new AggregateOperand(null, aggregatedExpression, aggregateType, null);
                string name = string.Concat(summaryItem.FieldName, summaryItem.SummaryType);
                data.AddProperty(name, property);
            }
            ArrayList result = new ArrayList();
            ViewRecord rec = data[0];
            for(int i = 0; i < data.Properties.Count; i++)
                result.Add(rec[i]);
            e.Data = result;
        }

        public void GetGroupingInfo(GridViewCustomBindingGetGroupingInfoArgs e) {
            XPView data = new XPView(Session, ClassInfo);
            data.Criteria = CriteriaOperator.Parse(e.FilterExpression) & GetGroupFilter(e.GroupInfoList);
            SortDirection sorting = GetSortDirection(e.SortOrder);
            data.AddProperty(e.FieldName, e.FieldName, true, true, sorting);
            data.AddProperty("Count", "count", false);
            List<GridViewGroupInfo> groupInfo = new List<GridViewGroupInfo>();
            foreach(ViewRecord rec in data) {
                GridViewGroupInfo gi = new GridViewGroupInfo();
                gi.FieldName = e.FieldName;
                gi.KeyValue = rec[e.FieldName];
                gi.DataRowCount = (int)rec["Count"];
                groupInfo.Add(gi);
            }
            e.Data = groupInfo;
        }

        public void GetUniqueHeaderFilterValues(GridViewCustomBindingGetUniqueHeaderFilterValuesArgs e) {
            XPView data = new XPView(Session, ClassInfo);
            data.CriteriaString = e.FilterExpression;
            data.AddProperty(e.FieldName, e.FieldName, true);
            ArrayList result = new ArrayList();
            foreach(ViewRecord rec in data)
                result.Add(rec[e.FieldName]);
            e.Data = result;
        }

        private CriteriaOperator GetGroupFilter(IEnumerable<GridViewGroupInfo> groupInfoList) {
            CriteriaOperator groupFilter = null;
            foreach(GridViewGroupInfo groupInfo in groupInfoList)
                groupFilter &= new OperandProperty(groupInfo.FieldName) == new OperandValue(groupInfo.KeyValue);
            return groupFilter;
        }

        private SortingCollection GetSorting(IEnumerable<GridViewColumnState> sortedColumns, bool isAlwaysSorted) {
            SortingCollection sorting = new SortingCollection();
            foreach(GridViewColumnState sortedColumn in sortedColumns) {
                SortingDirection sortingDirection = GetSortingDirection(sortedColumn.SortOrder);
                SortProperty sortProperty = new SortProperty(sortedColumn.FieldName, sortingDirection);
                sorting.Add(sortProperty);
            }
            if(sorting.Count == 0 && isAlwaysSorted) {
                SortProperty sortProperty = new SortProperty(ClassInfo.KeyProperty.Name, SortingDirection.Ascending);
                sorting.Add(sortProperty);
            }
            return sorting;
        }

        private SortingDirection GetSortingDirection(ColumnSortOrder sortOrder) {
            switch(sortOrder) {
                case ColumnSortOrder.Descending:
                    return SortingDirection.Descending;
                default:
                    return SortingDirection.Ascending;
            }
        }

        private SortDirection GetSortDirection(ColumnSortOrder sortOrder) {
            switch(sortOrder) {
                case ColumnSortOrder.Ascending:
                    return SortDirection.Ascending;
                case ColumnSortOrder.Descending:
                    return SortDirection.Descending;
                default:
                    return SortDirection.None;
            }
        }

        private Aggregate GetAggregateType(SummaryItemType summaryType) {
            switch(summaryType) {
                case SummaryItemType.Average:
                    return Aggregate.Avg;
                case SummaryItemType.Count:
                    return Aggregate.Count;
                case SummaryItemType.Max:
                    return Aggregate.Max;
                case SummaryItemType.Min:
                    return Aggregate.Min;
                case SummaryItemType.Sum:
                    return Aggregate.Sum;
                default:
                    string msg = string.Format(CultureInfo.CurrentCulture, "The specified summary type is not supported: {0}", summaryType);
                    throw new System.NotSupportedException(msg);
            }
        }
    }
}