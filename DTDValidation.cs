using System;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Text;

namespace RWS2._2._01.RWSLoanFinder
{
	/// <summary>
	/// Summary description for XMLValidation.
	/// </summary>
	public class XMLValidation
	{
		public XMLValidation()
		{
			//
			// TODO: Add constructor logic here
			//
		}		

		//private string xmlSchemaPath = "";
		public bool xmlValid = true;
		private string valMessage = "";
		XmlTextReader xmlReader = null;
		XmlValidatingReader vReader = null;

		public string ValidationMessage
		{
			get
			{
				return valMessage;
			}
			set
			{
				valMessage = value;
			}
		}

		public bool Validate(Stream xmlStream, string Schema, string SchemaLocation)
		{
			try
			{
				xmlReader = new XmlTextReader(xmlStream);
				vReader = new XmlValidatingReader(xmlReader);

				vReader.ValidationType = ValidationType.Schema;
				
				//no need for a shema collection object in this iteration
//				XmlSchema xmlSchema = new XmlSchema();
//				xmlSchema.TargetNamespace = "la:rwslfdatapass-schema";
//				xmlSchema.SourceUri = SchemaLocation;
					
//				XmlSchemaCollection schemaCol = new XmlSchemaCollection();
//				schemaCol.Add(xmlSchema);

//				if(schemaCol != null)
//				{
//					vReader.Schemas.Add(schemaCol);
//				}

				vReader.Schemas.Add(Schema, SchemaLocation);
				//vReader.Schemas.Add("", SchemaLocation);

				vReader.ValidationEventHandler += new ValidationEventHandler(this.ValidationErrorHandler);
				while(vReader.Read()){}
			}
			catch(Exception e)
			{
				string exception = e.Message;
				xmlValid = false;
			}
			finally
			{
				if(xmlReader.ReadState != ReadState.Closed)
				{
					xmlReader.Close();
				}
				if(vReader.ReadState != ReadState.Closed)
				{
					vReader.Close();
				}
			}
			if(xmlValid)
			{
				ValidationMessage = "Validation Successful";
			}
			return xmlValid;
		}


		public bool Validate(Stream xmlStream, XmlSchemaCollection schemaCol)
		{
			try
			{
				xmlReader = new XmlTextReader(xmlStream);
				vReader = new XmlValidatingReader(xmlReader);

				vReader.ValidationType = ValidationType.Schema;
				
				//no need for a shema collection object in this iteration
				//				XmlSchema xmlSchema = new XmlSchema();
				//				xmlSchema.TargetNamespace = "la:rwslfdatapass-schema";
				//				xmlSchema.SourceUri = SchemaLocation;
					
				//				XmlSchemaCollection schemaCol = new XmlSchemaCollection();
				//				schemaCol.Add(xmlSchema);

				if(schemaCol != null)
				{
					vReader.Schemas.Add(schemaCol);
				}

				vReader.ValidationEventHandler += new ValidationEventHandler(this.ValidationErrorHandler);
				while(vReader.Read()){}
			}
			catch(Exception e)
			{
				string exception = e.Message;
				xmlValid = false;
			}
			finally
			{
				if(xmlReader.ReadState != ReadState.Closed)
				{
					xmlReader.Close();
				}
				if(vReader.ReadState != ReadState.Closed)
				{
					vReader.Close();
				}
			}
			if(xmlValid)
			{
				ValidationMessage = "Validation Successful";
			}
			return xmlValid;
		}

		private void ValidationErrorHandler(object sender, ValidationEventArgs args)
		{
			xmlValid = false;
			ValidationMessage = "Validation Error Message: " + args.Message;
		}
	}
}
