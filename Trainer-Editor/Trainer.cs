using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrainerEditor
{
    public class Trainer
    {
        private static Regex identifierRegex = new Regex(@"\[TRAINER_(\w+)\]", RegexOptions.Compiled);

        public static IEnumerable<string> LoadIdentifiers()
        {
            var trainers = ReadTrainers();
            if (trainers != null)
            {
                foreach (Match match in identifierRegex.Matches(trainers))
                {
                    // The regex places everything between [TRAINER_ and ] in group 1
                    yield return match.Groups[1].Value;
                }
            }
        }

        public static void Load(string trainer)
        {
            var trainers = ReadTrainers();
            if (trainers != null)
            {
                var identifier = $"[TRAINER_{trainer}]";
                var lines = trainers
                    .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToArray();

                // ------------------------------------------------------------
                // Find the start of the trainer
                // ------------------------------------------------------------

                var start = 0;

                while (start < lines.Length)
                {
                    if (lines[start].StartsWith(identifier))
                    {
                        break;
                    }

                    start++;
                }

                if (start == lines.Length)
                    throw new InvalidDataException($"Could not find {trainer}.");

                // ------------------------------------------------------------
                // Find the end of the trainer
                // ------------------------------------------------------------
                var end = start;

                while (end < lines.Length)
                {
                    if (lines[end] == "},")
                    {
                        break;
                    }

                    end++;
                }

                if (end == lines.Length)
                    throw new InvalidDataException($"Could not isolate {trainer}.");

                // ------------------------------------------------------------
                // Copy trainer definition
                // ------------------------------------------------------------
                var definition = lines.Skip(start).Take(end - start + 1);

                // ------------------------------------------------------------
                // Parse the trainer definition
                // ------------------------------------------------------------
                Parse(definition, identifier);
            }
        }

        public static void Parse(IEnumerable<string> lines, string identifier)
        {
            foreach (var line in lines)
            {
                var assignment = ParseAssignment(line);
                switch (assignment[0])
                {
                    case ".partyFlags":
                        break;
                    case ".trainerClass":
                        break;
                    case ".encounterMusic_gender":
                        break;
                    case ".trainerPic":
                        break;
                    case ".trainerName":
                        break;
                    case ".items":
                        break;
                    case ".doubleBattle":
                        break;
                    case ".aiFlags":
                        break;
                    case ".partySize":
                        break;
                    case ".party":
                        break;
                }
            }
        }

        private static string[] ParseAssignment(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new[] { line ?? string.Empty, string.Empty };

            if (line.EndsWith(","))
                line = line.Remove(line.Length - 1);

            var index = line.IndexOf('=');
            if (index >= 0)
                return new[] { line.Substring(0, index).TrimEnd(), line.Substring(index + 1).TrimStart() };

            return new[] { line, string.Empty };
        }

        private static string ReadTrainers()
        {
            var path = Program.GetProjectFile("src", "data", "trainers.h");
            if (path != null)
            {
                return File.ReadAllText(path);
            }

            return null;
        }
    }
}
