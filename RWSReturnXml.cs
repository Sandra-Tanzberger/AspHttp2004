using System;
using System.Web;
using System.Xml;
using System.IO;
using System.Text;

namespace RWS2._2._01.RWSLoanFinder
{
	/// <summary>
	/// Summary description for RWSReturnXml.
	/// </summary>
	public class RWSReturnXml
	{
		HttpContext context = HttpContext.Current;
		public RWSReturnXml()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void ParseXml(string XmlString)
		{
			//for testing
//			int XmlStart = XmlString.IndexOf("<IFX>");
//			int XmlEnd = XmlString.LastIndexOf("</IFX>")+6;
//			string xmlString = ""; //XmlString.Substring(XmlStart, XmlEnd-XmlStart);

			ASCIIEncoding encoding=new ASCIIEncoding();
			byte[]  byte1 = encoding.GetBytes(XmlString);

			MemoryStream xmlStream = new MemoryStream(byte1, 0, byte1.Length);

//			//for testing
//			ASCIIEncoding encoding2=new ASCIIEncoding();
//			xmlString = encoding2.GetString(xmlStream.ToArray());
//			context.Response.Write(xmlString);

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlStream);

			XmlNode errorNode = xmlDoc.GetElementsByTagName("MORTGAGEACK").Item(0);
			if(errorNode!=null)
			{
				ParseXmlError(errorNode);
			}
			else
			{
				XmlNode GUID = xmlDoc.GetElementsByTagName("TRNUID").Item(0);
				context.Response.Redirect("http://uat3.loanfinder.republic.lpx.com/default.aspx?ticketuid=" + GUID.InnerText);
			}
		}

		private void ParseXmlError(XmlNode xmlNode)
		{
		   string ErrorMessage = xmlNode.ChildNodes[0].ChildNodes[2].InnerText;
		   throw new Exception(ErrorMessage);
		}
	}
}
