using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using System.Text;
using System.Web;
using System.Collections;

namespace RWS2._2._01.RWSLoanFinder.RWSLFConsumerData
{
	/// <summary>
	/// Summary description for ConsumerXml.
	/// </summary>
	public class ConsumerXml
	{
		public ConsumerXml()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		XmlTextWriter xmlWriter = null;
		MemoryStream xmlStream = null;
		HttpContext context = HttpContext.Current;

		public string WriteConsumerXml()
		{
			string xmlString = "";
			xmlStream = new MemoryStream();

			xmlWriter = new XmlTextWriter(xmlStream, Encoding.UTF8);
			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.WriteStartDocument(false);
				xmlWriter.WriteStartElement("IFX");
					xmlWriter.WriteAttributeString("xmlns", "la:rwslfdatapass-schema");
					xmlWriter.WriteAttributeString("xmlns:xs", "http://www.w3.org/2001/XMLSchema");
	
					//Elements of IFX: SONRQ, LPXRQ
					WriteSONRQ();
					WriteLPXRQ();
					//END Elements of IFX

				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument(); 
			xmlWriter.Flush();

			xmlStream.Position = 3;

			XMLValidation xmlValidate = new XMLValidation();
			xmlValidate.Validate(xmlStream, "la:rwslfdatapass-schema", context.Server.MapPath("/RWSLoanFinder/XSD/RWS_LeadsImport.xsd"));
			context.Response.Write(xmlValidate.ValidationMessage);
			context.Response.End();

			ASCIIEncoding encoding=new ASCIIEncoding();
			xmlString = encoding.GetString(xmlStream.ToArray());

			xmlWriter.Close();

			return xmlString.Replace("o;?", "");
		}

		private void WriteSONRQ()
		{
			//Elements of SONRQ: SONTYPE, DTCLIENT, CUSTLANGPREF, CLIENTAPP
			xmlWriter.WriteStartElement("SONRQ");
			xmlWriter.WriteElementString("SONTYPE", "TRANSPCERT");
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
			xmlWriter.WriteElementString("APPVER", "2.2.02");
			xmlWriter.WriteEndElement();
		}

		private void WriteLPXRQ()
		{
			xmlWriter.WriteStartElement("LPXRQ");
				WriteBroker();
				WriteLoan();
			xmlWriter.WriteEndElement();
		}

		private void WriteLoan()
		{
			xmlWriter.WriteStartElement("LOAN");
				xmlWriter.WriteElementString("APPLICATIONTYPEID", GetApplicationType());
				xmlWriter.WriteElementString("PROGRAMGROUPID", GetProgramType());
				//xmlWriter.WriteElementString("LOANTYPEID", GetLoanType());
				//xmlWriter.WriteElementString("OCCUPANCYTYPEID", PropertyUse());
				xmlWriter.WriteElementString("LOANAMOUNT", GetSession("Amount", "double"));
				xmlWriter.WriteElementString("LTV", (Convert.ToDouble(GetSession("LTVRatio", "double"))/100).ToString());
				xmlWriter.WriteElementString("INTERESTRATE", (Convert.ToDouble(GetSession("Rate", "double"))/100).ToString());
				//xmlWriter.WriteElementString("SELLERPRICE", GetSession("PropertyValue", "double"));
				//write borrowers
				xmlWriter.WriteStartElement("BORROWERS");
					WriteBorrower();
					WriteCoBorrower();
				xmlWriter.WriteEndElement();
				//write subject property
				WriteSubjectProperty();
			xmlWriter.WriteEndElement();
		}

		private void WriteBorrower()
		{
			xmlWriter.WriteStartElement("BORROWER");
				xmlWriter.WriteElementString("BORROWERTYPEID", "1");
				xmlWriter.WriteElementString("PRIMARYBORROWER", "1");
				xmlWriter.WriteElementString("FIRSTNAME", GetSession("FirstName"));
				xmlWriter.WriteElementString("LASTNAME", GetSession("LastName"));
				xmlWriter.WriteElementString("SSN", "0");
				xmlWriter.WriteElementString("SELFEMPLOYED", GetSelfEmployed());
				xmlWriter.WriteElementString("MONTHSATEMPLOYER", "0");
				xmlWriter.WriteElementString("CREDITSCORE", "0");
				xmlWriter.WriteStartElement("MAILINGADDRESS");
					xmlWriter.WriteElementString("ADDRESS1", GetSession("Address"));
					xmlWriter.WriteElementString("ADDRESS2", "");
					xmlWriter.WriteElementString("CITY", GetSession("City"));
					xmlWriter.WriteElementString("STATE", StateCodeLookup(GetSession("State")));
					xmlWriter.WriteElementString("ZIPCODE", GetSession("ZipCode"));
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("CONTACTS");
					GetContacts();
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

		private void WriteCoBorrower()
		{
			xmlWriter.WriteStartElement("BORROWER");
				xmlWriter.WriteElementString("BORROWERTYPEID", "2");
				xmlWriter.WriteElementString("PRIMARYBORROWER", "0");
				xmlWriter.WriteElementString("FIRSTNAME", GetSession("CoFirstName"));
				xmlWriter.WriteElementString("LASTNAME", GetSession("CoLastName"));
				xmlWriter.WriteElementString("SSN", "0");
				xmlWriter.WriteElementString("SELFEMPLOYED", GetCoSelfEmployed());
				xmlWriter.WriteElementString("MONTHSATEMPLOYER", "0");
				xmlWriter.WriteElementString("CREDITSCORE", "0");
				xmlWriter.WriteStartElement("MAILINGADDRESS");
				xmlWriter.WriteElementString("ADDRESS1", GetSession("Address"));
				xmlWriter.WriteElementString("ADDRESS2", "");
				xmlWriter.WriteElementString("CITY", GetSession("City"));
				xmlWriter.WriteElementString("STATE", StateCodeLookup(GetSession("State")));
				xmlWriter.WriteElementString("ZIPCODE", GetSession("ZipCode"));
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("CONTACTS");
					GetContacts();
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

		private void WriteBroker()
		{
			xmlWriter.WriteStartElement("BROKER");
				xmlWriter.WriteElementString("USERGUID", "UserGuid");
			xmlWriter.WriteEndElement();
		}

		private void WriteSubjectProperty()
		{
			xmlWriter.WriteStartElement("SUBJECTPROPERTY");
				//xmlWriter.WriteElementString("PROPERTYTYPE", GetPropertyType());
				xmlWriter.WriteElementString("APPRAISEDVALUE", GetSession("PropertyValue", "double"));
				xmlWriter.WriteElementString("HAZARDINSURANCE", GetSession("HazardInsurance", "double"));
				xmlWriter.WriteElementString("TAXES", GetSession("RETaxes", "double"));
			xmlWriter.WriteEndElement();
		}

		private string GetSession(string Key, string Type)
		{
			if(context.Session[Key]!=null)
				return context.Session[Key].ToString();
			else
			{
				return GetStringType(Type);
			}
		}

		private string GetSession(string Key)
		{
			if(context.Session[Key]!=null)
				return context.Session[Key].ToString();
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

		private string GetSelfEmployed()
		{
			string SelfEmployed = "0";
			if(context.Session["EmploymentStatus"]!=null)
			{
				if(context.Session["EmploymentStatus"].ToString().ToLower().Replace(" ","")=="selfemployed")
					SelfEmployed = "-1";
			}
			return SelfEmployed;
		}

		private string GetCoSelfEmployed()
		{
			string CoSelfEmployed = "0";
			if(context.Session["CoEmploymentStatus"]!=null)
			{
				if(context.Session["CoEmploymentStatus"].ToString().ToLower().Replace(" ","")=="selfemployed")
					CoSelfEmployed = "-1";
			}
			return CoSelfEmployed;
		}

		private void GetContacts()
		{
			xmlWriter.WriteStartElement("CONTACT");
				xmlWriter.WriteElementString("INFOTYPEID", "OfficePhone");
				xmlWriter.WriteElementString("INFO", GetSession("WorkPhone"));
				xmlWriter.WriteElementString("BESTCONTACTMETHOD", "true");
				xmlWriter.WriteElementString("TIMETOCONTACT", GetSession("TimeToCall"));
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("CONTACT");
				xmlWriter.WriteElementString("INFOTYPEID", "HomePhone");
				xmlWriter.WriteElementString("INFO", GetSession("HomePhone"));
				xmlWriter.WriteElementString("BESTCONTACTMETHOD", "false");
				xmlWriter.WriteElementString("TIMETOCONTACT", GetSession("TimeToCall"));
			xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("CONTACT");
				xmlWriter.WriteElementString("INFOTYPEID", "OfficeFax");
				xmlWriter.WriteElementString("INFO", GetSession("Fax"));
				xmlWriter.WriteElementString("BESTCONTACTMETHOD", "false");
				xmlWriter.WriteElementString("TIMETOCONTACT", GetSession("TimeToCall"));
			xmlWriter.WriteEndElement();
		}

		private string GetProgramType()
		{
			string ProgramTypeId = "1";
			if(context.Session["PID"]!=null)
			{
				switch(context.Session["PID"].ToString())
				{
					case "1":
						ProgramTypeId = "1";
						break;
					case "2":
						ProgramTypeId = "2";
						break;
					case "3":
						ProgramTypeId = "7";
						break;
					case "4":
						ProgramTypeId = "8";
						break;
					case "5":
						ProgramTypeId = "15";
						break;
					case "6":
						ProgramTypeId = "16";
						break;
					case "7":
						ProgramTypeId = "3";
						break;
					case "8":
						ProgramTypeId = "4";
						break;
					case "9":
						ProgramTypeId = "5";
						break;
					case "10":
						ProgramTypeId = "9";
						break;
					case "11":
						ProgramTypeId = "10";
						break;
					case "12":
						ProgramTypeId = "11";
						break;
					case "13":
						ProgramTypeId = "13";
						break;
					case "15":
						ProgramTypeId = "14";
						break;
					case "16":
						ProgramTypeId = "6";
						break;
					case "18":
						ProgramTypeId = "12";
						break;
					default:
						ProgramTypeId = "1";
						break;
				}
			}
			return ProgramTypeId;
		}

		private string GetLoanType()
		{
			string LoanType = "Conventional";
			if(context.Session["LoanTypeId"]!=null)
			{
				switch(context.Session["LoanTypeId"].ToString())
				{
					case "13":
						LoanType = "VA";
						break;
					case "9":
						LoanType = "FHA";
						break;
					default:
						LoanType = "Conventional";
						break;
				}
			}
			return LoanType;
		}

		private string PropertyUse()
		{
			string PropertyUse = "PrimaryResidence";
			if(context.Session["PropertyUseId"]!=null)
			{
				switch(context.Session["PropertyUseId"].ToString())
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
			if(context.Session["PropertyTypeId"]!=null)
			{
				switch(context.Session["PropertyTypeId"].ToString())
				{
					case "1":
						PropertyType = "SingFamily";
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

		private string GetApplicationType()
		{
			string ApplicationTypeId = "14";
			return ApplicationTypeId;
		}

		public void ParseXml(string XmlString)
		{
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
				//? don't know - maybe do nothing
			}
		}

		private void ParseXmlError(XmlNode xmlNode)
		{
			string ErrorMessage = xmlNode.ChildNodes[0].ChildNodes[2].InnerText;
			throw new Exception(ErrorMessage);
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
					{HelperFunctions helperFunctions = new HelperFunctions();
					return helperFunctions.GetStateCodeByStateName(stateName);}
					catch{return "";} 
			}
		}
	}
}
