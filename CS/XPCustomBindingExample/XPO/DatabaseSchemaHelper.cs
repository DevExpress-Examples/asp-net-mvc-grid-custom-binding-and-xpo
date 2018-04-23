using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace XPCustomBindingExample.XPO {
    public static class DatabaseSchemaHelper {
        public static XPDictionary GetDefaultDatabaseSchema() {
            XPDictionary dictionary = new ReflectionDictionary();

            XPClassInfo ordersTable = dictionary.CreateClass("Order");
            ordersTable.CreateMember("ID", typeof(int), new KeyAttribute(true));
            ordersTable.CreateMember("ProductName", typeof(string));
            ordersTable.CreateMember("CustomerName", typeof(string));
            ordersTable.CreateMember("OrderDate", typeof(DateTime));
            ordersTable.CreateMember("Freight", typeof(decimal));

            return dictionary;
        }
    }
}