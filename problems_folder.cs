// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;

using System;
using System.IO;

class FolderSynchronizer
{
    public static void SynchronizeFolders(string sourceDir, string replicaDir)
    {
        try
        {
            // Create replica directory if it does not exist
            if (!Directory.Exists(replicaDir))
            {
                Directory.CreateDirectory(replicaDir);
            }

            // Get the list of files in source directory
            foreach (string sourceFilePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(sourceDir, sourceFilePath);
                string replicaFilePath = Path.Combine(replicaDir, relativePath);

                // Create directory if it doesn't exist in replica
                string replicaFileDir = Path.GetDirectoryName(replicaFilePath);
                if (!Directory.Exists(replicaFileDir))
                {
                    Directory.CreateDirectory(replicaFileDir);
                }

                // Copy file if it doesn't exist in replica or if the source file is newer
                if (!File.Exists(replicaFilePath) ||
                    File.GetLastWriteTime(sourceFilePath) > File.GetLastWriteTime(replicaFilePath))
                {
                    File.Copy(sourceFilePath, replicaFilePath, true);
                    Console.WriteLine($"Copied: {relativePath}");
                }
            }

            // Delete files from replica that no longer exist in source
            foreach (string replicaFilePath in Directory.GetFiles(replicaDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(replicaDir, replicaFilePath);
                string sourceFilePath = Path.Combine(sourceDir, relativePath);

                if (!File.Exists(sourceFilePath))
                {
                    File.Delete(replicaFilePath);
                    Console.WriteLine($"Deleted: {relativePath}");
                }
            }

            // Remove empty directories in replica
            foreach (string directory in Directory.GetDirectories(replicaDir, "*", SearchOption.AllDirectories))
            {
                if (Directory.GetFileSystemEntries(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                    Console.WriteLine($"Removed empty directory: {directory}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: FolderSynchronizer.exe <sourceDir> <replicaDir>");
            return;
        }

        string sourceDir = args[0];
        string replicaDir = args[1];

        SynchronizeFolders(sourceDir, replicaDir);
    }
}