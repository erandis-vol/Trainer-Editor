using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace HTE
{
    public class IniFile
    {
        private string path;

        private List<Section> sections;

        public IniFile(string file)
        {
            path = file;
            Load();
        }

        private void Load()
        {
            StreamReader sr = File.OpenText(path);
            sections = new List<Section>();
            int lineNo = 0;
            Section section = null;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine().Trim();
                lineNo += 1;

                // comment
                if (line.Contains("#"))
                {
                    int index = line.IndexOf("#");
                    line = line.Remove(index).TrimEnd();
                }

                // check if it's empty
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;

                // parse
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // add current section to collection so we can start anew
                    if (section != null)
                    {
                        sections.Add(section);
                    }

                    // get section name
                    section = new Section(line.Substring(1, line.Length - 2));
                }
                else if (line.Contains("="))
                {
                    string[] parts = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 2) throw new Exception("Line " + lineNo + ": Bad section entry format!");

                    if (section == null) throw new Exception("Line " + lineNo + ": Cannot declare an entry without being in a section!");

                    // add or replace this entry
                    if (section.entries.ContainsKey(parts[0])) section.entries[parts[0]] = parts[1];
                    else section.entries.Add(parts[0], parts[1]);
                }
                else
                {
                    throw new Exception("Unsure how to parse line " + lineNo + "!");
                }
            }

            // add final section
            if (section != null)
            {
                sections.Add(section);
            }

            sr.Dispose();
        }

        public void Save()
        {
            StreamWriter sw = File.CreateText(path);
            sw.Flush();

            // if there's nothing, it will be a blank file...
            if (sections.Count > 0)
            {
                // write each section
                foreach (Section section in sections)
                {
                    // write section name
                    sw.WriteLine("[" + section.name + "]");

                    // write keys
                    if (section.entries.Count > 0)
                    {
                        foreach (string key in section.entries.Keys)
                        {
                            sw.WriteLine(key + "=" + section.entries[key]);
                        }
                    }

                    // add some buffer space
                    sw.WriteLine();
                }
            }

            sw.Dispose();
        }

        public string this[string section, string key]
        {
            get
            {
                for (int i = 0; i < sections.Count; i++)
                {
                    if (sections[i].name == section)
                    {
                        return sections[i].entries[key];
                    }
                }

                return string.Empty;
            }
            set
            {
                for (int i = 0; i < sections.Count; i++)
                {
                    if (sections[i].name == section)
                    {
                        sections[i].entries[key] = value;
                    }
                }
            }
        }

        public string GetString(string section, string key)
        {
            for (int i = 0; i < sections.Count; i++)
            {
                if (sections[i].name == section)
                {
                    return sections[i].entries[key];
                }
            }

            return string.Empty;
        }

        public uint GetUInt32(string section, string key)
        {
            string val = GetString(section, key);

            uint u;
            if (val.StartsWith("0x"))
            {
                val = val.Substring(2);
                if (uint.TryParse(val, NumberStyles.HexNumber, null, out u)) return u;
            }
            else
            {
                if (uint.TryParse(val, out u)) return u;
            }

            return 0;
        }

        public int GetInt32(string section, string key)
        {
            return (int)GetUInt32(section, key);
        }

        public TreeNode ToTreeNodes()
        {
            TreeNode root = new TreeNode(path);
            for (int i = 0; i < sections.Count; i++)
            {
                TreeNode section = new TreeNode(sections[i].name);
                foreach (string key in sections[i].entries.Keys)
                {
                    TreeNode kEy = new TreeNode(key);
                    kEy.Nodes.Add(new TreeNode(sections[i].entries[key]));
                    section.Nodes.Add(kEy);
                }
                root.Nodes.Add(section);
            }
            return root;
        }

        public string[] GetSections()
        {
            string[] sectionNames = new string[sections.Count];
            for (int i = 0; i < sectionNames.Length; i++)
            {
                sectionNames[i] = sections[i].name;
            }
            return sectionNames;
        }

        private class Section
        {
            public Dictionary<string, string> entries;
            public string name;

            public Section(string name)
            {
                this.name = name;
                this.entries = new Dictionary<string, string>();
            }
        }
    }
}
