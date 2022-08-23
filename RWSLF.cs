using System;
using System.Xml;
using System.Web;

namespace RWS2._2._01.RWSLoanFinder
{
	/// <summary>
	/// Summary description for RWSLF.
	/// </summary>
	public class RWSLF
	{
		public RWSLF()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void RWSLFXML()
		{
			HttpContext _context = HttpContext.Current;
			WriteFLXml writeFLXml = new WriteFLXml("", _context.Session.SessionID, 0);
			HttpPost post = new HttpPost();
			try
			{
				//returnString = post.Post(writeFLXml.WriteXml(), "http://10.2.6.18/RWSLFPostHandler.asp?pvlid=6882");//my test page
				RWSReturnXml returnXml = new RWSReturnXml();
				returnXml.ParseXml(post.Post(writeFLXml.WriteXml(), "http://uat3.loanfinder.republic.lpx.com/ReceiveXML.aspx", "Denver_Republic@republic.com", "Test1234"));
			}
			catch(Exception ex)
			{
				//if failure resulting from http post request or response xml
				//need to handle error logging internally and then throw app
				//error in order to display user friendly error page
				throw ex.GetBaseException();
			}
			//for testing purposes only - will save xml to file
//			XmlDocument xmlDoc = new XmlDocument();
//			xmlDoc.LoadXml(writeFLXml.WriteXml());;
//			xmlDoc.Save(HttpContext.Current.Server.MapPath("/RWSLoanFinder/XML/RWSLFTest.xml"));
		}
	}
}
