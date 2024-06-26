<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128551043/17.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T602001)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->

# GridView for ASP.NET MVC - How to use Custom Data Binding and XPO to bind a grid to a table with an unknown schema

The approach demonstrated in this example can be used when you need to take advantage of the [partial data loading](https://docs.devexpress.com/AspNetMvc/14760/components/grid-view/binding-to-data/binding-to-large-data-database-server-mode) feature, but cannot use statically declared business models mapped to the database. This approach allows you to add and remove database columns without modifying the application code, to implement a generic View consuming data from an arbitrary table (or even database) selected by the user, or to implement a SaaS application.

## Prerequisites

This example is based on the following technologies:
1. [eXpress Persistent Objects](https://docs.devexpress.com/XPO/1998/express-persistent-objects) (XPO) are used as a data access layer. Refer to tutorials in our online documentation to get started with XPO: [Getting Started](https://docs.devexpress.com/XPO/2263/getting-started).
2. XPO supports dynamic mapping to the database tables without declaring any classes.  Refer to the following article for more information about this functionality: [How to create persistent metadata on the fly and load data from an arbitrary table](https://supportcenter.devexpress.com/ticket/details/k18482/how-to-create-persistent-metadata-on-the-fly-and-load-data-from-an-arbitrary-table).
3. Our [GridView](https://docs.devexpress.com/AspNetMvc/8966/components/grid-view) component for ASP.NET MVC platform supports [Custom Data Binding](https://docs.devexpress.com/AspNetMvc/14321/components/grid-view/binding-to-data/custom-data-binding) that allows you to populate a grid with data while taking into account the current grid state (filtering, sorting, grouping).
4. The [XPCollection](https://docs.devexpress.com/XPO/DevExpress.Xpo.XPCollection) object can be used to load objects with unknown type at compile time. The [XPView](https://docs.devexpress.com/XPO/DevExpress.Xpo.XPView) object allows you to build complex queries with grouping, filtering, sorting, and data aggregation.

Refer to our online documentation for more information about the concepts used in this example.


## Implementation details

1. To change the persistent classes schema at run time, you need to create a separate [Data Access Layer](https://docs.devexpress.com/XPO/2123/connect-to-a-data-store) for each user instead of sharing a single [ThreadSafeDataLayer](https://docs.devexpress.com/XPO/DevExpress.Xpo.ThreadSafeDataLayer) instance between all users.  
Refer to the [XpoHelper.cs](./CS/XPCustomBindingExample/XPO/XpoHelper.cs) (VB: [XpoHelper.vb](./VB/XPCustomBindingExample/XPO/XpoHelper.vb)) file for implementation details.
 
1. To handle the concurrent access to a [data store](https://docs.devexpress.com/CoreLibraries/DevExpress.Xpo.DB.IDataStore), this example uses the `DataStorePool` component. XPO automatically creates `DataStorePool` when the connection string contains the special parameter. The [XpoDefault.GetConnectionPoolString](https://docs.devexpress.com/XPO/devexpress.xpo.xpodefault.getconnectionpoolstring.overloads) method is used to prepare such connection string.
 
1. `GetNewSession` and `GetNewUnitOfWork` methods implemented in the extended `XpoHelper` class accept an [XPDictionary](https://docs.devexpress.com/XPO/DevExpress.Xpo.Metadata.XPDictionary) instance as the last parameter. `XPDictionary` provides metadata information used to map persistent objects to database tables. In the example, the `XPDictionary` instance is prepared in the [DatabaseSchemaHelper.cs](./CS/XPCustomBindingExample/XPO/DatabaseSchemaHelper.cs) (VB: [DatabaseSchemaHelper.vb](./VB/XPCustomBindingExample/XPO/DatabaseSchemaHelper.vb)) file. This implementation is intended for demonstration purposes only. In real projects, it should be replaced with a custom implementation integrated with the application architecture and business requirements.
 
1. The [XpoBindingHandlers.cs](./CS/XPCustomBindingExample/XPO/XpoBindingHandlers.cs) (VB: [XpoBindingHandlers.vb](./VB/XPCustomBindingExample/XPO/XpoBindingHandlers.vb)) file contains a universal class that can be used in real projects without modifications. It provides the [implementation of typed method delegates](https://docs.devexpress.com/AspNetMvc/14553/components/grid-view/binding-to-data/custom-data-binding/implementation-of-typed-method-delegates) required to calling to a grid view model's [GridViewModel.ProcessCustomBinding](https://docs.devexpress.com/AspNetMvc/DevExpress.Web.Mvc.GridViewModel.ProcessCustomBinding.overloads) method. The usage is demonstrated in the [OrdersController.cs](./CS/XPCustomBindingExample/Controllers/OrdersController.cs) (VB: [OrdersController.vb](./VB/XPCustomBindingExample/Controllers/OrdersController.vb)) file.
 
 
## How to use


To add this functionality to an existing ASP.NET MVC project, do the following:

1. Copy the [XpoHelper.cs](./CS/XPCustomBindingExample/XPO/XpoHelper.cs) (VB: [XpoHelper.vb](./VB/XPCustomBindingExample/XPO/XpoHelper.vb)) file to your project. If the project already contains a similar helper class, you can replace it or use both implementations together.
2. Add required connection strings to your **Web.config** file. Refer to the Microsoft documentation to learn more about the `<connectionStrings>` configuration section: 
    * [Creating a Connection String and Working with SQL Server LocalDB](https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/creating-a-connection-string)
    * [SQL Server Connection Strings for ASP.NET Web Applications](https://learn.microsoft.com/en-us/previous-versions/aspnet/jj653752(v=vs.110)?redirectedfrom=MSDN). 
    
    If the application uses a database other than MS SQL Server, it is necessary to add a special parameter to a connection string. Refer to the following article for details: [How To Create a Correct Connection String](https://docs.devexpress.com/XPO/2114/product-information/database-systems-supported-by-xpo#how-to-create-a-correct-connection-string).
3. Copy the [XpoBindingHandlers.cs](./CS/XPCustomBindingExample/XPO/XpoBindingHandlers.cs) (VB: [XpoBindingHandlers.vb](./VB/XPCustomBindingExample/XPO/XpoBindingHandlers.vb)) file to your project.
4. Implement a helper class that builds metadata to map persistent objects to specific tables according to your business requirements. For more information, refer to the following resources:

    * [KB Article: How to create persistent metadata on the fly and load data from an arbitrary table](https://supportcenter.devexpress.com/ticket/details/k18482/how-to-create-persistent-metadata-on-the-fly-and-load-data-from-an-arbitrary-table)
    * [DatabaseSchemaHelper.cs](./CS/XPCustomBindingExample/XPO/DatabaseSchemaHelper.cs) (VB: [DatabaseSchemaHelper.vb](./VB/XPCustomBindingExample/XPO/DatabaseSchemaHelper.vb))

Use the `XpoBindingHandler` class in your Controllers the same way as it is demonstrated in the [OrdersController.cs](./CS/XPCustomBindingExample/Controllers/OrdersController.cs) (VB: [OrdersController.vb](./VB/XPCustomBindingExample/Controllers/OrdersController.vb)) file. This class is independent from other parts of the project and can be re-used without modifications.

## Files to Review

* [OrdersController.cs](./CS/XPCustomBindingExample/Controllers/OrdersController.cs) (VB: [OrdersController.vb](./VB/XPCustomBindingExample/Controllers/OrdersController.vb))
* [DatabaseSchemaHelper.cs](./CS/XPCustomBindingExample/XPO/DatabaseSchemaHelper.cs) (VB: [DatabaseSchemaHelper.vb](./VB/XPCustomBindingExample/XPO/DatabaseSchemaHelper.vb))
* [XpoBindingHandlers.cs](./CS/XPCustomBindingExample/XPO/XpoBindingHandlers.cs) (VB: [XpoBindingHandlers.vb](./VB/XPCustomBindingExample/XPO/XpoBindingHandlers.vb))
* [XpoHelper.cs](./CS/XPCustomBindingExample/XPO/XpoHelper.cs) (VB: [XpoHelper.vb](./VB/XPCustomBindingExample/XPO/XpoHelper.vb))

## Documentation

* [How to bind editors to XPO objects in an ASP.NET MVC application](https://supportcenter.devexpress.com/ticket/details/k18525/how-to-bind-editors-to-xpo-objects-in-an-asp-net-mvc-application)
* [DevExpress XPO ORM for .NET Framework / .NET Core / .NET Standard 2.0](https://github.com/DevExpress/XPO/tree/master/Tutorials/ASP.NET/WebForms/CS)

## More Examples

* [DevExpress XPO ORM for .NET Framework / .NET Core / .NET Standard 2.0](https://github.com/DevExpress/XPO/tree/master/Tutorials/ASP.NET/WebForms/CS)
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=asp-net-mvc-grid-custom-binding-and-xpo&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=asp-net-mvc-grid-custom-binding-and-xpo&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
