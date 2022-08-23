using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Xml;
using System.Net;

namespace RWS2._2._01.RWSLoanFinder.RWSLFConsumerData
{
	/// <summary>
	/// Summary description for RWSLFConsumer.
	/// </summary>
	public class RWSLFConsumer : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.InputStream!=null)
			{
				Request.ContentType = "text/xml";
				//******************Method 1********************//
				//				string returnLine="",returnMessage="";
				//
				//				using(StreamReader returnStream = new StreamReader(Request.InputStream))
				//				{
				//					returnLine = "";
				//					while ((returnLine = returnStream.ReadLine()) != null) 
				//					{
				//						returnMessage += returnLine;
				//					}
				//				}  
				//				Response.Write(returnMessage);
				//***********************************************//

				//******************Method 2*********************//
				XmlTextReader reader = new XmlTextReader(Request.InputStream);
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(reader);
				//				for(int i=0;i<Request.Headers.Count;i++)
				//					Response.Write("<!--" + Request.Headers.GetKey(i).ToString() + "-->");
				Response.Write(xmlDoc.OuterXml);
				xmlDoc.Save(HttpContext.Current.Server.MapPath("/RWSLoanFinder/XML/Consumer.xml"));
				//***********************************************//

				//*******************Method 3********************//
				//				ASCIIEncoding encoding=new ASCIIEncoding();
				//				string xmlString = encoding.GetString(Request.BinaryRead(Request.TotalBytes));
				//				Response.Write(xmlString);
				//***********************************************//
			}
			else
			{
				Response.Write("No Input Stream");
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
