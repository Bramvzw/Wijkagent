using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using wijkagent.Core.Models;
using wijkagent.Core.Services;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System.Threading;
using Windows.UI.Notifications;

namespace wijkagent.BackgroundTasks
{
    public sealed class CheckForNewDelicts : BackgroundTask
    {
        public static string Message { get; set; }
        public static StorageFolder localFolder = ApplicationData.Current.LocalFolder; //gets localfolder where the delictID file gets stored
        public static StorageFile delictFile; //the variable for the delictIDs file
        private static string _delictFileName = "DelictIDs.txt";
        private volatile bool _cancelRequested = false;
        private IBackgroundTaskInstance _taskInstance;
        private BackgroundTaskDeferral _deferral;

        public override void Register()
        {
            var taskName = GetType().Name;
            var taskRegistration = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == taskName).Value;
            if (taskRegistration == null)
            {
                var builder = new BackgroundTaskBuilder()
                {
                    Name = taskName
                };

                // TODO WTS: Define the trigger for your background task and set any (optional) conditions
                // More details at https://docs.microsoft.com/windows/uwp/launch-resume/create-and-register-an-inproc-background-task
                builder.SetTrigger(new TimeTrigger(5, false));
                //not entirely sure the condition \/ is needed
                //builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent)); 

                builder.Register();
            }
        }

        public override Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                return null;
            }

            _deferral = taskInstance.GetDeferral();

            return Task.Run(() =>
            {
                //make a hashset of all delicts
                HashSet<Delict> allDelicts = new HashSet<Delict>(SqlDataService.AllDelicts());
                Task<List<int>> _lastActiveIDsTask = checkForDelicts();
                List<int> _lastActiveIDs = _lastActiveIDsTask.Result;
                foreach (Delict delict in allDelicts)
                {
                    if (!_lastActiveIDs.Contains(delict.Id)) //if the id is not in the lastActiveIDs, it is a new delict, so there needs to be a notification
                    {
                        SendNotification(delict);
                    }
                }
                UpdateDelictFile(SqlDataService.AllDelicts());
                //all of the lower comments were already here, but i don't think this is what this task is doing
                //// This sample initializes a timer that counts to 100 in steps of 10.  It updates Message each time.

                //// Documentation:
                ////      * General: https://docs.microsoft.com/windows/uwp/launch-resume/support-your-app-with-background-tasks
                ////      * Debug: https://docs.microsoft.com/windows/uwp/launch-resume/debug-a-background-task
                ////      * Monitoring: https://docs.microsoft.com/windows/uwp/launch-resume/monitor-background-task-progress-and-completion

                //// To show the background progress and message on any page in the application,
                //// subscribe to the Progress and Completed events.
                //// You can do this via "BackgroundTaskService.GetBackgroundTasksRegistration"

                _taskInstance = taskInstance;
                // this seems to be an example, that's why it's commented out
                //ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(SampleTimerCallback), TimeSpan.FromSeconds(1));
            });
        }

        public override void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;

            // TODO WTS: Insert code to handle the cancelation request here.
            // Documentation: https://docs.microsoft.com/windows/uwp/launch-resume/handle-a-cancelled-background-task
        }

        private void SampleTimerCallback(ThreadPoolTimer timer)
        {
            if ((_cancelRequested == false) && (_taskInstance.Progress < 100))
            {
                _taskInstance.Progress += 10;
                Message = $"Background Task {_taskInstance.Task.Name} running";
            }
            else
            {
                timer.Cancel();

                if (_cancelRequested)
                {
                    Message = $"Background Task {_taskInstance.Task.Name} cancelled";
                }
                else
                {
                    Message = $"Background Task {_taskInstance.Task.Name} finished";
                }

                _deferral?.Complete();
            }
        }
        private void SendNotification(Delict delict)
        {
            ToastContentBuilder content = new ToastContentBuilder();
            //constructing content for notification
            //this is what makes the OnActivated work, Delict_ shows that it is a delict, then the ID is how the application can show the right information
            content.AddToastActivationInfo("Delict_" + delict.Id, ToastActivationType.Foreground);
            content.AddText("Nieuw delict");
            content.AddText(delict.Name);
            ToastContent toastContent = content.GetToastContent();
            //create the notification
            var notif = new ToastNotification(toastContent.GetXml());
            //show it
            ToastNotificationManager.CreateToastNotifier().Show(notif);
        }
        private async Task<List<int>> checkForDelicts()
        {
            bool newfile = false;
            //lastActiveIDs list gets cleared each time
            List<int> _lastActiveIDs = new List<int>();
            if (delictFile == null)
            {
                try
                {
                    delictFile = await localFolder.GetFileAsync(_delictFileName); //see if there is a file, if not (and FileNotFoundException happens, make a file
                }
                catch (FileNotFoundException)
                {
                    //a new file needs to be created
                    await makeFile();
                    newfile = true;
                }
            }
            if (!newfile)
            {
                //read text in the file
                string textFromFile = await FileIO.ReadTextAsync(delictFile);
                // Data is contained in delictsIdsString
                string[] delictIdStrings = textFromFile.Split(","); //inbetween different ids is a comma
                foreach (string idString in delictIdStrings)
                {
                    //add the id to the lastActiveIDs list
                    _lastActiveIDs.Add(int.Parse(idString));
                }
            }
            return _lastActiveIDs;
        }
        private async void UpdateDelictFile(List<Delict> delicts)
        {
            if (!(delictFile == null))
            {
                //the file gets deleted and a new one gets made, so if a delict gets deleted its id is not in the file
                await delictFile.DeleteAsync();
            }
            await makeOrFindFile();
            string idsInStringFormat = "";
            foreach (Delict delict in delicts) //making the string for what will go into the DelictIDs file
            {
                if (idsInStringFormat == "")
                {
                    idsInStringFormat = delict.Id.ToString();
                }
                else
                {
                    idsInStringFormat = idsInStringFormat + "," + delict.Id.ToString();
                }
            }
            await FileIO.WriteTextAsync(delictFile, idsInStringFormat); //write it into the file
        }
        private async Task makeOrFindFile()
        {
            try
            {
                delictFile = await localFolder.GetFileAsync(_delictFileName); //see if there is a file, if not (and FileNotFoundException happens, make a file
            }
            catch (FileNotFoundException)
            {
                await makeFile();
            }
        }
        private async Task makeFile()
        {
            delictFile = await localFolder.CreateFileAsync(_delictFileName, CreationCollisionOption.ReplaceExisting); //create the file
            StorageApplicationPermissions.FutureAccessList.Add(delictFile); //add it to the futureAccessList
        }
    }
}
