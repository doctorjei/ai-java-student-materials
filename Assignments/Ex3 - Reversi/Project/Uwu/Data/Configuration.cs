// Modified: 2011 Derek Bliss (Full Sail University)
// Modified: 2019, 2025 Jeremiah Blanchard (University of Florida)
using System;

namespace Uwu.Data;

/// <summary>Program Settings</summary>
public class Configuration
{
	// The file name used to save the program settings and the document in memory..
	private readonly string filename;
	private readonly System.Xml.XmlDocument document;

	public Configuration(string _filename)
	{
		// Assign the file name.
		this.filename = _filename;

		// If the file already exists, load it. Otherwise, create a new document.
		this.document = new System.Xml.XmlDocument();
		try
		{
			document.Load(this.filename);
		}
		catch (Exception e)
		{
			// Create a new XML document and set the root node.
			document = new System.Xml.XmlDocument();
			document.AppendChild(document.CreateElement("ProgramSettings"));
		}
	}

	public void Save() { this.document.Save(this.filename); } // Saves settings in XML

	// Reads a value from the settings file; internal - NO ERROR CHECKING.
	private string GetRawValue(string section, string name, string defaultValue = "") =>
		document.DocumentElement!.SelectSingleNode(section + "/" + name)?.InnerText ?? defaultValue;

	// Reads a value from the settings file.
	public T Get<T>(string section, string name, T? fallback = default) // Removed constraint?
	{
		string content = GetRawValue(section, name);

		if (content == "")
			return fallback != null ? fallback : throw new ArgumentNullException(nameof(fallback));

		try
		{
			// Try using TypeConverter first (covers many primitive and framework types).
			var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
			if (converter != null && converter.CanConvertFrom(typeof(string)))
				if (converter.ConvertFromInvariantString(content) is T result)
					return result;

			// If that fails, try reflection to find static Parse() variant (if available).
			var parseMethod = typeof(T).GetMethod("Parse", [typeof(string), typeof(IFormatProvider)]);
			if (parseMethod != null && parseMethod.IsStatic)
				if (parseMethod.Invoke(null, [content, null]) is T result)
					return result;

			parseMethod = typeof(T).GetMethod("Parse", [typeof(string)]);
			if (parseMethod != null && parseMethod.IsStatic)
				if (parseMethod.Invoke(null, [content]) is T result)
					return result;
		}
		catch (Exception ex) { Console.WriteLine(ex.Message); }

		// If all else fails, use the fallback value. (If that fails, throw exception.)
		return fallback != null ? fallback : throw new ArgumentNullException(nameof(fallback));
	}

	public System.Xml.XmlNode FetchNode(string section, string name)
	{
		// If the section or node do not exist, create them.
		System.Xml.XmlNode sectionNode = document.DocumentElement!.SelectSingleNode(section) ??
			document.DocumentElement.AppendChild(document.CreateElement(section))!;
		System.Xml.XmlNode node = sectionNode!.SelectSingleNode(name) ??
			sectionNode.AppendChild(document.CreateElement(name))!;
		return node;
	}

	// Writes a value to the settings file.
	public void Set<T>(string section, string name, T? value, Func<T, string>? toStr = null)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));

		// Convert the value to a string and store in the correct section / name.
		FetchNode(section, name).InnerText =
			(toStr != null ? toStr(value) : value.ToString()) ??
						throw new ArgumentNullException(nameof(value));
	}
}