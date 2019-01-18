using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using WindowsSharp.DiskItems;
using Microsoft.VisualBasic.FileIO;
//using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;

namespace NT6FileManagerBase
{
    /// <summary>
    /// Interaction logic for FileManagerBase.xaml
    /// </summary>
    public partial class FileManagerBase : UserControl
    {
        public string CurrentFolderTitle
        {
            get
            {
                try
                {
                    return Path.GetFileName(CurrentPath);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public List<string> HistoryList { get; set; } = new List<string>();
        /*{
            get => (List<String>)GetValue(HistoryListProperty);
            set => SetValue(HistoryListProperty, value);
        }*/

        //public static readonly DependencyProperty HistoryListProperty = DependencyProperty.Register("HistoryList", typeof(List<String>), typeof(FileManagerBase), new PropertyMetadata(new List<String>()));

        public Int32 HistoryIndex
        {
            get => (Int32)GetValue(HistoryIndexProperty);
            set => SetValue(HistoryIndexProperty, value);
        }

        public static readonly DependencyProperty HistoryIndexProperty = DependencyProperty.Register("HistoryIndex", typeof(Int32), typeof(FileManagerBase), new PropertyMetadata(0, OnHistoryIndexPropertyChangedCallback));

        public bool IsRenamingFiles
        {
            get => (bool)GetValue(IsRenamingFilesProperty);
            set => SetValue(IsRenamingFilesProperty, value);
        }

        public static readonly DependencyProperty IsRenamingFilesProperty = DependencyProperty.Register("IsRenamingFiles", typeof(bool), typeof(FileManagerBase), new PropertyMetadata(false));

        static void OnHistoryIndexPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileManagerBase sender = (d as FileManagerBase);
            var oldValue = (Int32)e.OldValue;
            var newValue = (Int32)e.NewValue;

            ////////sender.ValidateNavButtonStates();

            sender.Navigate(sender.HistoryList[newValue]);
        }

        public string CurrentPath
        {
            get => HistoryList[HistoryIndex];
        }

        /*public ObservableCollection<DiskItem> Favorites
        {
            get => (ObservableCollection<DiskItem>)GetValue(FavoritesProperty);
            set => SetValue(FavoritesProperty, value);
        }

        public static readonly DependencyProperty FavoritesProperty = DependencyProperty.Register("Favorites", typeof(ObservableCollection<DiskItem>), typeof(FileManagerBase), new PropertyMetadata(new ObservableCollection<DiskItem>()));*/

        /*public ObservableCollection<DiskItem> ComputerSubfolders
        {
            get => (ObservableCollection<DiskItem>)GetValue(ComputerSubfoldersProperty);
            set => SetValue(ComputerSubfoldersProperty, value);
        }

        public static readonly DependencyProperty ComputerSubfoldersProperty = DependencyProperty.Register("ComputerSubfolders", typeof(ObservableCollection<DiskItem>), typeof(FileManagerBase), new PropertyMetadata(new ObservableCollection<DiskItem>()));*/

        public Double IconSize
        {
            get => (Double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(Double), typeof(FileManagerBase), new PropertyMetadata((Double)48.0));

        public bool ShowDetailsPane
        {
            get => (bool)GetValue(ShowDetailsPaneProperty);
            set => SetValue(ShowDetailsPaneProperty, value);
        }

        public static readonly DependencyProperty ShowDetailsPaneProperty = DependencyProperty.Register("ShowDetailsPane", typeof(bool), typeof(FileManagerBase), new PropertyMetadata(false));

        public bool ShowPreviewPane
        {
            get => (bool)GetValue(ShowPreviewPaneProperty);
            set => SetValue(ShowPreviewPaneProperty, value);
        }

        public static readonly DependencyProperty ShowPreviewPaneProperty = DependencyProperty.Register("ShowPreviewPane", typeof(bool), typeof(FileManagerBase), new PropertyMetadata(false));

        public bool ShowNavigationPane
        {
            get => (bool)GetValue(ShowNavigationPaneProperty);
            set => SetValue(ShowNavigationPaneProperty, value);
        }

        public static readonly DependencyProperty ShowNavigationPaneProperty = DependencyProperty.Register("ShowNavigationPane", typeof(bool), typeof(FileManagerBase), new PropertyMetadata(true));

        public enum FileBrowserView
        {
            Icons,
            List,
            Details,
            Tiles,
            Content
        }

        public FileBrowserView CurrentView
        {
            get => (FileBrowserView)GetValue(CurrentViewProperty);
            set => SetValue(CurrentViewProperty, value);
        }

        public static readonly DependencyProperty CurrentViewProperty = DependencyProperty.Register("CurrentView", typeof(FileBrowserView), typeof(FileManagerBase), new PropertyMetadata(FileBrowserView.Icons));

        public bool ShowItemCheckboxes
        {
            get => (bool)GetValue(ShowItemCheckboxesProperty);
            set => SetValue(ShowItemCheckboxesProperty, value);
        }

        public static readonly DependencyProperty ShowItemCheckboxesProperty = DependencyProperty.Register("ShowItemCheckboxes", typeof(bool), typeof(FileManagerBase), new PropertyMetadata(false));


        public event EventHandler<SelectionChangedEventArgs> CurrentDirectorySelectionChanged;

        public event EventHandler<EventArgs> Navigated;


        public FileManagerBase()
        {
            InitializeComponent();
        }

        private void FileManagerBase_Loaded(object sender, RoutedEventArgs e)
        {
            /*foreach (ResourceDictionary d in Window.GetWindow(this).Resources.MergedDictionaries)
                Resources.MergedDictionaries.Add(d);*/

            Initialize();
        }

        void Initialize()
        {
            //Manager.ClipboardContents.CollectionChanged += Clipboard_CollectionChanged;
            //Clipboard_CollectionChanged(null, null);

            Binding checkBoxBinding = new Binding()
            {
                Source = typeof(Manager),
                Path = new PropertyPath("ShowItemCheckboxes"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(this, FileManagerBase.ShowItemCheckboxesProperty, checkBoxBinding);
        }

        public bool NavigateBack()
        {
            if (HistoryIndex > 0)
            {
                HistoryIndex--;
                return true;
            }
            else return false;
        }

        public bool NavigateForward()
        {
            if (HistoryIndex < (HistoryList.Count - 1))
            {
                HistoryIndex++;
                return true;
            }
            else return false;
        }

        public bool NavigateUp()
        {
            bool returnValue = false;
            try
            {
                string path = Path.GetDirectoryName(CurrentPath);
                if (Directory.Exists(path))
                {
                    Navigate(path);
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return returnValue;
        }

        public void CopySelection()
        {
            Manager.Cut = false;
            SetClipboard();
        }

        public void CutSelection()
        {
            Manager.Cut = true;
            SetClipboard();
        }

        public void CopyPathToSelection()
        {
            var paths = "";
            for (var i = 0; i < CurrentDirectoryListView.SelectedItems.Count; i++)
            {
                var d = (DiskItem)(CurrentDirectoryListView.SelectedItems[i]);
                paths = paths + "\"" + d.ItemPath + "\"";
                if (i < (CurrentDirectoryListView.SelectedItems.Count - 1))
                {
                    paths = paths + "\n";
                }
            }
            Clipboard.SetText(paths);
        }

        public void PasteCurrent()
        {
            var items = Manager.CopyTo(HistoryList[HistoryIndex]);
            var source = ((List<DiskItem>)CurrentDirectoryListView.ItemsSource);
            foreach (var d in items)
            {
                if (source.Contains(d))
                {
                    source.Remove(d);
                }
                else
                {
                    source.Add(d);
                }
            }

            Refresh();
        }

        public void SetClipboard()
        {
            Manager.ClipboardContents.Clear();
            foreach (DiskItem d in CurrentDirectoryListView.SelectedItems)
            {
                Manager.ClipboardContents.Add(d);
            }
        }

        public void Refresh()
        {
            Navigate(CurrentPath);
        }

        public void PasteShortcut()
        {
            /*foreach (DiskItem d in CurrentDirectoryListView.SelectedItems)
            {
                Shortcut.CreateShortcut(d.ItemName + " - Shortcut", null, d.ItemPath, HistoryList[HistoryIndex]);
            }*/
            Refresh();
        }

        public void DeleteSelection()
        {
            //var source = ((List<DiskItem>)CurrentDirectoryListView.ItemsSource);
            for (var i = 0; i < CurrentDirectoryListView.SelectedItems.Count; i++)
            {
                var d = (CurrentDirectoryListView.SelectedItems[i] as DiskItem);

                /*if (source.Contains(d))
                {*/
                if (d.ItemCategory == DiskItem.DiskItemCategory.Directory)
                    FileSystem.DeleteDirectory(d.ItemPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                else if ((d.ItemCategory == DiskItem.DiskItemCategory.File) || (d.ItemCategory == DiskItem.DiskItemCategory.Shortcut))
                    FileSystem.DeleteFile(d.ItemPath, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                //}
            }

            Refresh();
        }

        public void RenameSelection()
        {
            IsRenamingFiles = true;
        }

        public void CreateNewFolder()
        {
            string path = CurrentPath + @"\New Folder";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
            {
                int cycle = 1;
                while (Directory.Exists(path))
                {
                    path = CurrentPath + @"\New Folder (" + cycle.ToString() + ")";
                    MessageBox.Show(path);
                    cycle++;
                }
                Directory.CreateDirectory(path);
            }
            Refresh();
        }

        public void ShowPropertiesForSelection()
        {
            foreach (DiskItem i in CurrentDirectoryListView.SelectedItems)
            {
                /*string path = i.ItemPath;

                /*if (Directory.Exists(path))
                    Manager.CreateWindow(path);
                else*
                try
                {
                    var info = new ProcessStartInfo(path)
                    {
                        Verb = "properties",
                        UseShellExecute = true
                    };
                    Process.Start(info);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }*/
                Debug.WriteLine("properties result: " + i.ShowProperties());
            }
        }

        public void OpenSelection()
        {
            OpenSelection(DiskItem.OpenVerbs.Normal);
        }

        public void OpenSelection(DiskItem.OpenVerbs verb)
        {
            //var source = ((List<DiskItem>)CurrentDirectoryListView.ItemsSource);
            foreach (DiskItem i in CurrentDirectoryListView.SelectedItems)
            {
                string path = i.ItemPath;

                if (Directory.Exists(path))
                {
                    if (CurrentDirectoryListView.SelectedItems.Count == 1)
                    {
                        Navigate(path);
                        break;
                    }
                    else
                    {
                        ////////Manager.CreateWindow(path);
                    }
                }
                else
                    try
                    {
                        //Process.Start(path);
                        i.Open(verb);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
            }
        }

        public void EditSelection()
        {

        }

        public void SelectAll()
        {
            CurrentDirectoryListView.SelectAll();
        }

        public void SelectNone()
        {
            CurrentDirectoryListView.SelectedItem = null;
        }

        public void InvertSelection()
        {

        }

        private void NavigationPaneTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var val = e.NewValue as DiskItem;

            if (val != null)
                Navigate(Environment.ExpandEnvironmentVariables(val.ItemPath));
        }


        public void Navigate(string targetPath)
        {
            /*if ((HistoryList.Count == 0) || (HistoryIndex >= (HistoryList.Count - 1)))
            {
                HistoryList.Add(path);
                HistoryIndex++;
            }
            else
            {
                HistoryList.Insert(HistoryIndex, path);

                while (HistoryList.Count > (HistoryIndex + 1))
                    HistoryList.RemoveAt(HistoryIndex + 1);
            }

            CurrentDirectoryListView.ItemsSource = new DiskItem(currentPath).SubItems;*/
            string path = Environment.ExpandEnvironmentVariables(targetPath);
            var item = new DiskItem(path);
            if (Directory.Exists(path))
            {
                ItemCounter.Content = item.SubItems.Count.ToString() + " items";

                if (!(HistoryList.Contains(path)))
                {
                    if (HistoryIndex < (HistoryList.Count - 1))
                    {
                        for (int i = HistoryList.Count - 1; i > HistoryIndex; i--)
                        {
                            HistoryList.RemoveAt(i);
                        }
                    }

                    HistoryList.Add(path);
                }

                HistoryIndex = HistoryList.IndexOf(path);
                
                SetPanes(item);

                CurrentDirectoryListView.ItemsSource = item.SubItems;
            }
            CurrentDirectoryListView_SelectionChanged(null, null);
            Navigated?.Invoke(this, null);
        }

        public void InitialNavigate(string path)
        {
            HistoryList.Add(path);
            Navigate(path);
        }

        private void CurrentDirectoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentDirectoryListView.SelectedItems.Count == 0)
            {
                //SetDetailsPane(HistoryList[HistoryIndex]);
                ////////CopyButton.IsEnabled = false;
                ////////CopyPathButton.IsEnabled = false;
                ////////CutButton.IsEnabled = false;
                ////////MoveToButton.IsEnabled = false;
                ////////CopyToButton.IsEnabled = false;
                ////////DeleteButton.IsEnabled = false;
                ////////RenameButton.IsEnabled = false;
                ////////OpenButton.IsEnabled = false;
                ////////EditButton.IsEnabled = false;
                SelectedItemCounter.Visibility = Visibility.Hidden;
                SetPanes(new DiskItem(CurrentPath));
            }
            else
            {
                /*if (CurrentDirectoryListView.SelectedItems.Count == 1)
                {
                    SetDetailsPane(((ObservableCollection<DiskItem>)CurrentDirectoryListView.ItemsSource)[CurrentDirectoryListView.SelectedIndex].ItemPath);
                }*/

                ////////CopyButton.IsEnabled = true;
                ////////CopyPathButton.IsEnabled = true;
                ////////CutButton.IsEnabled = true;
                ////////MoveToButton.IsEnabled = true;
                ////////CopyToButton.IsEnabled = true;
                ////////DeleteButton.IsEnabled = true;
                ////////RenameButton.IsEnabled = true;
                ////////OpenButton.IsEnabled = true;
                ////////EditButton.IsEnabled = true;
                SelectedItemCounter.Visibility = Visibility.Visible;
                SetPanes((DiskItem)(CurrentDirectoryListView.SelectedItem));
            }

            SelectedItemCounter.Content = CurrentDirectoryListView.SelectedItems.Count.ToString() + " items selected";

            CurrentDirectorySelectionChanged?.Invoke(sender, e);
        }

        public void SetPanes(DiskItem item)
        {
            /*DetailsFileIconBorder.Background*/
            Debug.WriteLine("ActualHeight: " + DetailsFileIconRectangle.ActualHeight.ToString());
            double size = DetailsFileIconRectangle.ActualHeight;
            if (size <= 0)
                size = 48;
            DetailsFileIconRectangle.Fill = (ImageBrush)((new Start9.UI.Wpf.Converters.IconToImageBrushConverter()).Convert(item.ItemJumboIcon, null, size.ToString(), null));
            if ((item.ItemCategory == DiskItem.DiskItemCategory.Directory) && (item.ItemRealName == Path.GetFileName(CurrentPath)))
                DetailsFileNameTextBlock.Text = item.SubItems.Count.ToString() + " items";
            else
                DetailsFileNameTextBlock.Text = item.ItemDisplayName;



            if (item.ItemPath.ToLowerInvariant() == CurrentPath.ToLowerInvariant())
            {
                SetPreviewPaneLayer(0);
            }
            else
            {
                string ext = Path.GetExtension(item.ItemPath).ToLowerInvariant();
                if (ext == "bmp" || ext == "png" || ext == "jpg" || ext == "jpeg")
                {
                    (PreviewPaneGrid.Children[2] as System.Windows.Shapes.Rectangle).Fill = new ImageBrush(new BitmapImage(new Uri(item.ItemPath, UriKind.RelativeOrAbsolute)));
                    SetPreviewPaneLayer(2);
                }
                else
                {
                    bool isMediaFile = true;
                    PreviewPlayer.Source = new Uri(item.ItemPath, UriKind.RelativeOrAbsolute);
                    PreviewPlayer.MediaFailed += (sneder, args) =>
                    {
                        isMediaFile = false;
                    };

                    if (isMediaFile)
                        SetPreviewPaneLayer(3);
                    else
                        SetPreviewPaneLayer(1);
                }
                /*else if (ext == "wav" || ext == "wma" || ext == "mp3" || ext == "m4a")
                {

                }
                else if (ext == "mp4" || ext == "wmv" || ext == "mp3" || ext == "m4a")
                {

                }*/
            }
        }

        void SetPreviewPaneLayer(int index)
        {
            for (int i = 0; i < PreviewPaneGrid.Children.Count; i++)
            {
                var control = PreviewPaneGrid.Children[i];
                if (i == index)
                    control.Visibility = Visibility.Visible;
                else
                    control.Visibility = Visibility.Collapsed;
            }
        }

        private void CurrentDirectoryListView_Item_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
        {
            if (CurrentDirectoryListView.SelectedItems.Count == 1)
            {
                string path = ((List<DiskItem>)CurrentDirectoryListView.ItemsSource)[CurrentDirectoryListView.SelectedIndex].ItemPath;
                if (Directory.Exists(path))
                    Navigate(path);
                else
                    try
                    {
                        //Process.Start(path);
                        ((List<DiskItem>)CurrentDirectoryListView.ItemsSource)[CurrentDirectoryListView.SelectedIndex].Open();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
            }
            else if (CurrentDirectoryListView.SelectedItems.Count > 1)
            {
                //var source = (List<DiskItem>)(CurrentDirectoryListView.ItemsSource);
                foreach (DiskItem i in CurrentDirectoryListView.SelectedItems)
                {
                    string path = i.ItemPath;

                    if (Directory.Exists(path))
                    {
                        ////////WindowManager.CreateWindow(path);
                    }
                    else
                        try
                        {
                            //Process.Start(path);
                            i.Open();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                }
            }
        }

        private void DetailsViewButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentView = FileBrowserView.Details;
            IconsViewButton.IsChecked = false;
            DetailsViewButton.IsChecked = true;
        }

        private void IconsViewButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentView = FileBrowserView.Icons;
            DetailsViewButton.IsChecked = false;
            IconsViewButton.IsChecked = true;
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            /*var item = (sender as MenuItem).Tag as ListViewItem;

            string path = ((List<DiskItem>)CurrentDirectoryListView.ItemsSource)[(item.Parent as ListView).Items.IndexOf(item)].ItemPath;*/
            foreach (DiskItem i in CurrentDirectoryListView.SelectedItems)
            {
                string path = i.ItemPath;

                if (Directory.Exists(path))
                {
                    Navigate(path);
                    break;
                }
                else
                    try
                    {
                        //Process.Start(path);
                        i.Open();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
            }
        }

        private void RunAsAdminMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //var item = (sender as MenuItem).Tag as ListViewItem;
            foreach (DiskItem i in CurrentDirectoryListView.SelectedItems)
            {
                string path = i.ItemPath; //((List<DiskItem>)CurrentDirectoryListView.ItemsSource)[(item.Parent as ListView).Items.IndexOf(item)].ItemPath;
                if (File.Exists(path))
                    try
                    {
                        /*Process.Start(new ProcessStartInfo(path)
                        {
                            Verb = "runas"
                        });*/
                        i.Open(DiskItem.OpenVerbs.Admin);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
            }
        }

        private void TouchableContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            bool canRunAsAdmin = true;
            foreach (DiskItem i in CurrentDirectoryListView.SelectedItems)
            {
                if (i.ItemCategory == DiskItem.DiskItemCategory.Directory)
                {
                    canRunAsAdmin = false;
                    break;
                }
            }

            foreach (MenuItem m in (sender as ContextMenu).Items)
            {
                if (m.Name == "RunAsAdminMenuItem")
                {
                    if (canRunAsAdmin)
                        m.Visibility = Visibility.Visible;
                    else
                        m.Visibility = Visibility.Collapsed;

                    break;
                }
            }
        }

        void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CopySelection();
        }

        void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CutSelection();
        }

        void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelection();
        }

        void PropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowPropertiesForSelection();
        }

        private void CurrentDirectoryListView_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.C)
                    CopySelection();
                else if (e.Key == Key.X)
                    CutSelection();
                else if (e.Key == Key.V)
                    PasteCurrent();
            }
            else if (e.Key == Key.Delete)
                DeleteSelection();
        }
    }
}
