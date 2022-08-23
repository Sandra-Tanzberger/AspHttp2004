using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Text;
using System.Web;
using System.Collections;

namespace RWS2._2._01.RWSLoanFinder
{
	/// <summary>
	/// Summary description for WriteFLXml.
	/// </summary>
	public class WriteFLXml
	{
		private string _ConnectionString = "";
		private string _SessionId = "";
		private int _ConsumerId = 0;
		XmlTextWriter xmlWriter = null;
		MemoryStream xmlStream = null;
		HttpContext _context = HttpContext.Current;

		//Template Header and Footer elements
		string Header = "";
		string Footer = "";

		public WriteFLXml(string ConnectionString, string SessionId, int ConsumerId)
		{
			_ConnectionString = ConnectionString;
			_SessionId = SessionId;
			_ConsumerId = ConsumerId;

			HeaderFooterIncludes includes = new HeaderFooterIncludes();
			includes.GetHeaderFooter(out Header, out Footer);
		}

		public string WriteXml()
		{
//			HttpContext.Current.Response.Write("Header: <br>" + Header + "<br><br>Footer: <br>" + Footer);
//			HttpContext.Current.Response.End();
			string xmlString = "";
			xmlStream = new MemoryStream();

			xmlWriter = new XmlTextWriter(xmlStream, Encoding.UTF8);
			xmlWriter.Formatting = Formatting.Indented;			
			xmlWriter.WriteStartDocument(false);
				xmlWriter.WriteStartElement("IFX");
//					xmlWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
//					xmlWriter.WriteAttributeString("xsi:noNamespaceSchemaLocation", _context.Server.MapPath("/RWSLoanFinder/XSD/RWS_LFDataPass.xsd"));
				xmlWriter.WriteAttributeString("xmlns", "lpx.com/transactionWrapper");
				xmlWriter.WriteAttributeString("xmlns:xs", "http://www.w3.org/2001/XMLSchema");

//				//Elements of IFX: SONRQ, MORTGAGESVCRQ
				WriteSONRQ();
				WriteMORTGAGESVCRQ();
//				//END Elements of IFX

				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();

			xmlStream.Position = 3;
				
			XmlSchemaCollection schemaCol = new XmlSchemaCollection();
			schemaCol.Add("lpx.com/leads/4.0", _context.Server.MapPath("/RWSLoanFinder/XSD/LFLead.xsd"));
			schemaCol.Add("lpx.com/transactionWrapper", _context.Server.MapPath("/RWSLoanFinder/XSD/TransactionWrapper.xsd"));

			XMLValidation xmlValidate = new XMLValidation();
			xmlValidate.Validate(xmlStream, schemaCol);
//			_context.Response.Write(xmlValidate.ValidationMessage);
//			_context.Response.End();
			if(!xmlValidate.xmlValid)
			{
				throw new Exception(xmlValidate.ValidationMessage);
			}

			ASCIIEncoding encoding=new ASCIIEncoding();
			xmlString = encoding.GetString(xmlStream.ToArray());						
									
			xmlWriter.Close(); //this will close the textwriter object and the backing stream object
			//the stream cannot be accessed after this point

			return xmlString.Replace("o;?", "");
		}

		//////////////////////////////////////////////////////////////////////
		/// <summary>
		/// START WRITE SONRQ ELEMENTS/AGGREGATES
		/// </summary>
		private void WriteSONRQ()
		{
			//Elements of SONRQ: SONTYPE, DTCLIENT, CUSTLANGPREF, CLIENTAPP
			xmlWriter.WriteStartElement("SONRQ");
				xmlWriter.WriteElementString("SONTYPE", "Import_LFLead");
			xmlWriter.WriteElementString("DTCLIENT", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffzzzz"));
				WriteCUSTLANGPREF();
				WriteCLIENTAPP();
			xmlWriter.WriteEndElement();
		}
		private void WriteCUSTLANGPREF()
		{
			xmlWriter.WriteStartElement("CUSTLANGPREF");
				xmlWriter.WriteElementString("LANGUAGE", "ENU");
			xmlWriter.WriteEndElement();
		}
		private void WriteCLIENTAPP()
		{
			//Elements of CLIENTAPP: ORG, NAME, APPVER
			xmlWriter.WriteStartElement("CLIENTAPP");
				xmlWriter.WriteElementString("ORG", "Lion, Inc");
				xmlWriter.WriteElementString("NAME", "RWS");
				xmlWriter.WriteElementString("APPVER", "2.2.01");
			xmlWriter.WriteEndElement();
		}
		/// <summary>
		/// END WRITE SONRQ
		/// </summary>
		////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// BEGIN WRITE MORTGAGESVCRQ ELEMENTS/AGGREGATES
		/// </summary>
		///
		private void WriteMORTGAGESVCRQ()
		{
			xmlWriter.WriteStartElement("LPXRQ");
				xmlWriter.WriteStartElement("LFLEAD");
					xmlWriter.WriteAttributeString("xmlns", "lpx.com/leads/4.0");
					xmlWriter.WriteAttributeString("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
					WriteBROKERDATA();
					WriteCONSUMERDATA();
					WritePROPERTYDATA();
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}
		private void WriteBROKERDATA()
		{
			xmlWriter.WriteStartElement("BROKERDATA");
				xmlWriter.WriteElementString("TRNUID", _context.Session.SessionID.ToString());
				xmlWriter.WriteElementString("COMPANYNAME", _context.Session["b_COMPANYNAME"].ToString());
				xmlWriter.WriteElementString("MIDDLENAME", _context.Session["b_MIDDLENAME"].ToString());
				xmlWriter.WriteElementString("FIRSTNAME", _context.Session["b_FIRSTNAME"].ToString());
				xmlWriter.WriteElementString("LASTNAME", _context.Session["b_LASTNAME"].ToString());
				xmlWriter.WriteElementString("EMAIL", _context.Session["b_EMAIL"].ToString());
				xmlWriter.WriteElementString("PHONE1", _context.Session["b_PHONE1"].ToString());
				xmlWriter.WriteElementString("EXT1", _context.Session["b_EXT1"].ToString());
				xmlWriter.WriteElementString("PHONE2", _context.Session["b_PHONE2"].ToString());
				xmlWriter.WriteElementString("EXT2", _context.Session["b_EXT2"].ToString());
				xmlWriter.WriteElementString("PHONE3", _context.Session["b_PHONE3"].ToString());
				xmlWriter.WriteElementString("EXT3", _context.Session["b_EXT3"].ToString());
				xmlWriter.WriteElementString("FAX", _context.Session["b_FAX"].ToString());
				xmlWriter.WriteElementString("CONTENTMGMTID", _context.Session["b_P"].ToString());
				xmlWriter.WriteElementString("HOMEPAGEURL", _context.Session["b_URL"].ToString());
				xmlWriter.WriteStartElement("URLHEADER");
					xmlWriter.WriteString(Header);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("URLFOOTER");
					xmlWriter.WriteString(Footer);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteElementString("URLPRIVACYPOLICY", _context.Session["b_URLPRIVACYPOLICY"].ToString());
				xmlWriter.WriteElementString("URLDOC", "");
				xmlWriter.WriteElementString("URLLOANSTATUS", "");
				xmlWriter.WriteElementString("RETURNURL", "");
				xmlWriter.WriteElementString("IMAGESURL", "");
				xmlWriter.WriteElementString("PLVID", "7100");
				xmlWriter.WriteElementString("USERGUID", "F59314FF931B7802B0AD4A5FAA709A8A");
//				xmlWriter.WriteElementString("USERGUID", LOGUID.UserGUIDFromSellerID(101, "aaffleck", "dev.ldap.lpx.com"));
				xmlWriter.WriteElementString("SENDCONFIRMATIONEMAIL", "N");
			xmlWriter.WriteEndElement();
		}
		private void WriteCONSUMERDATA()
		{
			xmlWriter.WriteStartElement("CONSUMERDATA");
				xmlWriter.WriteElementString("SESSIONID", _SessionId);
				xmlWriter.WriteElementString("PURCHASETYPE", GetPurchaseType());
				xmlWriter.WriteElementString("FIRSTNAME", GetSession("FirstName"));
				xmlWriter.WriteElementString("LASTNAME", GetSession("LastName"));
				xmlWriter.WriteElementString("DAYPHONE", GetSession("WorkPhone"));
				xmlWriter.WriteElementString("EVENINGPHONE", GetSession("HomePhone"));
				xmlWriter.WriteElementString("EMAIL", GetSession("Email"));
				xmlWriter.WriteElementString("TOTALINCOME", GetSession("MontlyIncome", "double"));
				xmlWriter.WriteElementString("TOTALDEBT", GetSession("MonthlyDebt", "double"));
				xmlWriter.WriteElementString("DOWNPAYMENTAMOUNT", (Convert.ToDouble(GetSession("PropertyValue", "double")) - Convert.ToDouble(GetSession("Amount", "double"))).ToString());
				xmlWriter.WriteElementString("LOANAMOUNT", GetSession("Amount", "double"));
				xmlWriter.WriteElementString("OCCUPANCYTYPE", PropertyUse());
				xmlWriter.WriteElementString("MARITALSTATUS", "NotProvided");
				xmlWriter.WriteElementString("NUMMONTHSATJOB", "0");
			xmlWriter.WriteEndElement();
		}
		private void WritePROPERTYDATA()
		{     
			xmlWriter.WriteStartElement("PROPERTYDATA");
				xmlWriter.WriteElementString("PROPERTYTYPE", GetPropertyType());
				xmlWriter.WriteElementString("APPRAISEDVALUE", "0.00");
				xmlWriter.WriteElementString("CITY", GetSession("City"));
				xmlWriter.WriteElementString("STATE", StateCodeLookup(GetSession("State")));
				xmlWriter.WriteElementString("ZIPCODE", GetZipCode());
				xmlWriter.WriteElementString("RENTPAYMENT", "0.00");
				xmlWriter.WriteElementString("TAXES", GetSession("RETaxes", "double"));
				xmlWriter.WriteElementString("PAYMENTPI", "0.00");
				xmlWriter.WriteElementString("OTHERFINANCINGPI", "0.00");
			xmlWriter.WriteEndElement();
		}
		/// <summary>
		/// END WRITE MORTGAGESVCRQ
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		///

		private string GetSession(string Key, string Type)
		{
			if(_context.Session[Key]!=null)
				if(Type == "double")
				{
					return Math.Round(System.Convert.ToDouble(_context.Session[Key].ToString()),2).ToString();
				}
				else
				{
					return _context.Session[Key].ToString();
				}
			else
			{
				return GetStringType(Type);
			}
		}

		private string GetSession(string Key)
		{
			if(_context.Session[Key]!=null)
				return _context.Session[Key].ToString();
			else
			{
				return "";
			}
		}

		private string GetStringType(string Type)
		{
			switch(Type)
			{
				case "string":
					return "";
				case "integer":
					return "0";
				case "double":
					return "0.00";
				default:
					return "";
			}
		}

		private string GetPurchaseType()
		{
			string PurchaseType = "Purchase";
			if(_context.Session["MortgageType"]!=null)
			{
				switch(_context.Session["MortgageType"].ToString().ToLower())
				{
					case "purchase":
						PurchaseType = "Purchase";
						break;
					case "refinance":
						PurchaseType = "Refinance";
						break;
					default:
						PurchaseType = "Other";
						break;
				}
			}
			return PurchaseType;
		}

		private string PropertyUse()
		{
			string PropertyUse = "PrimaryResidence";
			if(_context.Session["PropertyUseId"]!=null)
			{
				switch(_context.Session["PropertyUseId"].ToString())
				{
					case "1":
						PropertyUse = "PrimaryResidence";
						break;
					case "2":
						PropertyUse = "Investment";
						break;
					case "3":
						PropertyUse = "SecondHome";
						break;
					default:
						PropertyUse = "PrimaryResidence";
						break;
				}
			}
			return PropertyUse;
		}

		private string GetPropertyType()
		{
			string PropertyType = "SingleFamily";
			if(_context.Session["PropertyTypeId"]!=null)
			{
				switch(_context.Session["PropertyTypeId"].ToString())
				{
					case "1":
						PropertyType = "SingleFamily";
						break;
					case "2":
						PropertyType = "Condominium";
						break;
					case "5":
						PropertyType = "ManufacturedMobileHome";
						break;
					case "20":
						PropertyType = "CommercialNonResidential";
						break;
					case "43":
						PropertyType = "Cooperative";
						break;
					case "44":
						PropertyType = "MultifamilyMoreThanFourUnits";
						break;
					default:
						PropertyType = "SingleFamily";
						break;
				}
			}
			return PropertyType;
		}

		private string StateCodeLookup(string stateName)
		{
			switch(stateName.Length)
			{
				case 0:
					return "";
				case 2:
					return stateName;
				default:
					try
					{
							HelperFunctions helperFunctions = new HelperFunctions();
						return helperFunctions.GetStateCodeByStateName(stateName);}
					catch{return "";} 
			}
		}

		private string GetZipCode()
		{
			if(_context.Session["ZipCode"]!=null)
				return _context.Session["ZipCode"].ToString();
			else if(_context.Session["b_ZipCode"]!=null)
				return _context.Session["b_ZipCode"].ToString();
			else
				return "00000";
		}
	}
}
