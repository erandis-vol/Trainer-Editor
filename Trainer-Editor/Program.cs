using System;
using System.IO;
using System.Windows.Forms;

namespace TrainerEditor
{
    internal static class Program
    {
        private static string projectDirectory = "pokeemerald";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Sets the specified project directory.
        /// </summary>
        /// <param name="directory"></param>
        public static void SetProjectDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException();
            }

            if (!Path.IsPathRooted(directory))
            {
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
            }

            projectDirectory = directory;
        }

        /// <summary>
        /// Determines whether a project has been opened.
        /// </summary>
        /// <returns></returns>
        public static bool IsProjectOpen()
        {
            return projectDirectory != null && Directory.Exists(projectDirectory);
        }

        /// <summary>
        /// Returns the full name of the specified project file.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>The full name of the project file, or null if there is no project.</returns>
        public static string GetProjectFile(params string[] paths)
        {
            return projectDirectory + Path.DirectorySeparatorChar + Path.Combine(paths);
        }
    }
}
