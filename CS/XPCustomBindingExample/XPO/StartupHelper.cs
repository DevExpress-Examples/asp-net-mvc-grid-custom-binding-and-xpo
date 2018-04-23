using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace XPCustomBindingExample.XPO {
    public static class StartupHelper {
        public static void Seed() {
            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            XPDictionary dict = DatabaseSchemaHelper.GetDefaultDatabaseSchema();
            IDataLayer dal = XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema);
            UnitOfWork uow = new UnitOfWork(dal);
            uow.UpdateSchema();
            uow.CreateObjectTypeRecords();

            XPClassInfo classInfo = uow.GetClassInfo("", "Order");

            int ordersCnt = (int)uow.Evaluate(classInfo, CriteriaOperator.Parse("count"), null);
            if(ordersCnt > 0) return;
            var rnd = new Random(DateTime.Now.Millisecond);
            var alphabet = "abcdefghijklmnopqrstuvwxyz";
            var getRandomName = new Func<string>(() =>
            {
                var result = new StringBuilder();
                for(int i = 0; i < 5; i++) result.Append(alphabet[rnd.Next(26)]);
                return result.ToString();
            });
            int ordersCount = 1000;
            int customersCount = 100;
            List<string> customerNames = new List<string>();
            for(int i = 0; i < customersCount; i++) {
                customerNames.Add(getRandomName());
            }
            for(int i = 0; i < ordersCount; i++) {
                object order = classInfo.CreateNewObject(uow);
                classInfo.GetMember("ID").SetValue(order, i + 1);
                classInfo.GetMember("ProductName").SetValue(order, getRandomName());
                classInfo.GetMember("CustomerName").SetValue(order, customerNames[rnd.Next(customersCount)]);
                classInfo.GetMember("OrderDate").SetValue(order, new DateTime(rnd.Next(2012, 2022), rnd.Next(1, 12), rnd.Next(1, 28)));
                classInfo.GetMember("Freight").SetValue(order, rnd.Next(1000) / 100m);
            }
            uow.CommitChanges();
            dal.Dispose();
        }
    }
}