using System;
using System.Net;
using System.IO;
using System.Text;
using System.Web;

namespace RWS2._2._01.RWSLoanFinder
{
	/// <summary>
	/// Summary description for IMXLoanPost.
	/// </summary>
	public class HttpPost
	{
		public HttpPost()
		{
		}

		public string Post(string XmlDataString, string uri, string username, string password)
		{
			string returnMessage = "";

			//char[] XmlData = XmlDataString.ToCharArray();

			ASCIIEncoding encoding=new ASCIIEncoding();
			byte[]  byte1 = encoding.GetBytes(XmlDataString);

			//string bytetest = encoding.GetString(byte1, 0, byte1.Length); //test purposes only
			
			try
			{
				CredentialCache credentialCache = new CredentialCache();
				credentialCache.Add(new System.Uri(uri), "Basic", new NetworkCredential(username, password));
				HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
				webRequest.Method = "POST";
				//webRequest.ContentType = "application/x-www-form-urlencoded";
				webRequest.ContentType = "text/xml";
				webRequest.ContentLength = XmlDataString.Length;
				webRequest.Timeout = 20000;
				webRequest.Credentials = credentialCache;
//				webRequest.AllowAutoRedirect = true;
				Stream webStreamRequest = webRequest.GetRequestStream();
				webStreamRequest.Write(byte1,0,byte1.Length);
				webStreamRequest.Close();
				HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
//				returnMessage = "Status Code: " + webResponse.StatusCode;
//				returnMessage += "Status Description: " + webResponse.StatusDescription;
				Stream webResponseStream = webResponse.GetResponseStream();
				
				using(StreamReader returnStream = new StreamReader(webResponseStream))
				{
					string returnLine = "";
					while ((returnLine = returnStream.ReadLine()) != null) 
					{
						returnMessage += returnLine;
					}
				}

				webResponse.Close();
				webResponseStream.Close();				
			}
			catch(WebException webEx)
			{
				//returnMessage = webEx.Message;
				throw webEx;
			}
			finally
			{
				//for future use of stream reader or text reader objects
			}
			return returnMessage;
		}

	}
}
