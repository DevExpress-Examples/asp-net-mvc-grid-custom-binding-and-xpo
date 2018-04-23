using System;
using System.Collections;
using System.Web.Mvc;
using DevExpress.Data;
using DevExpress.Web.Mvc;
using DevExpress.Xpo.Metadata;
using XPCustomBindingExample.XPO;

namespace XPCustomBindingExample.Controllers {
    public class OrdersController :Controller {
        public ActionResult Index() {
            return View();
        }

        public ActionResult GridViewPart() {
            GridViewModel viewModel = GridViewExtension.GetViewModel("OrdersGridView");
            if(viewModel == null)
                viewModel = CreateGridViewModel();
            return OrdersCustomBindingCore(viewModel);
        }

        public ActionResult OrdersPagingAction(GridViewPagerState pager) {
            GridViewModel viewModel = GridViewExtension.GetViewModel("OrdersGridView");
            viewModel.ApplyPagingState(pager);
            return OrdersCustomBindingCore(viewModel);
        }

        public ActionResult OrdersSortingAction(GridViewColumnState column, bool reset) {
            GridViewModel viewModel = GridViewExtension.GetViewModel("OrdersGridView");
            viewModel.ApplySortingState(column, reset);
            return OrdersCustomBindingCore(viewModel);
        }

        public ActionResult OrdersFilteringAction(GridViewFilteringState filteringState) {
            GridViewModel viewModel = GridViewExtension.GetViewModel("OrdersGridView");
            viewModel.ApplyFilteringState(filteringState);
            return OrdersCustomBindingCore(viewModel);
        }

        public ActionResult OrdersGroupingAction(GridViewColumnState column) {
            GridViewModel viewModel = GridViewExtension.GetViewModel("OrdersGridView");
            viewModel.ApplyGroupingState(column);
            return OrdersCustomBindingCore(viewModel);
        }

        private static GridViewModel CreateGridViewModel() {
            GridViewModel viewModel = new GridViewModel();
            viewModel.KeyFieldName = "ID";
            viewModel.Columns.Add("ProductName");
            viewModel.Columns.Add("CustomerName");
            GridViewColumnState orderColumn = viewModel.Columns.Add("OrderDate");
            orderColumn.SortOrder = ColumnSortOrder.Ascending;
            orderColumn.SortIndex = 0;
            viewModel.Columns.Add("Freight");

            viewModel.TotalSummary.Add(new GridViewSummaryItemState() { FieldName = "Freight", SummaryType = SummaryItemType.Average });
            viewModel.TotalSummary.Add(new GridViewSummaryItemState() { FieldName = "OrderDate", SummaryType = SummaryItemType.Max });
            viewModel.TotalSummary.Add(new GridViewSummaryItemState() { FieldName = "OrderDate", SummaryType = SummaryItemType.Min });
            viewModel.TotalSummary.Add(new GridViewSummaryItemState() { FieldName = "ProductName", SummaryType = SummaryItemType.Count });
            GridViewSummaryItemState groupSummary = new GridViewSummaryItemState();
            groupSummary.SummaryType = SummaryItemType.Sum;
            groupSummary.FieldName = "Freight";

            return viewModel;
        }

        private PartialViewResult OrdersCustomBindingCore(GridViewModel viewModel) {
            XPDictionary databaseSchema = DatabaseSchemaHelper.GetDefaultDatabaseSchema();
            DevExpress.Xpo.Session session = XpoHelper.GetNewSession("DefaultConnection", databaseSchema);
            XPClassInfo classInfo = session.GetClassInfo("", "Order");
            XpoBindingHandlers bindingHandlers = new XpoBindingHandlers(session, classInfo);
            viewModel.ProcessCustomBinding(
                bindingHandlers.GetDataRowCount,
                bindingHandlers.GetData,
                bindingHandlers.GetSummaryValues,
                bindingHandlers.GetGroupingInfo,
                bindingHandlers.GetUniqueHeaderFilterValues
                );
            return PartialView("GridViewPartial", viewModel);
        }
    }
}