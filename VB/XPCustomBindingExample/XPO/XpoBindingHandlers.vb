Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Web
Imports DevExpress.Data
Imports DevExpress.Data.Filtering
Imports DevExpress.Web.Mvc
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.Metadata

Namespace XPCustomBindingExample.XPO
	Public Class XpoBindingHandlers
		Private Session As Session
		Private ClassInfo As XPClassInfo

		Public Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
			Me.Session = session
			Me.ClassInfo = classInfo
		End Sub

		Public Sub GetDataRowCount(ByVal e As GridViewCustomBindingGetDataRowCountArgs)
			Dim filter As CriteriaOperator = CriteriaOperator.Parse(e.FilterExpression)
			Dim expression As CriteriaOperator = CriteriaOperator.Parse("count")
			e.DataRowCount = DirectCast(Session.Evaluate(ClassInfo, expression, filter), Integer)
		End Sub

		Public Sub GetData(ByVal e As GridViewCustomBindingGetDataArgs)
			Dim data As New XPCollection(Session, ClassInfo)
			data.Criteria = CriteriaOperator.Parse(e.FilterExpression) And GetGroupFilter(e.GroupInfoList)
			data.Sorting = GetSorting(e.State.SortedColumns, e.StartDataRowIndex <> 0 OrElse e.DataRowCount <> 0)
			data.SkipReturnedObjects = e.StartDataRowIndex
			data.TopReturnedObjects = e.DataRowCount
			e.Data = data
		End Sub

		Public Sub GetSummaryValues(ByVal e As GridViewCustomBindingGetSummaryValuesArgs)
			Dim data As New XPView(Session, ClassInfo)
			data.Criteria = CriteriaOperator.Parse(e.FilterExpression) And GetGroupFilter(e.GroupInfoList)
			For Each summaryItem As GridViewSummaryItemState In e.SummaryItems
				Dim aggregateType As Aggregate = GetAggregateType(summaryItem.SummaryType)
				Dim aggregatedExpression As CriteriaOperator = New OperandProperty(summaryItem.FieldName)
				Dim [property] As CriteriaOperator = New AggregateOperand(Nothing, aggregatedExpression, aggregateType, Nothing)
				Dim name As String = String.Concat(summaryItem.FieldName, summaryItem.SummaryType)
				data.AddProperty(name, [property])
			Next summaryItem
			Dim result As New ArrayList()
			Dim rec As ViewRecord = data(0)
			For i As Integer = 0 To data.Properties.Count - 1
				result.Add(rec(i))
			Next i
			e.Data = result
		End Sub

		Public Sub GetGroupingInfo(ByVal e As GridViewCustomBindingGetGroupingInfoArgs)
			Dim data As New XPView(Session, ClassInfo)
			data.Criteria = CriteriaOperator.Parse(e.FilterExpression) And GetGroupFilter(e.GroupInfoList)
			Dim sorting As SortDirection = GetSortDirection(e.SortOrder)
			data.AddProperty(e.FieldName, e.FieldName, True, True, sorting)
			data.AddProperty("Count", "count", False)
			Dim groupInfo As New List(Of GridViewGroupInfo)()
			For Each rec As ViewRecord In data
				Dim gi As New GridViewGroupInfo()
				gi.FieldName = e.FieldName
				gi.KeyValue = rec(e.FieldName)
				gi.DataRowCount = DirectCast(rec("Count"), Integer)
				groupInfo.Add(gi)
			Next rec
			e.Data = groupInfo
		End Sub

		Public Sub GetUniqueHeaderFilterValues(ByVal e As GridViewCustomBindingGetUniqueHeaderFilterValuesArgs)
			Dim data As New XPView(Session, ClassInfo)
			data.CriteriaString = e.FilterExpression
			data.AddProperty(e.FieldName, e.FieldName, True)
			Dim result As New ArrayList()
			For Each rec As ViewRecord In data
				result.Add(rec(e.FieldName))
			Next rec
			e.Data = result
		End Sub

		Private Function GetGroupFilter(ByVal groupInfoList As IEnumerable(Of GridViewGroupInfo)) As CriteriaOperator
			Dim groupFilter As CriteriaOperator = Nothing
			For Each groupInfo As GridViewGroupInfo In groupInfoList
				groupFilter = groupFilter And New OperandProperty(groupInfo.FieldName) = New OperandValue(groupInfo.KeyValue)
			Next groupInfo
			Return groupFilter
		End Function

		Private Function GetSorting(ByVal sortedColumns As IEnumerable(Of GridViewColumnState), ByVal isAlwaysSorted As Boolean) As SortingCollection
			Dim sorting As New SortingCollection()
			For Each sortedColumn As GridViewColumnState In sortedColumns
				Dim sortingDirection As SortingDirection = GetSortingDirection(sortedColumn.SortOrder)
				Dim sortProperty As New SortProperty(sortedColumn.FieldName, sortingDirection)
				sorting.Add(sortProperty)
			Next sortedColumn
			If sorting.Count = 0 AndAlso isAlwaysSorted Then
				Dim sortProperty As New SortProperty(ClassInfo.KeyProperty.Name, SortingDirection.Ascending)
				sorting.Add(sortProperty)
			End If
			Return sorting
		End Function

		Private Function GetSortingDirection(ByVal sortOrder As ColumnSortOrder) As SortingDirection
			Select Case sortOrder
				Case ColumnSortOrder.Descending
					Return SortingDirection.Descending
				Case Else
					Return SortingDirection.Ascending
			End Select
		End Function

		Private Function GetSortDirection(ByVal sortOrder As ColumnSortOrder) As SortDirection
			Select Case sortOrder
				Case ColumnSortOrder.Ascending
					Return SortDirection.Ascending
				Case ColumnSortOrder.Descending
					Return SortDirection.Descending
				Case Else
					Return SortDirection.None
			End Select
		End Function

		Private Function GetAggregateType(ByVal summaryType As SummaryItemType) As Aggregate
			Select Case summaryType
				Case SummaryItemType.Average
					Return Aggregate.Avg
				Case SummaryItemType.Count
					Return Aggregate.Count
				Case SummaryItemType.Max
					Return Aggregate.Max
				Case SummaryItemType.Min
					Return Aggregate.Min
				Case SummaryItemType.Sum
					Return Aggregate.Sum
				Case Else
					Dim msg As String = String.Format(CultureInfo.CurrentCulture, "The specified summary type is not supported: {0}", summaryType)
					Throw New System.NotSupportedException(msg)
			End Select
		End Function
	End Class
End Namespace