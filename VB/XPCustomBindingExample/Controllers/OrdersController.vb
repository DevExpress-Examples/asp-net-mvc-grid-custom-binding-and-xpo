Imports System
Imports System.Collections
Imports System.Web.Mvc
Imports DevExpress.Data
Imports DevExpress.Web.Mvc
Imports DevExpress.Xpo.Metadata
Imports XPCustomBindingExample.XPO

Namespace XPCustomBindingExample.Controllers
	Public Class OrdersController
		Inherits Controller

		Public Function Index() As ActionResult
			Return View()
		End Function

		Public Function GridViewPart() As ActionResult
			Dim viewModel As GridViewModel = GridViewExtension.GetViewModel("OrdersGridView")
			If viewModel Is Nothing Then
				viewModel = CreateGridViewModel()
			End If
			Return OrdersCustomBindingCore(viewModel)
		End Function

		Public Function OrdersPagingAction(ByVal pager As GridViewPagerState) As ActionResult
			Dim viewModel As GridViewModel = GridViewExtension.GetViewModel("OrdersGridView")
			viewModel.ApplyPagingState(pager)
			Return OrdersCustomBindingCore(viewModel)
		End Function

		Public Function OrdersSortingAction(ByVal column As GridViewColumnState, ByVal reset As Boolean) As ActionResult
			Dim viewModel As GridViewModel = GridViewExtension.GetViewModel("OrdersGridView")
			viewModel.ApplySortingState(column, reset)
			Return OrdersCustomBindingCore(viewModel)
		End Function

		Public Function OrdersFilteringAction(ByVal filteringState As GridViewFilteringState) As ActionResult
			Dim viewModel As GridViewModel = GridViewExtension.GetViewModel("OrdersGridView")
			viewModel.ApplyFilteringState(filteringState)
			Return OrdersCustomBindingCore(viewModel)
		End Function

		Public Function OrdersGroupingAction(ByVal column As GridViewColumnState) As ActionResult
			Dim viewModel As GridViewModel = GridViewExtension.GetViewModel("OrdersGridView")
			viewModel.ApplyGroupingState(column)
			Return OrdersCustomBindingCore(viewModel)
		End Function

		Private Shared Function CreateGridViewModel() As GridViewModel
			Dim viewModel As New GridViewModel()
			viewModel.KeyFieldName = "ID"
			viewModel.Columns.Add("ProductName")
			viewModel.Columns.Add("CustomerName")
			Dim orderColumn As GridViewColumnState = viewModel.Columns.Add("OrderDate")
			orderColumn.SortOrder = ColumnSortOrder.Ascending
			orderColumn.SortIndex = 0
			viewModel.Columns.Add("Freight")

			viewModel.TotalSummary.Add(New GridViewSummaryItemState() With {.FieldName = "Freight", .SummaryType = SummaryItemType.Average})
			viewModel.TotalSummary.Add(New GridViewSummaryItemState() With {.FieldName = "OrderDate", .SummaryType = SummaryItemType.Max})
			viewModel.TotalSummary.Add(New GridViewSummaryItemState() With {.FieldName = "OrderDate", .SummaryType = SummaryItemType.Min})
			viewModel.TotalSummary.Add(New GridViewSummaryItemState() With {.FieldName = "ProductName", .SummaryType = SummaryItemType.Count})
			Dim groupSummary As New GridViewSummaryItemState()
			groupSummary.SummaryType = SummaryItemType.Sum
			groupSummary.FieldName = "Freight"

			Return viewModel
		End Function

		Private Function OrdersCustomBindingCore(ByVal viewModel As GridViewModel) As PartialViewResult
			Dim databaseSchema As XPDictionary = DatabaseSchemaHelper.GetDefaultDatabaseSchema()
            Dim xpoSession As DevExpress.Xpo.Session = XpoHelper.GetNewSession("DefaultConnection", databaseSchema)
            Dim classInfo As XPClassInfo = xpoSession.GetClassInfo("", "Order")
            Dim bindingHandlers As New XpoBindingHandlers(xpoSession, classInfo)
            viewModel.ProcessCustomBinding(AddressOf bindingHandlers.GetDataRowCount, AddressOf bindingHandlers.GetData, AddressOf bindingHandlers.GetSummaryValues, AddressOf bindingHandlers.GetGroupingInfo, AddressOf bindingHandlers.GetUniqueHeaderFilterValues)
			Return PartialView("GridViewPartial", viewModel)
		End Function
	End Class
End Namespace