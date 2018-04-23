Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Configuration
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports DevExpress.Xpo.Metadata

Namespace XPCustomBindingExample.XPO
	Public NotInheritable Class XpoHelper

		Private Sub New()
		End Sub

		Private Shared ConnectionProviders As New ConcurrentDictionary(Of String, IDataStore)()
		''' <summary>
		''' This option reduces unnecessary SELECT operations 
		''' Change this option if the inheritance mapping is required
		''' </summary>
		Public Shared DontLoadClassInfoFromDatabase As Boolean = True

		Shared Sub New()
			XpoDefault.Session = Nothing
			XpoDefault.DataLayer = Nothing
		End Sub

		Public Shared Function GetNewSession(ByVal connectionName As String, ByVal databaseSchema As XPDictionary) As Session
			Dim dataLayer As IDataLayer = GetDataLayer(connectionName, databaseSchema)
			Return New Session(dataLayer)
		End Function


		Public Shared Function GetNewUnitOfWork(ByVal connectionName As String, ByVal databaseSchema As XPDictionary) As UnitOfWork
			Dim dataLayer As IDataLayer = GetDataLayer(connectionName, databaseSchema)
			Return New UnitOfWork(dataLayer)
		End Function

		Private Shared Function GetDataLayer(ByVal connectionName As String, ByVal databaseSchema As XPDictionary) As IDataLayer
			Dim connectionProvider As IDataStore = GetConnectionProvider(connectionName)
			Dim dataLayer As IDataLayer = New SimpleDataLayer(databaseSchema, connectionProvider)

			If DontLoadClassInfoFromDatabase Then
				ConfigureDataLayerNotToLoadClassInfoFromDatabase(dataLayer)
			End If

			Return dataLayer
		End Function

		Private Shared Function GetConnectionProvider(ByVal connectionName As String) As IDataStore
			Return ConnectionProviders.GetOrAdd(connectionName, AddressOf GetConnectionPoolProvider)
		End Function

		Private Shared Function GetConnectionPoolProvider(ByVal connectionName As String) As IDataStore
			Dim connectionString As String = ConfigurationManager.ConnectionStrings(connectionName).ConnectionString
			connectionString = XpoDefault.GetConnectionPoolString(connectionString)
			Return XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists)
		End Function

		Private Shared Sub ConfigureDataLayerNotToLoadClassInfoFromDatabase(ByVal dataLayer As IDataLayer)
			Dim objectLayer As New SimpleObjectLayer(dataLayer)
			Dim loadedTypes As New Dictionary(Of XPClassInfo, XPObjectType)()
			objectLayer.SetObjectLayerWideObjectTypes(loadedTypes)
		End Sub
	End Class
End Namespace