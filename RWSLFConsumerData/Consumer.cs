using System;
using System.Web;

namespace RWS2._2._01.RWSLoanFinder.RWSLFConsumerData
{
	/// <summary>
	/// Summary description for ConsumerData.
	/// </summary>
	public class Consumer
	{
		public Consumer()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void PostConsumerData()
		{
			HttpContext _context = HttpContext.Current;
			ConsumerXml consumerXml = new ConsumerXml();
			HttpPost post = new HttpPost();
			try
			{
				//consumerXml.ParseXml(post.Post(consumerXml.WriteConsumerXml(), "http://10.2.6.18/RWSLoanFinder/RWSLFConsumerData/RWSLFConsumer.aspx",));
				consumerXml.ParseXml(post.Post(consumerXml.WriteConsumerXml(), "http://test2.loan.lpx.com/ReceiveXML.aspx", "Denver@republic.com", "Test1234"));
			}
			catch(Exception ex)
			{
				throw ex;
			}
			//for testing purposes only - will save xml to file
//			XmlDocument xmlDoc = new XmlDocument();
//			xmlDoc.LoadXml(writeFLXml.WriteXml());;
//			xmlDoc.Save(HttpContext.Current.Server.MapPath("/RWSLoanFinder/XML/ConsumerData.xml"));
		}
	}
}
