using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;

namespace ImageSliderMediaClass
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string _assetsFolderName = "Assets";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            CopySampleFilesToIsolatedStorage();

            //this.MediaViewer.Items = null;
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            List<LocalFolderThumbnailedImage> picList = new List<LocalFolderThumbnailedImage>();
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string[] photoFileNames = store.GetFileNames(Path.Combine(_assetsFolderName, "image*.*"));

                foreach (string photoFileName in photoFileNames.Where(item => !item.Contains("thumb")))
                {
                    string photoFileNameAndPath = Path.Combine(_assetsFolderName, photoFileName);
                    //string photoThumbFileNameAndPath = Path.Combine(_assetsFolderName, photoFileName.Replace(".jpg", ".thumb.jpg"));
                    picList.Add(new LocalFolderThumbnailedImage(null, photoFileNameAndPath)); //, photoThumbFileNameAndPath));
                }
            }

            this.MediaViewer.Items = new ObservableCollection<object>(picList);
        }

        private void CopySampleFilesToIsolatedStorage()
        {
            // Visual Studio deploys our sample photos to the installation directory of the app.
            // To better simulate real world usage of photos in isolated storage
            // (e.g. you'll definitely need write access to the location they are stored in, and you don't have write access to the installation folder)
            // we'll copy them to isolated storage if we haven't already.
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.GetDirectoryNames().Contains<string>(_assetsFolderName))
                    return;

                store.CreateDirectory(_assetsFolderName);

                string[] photoFileNames = { "Assets\\image1.jpg", "Assets\\image2.jpg", "Assets\\image3.jpg", "Assets\\image4.jpg", "Assets\\image1.thumb.jpg", "Assets\\image2.thumb.jpg", "Assets\\image3.thumb.jpg", "Assets\\image4.thumb.jpg" };

                foreach (string photoFileName in photoFileNames)
                {
                    using (Stream input = Application.GetResourceStream(new Uri(photoFileName, UriKind.Relative)).Stream)
                    {
                        using (IsolatedStorageFileStream output = store.CreateFile(photoFileName))
                        {
                            byte[] readBuffer = new byte[4096];
                            int bytesRead = -1;

                            // Copy the file from the installation folder to isolated storage. 
                            while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                            {
                                output.Write(readBuffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }
    }
}