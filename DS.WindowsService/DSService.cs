using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using DS.DL.DataContext;
using DS.DL.DataContext.Base;
using DS.DL.DataContext.Base.Interfaces;
using DS.Domain;
using DS.Domain.Interface;
using DS.Service;
using DS.WindowsService.Debugger;
using NLog;
using StructureMap;
using Timer = System.Timers.Timer;

namespace DS.WindowsService
{
    public partial class DSService : DebuggableService
    {
        private static readonly int _hourToRun = Convert.ToInt32(ConfigurationManager.AppSettings["HourToRun"]);
        private static readonly int _minuteToRun = Convert.ToInt32(ConfigurationManager.AppSettings["MinuteToRun"]);
        private static FileSystemWatcher watcher = new FileSystemWatcher();
        private static Timer timer;
        private static DateTime nextRun = DateTime.Now;

        private static IDataSetSchemaService _dataSetSchemaService;
        private static IDataSetDetailUploaderService _uploaderService;
        private static DebugInfoService _debugInfoService;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #region Constructor

        public DSService()
        {
            InitializeComponent();
            InitialiseDatabase();
            
            CanPauseAndContinue = true;
        }

        #endregion

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        private static void StartService()
        {
            try
            {
                AddDebugInfo(new DebugInfo("Service started...", DebugInfoTypeEnum.Developer));
                AddMissingSchemaFolders();

                //perform tasks
                WatchForNewDataset();
                DailyTaskTimer();

            }
            catch (ReflectionTypeLoadException ex)
            {
                var loaderExceptions = ex.LoaderExceptions;
                foreach (var loaderException in loaderExceptions)
                {
                    AddDebugInfo(new DebugInfo(string.Format("EF ERROR: {0}", loaderException.Message), DebugInfoTypeEnum.Error), loaderException);
                }
                AddDebugInfo(new DebugInfo(string.Format("ERROR: {0}", ex.Message), DebugInfoTypeEnum.Error), ex);
            }
            catch (Exception ex)
            {
                AddDebugInfo(new DebugInfo(String.Format("ERROR: Service about to be re started... {0}", ex.Message), DebugInfoTypeEnum.Error), ex);

                //restart the service
                RestartService("DataShare.Service", 10000);
            }
        }

        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                Logger.Debug("Restarting service");
                var millisec1 = Environment.TickCount;
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                var millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error restarting service");
            }
        }
        #region Raised Events

        /// <summary>
        /// Validate/Upload then delete the new dataSet. Email if there is an error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DatasetUploaded(object sender, FileSystemEventArgs e)
        {
            const int maximumRetryPeriod = 5; //max retry for 5 minutes
            const int retryDelay = 10000;  //try every 10 seconds

            var fileReceived = DateTime.Now;

            if (e.FullPath.Contains(".csv"))
            {
                try
                {
                    while (true) //have to wait until the file is fully uploaded
                    {
                        if (FileUploadCompleted(e.FullPath))
                        {
                            //get the title
                            var fileDetail = e.FullPath.Split('\\');
                            var schemaId = Convert.ToInt32(fileDetail[fileDetail.Length - 2]);
                            var schemaDetail = _dataSetSchemaService.Get(schemaId);
                            var fileName = fileDetail[fileDetail.Length - 1];

                            //Validate and upload the data
                            var data = new MediaAssetUploadModel
                            {
                                SchemaId = schemaId,
                                Title = String.Format("{0} at {1} {2}", schemaDetail.Title, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString())
                            };

                            var result = _uploaderService.SaveCsv(schemaId, data.Title, e.FullPath);

                            if (result.Errors.Count > 0)
                            {
                                var email = BuildFailedUploadEmail(result, fileName, schemaDetail.Category.Title, schemaDetail.Title);
                                AddDebugInfo(new DebugInfo(String.Format("Category title:{0}, Schema title:{1}. Automatic csv upload failed as the csv had invalid data entered. File: {2} . Errors: {3}", schemaDetail.Category.Title, schemaDetail.Title, fileName, email.Length > 3500 ? email.Substring(0, 3500) : email), DebugInfoTypeEnum.FolderWatchTriggered));
                                SendEmail(schemaDetail.OwnerEmail, email, false, String.Format("Category title:{0}, Schema title:{1}. Automatic csv upload failed as the csv had invalid data entered", schemaDetail.Category.Title, schemaDetail.Title));
                            }
                            else
                            {
                                AddDebugInfo(new DebugInfo(String.Format(@"Category title:{0}, Schema title:{1}. Dataset was successfully added to datashare. Title: {2}", schemaDetail.Category.Title, schemaDetail.Title, data.Title), DebugInfoTypeEnum.FolderWatchTriggered));
                            }
                            break;
                        }
                        // Calculate the elapsed time and stop if the maximum retry period has been reached.
                        var timeElapsed = DateTime.Now - fileReceived;
                        if (timeElapsed.TotalMinutes > maximumRetryPeriod)
                        {
                            break;
                        }
                        Thread.Sleep(retryDelay);
                    }
                }
                catch (Exception ex)
                {
                    AddDebugInfo(new DebugInfo(String.Format(@"Error uploading dataset: {0}", ex.Message), DebugInfoTypeEnum.Error), ex);
                    RestartService("DataShare.Service", 10000);
                }
            }
        }

        private static void PerformDailyTasks()
        {
            CheckForOverdueUploads();

            AddMissingSchemaFolders();
        }

        private static void AddMissingSchemaFolders()
        {
            try
            {
                Logger.Debug("Adding missing schema folders");
                var schemas = _dataSetSchemaService.GetFullList();
                var createdFolders = new List<string>();
                foreach (var s in schemas)
                {
                    var dirToCheck = String.Format("{0}\\FolderWatch\\{1}", ConfigurationManager.AppSettings["FileRoot"], s.Id);
                    if ((Directory.Exists(dirToCheck))) continue;

                    Directory.CreateDirectory(dirToCheck);
                    createdFolders.Add(s.Id.ToString());
                }

                AddDebugInfo(createdFolders.Count > 0
                                 ? new DebugInfo(String.Format(@"The following schema folders were created: {0}", String.Join(",", createdFolders.ToArray())), DebugInfoTypeEnum.Developer)
                                 : new DebugInfo("No new schema folders were created", DebugInfoTypeEnum.Developer));
            }
            catch (Exception ex)
            {
                AddDebugInfo(new DebugInfo(String.Format(@"AddMissingSchemaFolders threw an error: {0}", ex.Message), DebugInfoTypeEnum.Error), ex);
                RestartService("DataShare.Service", 10000);
            }
        }

        /// <summary>
        /// Check for any Overdue uploads and email if necessary
        /// </summary>
        private static void CheckForOverdueUploads()
        {
            try
            {
                AddDebugInfo(new DebugInfo("CheckForOverdueUploads called.", DebugInfoTypeEnum.Developer));

                var overdueSchemas = _dataSetSchemaService.GetOverDue();
                //send reminder emails
                if (overdueSchemas.Count > 0)
                {
                    AddDebugInfo(new DebugInfo(overdueSchemas.Count + " overdue reminder emails will be sent.", DebugInfoTypeEnum.EmailSent));
                }
                foreach (var schema in overdueSchemas)
                {
                    var template = String.Format("{0}\\EmailTemplates\\UploadReminder.txt", ConfigurationManager.AppSettings["FileRoot"]);
                    var emailText = new System.Text.StringBuilder();
                    emailText.Append(File.ReadAllText(template));
                    emailText.Replace("#Schema#", schema.Title);
                    emailText.Replace("#LastUploadDate#", schema.DateLastUploadedTo.ToString());
                    emailText.Replace("#DatashareURL#", ConfigurationManager.AppSettings["DatashareURL"]);

                    SendEmail(schema.OwnerEmail, emailText.ToString(), true, "Overdue upload reminder");
                    AddDebugInfo(new DebugInfo(String.Format(@"Schema: {0}. Overdue upload reminder sent to:""{1}""", schema.Title, schema.OwnerEmail), DebugInfoTypeEnum.EmailSent));
                    schema.DateLastReminded = DateTime.Now;
                    _dataSetSchemaService.Save(schema);
                }
            }
            catch (Exception ex)
            {
                AddDebugInfo(new DebugInfo(String.Format(@"CheckForOverdueUploads threw an error: {0}", ex.Message), DebugInfoTypeEnum.Error), ex);
                RestartService("DataShare.Service", 10000);
            }
        }

        #endregion

        #region Timer methods

        /// <summary>
        /// Timer code used to call CheckForOverdueUploads method once a day
        /// </summary>
        private static void DailyTaskTimer()
        {
            //This equates to runnng at _hourToRun:_minuteToRun each day
            var today = DateTime.Today;
            nextRun = new DateTime(today.Year, today.Month, today.Day, _hourToRun, _minuteToRun, 0);

            timer = new Timer();
            timer.Elapsed += TimerElapsed;
            timer.Start();
            AddDebugInfo(new DebugInfo(String.Format(@"Next call to PerformDailyTasks wil be on {0} at {1}", nextRun.ToShortDateString(), nextRun.ToShortTimeString()), DebugInfoTypeEnum.Developer));

        }

        private static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Check the time);
            if (nextRun > DateTime.Now) return;

            timer.Stop();

            //perform task
            PerformDailyTasks();

            var nextDate = DateTime.Today.Date.AddDays(1);
            nextRun = new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, _hourToRun, _minuteToRun, 0);
            AddDebugInfo(new DebugInfo(String.Format(@"Next call to PerformDailyTasks wil be on {0} at {1}", nextRun.ToShortDateString(), nextRun.ToShortTimeString()), DebugInfoTypeEnum.Developer));

            timer.Start();
        }


        /// <summary>
        /// Watch upload folder for any new file and call DatasetUploaded() when one is added
        /// </summary>
        private static void WatchForNewDataset()
        {
            // ***** Change this as required
            watcher.Path = String.Format("{0}\\FolderWatch", ConfigurationManager.AppSettings["FileRoot"]);
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;
            watcher.Error += FolderWatchError;
            watcher.Created += DatasetUploaded;

            AddDebugInfo(new DebugInfo(String.Format(@"Folder watcher started successfully for {0}", watcher.Path), DebugInfoTypeEnum.Developer));
        }

        private static void FolderWatchError(object source, ErrorEventArgs e)
        {
            var watchException = e.GetException();
            AddDebugInfo(new DebugInfo(String.Format(@"A FolderWatch error has occurred {0}", watchException.Message), DebugInfoTypeEnum.Developer));

            // We need to create new version of the object because the old one is now corrupted
            watcher = new FileSystemWatcher();
            while (!watcher.EnableRaisingEvents)
            {
                try
                {
                    // This will throw an error at the watcher.NotifyFilter line if it can't get the path.
                    WatchForNewDataset();
                    AddDebugInfo(new DebugInfo(String.Format(@"FolderWatch restarted"), DebugInfoTypeEnum.Developer));
                }
                catch
                {
                    // Sleep for a bit; otherwise, it takes a bit of processor time
                    Thread.Sleep(5000);
                }
            }
        }

        #endregion

        #region Methods
        private static void SendEmail(string toEmail, string emailBody, bool isHtml, string action)
        {
            try
            {
                if (string.IsNullOrEmpty(toEmail)) throw new Exception("toEmail not supplied");
                var client = new SmtpClient {DeliveryMethod = SmtpDeliveryMethod.Network};
                var oCredential = new NetworkCredential("", "");
                client.UseDefaultCredentials = false;
                client.Credentials = oCredential;

                var message = new MailMessage {IsBodyHtml = isHtml};
                message.To.Add(new MailAddress(toEmail, toEmail));
                message.Subject = "DataShare - " + action;
                message.Body = emailBody;
                client.Send(message);
            }
            catch (Exception ex)
            {
                AddDebugInfo(new DebugInfo(String.Format(@"Email failed to send. Send reason: {0}, Sent to: ""{1}"", Email body: {2}", action, toEmail, emailBody), DebugInfoTypeEnum.Error), ex);
            }
        }

        private static string BuildFailedUploadEmail(UploadResult result, string fileName, string category, string schema)
        {
            var template = String.Format("{0}\\EmailTemplates\\FailedUpload.txt", ConfigurationManager.AppSettings["FileRoot"]);
            var emailText = File.ReadAllText(template);
            emailText = emailText.Replace("#FileName#", fileName);
            emailText = emailText.Replace("#CategoryName#", category);
            emailText = emailText.Replace("#SchemaName#", schema);

            emailText = emailText.Replace("#Errors#", String.Join("\n\n", result.Errors));
            return emailText;
        }

        /// <summary>
        /// Used to makes sure the file is fully uploaded
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static bool FileUploadCompleted(string filename)
        {
            // If the file can be opened for exclusive access it means that the file is no longer locked by another process.
            try
            {
                using (var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void InitialiseDatabase()
        {
            // Setup StructureMap to determine the concrete repository pattern to use.
            ObjectFactory.Initialize(
               x =>
               {
                   x.For<IUnitOfWorkFactory>().Use<EFUnitOfWorkFactory>();
                   x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                   x.For<ISqlTableUtility>().Use<SqlTableUtility>();
                   x.For<IDataSetSchemaDefinitionService>().Use<DataSetSchemaDefinitionService>();
                   x.For<IDataSetDetailSqlRepo>().Use<DataSetDetailSqlRepo>();
                   x.For<ISystemConfigurationService>().Use<SystemConfigurationService>();
                   x.For<ICacheProvider>().Use<HttpCache>();
                   x.For<IDataSetDetailCsvProcessor>().Use<DataSetDetailCsvProcessor>();
                   x.For<IDataSetSchemaService>().Use<DataSetSchemaService>();
               }
            );

            
            // Select an Entity Framework model to use with the factory.
            EFUnitOfWorkFactory.SetObjectContext(() => new DataShareContext());

            //Never recreate the database
            Database.SetInitializer<DataShareContext>(null);



            //initialise all services 
            _dataSetSchemaService = new DataSetSchemaService(
                ObjectFactory.GetInstance<IRepository<DataSetSchema>>()
                , ObjectFactory.GetInstance<DataSetSchemaDefinitionService>()
                , ObjectFactory.GetInstance<IRepository<DataSetDetail>>()
                ,ObjectFactory.GetInstance<ISqlTableUtility>());

            _uploaderService = new DataSetDetailUploaderService(
                _dataSetSchemaService
                , ObjectFactory.GetInstance<IRepository<DataSetDetail>>()
                , ObjectFactory.GetInstance<IDataSetDetailCsvProcessor>()
                , ObjectFactory.GetInstance<IDataSetDetailSqlRepo>());

            _debugInfoService = new DebugInfoService(ObjectFactory.GetInstance<IRepository<DebugInfo>>());
        }

        private static void AddDebugInfo(DebugInfo info, Exception e = null)
        {
            if (e == null)
            {
                Logger.Debug(string.Format("{0} {1}", info.Type, info.Description));
            }
            else
            {
                Logger.Error(e, info.Description);
            }
            if (ConfigurationManager.AppSettings["RecordDebugInfo"] == "true")
            {
                _debugInfoService.Add(info);
            }
        }
        #endregion

    }
}
