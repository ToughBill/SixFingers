using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WorkstationController.Core.Utility;

namespace WTPipetting.Utility
{
    public class FolderHelperEx:FolderHelper
    {
       
        static public string GetLastRunInfoFile()
        {
            return FolderHelperEx.GetExeFolder() + "lastRunInfo.xml";
        }

        static public void WriteResult(bool bok)
        {
            string file = GetOutputFolder() + "result.txt";
            File.WriteAllText(file, bok.ToString());
        }

      

        public static string GetImageFolder()
        {
            return GetExeFolder() + "Images\\";
        }

        internal static string GetProtocolFolder()
        {
            return GetExeFolder() + "Protocols\\";
        }

        internal static void WriteVariable(string file, string s)
        {
            string filePath = GetOutputFolder() + file + ".txt";
            File.WriteAllText(filePath, s);
        }

        internal static void WriteRunInfo(string info)
        {
            string filePath = GetOutputFolder() + "history.txt";
            string timeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            File.AppendAllText(filePath, timeStr +"  "+ info+"\r\n");
        }

        internal static string GetBackupFolder()
        {
            string backupFolder =  GetExeParentFolder() + "backup\\";
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);
            backupFolder += string.Format(DateTime.Now.ToString("yyMMddHHmmss"));
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);
            return backupFolder;
        }

        internal static void Backup()
        {
            string sBackup = GetBackupFolder();
            string soutPut = GetOutputFolder();
            CopyFolder(soutPut, sBackup);
        }

        

        
    }
}
