using System;
using System.Data;

#region USER GUID CODE SECTION 1

using System.DirectoryServices;
using Oracle.DataAccess.Client;

#endregion

namespace RWS2._2._01.RWSLoanFinder
{
	/// <summary>
	/// Summary description for LOGUID.
	/// </summary>
	public class LOGUID
	{
		public LOGUID(/*string _LDAPUsername, string _LDAPPassword, string _LDAPEnvironment*/)
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region USER GUID SAMPLE USE
		//string guid = UserGUIDFromSellerID(101, "aaffleck", "dev.ldap.lpx.com");
		#endregion

		public static string _LDAPUsername = "cn=denver,ou=members,o=tuttle.com";		// the distinguished name of a user who has access to the directory database
		public static string _LDAPPassword = "cSh4rpR0cks";								// the password for the user
		public static string _LDAPEnvironment = "test2";	

		public static string UserGUIDFromSellerID(int lpxLenderID, string PLVID, string LDAPServer)
		{
			//	AUTHOR
			//		Eric Peterson (epeterson@lionimts.com)
			//
			//	LAST DATE MODIFIED
			//		08/09/2004
			//
			//	DESCRIPTION
			//		"MAIN" function (calls all the functions that actually do the work)
			//
			//	INPUT
			//		int lpxLenderID -- lpx primary key.  republic = 101, LION = 10001
			//		string PLVID -- this is the PLVID associated with a broker (on the Denver side)
			//		string LDAPServer -- this is the URL to the Directory database (i.e. - "dev.ldap.lpx.com" for dev/test)
			//		
			//	OUTPUT
			//		string -- will output the guid of the user associated with the PLVID
			//			(in the format "{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}")
			//

			string path = "LDAP://" + LDAPServer + "/o=Tuttle.com/ou=Members/ou=hti/ou=UserApps";
			int lpxSellerID = PLVtoSellerID(lpxLenderID, PLVID, LDAPServer);
			string crit = "(&(appName=LPXWEB)(&(lpxSellerID=" + Convert.ToString(lpxSellerID) + ")(lpxLenderID=" + lpxLenderID + ")))";

			DirectoryEntry userApp;

			userApp = Search(path, crit, SearchScope.Subtree);

			if(userApp != null && userApp.Properties.Contains("GUID"))
				return Convert.ToString(userApp.Properties["GUID"][0]);
			else
				return string.Empty;
		}

		private static int PLVtoSellerID(int lenderID, string PLVID, string server)
		{
			//	AUTHOR
			//		Eric Peterson (epeterson@lionimts.com)
			//
			//	LAST DATE MODIFIED
			//		08/09/2004
			//
			//	DESCRIPTION
			//		this function accesses oracle and retrieves the internal value corresponding to the PLVID
			//
			//	INPUT
			//		int lenderID -- lpx primary key.  republic = 101, LION = 10001
			//		string PLVID -- this is the PLVID associated with a broker (on the Denver side)
			//		string server -- this is the URL to the Directory database (i.e. - "dev.ldap.lpx.com" for dev/test)
			//		
			//	OUTPUT
			//		int -- will output a number > 0 if successful
			//
			//	GLOBALS
			//		_LDAPEnvironment -- the environment that we are working in (i.e. - test2, uat3, stage, live)
			//

			string LDAPBase = "LDAP://" + server + "/o=Tuttle.com/";
			string myPath;
			int rtnValue = -2;
			OracleConnection oConn;
			OracleCommand oCmd;

			myPath = LDAPBase + "ou=WebServers/cn=" + _LDAPEnvironment;
			DirectoryEntry lenderWebSite = Search(myPath, "(lpxLenderID=" + Convert.ToString(lenderID) + ")", SearchScope.OneLevel);
			if(lenderWebSite != null && lenderWebSite.Properties.Contains("cn"))
			{
				myPath += "/cn=" + Convert.ToString(lenderWebSite.Properties["cn"][0]) + "/cn=live";
				DirectoryEntry dbName = Bind(myPath);
				if(dbName.Properties.Contains("lpxDatabaseLink"))
				{
					myPath = LDAPBase + "ou=Databases/cn=" + Convert.ToString(dbName.Properties["lpxDatabaseLink"][0]);
					DirectoryEntry dbConnection = Bind(myPath);

					if(dbConnection != null)
					{
						string dbService = Convert.ToString(dbConnection.Properties["lpxDBServiceName"][0]);
						string dbUsername = Convert.ToString(dbConnection.Properties["lpxDBUsername"][0]);
						string dbPassword = Convert.ToString(dbConnection.Properties["userPassword"][0]);
						string dbConnString = "data source=" + dbService + ";user id=" + dbUsername + ";Password=" + dbPassword + ";";
						System.Text.StringBuilder dbSQL = new System.Text.StringBuilder();

						dbSQL.Append("SELECT SELLERID FROM TBLLENDERSELLEREXTERNALNUM WHERE LENDERID = ");
						dbSQL.Append(Convert.ToString(lenderID));
						dbSQL.Append(" AND EXTERNALID = 1 AND LENDERSELLERNUMBER = '");
						dbSQL.Append(PLVID);
						dbSQL.Append("'");

						oConn = new OracleConnection(dbConnString);
						oConn.Open();
					
						try
						{
							oCmd = new OracleCommand(dbSQL.ToString(), oConn);
							oCmd.CommandType = CommandType.Text;

							rtnValue = Convert.ToInt32(oCmd.ExecuteScalar().ToString());
						}
						catch(Exception e)
						{
							throw new ApplicationException(e.Message);
						}
						finally
						{
							oConn.Close();
						}
					}
				}
			}
			else
			{
				throw new ApplicationException("Lender settings not found for LenderID: " + Convert.ToString(lenderID));
			}
			return rtnValue;
		}

		private static DirectoryEntry Bind(string path)
		{
			//	AUTHOR
			//		Eric Peterson (epeterson@lionimts.com)
			//
			//	LAST DATE MODIFIED
			//		08/09/2004
			//
			//	DESCRIPTION
			//		binds to a directory object specified by path using global variables that specify
			//		username and password to connect with
			//
			//	INPUT
			//		string path -- this is the path that describes where the object is located in the directory
			//		
			//	OUTPUT
			//		DirectoryEntry -- an object containing all the values/properties of the object specified by path
			//
			//	GLOBALS
			//		_LDAPUsername -- the distinguished name of a user who has access to the directory database
			//		_LDAPPassword -- the password for the user
			//

			DirectoryEntry _de = new DirectoryEntry(path, _LDAPUsername, _LDAPPassword, AuthenticationTypes.None);

			return _de;
		}

		private static DirectoryEntry Search(string path, string criteria, SearchScope scope)
		{
			//	AUTHOR
			//		Eric Peterson (epeterson@lionimts.com)
			//
			//	LAST DATE MODIFIED
			//		08/09/2004
			//
			//	DESCRIPTION
			//		searches for and binds to a directory object meeting criteria specified
			//		begins searching in location specified by path using global variables that specify
			//		username and password to connect with
			//
			//	INPUT
			//		string path -- this is the path that describes where in the directory to begin searching
			//		string criteria -- specifies the criteria to search with (e.g. - "(&(className=userApp)(lpxLenderID=101))")
			//		SearchScope scope -- specifies whether to search the base object, one level from the base, or the entire
			//			subtree under location specified by path
			//		
			//	OUTPUT
			//		DirectoryEntry -- an object containing all the values/properties of the object found (if any)
			//
			//	GLOBALS
			//		_LDAPUsername -- the distinguished name of a user who has access to the directory database
			//		_LDAPPassword -- the password for the user
			//

			DirectoryEntry directoryBase;
			DirectorySearcher searcher;
			SearchResult result;

			try
			{
				directoryBase = new DirectoryEntry(path, _LDAPUsername, _LDAPPassword, AuthenticationTypes.None);
				searcher = new DirectorySearcher(directoryBase, criteria);
				searcher.SearchScope = scope;
				result = searcher.FindOne();

				if(result == null)
				{
					return null;
				}
				else
				{
					string pathToObject = result.Properties["adspath"][0].ToString();
					return new DirectoryEntry(pathToObject, _LDAPUsername, _LDAPPassword, AuthenticationTypes.None);
				}
			}
			catch(Exception e)
			{
				throw new ApplicationException("Search Error!  " + e.Message);
			}
		}
	}
}
