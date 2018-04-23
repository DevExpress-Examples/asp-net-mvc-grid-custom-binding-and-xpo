Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Text
Imports DevExpress.Data.Filtering
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.Metadata

Namespace XPCustomBindingExample.XPO
	Public NotInheritable Class StartupHelper

		Private Sub New()
		End Sub

		Public Shared Sub Seed()
			Dim conn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
			Dim dict As XPDictionary = DatabaseSchemaHelper.GetDefaultDatabaseSchema()
			Dim dal As IDataLayer = XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema)
			Dim uow As New UnitOfWork(dal)
			uow.UpdateSchema()
			uow.CreateObjectTypeRecords()

			Dim classInfo As XPClassInfo = uow.GetClassInfo("", "Order")

			Dim ordersCnt As Integer = DirectCast(uow.Evaluate(classInfo, CriteriaOperator.Parse("count"), Nothing), Integer)
			If ordersCnt > 0 Then
				Return
			End If
			Dim rnd = New Random(Date.Now.Millisecond)
			Dim alphabet = "abcdefghijklmnopqrstuvwxyz"
			Dim getRandomName = New Func(Of String)(Function()
				Dim result = New StringBuilder()
				For i As Integer = 0 To 4
					result.Append(alphabet.Chars(rnd.Next(26)))
				Next i
				Return result.ToString()
			End Function)
			Dim ordersCount As Integer = 1000
			Dim customersCount As Integer = 100
			Dim customerNames As New List(Of String)()
			For i As Integer = 0 To customersCount - 1
				customerNames.Add(getRandomName())
			Next i
			For i As Integer = 0 To ordersCount - 1
				Dim order As Object = classInfo.CreateNewObject(uow)
				classInfo.GetMember("ID").SetValue(order, i + 1)
				classInfo.GetMember("ProductName").SetValue(order, getRandomName())
				classInfo.GetMember("CustomerName").SetValue(order, customerNames(rnd.Next(customersCount)))
				classInfo.GetMember("OrderDate").SetValue(order, New Date(rnd.Next(2012, 2022), rnd.Next(1, 12), rnd.Next(1, 28)))
				classInfo.GetMember("Freight").SetValue(order, rnd.Next(1000) / 100D)
			Next i
			uow.CommitChanges()
			dal.Dispose()
		End Sub
	End Class
End Namespace