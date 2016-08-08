using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace HTE
{
    using Section = Dictionary<string, string>;

    // basic UTF-8 ini/xml settings data
    // yml/json would be nice to have as well
    public class Settings
    {
        Dictionary<string, Section> sections = new Dictionary<string, Section>();
        public Settings()
        { }

        // copy constructor
        public Settings(Settings parent)
        {
            foreach (var section in parent.sections.Keys)
            {
                sections[section] = new Section(parent.sections[section]);
            }
        }

        public static Settings FromFile(string filePath, string format)
        {
            var settings = new Settings();


            switch (format)
            {
                case "ini":
                    settings.LoadINI(filePath);
                    break;
                case "xml":
                    settings.LoadXML(filePath);
                    break;
                
                default:
                    throw new NotSupportedException($"Settings format {format} is not supported!");
            }

            return settings;
        }

        void LoadINI(string filePath)
        {
            using (var reader = File.OpenText(filePath))
            {
                int n = 0;
                string section = string.Empty;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().TrimStart();
                    n++;

                    // skip comment lines
                    if (line.StartsWith(";") || line.StartsWith("#")) continue;

                    // parse line, ignoring empty ones
                    if (line.StartsWith("["))
                    {
                        // new section
                        var end = line.IndexOf(']');
                        if (end <= 0) throw new Exception($"line {n}: Invalid section header!");

                        var header = line.Substring(1, end - 1);
                        if (sections.ContainsKey(header)) throw new Exception($"line {n}: Repeated section header!");

                        section = header;
                        sections[header] = new Section();
                    }
                    else if (line.Contains('='))
                    {
                        // new key-value pair
                        var index = line.IndexOf('=');
                        if (index <= 0) throw new Exception($"line {n}: Invalid key!");

                        var key = line.Substring(0, index);
                        var value = line.Substring(index + 1);

                        if (section == string.Empty) throw new Exception($"line {n}: Found key-value pair outside section!");
                        sections[section][key] = value; // overwrites any existing kvp
                    }
                }
            }
        }

        void LoadXML(string filePath)
        {
            // load the file into an xml document
            var doc = new XmlDocument();
            doc.Load(filePath);

            var root = doc.SelectSingleNode("/settings");

            // get the root node 'settings'
            foreach (XmlElement x in root.ChildNodes)
            {
                // create the next section
                var header = x.LocalName;
                if (sections.ContainsKey(header)) throw new Exception($"Section {header} already exists!");
                sections[header] = new Section();

                // get all elements
                foreach (XmlElement kvp in x.ChildNodes)
                {
                    var key = kvp.LocalName;
                    var value = kvp.InnerText;

                    sections[header][key] = value;
                }
            }
        }

        public void Save(string filePath, string format)
        {
            switch (format)
            {
                case "ini":
                    SaveINI(filePath);
                    break;
                case "xml":
                    SaveXML(filePath);
                    break;

                default:
                    throw new NotSupportedException($"Settings format {format} is not supported!");
            }
        }

        void SaveINI(string filePath)
        {
            // export an .ini file
            using (var writer = File.CreateText(filePath))
            {
                foreach (var section in sections.Keys)
                {
                    writer.WriteLine("[{0}]", section);
                    foreach (var entry in sections[section].Keys)
                    {
                        writer.WriteLine("{0}={1}", entry, sections[section][entry]);
                    }
                    writer.WriteLine();
                }
            }
        }

        void SaveXML(string filePath)
        {
            // don't bother wasting time with xml classes
            using (var writer = File.CreateText(filePath))
            {
                writer.WriteLine("<settings>");
                foreach (var section in sections.Keys)
                {
                    writer.WriteLine("\t<{0}>", section);
                    foreach (var entry in sections[section].Keys)
                    {
                        writer.WriteLine("\t\t<{0}>{1}</{0}>", entry, sections[section][entry]);
                    }
                    writer.WriteLine("\t</{0}>", section);
                }
                writer.WriteLine("</settings>");
            }
        }

        public string GetString(string section, string key)
        {
            if (sections.ContainsKey(section) && sections[section].ContainsKey(key))
                return sections[section][key];
            return string.Empty;
        }

        public bool GetBoolean(string section, string key)
        {
            if (GetString(section, key).ToLower() == "true") return true;
            return false;
        }

        public int GetInt32(string section, string key)
        {
            int i;
            if (int.TryParse(GetString(section, key), out i)) return i;
            return 0;
        }

        public double GetDouble(string section, string key)
        {
            double d;
            if (double.TryParse(GetString(section, key), out d)) return d;
            return 0d;
        }

        public string[] GetStrings(string section, string key)
        {
            return GetString(section, key).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public void Set(string section, string key, string value)
        {
            if (!sections.ContainsKey(section)) sections[section] = new Section();
            sections[section][key] = value;
        }

        public void Set(string section, string key, bool value)
        {
            Set(section, key, value ? "true" : "false");
        }

        public void Set(string section, string key, int value, string format = "")
        {
            Set(section, key, value.ToString(format));
        }

        public void Set(string section, string key, double value)
        {
            Set(section, key, value.ToString());
        }

        public void Set(string section, string key, string[] values)
        {
            Set(section, key, string.Join(",", values));
        }

        public bool ContainsSection(string section)
        {
            return sections.ContainsKey(section);
        }

        public bool ContainsKey(string section, string key)
        {
            if (sections.ContainsKey(section))
                return sections.ContainsKey(key);

            return false;
        }

#if DEBUG
        public override string ToString()
        {
            var writer = new StringBuilder();
            foreach (var section in sections.Keys)
            {
                writer.AppendFormat("{0}:\n", section);
                foreach (var entry in sections[section].Keys)
                {
                    writer.AppendFormat("\t{0}={1}\n", entry, sections[section][entry]);
                }
                writer.AppendLine();
            }
            return writer.ToString();
        }
#endif
    }
}
