using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitIntoMonthlyFolders
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				PrintUsage();
				return;
			}
			DirectoryInfo di = new DirectoryInfo(args[0]);
			if (!di.Exists)
			{
				PrintUsage();
				return;
			}
			string baseDir = di.FullName.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
			FileInfo[] files = di.GetFiles();
			Console.WriteLine("Found " + files.Length + " files in " + di.FullName);
			HashSet<string> foldersCreated = new HashSet<string>();
			int filesMoved = 0;
			double divideBy = files.Length / 100.0;
			int lastPercentReported = 0;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			foreach (FileInfo file in files)
			{
				DateTime dateMod = file.LastWriteTime;
				string folderName = dateMod.Year + "_" + dateMod.Month.ToString().PadLeft(2, '0');
				string destDir = baseDir + folderName;
				if (!foldersCreated.Contains(folderName))
				{
					if (!Directory.Exists(destDir))
						Directory.CreateDirectory(destDir);
					foldersCreated.Add(folderName);
				}
				file.MoveTo(destDir + Path.DirectorySeparatorChar + file.Name);
				filesMoved++;
				int percent = (int)(filesMoved / divideBy);
				if (percent != lastPercentReported)
				{
					double percentComplete = filesMoved / (double)files.Length;
					long timeMs = sw.ElapsedMilliseconds;
					TimeSpan TimeWaited = TimeSpan.FromMilliseconds(timeMs);
					TimeSpan EstimatedTotal = TimeSpan.FromMilliseconds((long)sw.ElapsedMilliseconds / percentComplete);
					TimeSpan EstimatedRemaining = EstimatedTotal - TimeWaited;
					Console.WriteLine(percent + "% (" + filesMoved + " / " + files.Length + "). Time: " + TS2S(TimeWaited) + ", ETA: " + TS2S(EstimatedRemaining));
					lastPercentReported = percent;
				}
			}
		}
		private static string TS2S(TimeSpan time)
		{
			return (time.Days == 0 ? "" : time.Days + ":")
				+ time.Hours.ToString().PadLeft(2, '0') + ":"
				+ time.Minutes.ToString().PadLeft(2, '0') + ":"
				+ time.Seconds.ToString().PadLeft(2, '0');
		}
		private static void PrintUsage()
		{
			Console.WriteLine("Drag one folder onto this executable, and all files inside the folder will be inserted into subfolders named YYYY_MM depending on the date modified of the files.");
		}
	}
}
