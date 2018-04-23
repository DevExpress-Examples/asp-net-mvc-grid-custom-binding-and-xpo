Imports DevExpress.Xpo
Imports DevExpress.Xpo.Metadata

Namespace XPCustomBindingExample.DAL
	<NonPersistent>
	Public Class BasePersistentClass
		Inherits XPLiteObject

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub
		Public Sub New(ByVal session As Session, ByVal classInfo As XPClassInfo)
			MyBase.New(session, classInfo)
		End Sub
	End Class
End Namespace