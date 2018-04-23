using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace XPCustomBindingExample.XPO {
    public static class XpoHelper {
        private static ConcurrentDictionary<string, IDataStore> ConnectionProviders = new ConcurrentDictionary<string,IDataStore>();
        /// <summary>
        /// This option reduces unnecessary SELECT operations 
        /// Change this option if the inheritance mapping is required
        /// </summary>
        public static bool DontLoadClassInfoFromDatabase = true;

        static XpoHelper() {
            XpoDefault.Session = null;
            XpoDefault.DataLayer = null;
        }

        public static Session GetNewSession(string connectionName, XPDictionary databaseSchema) {
            IDataLayer dataLayer = GetDataLayer(connectionName, databaseSchema);
            return new Session(dataLayer);
        }


        public static UnitOfWork GetNewUnitOfWork(string connectionName, XPDictionary databaseSchema) {
            IDataLayer dataLayer = GetDataLayer(connectionName, databaseSchema);
            return new UnitOfWork(dataLayer);
        }

        private static IDataLayer GetDataLayer(string connectionName, XPDictionary databaseSchema) {
            IDataStore connectionProvider = GetConnectionProvider(connectionName);
            IDataLayer dataLayer = new SimpleDataLayer(databaseSchema, connectionProvider);

            if(DontLoadClassInfoFromDatabase)
                ConfigureDataLayerNotToLoadClassInfoFromDatabase(dataLayer);

            return dataLayer;
        }

        private static IDataStore GetConnectionProvider(string connectionName) {
            return ConnectionProviders.GetOrAdd(connectionName, GetConnectionPoolProvider);
        }

        private static IDataStore GetConnectionPoolProvider(string connectionName) {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            connectionString = XpoDefault.GetConnectionPoolString(connectionString);
            return XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists);
        }

        private static void ConfigureDataLayerNotToLoadClassInfoFromDatabase(IDataLayer dataLayer) {
            SimpleObjectLayer objectLayer = new SimpleObjectLayer(dataLayer);
            Dictionary<XPClassInfo, XPObjectType> loadedTypes = new Dictionary<XPClassInfo, XPObjectType>();
            objectLayer.SetObjectLayerWideObjectTypes(loadedTypes);
        }
    }
}