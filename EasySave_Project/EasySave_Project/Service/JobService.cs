﻿using System;
using System.Collections.Generic;
using EasySave_Project.Model;
using EasySave_Project.Util;

namespace EasySave_Project.Service
{
    /// <summary>
    /// The JobService class provides methods for managing and executing backup jobs.
    /// </summary>
    public class JobService
    {

        /// <summary>
        /// Retrieves a list of all backup jobs configured in the system.
        /// Currently, the jobs are hardcoded for demonstration purposes.
        /// This method will be modified in the future to retrieve jobs from a persistent storage once the job recovery method is implemented.
        /// </summary>
        /// <returns>
        /// A list of <see cref="JobModel"/> objects representing the configured backup jobs. 
        /// Each job includes the name, source directory, target directory, type of save (differential or complete), 
        /// and the path to the last full backup if applicable.
        /// </returns>
        public List<JobModel> GetAllJob()
        {
            List<JobModel> jobsList = new List<JobModel> //TODO to be modified when the job recovery method exists
    {
        new JobModel("Save1", "C:\\Users\\Yanis\\Desktop\\CESI_A3_S5\\GenieLogicielle\\testpourcopy", "C:\\Users\\Yanis\\Desktop\\CESI_A3_S5\\GenieLogicielle\\copy", JobSaveTypeEnum.DIFFERENTIAL, null),
        new JobModel("Save2", "C:\\Users\\Yanis\\Desktop\\CESI_A3_S5\\GenieLogicielle\\testpourcopy", "C:\\Users\\Yanis\\Desktop\\CESI_A3_S5\\GenieLogicielle\\copy", JobSaveTypeEnum.DIFFERENTIAL, "C:\\Users\\Yanis\\Desktop\\CESI_A3_S5\\GenieLogicielle\\copy\\2025_01_31_11_19_02"),
        new JobModel("Save3", "C:\\Users\\Yanis\\Desktop\\CESI_A3_S5\\GenieLogicielle\\testpourcopy", "C:\\Users\\Yanis\\Desktop\\CESI_A3_S5\\GenieLogicielle\\copyComplete", JobSaveTypeEnum.COMPLETE, null),
        new JobModel("Save4", "C:/source4", "D:/backup4", JobSaveTypeEnum.COMPLETE, null),
        new JobModel("Save5", "C:/source5", "D:/backup5", JobSaveTypeEnum.DIFFERENTIAL, null)
    };

            return jobsList;
        }

        /// <summary>
        /// Executes a specific backup job.
        /// </summary>
        /// <param name="job">The JobModel object representing the job to execute.</param>
        public void ExecuteOneJob(JobModel job)
        {
            var translator = TranslationService.GetInstance();

            // Check if the source directory exists
            if (!FileUtil.ExistsDirectory(job.FileSource))
            {
                Console.WriteLine($"{translator.GetText("directorySourceDoNotExist")} : {job.FileSource}");
                return; // Exit the method if the source directory does not exist
            }

            Console.WriteLine($"{translator.GetText("startingBackup")} : {job.Name}");

            // Create the target directory if it doesn't exist
            if (!FileUtil.ExistsDirectory(job.FileTarget))
            {
                FileUtil.CreateDirectory(job.FileTarget); // Create target directory
            }

            // Create a timestamped subdirectory for the backup
            string timestampedBackupDir = FileUtil.CombinePath(job.FileTarget, DateUtil.GetTodayDate(DateUtil.YYYY_MM_DD_HH_MM_SS));
            FileUtil.CreateDirectory(timestampedBackupDir); // Create backup subdirectory

            // Select the appropriate strategy based on the job type
            IJobStrategyService strategy = job.SaveType switch
            {
                JobSaveTypeEnum.COMPLETE => new JobCompleteService(), // Use complete backup strategy
                JobSaveTypeEnum.DIFFERENTIAL => new JobDifferencialService(), // Use differential backup strategy
                _ => throw new InvalidOperationException("Invalid job type") // Throw exception for invalid job type
            };

            // Execute the job using the selected strategy
            strategy.Execute(job, timestampedBackupDir);

            Console.WriteLine($"{translator.GetText("backupCompleted")} : {job.Name}");
        }
    }
}
