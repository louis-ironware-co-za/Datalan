using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DxAimArb.Services;

using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ParameterValue = DevExpress.Xpo.DB.ParameterValue;

namespace DxAimArb.Helpers
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseXpoData(this IApplicationBuilder app, string conStr)
        {
            //TODO - Connect dataStore to a web dataservice here
            IDataStore dataStore = XpoDefault.GetConnectionProvider(conStr, AutoCreateOption.DatabaseAndSchema); 

            IDictionary<string, DBTable> schema = CreateSchema(dataStore);
            CreateData(dataStore, schema);
            return app;
        }
        static IDictionary<string, DBTable> CreateSchema(IDataStore dataStore)
        {
            DBTable industry = CreateTable("Industry");
            industry.AddColumn(new DBColumn("Name", false, null, SizeAttribute.Unlimited, DBColumnType.String));

            DBTable customer = CreateTable("Customer");
            customer.AddColumn(new DBColumn("FirstName", false, null, SizeAttribute.DefaultStringMappingFieldSize, DBColumnType.String));
            customer.AddColumn(new DBColumn("LastName", false, null, SizeAttribute.DefaultStringMappingFieldSize, DBColumnType.String));

            DBTable order = CreateTable("Order");
            order.AddColumn(new DBColumn("ProductName", false, null, 200, DBColumnType.String));
            order.AddColumn(new DBColumn("OrderDate", false, null, 0, DBColumnType.DateTime));
            order.AddColumn(new DBColumn("Freight", false, null, 0, DBColumnType.Decimal));
            order.AddColumn(new DBColumn("Customer", false, null, 0, DBColumnType.Int32));
            order.AddForeignKey(new DBForeignKey(new StringCollection() { "Customer" }, "Customer", new StringCollection() { "OID" }));
            
            DBTable objectType = new DBTable("XPObjectType");
            objectType.AddColumn(new DBColumn("OID", true, null, 0, DBColumnType.Int32) { IsIdentity = true });
            objectType.AddColumn(new DBColumn("TypeName", false, null, 254, DBColumnType.String));
            objectType.AddColumn(new DBColumn("AssemblyName", false, null, 154, DBColumnType.String));
            objectType.AddIndex(new DBIndex(new StringCollection() { "TypeName" }, true));
            objectType.PrimaryKey = new DBPrimaryKey(new StringCollection() { "OID" });
            


            //add tables for Authorization/Authentication================================================

            //USER
            DBTable user = CreateTable("User");
            user.AddColumn(new DBColumn("FirstName", false, null, SizeAttribute.DefaultStringMappingFieldSize, DBColumnType.String));
            user.AddColumn(new DBColumn("LastName", false, null, SizeAttribute.DefaultStringMappingFieldSize, DBColumnType.String));

            //PROFILE
            DBTable profile = CreateTable("Profile");
            profile.AddColumn(new DBColumn("AccountNumber", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("AccountType", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("AddressCity", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("AddressCountry", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("AddressLine1", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("AddressLine2", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("AnticipatedVolume", false, null, 0, DBColumnType.Int32));
            profile.AddColumn(new DBColumn("BankName", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("City", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("CountryOfBirth", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("DateOfBirth", false, null, 0, DBColumnType.DateTime));
            profile.AddColumn(new DBColumn("Email", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("EmployerIndustry", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("EmployerName", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("FirstNames", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("Gender", false, null, 10, DBColumnType.String));
            profile.AddColumn(new DBColumn("IdDocumentType", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("IDNumber", false, null, 13, DBColumnType.String));
            profile.AddColumn(new DBColumn("Initials", false, null, 10, DBColumnType.String));
            profile.AddColumn(new DBColumn("IsUnemployed", false, null, 0, DBColumnType.Boolean));
            profile.AddColumn(new DBColumn("JobTitle", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("MobilePhoneNumber", false, null, 50, DBColumnType.String));
            profile.AddColumn(new DBColumn("MonthlyIncome", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("Nationality", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("Occupation", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("PostalCode", false, null, 10, DBColumnType.String));
            profile.AddColumn(new DBColumn("PreferredName", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("ProofOfAddressType", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("Province", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("ReferredBy", false, null, 100, DBColumnType.String));
            profile.AddColumn(new DBColumn("SignupDate", false, null, 0, DBColumnType.DateTime));
            profile.AddColumn(new DBColumn("Surname", false, null, 250, DBColumnType.String));
            profile.AddColumn(new DBColumn("TaxNumber", false, null, 20, DBColumnType.String));
            profile.AddColumn(new DBColumn("Title", false, null, 10, DBColumnType.String));
            profile.AddColumn(new DBColumn("TradeSize", false, null, 0, DBColumnType.Int32));
            profile.AddColumn(new DBColumn("UserId", false, null, 450, DBColumnType.String));


            dataStore.UpdateSchema(false, industry, customer, order, user, profile, objectType);

            return new Dictionary<string, DBTable>() {
                { "Industry", industry },
                { "Customer", customer },
                { "User", order },
                { "Order", order },
                { "Profile", profile }
            };
        }
        static DBTable CreateTable(string name)
        {
            DBTable table = new DBTable(name);
            table.AddColumn(new DBColumn("OID", true, null, 0, DBColumnType.Int32) { IsIdentity = true });
            table.AddColumn(new DBColumn("OptimisticLockField", false, null, 0, DBColumnType.Int32));
            table.AddColumn(new DBColumn("GCRecord", false, null, 0, DBColumnType.Int32));
            table.PrimaryKey = new DBPrimaryKey(new StringCollection() { "OID" });
            return table;
        }
        static void CreateData(IDataStore dataStore, IDictionary<string, DBTable> tables)
        {
            if (IsEmpty(dataStore, tables["Customer"]))
            {
                ParameterValue[] identities = CreateCustomers(dataStore, tables["Customer"]);
                CreateOrders(dataStore, tables["Order"], identities);
            }
            if (IsEmpty(dataStore, tables["Industry"]))
            {
                CreateIndustries(dataStore, tables["Industry"], industryValues);
            }
        }
        static bool IsEmpty(IDataStore dataStore, DBTable table)
        {
            SelectStatement select = new SelectStatement(table, "N0")
            {
                Condition = new QueryOperand("GCRecord", "N0", DBColumnType.Int32).IsNull(),
                TopSelectedRecords = 1
            };
            select.Operands.AddRange(new[] { new QuerySubQueryContainer(null, null, Aggregate.Count) });
            SelectedData data = dataStore.SelectData(select);
            int cnt = (int)data.ResultSet[0].Rows[0].Values[0];
            return cnt == 0;
        }
        static ParameterValue[] CreateCustomers(IDataStore dataStore, DBTable table)
        {
            int combinationsNum = firstNames.Length * lastNames.Length;
            var names = new (string FirstName, string LastName)[combinationsNum];
            for (int i = 0; i < combinationsNum; i++)
            {
                int j = Random.Next(i + 1);
                names[i] = names[j];
                names[j] = (firstNames[i / lastNames.Length], lastNames[i % lastNames.Length]);
            }
            ModificationStatement[] dmlStatements = new ModificationStatement[names.Length];
            int tag = 0;
            for (int i = 0; i < names.Length; i++)
            {
                var name = names[i];
                InsertStatement stmt = CreateInsertStatement(table, tag++, tag++, tag++);
                stmt.Operands.AddRange(new[] {
                    new QueryOperand("FirstName", null, DBColumnType.String),
                    new QueryOperand("LastName", null, DBColumnType.String),
                });
                stmt.Parameters.AddRange(new[] {
                    CreateParameter(tag++, name.FirstName),
                    CreateParameter(tag++, name.LastName)
                });
                dmlStatements[i] = stmt;
            }
            return dataStore.ModifyData(dmlStatements).Identities;
        }
        static void CreateOrders(IDataStore dataStore, DBTable table, ParameterValue[] identities)
        {
            int combinationsNum = identities.Length * 10;
            ModificationStatement[] dmlStatements = new ModificationStatement[combinationsNum];
            int tag = 0;
            for (int i = 0; i < combinationsNum; i++)
            {
                InsertStatement stmt = CreateInsertStatement(table, tag++, tag++, tag++);
                stmt.Operands.AddRange(new[] {
                    new QueryOperand("ProductName", null, DBColumnType.String),
                    new QueryOperand("OrderDate", null, DBColumnType.DateTime),
                    new QueryOperand("Freight", null, DBColumnType.Decimal),
                    new QueryOperand("Customer", null, DBColumnType.Int32)
                });
                stmt.Parameters.AddRange(new[] {
                    CreateParameter(tag++, DBColumnType.String, 200, productNames[Random.Next(productNames.Length)]),
                    CreateParameter(tag++, DBColumnType.DateTime, new DateTime(Random.Next(2014, 2024), Random.Next(1, 12), Random.Next(1, 28))),
                    CreateParameter(tag++, DBColumnType.Decimal, Random.Next(1000) / 100m),
                    CreateParameter(tag++, DBColumnType.Int32, identities[Random.Next(identities.Length)].Value)
                });
                dmlStatements[i] = stmt;
            }
            dataStore.ModifyData(dmlStatements);
        }
        static InsertStatement CreateInsertStatement(DBTable table, int identityTag, int optimisticLockFieldTag, int gcRecordTag)
        {
            InsertStatement stmt = new InsertStatement(table, null)
            {
                IdentityColumn = "OID",
                IdentityColumnType = DBColumnType.Int32,
                IdentityParameter = new ParameterValue(identityTag)
                {
                    DBType = DBColumnType.Int32
                }
            };
            stmt.Operands.AddRange(new[] {
                new QueryOperand("OptimisticLockField", null, DBColumnType.Int32),
                new QueryOperand("GCRecord", null, DBColumnType.Int32)
            });
            stmt.Parameters.AddRange(new[] {
                new ParameterValue(optimisticLockFieldTag) { DBType = DBColumnType.Int32, Value = 0 },
                new ParameterValue(gcRecordTag) { DBType = DBColumnType.Int32 }
            });
            return stmt;
        }
        static ParameterValue CreateParameter(int tag, DBColumnType type, int size, object value)
        {
            return new ParameterValue(tag) { DBType = type, Size = size, Value = value };
        }
        static ParameterValue CreateParameter(int tag, DBColumnType type, object value)
        {
            return CreateParameter(tag, type, 0, value);
        }
        static ParameterValue CreateParameter(int tag, string value)
        {
            return CreateParameter(tag, DBColumnType.String, SizeAttribute.DefaultStringMappingFieldSize, value);
        }

        static void CreateIndustries(IDataStore dataStore, DBTable table, string[] industries)
        {
            int combinationsNum = industries.Length; // * 10;
            ModificationStatement[] dmlStatements = new ModificationStatement[combinationsNum];
            int tag = 0;
            for (int i = 0; i < combinationsNum; i++)
            {
                InsertStatement stmt = CreateInsertStatement(table, tag++, tag++, tag++);
                stmt.Operands.AddRange(new[] {
                    new QueryOperand("Name", null, DBColumnType.String)
                });
                stmt.Parameters.AddRange(new[] {
                    CreateParameter(tag++, DBColumnType.String, 200, industryValues[Random.Next(industries.Length)])
                });
                dmlStatements[i] = stmt;
            }
            dataStore.ModifyData(dmlStatements);
        }

        //Sample dummy data
        private static readonly string[] industryValues = new string[]
           {
            "Agriculture, hunting and related services",
            "Activities auxiliary to financial intermediation",
            "Activities auxiliary to financial intermediation, except insurance and pension funding",
            "Activities auxiliary to insurance and pension funding",
            "Activities of business, employers and professional organisations",
            "Activities of membership organisations n.e.c.",
            "Activities of other membership organisations",
            "Activities of trade unions",
            "Advertising",
            "Agricultural and animal husbandry services, except veterinary activities",
            "Air transport",
            "Ambiguous",
            "Architectural, engineering and other technical activities",
            "Building and repairing of ships and boats",
            "Building completion",
            "Building installation",
            "Building of complete constructions or parts thereof; civil engineering",
            "Business activities n.e.c.",
            "Casting of metals",
            "Central government activities",
            "Collection, purification and distribution of water",
            "Computer and related activities",
            "Construction",
            "Data base activities",
            "Data processing",
            "Dressing and dyeing of fur; manufacture of articles of fur",
            "Education",
            "Electricity, gas, steam and hot water supply",
            "Exterritorial organisations",
            "Extraction of crude petroleum and natural gas; service activities incidental to oil and gas extraction, excluding surveying",
            "Farming of animals",
            "Financial intermediation, except insurance and pension funding",
            "Fish hatcheries and fish farms",
            "Fishing, operation of fish hatcheries and fish farms",
            "Forestry and related services",
            "Forestry, logging and related services",
            "Growing of crops combined with farming of animals (mixed farming)",
            "Growing of crops; market gardening; horticulture",
            "Hardware consultancy",
            "Health and social work",
            "Hotels and restaurants",
            "Hotels, camping sites and other provision of short-stay accommodation",
            "Human health activities",
            "Hunting, trapping and game propagation, including related services",
            "Inland water transport",
            "Insurance and pension funding, except compulsory social security",
            "Insurance and pension funding, except compulsory social security",
            "Land transport; transport via pipelines",
            "Legal, accounting, bookkeeping and auditing activities; tax consultancy; market research and public-opinion research; business and management consultancy",
            "Library, archives, museums and other cultural activities",
            "Local authority activities",
            "Logging and related services",
            "Maintenance and repair of motor vehicles",
            "Maintenance and repair of office, accounting and computing machinery",
            "Manufacture of accumulators, primary cells and primary batteries",
            "Manufacture of aircraft and space-craft",
            "Manufacture of basic chemicals",
            "Manufacture of basic iron and steel",
            "Manufacture of basic metals, fabricated metal products, machinery and equipment and of office, accounting and computing machinery",
            "Manufacture of basic precious and non-ferrous metals",
            "Manufacture of beverages",
            "Manufacture of bodies (coachwork) for motor vehicles; manufacture of trailers and semi-trailers",
            "Manufacture of coke oven products",
            "Manufacture of coke, refined petroleum products and nuclear fuel; manufacture of chemicals and chemical products; manufacture of rubber and plastic products",
            "Manufacture of dairy products",
            "Manufacture of electric lamps and lighting equipment",
            "Manufacture of electric motors, generators and transformers",
            "Manufacture of electrical machinery and apparatus n.e.c.",
            "Manufacture of electricity distribution and control apparatus",
            "Manufacture of electronic valves and tubes and other electronic components",
            "Manufacture of food products, beverages and tobacco products",
            "Manufacture of footwear",
            "Manufacture of furniture",
            "Manufacture of furniture; manufacturing n.e.c.; recycling",
            "Manufacture of gas; distribution of gaseous fuels through mains",
            "Manufacture of general purpose machinery",
            "Manufacture of glass and glass products",
            "Manufacture of grain mill products, starches and starch products and prepared animal feeds",
            "Manufacture of household appliances n.e.c.",
            "Manufacture of insulated wire and cable",
            "Manufacture of knitted and crocheted fabrics and articles",
            "Manufacture of man-made fibres",
            "Manufacture of medical appliances and instruments and appliances for measuring, checking, testing, navigating and for other purposes, except optical instruments",
            "Manufacture of motor vehicles",
            "Manufacture of non-metallic mineral products n.e.c",
            "Manufacture of office, accounting and computing machinery",
            "Manufacture of optical instruments and photographic equipment",
            "Manufacture of other chemical products",
            "Manufacture of other electrical equipment n.e.c.",
            "Manufacture of other fabricated metal products; metalwork service activities",
            "Manufacture of other food products",
            "Manufacture of other non-metallic mineral products",
            "Manufacture of other textiles",
            "Manufacture of paper and paper products",
            "Manufacture of parts and accessories for motor vehicles and their engines",
            "Manufacture of plastic products",
            "Manufacture of products of wood, cork, straw and plaiting materials",
            "Manufacture of radio, television and communication equipment and apparatus and of medical, precision and optical instruments, watches and clocks",
            "Manufacture of railway and tramway locomotives and rolling stock",
            "Manufacture of rubber products",
            "Manufacture of special purpose machinery",
            "Manufacture of structural metal products, tanks, reservoirs and steam generators",
            "Manufacture of television and radio receivers, sound or video recording or reproducing apparatus and associated goods",
            "Manufacture of television and radio transmitters and apparatus for line telephony and line telegraphy",
            "Manufacture of textiles, clothing and leather goods",
            "Manufacture of tobacco products",
            "Manufacture of transport equipment",
            "Manufacture of transport equipment n.e.c.",
            "Manufacture of watches and clocks",
            "Manufacture of wearing apparel, except fur apparel",
            "Manufacture of wood and of products of wood and cork, except furniture; manufacture of articles of straw and plaiting materials; manufacture of paper and paper products;",  "publishing, printing and reproduction of recorded media",
            "Manufacturing n.e.c.",
            "Mining and quarrying n.e.c.",
            "Mining of coal and lignite",
            "Mining of diamonds (including alluvial diamonds)",
            "Mining of gold and uranium ore",
            "Mining of iron ore",
            "Mining of metal ores, except gold and uranium",
            "Mining of non-ferrous metal ores, except gold and uranium",
            "Monetary intermediation",
            "Motion picture, radio, television and other entertainment activities",
            "News agency activities",
            "Non-specialised retail trade in stores",
            "Ocean and coastal fishing",
            "Other activities not adequately defined",
            "Other business activities",
            "Other community, social and personal service activities",
            "Other computer related activities",
            "Other financial intermediation n.e.c.",
            "Other land transport",
            "Other mining and quarrying",
            "Other retail trade in new goods in specialised stores",
            "Other service activities",
            "Other wholesale trade",
            "Petroleum refineries / synthesisers",
            "Post and telecommunication",
            "Postal and related courier activities",
            "Printing and service activities related to printing",
            "Private households with employed persons",
            "Processing of nuclear fuel",
            "Production of organic fertilizer",
            "Production, collection and distribution of electricity",
            "Production, processing and preservation of meat, fish, fruit, vegetables, oils and fats",
            "Public administration and defence activities",
            "Publishing",
            "Railway transport",
            "Real estate activities",
            "Real estate activities on a fee or contract basis",
            "Real estate activities with own or leased property",
            "Recreational, cultural and sporting activities",
            "Recycling n.e.c.",
            "Regional services council activities",
            "Renting of construction of demolition equipment with operators",
            "Renting of machinery and equipment, without operator and of personal and household goods",
            "Renting of other machinery and equipment",
            "Renting of personal and household goods n.e.c.",
            "Renting of transport equipment",
            "Repair of personal and household goods",
            "Representatives of foreign governments",
            "Reproduction of recorded media",
            "Research and development",
            "Research and experimental development on natural sciences and engineering",
            "Research and experimental development on social sciences and humanities",
            "Restaurants, bars and canteens",
            "Retail sale of automotive fuel",
            "Retail trade in food, beverages and tobacco in specialised stores",
            "Retail trade in second-hand goods in stores",
            "Retail trade not in stores",
            "Retail trade, except of motor vehicles and motor cycles; repair of personal and household goods",
            "Sale of motor vehicle parts and accessories",
            "Sale of motor vehicles",
            "Sale, maintenance and repair of motor cycles and related parts and accessories",
            "Sale, maintenance and repair of motor vehicles and motor cycles; retail trade in automotive fuel",
            "Sawmilling and planing of wood",
            "Sea and coastal water transport",
            "Service activities incidental to mining of minerals",
            "Sewage and refuse disposal, sanitation and similar activities",
            "Site preparation",
            "Social work activities",
            "Software consultancy and supply",
            "Spinning, weaving and finishing of textiles",
            "Sporting and other recreational activities",
            "Steam and hot water supply",
            "Stone quarrying, clay and sand-pits",
            "Supporting and auxiliary transport activities; activities of travel agencies",
            "Tanning and dressing of leather; manufacture of luggage, handbag, saddlery and harness",
            "Telecommunication",
            "Transport via pipelines",
            "Undetermined",
            "Veterinary activities",
            "Water transport",
            "Wholesale and commission trade, except of motor vehicles and motor cycles",
            "Wholesale trade in agricultural raw materials, livestock, food, beverages and tobacco",
            "Wholesale trade in household goods",
            "Wholesale trade in machinery, equipment and supplies",
            "Wholesale trade in non-agricultural intermediate products, waste and scrap",
            "Wholesale trade on a fee or contract basis"
        };
        private static readonly string[] firstNames = new string[] {
            "Peter", "Ryan", "Richard", "Tom", "Mark", "Steve",
            "Jimmy", "Jeffrey", "Andrew", "Dave", "Bert", "Mike",
            "Ray", "Paul", "Brad", "Carl", "Jerry" };
        private static readonly string[] lastNames = new string[] {
            "Dolan", "Fischer", "Hamlett", "Hamilton", "Lee",
            "Lewis", "McClain", "Miller", "Murrel", "Parkins",
            "Roller", "Shipman", "Bailey", "Barnes", "Lucas", "Campbell" };
        private static readonly string[] productNames = new string[] {
            "Chai", "Chang", "Aniseed Syrup", "Chef Anton's Cajun Seasoning",
            "Chef Anton's Gumbo Mix", "Grandma's Boysenberry Spread",
            "Uncle Bob's Organic Dried Pears", "Northwoods Cranberry Sauce",
            "Mishi Kobe Niku", "Ikura", "Queso Cabrales", "Queso Manchego La Pastora",
            "Konbu", "Tofu", "Genen Shouyu", "Pavlova", "Alice Mutton",
            "Carnarvon Tigers", "Teatime Chocolate Biscuits",
            "Sir Rodney's Marmalade", "Sir Rodney's Scones",
            "Gustaf's Knäckebröd", "Tunnbröd", "Guaraná Fantástica",
            "NuNuCa Nuß-Nougat-Creme", "Gumbär Gummibärchen",
            "Schoggi Schokolade", "Rössle Sauerkraut", "Thüringer Rostbratwurst",
            "Nord-Ost Matjeshering", "Gorgonzola Telino", "Mascarpone Fabioli",
            "Geitost", "Sasquatch Ale", "Steeleye Stout", "Inlagd Sill",
            "Gravad lax", "Côte de Blaye", "Chartreuse verte",
            "Boston Crab Meat", "Jack's New England Clam Chowder",
            "Singaporean Hokkien Fried Mee", "Ipoh Coffee",
            "Gula Malacca", "Rogede sild", "Spegesild", "Zaanse koeken",
            "Chocolade", "Maxilaku", "Valkoinen suklaa", "Manjimup Dried Apples",
            "Filo Mix", "Perth Pasties", "Tourtière", "Pâté chinois",
            "Gnocchi di nonna Alice", "Ravioli Angelo", "Escargots de Bourgogne",
            "Raclette Courdavault", "Camembert Pierrot", "Sirop d'érable",
            "Tarte au sucre", "Vegie-spread", "Wimmers gute Semmelknödel",
            "Louisiana Fiery Hot Pepper Sauce", "Louisiana Hot Spiced Okra",
            "Laughing Lumberjack Lager", "Scottish Longbreads",
            "Gudbrandsdalsost", "Outback Lager", "Flotemysost",
            "Mozzarella di Giovanni", "Röd Kaviar", "Longlife Tofu",
            "Rhönbräu Klosterbier", "Lakkalikööri", "Original Frankfurter grüne Soße" };
        private static readonly Random Random = new Random(0);

    }
}
