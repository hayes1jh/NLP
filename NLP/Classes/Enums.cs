

namespace NLP
{
	public sealed class Config              // type-safe ENUM pattern
	{
		public readonly string Value;

		public Config(string value)
		{
			Value = value;
		}
		// Define values here.

		public static readonly Config DatabaseName = new Config("InboundEmail");
		public static readonly Config ServerName = new Config("localhost");

		public static readonly Config inboundpdf = new Config(@"C:\Users\jhayes\source\repos\PDFManipulator\input\tessy.pdf");
		public static readonly Config outboundpdfPath = new Config(@"C:\Users\jhayes\source\repos\PDFManipulator\output\");
		public static readonly Config tesseractPath = new Config(@"C:\Users\jhayes\source\repos\PDFManipulator\bin\Debug\runtimes\TesseractBinaries\4.0\x86\native");
		public static readonly Config tesseractDataPath = new Config(@"C:\Users\jhayes\source\repos\PDFManipulator\bin\Debug\runtimes\tessdata");
		public static readonly Config connectionString = new Config("Data Source=cp-cl-p05-lis01;Initial Catalog=SFAUDIT;Integrated Security=True;Connection Timeout=30;");
		public static readonly Config SelectStatement = new Config(@"select  i.Id, i.FromPerson, i.Subject, i.Body, '' Redacted from Inbox i order by i.Id");
		public static readonly Config SelectReplacementData = new Config(@"SELECT TOP (3000) ROW_NUMBER() OVER(ORDER BY Cid__c ASC) AS Row,[Email],[Phone],(Street + ' ' + city + ', ' + State + ' ' + postalcode) Address FROM[InboundEmail].[dbo].[FakeProspects]");
        public static readonly Config SelectStatement2 = new Config(@"
SELECT top 5 thedata.cid, thedata.value, thedata.X, thedata.Y, thedata.PAGE FROM ( 
	SELECT g.cid,cid AS value                , X=0,    y=0    , PAGE=0 FROM Generated g UNION
	SELECT g.cid,prefix                      , X=68,   Y=172  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,fullname                    , X=160,  Y=172  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,preferredname               , X=423,  Y=172  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,dob                         , X=106,  Y=192  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,SSN                         , X=423,  Y=192  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,country countrycit_country  , X=147,  Y=208  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,country countryres_country  , X=413,  Y=208  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,employer                    , X=361,  Y=225  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,homephone                   , X=117,  Y=268  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,cellphone                   , X=283,  Y=268  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,bussineassphone             , X=470,  Y=268  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,email                       , X=370,  Y=286  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,spouseprefix                , X= 68,  Y=336  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,spousefull                  , X=160,  Y=336  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,spousedob                   , X=106,  Y=355  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,SpouseSSN                   , X=423,  Y=355  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,country countryspc_country  , X=147,  Y=372  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,country countryspr_country  , X=413,  Y=372  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,spousehome                  , X=117,  Y=430  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,spousecell                  , X=283,  Y=430  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,street                      , X= 76,  Y=508  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,city                        , X=290,  Y=508  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,abbv                        , X=435,  Y=508  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,postal                      , X=500,  Y=508  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,individualvalue             , X=190,  Y=615  , PAGE=0 FROM Generated g UNION
	SELECT g.cid,jointvalue                  , X=190,  Y=635  , PAGE=1 FROM Generated g UNION
	SELECT g.cid,cash			             , X=380,  Y=118  , PAGE=1 FROM Generated g UNION
	SELECT g.cid,aftertax		             , X=380,  Y=135  , PAGE=1 FROM Generated g UNION
	SELECT g.cid,ira			             , X=380,  Y=150  , PAGE=1 FROM Generated g UNION
	SELECT g.cid,totalliquid                 , X=380,  Y=225  , PAGE=1 FROM Generated g UNION
	SELECT g.cid,residence		             , X=380,  Y=365  , PAGE=1 FROM Generated g UNION
	SELECT g.cid,totalnetworth               , X=380,  Y=513  , PAGE=1 FROM Generated g 
) AS thedata
WHERE thedata.value <> '' and thedata.cid not IN ('1014140','1022799')
order BY cast(thedata.cid AS int), thedata.PAGE, thedata.X");

		public static readonly Config SelectCIDs = new Config(@"
			SELECT DISTINCT cid as cids FROM Generated g
			WHERE cid not IN ('1014140','1022799')
			");



    }
}
