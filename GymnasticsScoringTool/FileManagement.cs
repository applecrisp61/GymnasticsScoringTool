using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace GymnasticsScoringTool {
    public static class FileManagement {

        public async static Task<IReadOnlyList<StorageFile>> SelectTeamsToLoad() {
            var picker = new FileOpenPicker() { SuggestedStartLocation = PickerLocationId.Desktop, ViewMode = PickerViewMode.List };
            picker.FileTypeFilter.Add(".team");

            var files = await picker.PickMultipleFilesAsync();
            return files;
        }

        public async static Task InitializeTeams(IReadOnlyList<StorageFile> teamFiles) {
            StringBuilder teamNameSB = new StringBuilder();

            foreach (var file in teamFiles) {
                IEnumerable<Gymnast> gymnastsOnTeam;
                using (IInputStream inStream = await file.OpenSequentialReadAsync()) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(IEnumerable<Gymnast>));
                    gymnastsOnTeam = (IEnumerable<Gymnast>)serializer.ReadObject(inStream.AsStreamForRead());
                }

                var divNames = (from g in gymnastsOnTeam
                                select g.division.name).Distinct();

                foreach (var name in divNames) {
                    if (!Meet.ContainsDivision(name)) {
                        Division d = new Division(name);
                        Meet.AddDivision(d);
                    }
                }

                string teamName = (from g in gymnastsOnTeam
                                   select g.team.name).Distinct().Single();

                Team t;
                if (!Meet.ContainsTeam(teamName)) {
                    t = new Team(teamName);
                    Meet.AddTeam(t);
                }
                else {
                    t = Meet.GetTeamWithName(teamName);
                }

                foreach (var g in gymnastsOnTeam) {
                    if (!Meet.ContainsGymnast(g.firstName, g.lastName, g.team.name)) {
                        Gymnast gymnast = new Gymnast(g.firstName, g.lastName, t, g.division.name);
                        Meet.AddGymnast(gymnast);
                    }
                }
                
            }
        }


        public async static Task SaveTeamFiles() {
            string exceptionMsg = "";

            StringBuilder filename = new StringBuilder();
            string dateString = DateTime.Today.Year.ToString() + (DateTime.Today.Month < 10 ? "0" : "") + DateTime.Today.Month.ToString()
                + (DateTime.Today.Day < 10 ? "0" : "") + DateTime.Today.Day.ToString();

            var query = Meet.GetGymnastListByTeam();

            FolderPicker picker;
            try {
                picker = new FolderPicker() { SuggestedStartLocation = PickerLocationId.Desktop };
                picker.FileTypeFilter.Add(".team");
            }
            catch (Exception e) {
                exceptionMsg = "During call to FolderPicker constructor";
                await ExceptionLogger(exceptionMsg + Environment.NewLine + e.Message);
                throw new Exception(exceptionMsg, e);
            }

            StorageFolder folder;
            try {
                folder = await picker.PickSingleFolderAsync();
            }
            catch (Exception e) {
                exceptionMsg = "During call to picker.PickSingleFolderAsync()";
                await ExceptionLogger(exceptionMsg + Environment.NewLine + e.Message);
                throw new Exception(exceptionMsg, e);
            }

            foreach (var entry in query) {
                filename.Clear();
                var tname = entry.Key;

                var gymnastsOnTeam = from g in entry
                                     select g;

                MemoryStream teamData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(IEnumerable<Gymnast>));
                serializer.WriteObject(teamData, gymnastsOnTeam);

                filename.Append(tname + " " + dateString + ".team");

                try { 
                    using (Stream stream = await folder.OpenStreamForWriteAsync(filename.ToString(), CreationCollisionOption.GenerateUniqueName)) {
                        teamData.Seek(0, SeekOrigin.Begin);
                        await teamData.CopyToAsync(stream);
                        await stream.FlushAsync();
                    }
                }
                catch(Exception e) {
                    exceptionMsg = "During attempt to set-up output stream and copy memory stream to it";
                    await ExceptionLogger(exceptionMsg + Environment.NewLine + e.Message);
                    throw new Exception(exceptionMsg, e);
                }
                
            }
            
            
        }

        public static async Task OutputRostersToTextAsync() {
            
            // Build the roster output
            StringBuilder sb = new StringBuilder();
            var roster = Meet.ProvideGymnastRosters();
            var iter = roster.GetEnumerator();

            string currentTeam = "";
            while (iter.MoveNext()) {
                if (iter.Current.Item1 != currentTeam) {
                    currentTeam = iter.Current.Item1;
                    sb.AppendLine();
                    sb.AppendLine(currentTeam.ToUpper());
                }
                sb.AppendLine($"{iter.Current.Item2.ToString()} {iter.Current.Item4}, {iter.Current.Item3}");
            }
            

            // Now save the roster output to a selected file
            string exceptionMsg;
            FileSavePicker picker;
            try {
                picker = new FileSavePicker() { SuggestedStartLocation = PickerLocationId.Desktop };
                var typeChoices = new KeyValuePair<string, IList<string>>("Text", new List<string> { ".txt"});
                picker.FileTypeChoices.Add(typeChoices);
            }
            catch (Exception e) {
                exceptionMsg = "During call to FileSavePicker constructor";
                await ExceptionLogger(exceptionMsg + Environment.NewLine + e.Message);
                throw new Exception(exceptionMsg, e);
            }

            StorageFile file;
            try {
                file = await picker.PickSaveFileAsync();
            }
            catch (Exception e) {
                exceptionMsg = "During call to picker.PickSaveFileAsync()";
                await ExceptionLogger(exceptionMsg + Environment.NewLine + e.Message);
                throw new Exception(exceptionMsg, e);
            }

            await Windows.Storage.FileIO.WriteTextAsync(file, sb.ToString());
        }


        public static async Task<bool> DoesAutoRecoverFileExist() {

            IStorageItem temp = await ApplicationData.Current.LocalFolder.TryGetItemAsync(ProgramConstants.AUTOSAVE_FOLDER_ADJ);
            if(temp == null) { return false; }
            StorageFolder arFolder = temp as StorageFolder;

            StorageFile arFile = await arFolder.GetFileAsync(ProgramConstants.AUTOSAVE_FILE_NAME);
            if(arFile == null) { return false; }
            return true;

        }

        public static async Task DeleteAutoRecoverFile() {
            IStorageItem temp = await ApplicationData.Current.LocalFolder.TryGetItemAsync(ProgramConstants.AUTOSAVE_FOLDER_ADJ);
            if (temp == null) { return; }
            StorageFolder arFolder = temp as StorageFolder;

            StorageFile arFile = await arFolder.GetFileAsync(ProgramConstants.AUTOSAVE_FILE_NAME);
            if (arFile != null) {
                await arFile.DeleteAsync();
            }

            await arFolder.DeleteAsync();
        }

        public static async Task Serialize<T>(T objectToSerialize, string fileTypeName, string fileExtension, string filename = "", string folderAdj = "") {
            // If filename and folderAdj are both left blank, then use the filepicker... 
            // Facilitates testing by specifying these optional parameters in the unit test file

            StorageFile outfile = null;

            #region SetFolderAndFile.

            if (filename != "") {
                StorageFolder outfolder = null;

                try {
                    // outfolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    outfolder = ApplicationData.Current.LocalFolder;
                    IStorageItem temp = await outfolder.TryGetItemAsync(folderAdj);
                    if (temp == null) {
                        outfolder = await outfolder.CreateFolderAsync(folderAdj, CreationCollisionOption.GenerateUniqueName);
                    }
                    else {
                        outfolder = temp as StorageFolder;
                    }
                }

                catch (System.ArgumentException e) { throw new ArgumentException("During call to sendForSerialization(set Folder): " + e.ParamName, e); }
                catch (System.IO.FileNotFoundException e) { throw new System.IO.FileNotFoundException("During call to sendForSerialization (set Folder)", e); }
                catch (System.UnauthorizedAccessException e) { throw new System.UnauthorizedAccessException("During call to sendForSerialization (set Folder)", e); }
                catch (System.Exception e) { throw new System.Exception("During call to sendForSerialization (set Folder)", e); }

                try {
                    outfile = await outfolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
                }

                catch (System.ArgumentException e) { throw new ArgumentException("During call to sendForSerialization(generate File): " + e.ParamName, e); }
                catch (System.IO.FileNotFoundException e) { throw new System.IO.FileNotFoundException("During call to sendForSerialization (generate File)", e); }
                catch (System.UnauthorizedAccessException e) { throw new System.UnauthorizedAccessException("During call to sendForSerialization (generate File)", e); }
                catch (System.Exception e) { throw new System.Exception("During call to sendForSerialization (generate File)", e); }
            }
            else {
                FileSavePicker picker = new FileSavePicker() {
                    SuggestedStartLocation = PickerLocationId.Desktop
                };
                // Dropdown of file types the user can save the file as
                picker.FileTypeChoices.Add(fileTypeName, new List<string>() { fileExtension });
                // Default file name if the user does not type one in or select a file to replace
                picker.SuggestedFileName = "DefaultFileName";

                outfile = await picker.PickSaveFileAsync();
                if (outfile == null) { throw new Exception("failure during call to sendForSerialization using FileSavePicker"); }
            }

            #endregion AccessFolderAndFile.

            // Copy the file to local app storage
            // Useful if we are overwriting an existing file so we can restore it if there is a failure
            var applicationData = Windows.Storage.ApplicationData.Current;
            var localFolder = applicationData.LocalFolder;
            var backup = await localFolder.CreateFileAsync(outfile.Name, CreationCollisionOption.ReplaceExisting);
            await outfile.CopyAndReplaceAsync(backup);

            try {
                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(sessionData, objectToSerialize);

                var newFile = await localFolder.CreateFileAsync("newfile.meet", CreationCollisionOption.ReplaceExisting);

                using (Stream fileStream = await newFile.OpenStreamForWriteAsync()) {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }

                await newFile.CopyAndReplaceAsync(outfile);
            }
            catch (Exception e) {
                // TODO: need to set up an exception logger
                // TODO: should also pop a content dialog to inform that there was a failure during save
                await backup.CopyAndReplaceAsync(outfile);
            }
        }


        public static async Task<T> Recover<T>(string fileExtension, string filename = "", string folderAdj = "") {
            // If filename and folderAdj are both left blank, then use the filepicker... 
            // Facilitates testing by specifying these optional parameters in the unit test file

            StorageFile file = null;

            if (filename != "") {
                // StorageFolder storageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                try { storageFolder = await storageFolder.GetFolderAsync(folderAdj); }

                catch (System.ArgumentException e) { throw new ArgumentException("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments(generate Folder): " + e.ParamName, e); }
                catch (System.IO.FileNotFoundException e) { throw new System.IO.FileNotFoundException("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments (generate Folder): " + storageFolder.Path, e); }
                catch (System.UnauthorizedAccessException e) { throw new System.UnauthorizedAccessException("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments (generate Folder): " + storageFolder.Path, e); }
                catch (System.Exception e) { throw new System.Exception("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments (generate Folder)", e); }

                try { file = await storageFolder.GetFileAsync(filename); }

                catch (System.ArgumentException e) { throw new ArgumentException("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments(generate File): " + e.ParamName, e); }
                catch (System.IO.FileNotFoundException e) { throw new System.IO.FileNotFoundException("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments (generate File)", e); }
                catch (System.UnauthorizedAccessException e) { throw new System.UnauthorizedAccessException("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments (generate File)", e); }
                catch (System.Exception e) { throw new System.Exception("During call to Helper_StorageAndRecovery.restoreFromSerialization_ExtractedDocuments (generate File)", e); }
            }

            else {
                var picker = new FileOpenPicker() {
                    SuggestedStartLocation = PickerLocationId.Desktop,
                };
                picker.FileTypeFilter.Add(fileExtension);

                file = await picker.PickSingleFileAsync();
                if (file == null) { throw new Exception("failure during call to restoreFromSerialization using FileOpenPicker"); }
            }

            using (IInputStream inStream = await file.OpenSequentialReadAsync()) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                var data = (T)serializer.ReadObject(inStream.AsStreamForRead());
                return data;
            }
        }

        public static async Task ExceptionLogger(string logEntry) {
            IStorageItem temp = await ApplicationData.Current.LocalFolder.TryGetItemAsync(ProgramConstants.EXCEPTION_LOG_FOLDER_ADJ);
            if (temp == null) { return; }
            StorageFolder exlogFolder = temp as StorageFolder;

            StorageFile exlogFile = await exlogFolder.CreateFileAsync(ProgramConstants.EXCEPTION_LOG_FILE_NAME, CreationCollisionOption.OpenIfExists);
            if (exlogFile == null) { return; }

            using (StorageStreamTransaction transaction = await exlogFile.OpenTransactedWriteAsync()) {
                using (DataWriter dataWriter = new DataWriter(transaction.Stream)) {
                    dataWriter.WriteString(logEntry);
                    transaction.Stream.Size = await dataWriter.StoreAsync(); // reset stream size to override the file
                    await transaction.CommitAsync();

                }
            }
        }
/*
        try
        {
            if (file != null)
            {
                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
                {
                    using (DataWriter dataWriter = new DataWriter(transaction.Stream))
                    {
                        dataWriter.WriteString("Swift as a shadow");
                        transaction.Stream.Size = await dataWriter.StoreAsync(); // reset stream size to override the file
            await transaction.CommitAsync();
        }
                }
            }
        }
        // Use catch blocks to handle errors
        catch (FileNotFoundException)
        {
            // For example, handle a file not found error
        }

    */
    }
}
