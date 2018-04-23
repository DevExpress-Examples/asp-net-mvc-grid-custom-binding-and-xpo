Imports System
Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata

Namespace XPCustomBindingExample.XPO
	Public NotInheritable Class DatabaseSchemaHelper

		Private Sub New()
		End Sub

		Public Shared Function GetDefaultDatabaseSchema() As XPDictionary
			Dim dictionary As XPDictionary = New ReflectionDictionary()

			Dim ordersTable As XPClassInfo = dictionary.CreateClass("Order")
			ordersTable.CreateMember("ID", GetType(Integer), New KeyAttribute(True))
			ordersTable.CreateMember("ProductName", GetType(String))
			ordersTable.CreateMember("CustomerName", GetType(String))
			ordersTable.CreateMember("OrderDate", GetType(Date))
			ordersTable.CreateMember("Freight", GetType(Decimal))

			Return dictionary
		End Function
	End Class
End Namespace