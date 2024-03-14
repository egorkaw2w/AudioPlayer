using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

        }
        public bool pause = true;
        public bool infinite = false;
        string[] files;
        List<string> filesList;
        List<string> history = new List<string>();



        private void Prew_Click(object sender, RoutedEventArgs e)
        {
            Media.Source = new Uri(filesList[filesList.IndexOf(Media.Source.ToString().Replace("file:///", "").Replace("/", "\\")) - 1]);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {

            if (pause)
            {
                Media.Pause();
            }
            else
            {
                Media.Play();
            }
            pause = !pause;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            save();
            nextTrack();
        }

        private void Again_Click(object sender, RoutedEventArgs e)
        {
            infinite = !infinite;

        }

        private void MusicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Media.Source = new Uri((string)MusicList.SelectedItem);
            Media.Play();
        }

        private void Source_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog() { IsFolderPicker = true };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                files = Directory.GetFiles(dialog.FileName);
                filesList = new List<string>(files.Where(item => item.EndsWith(".mp3") || item.EndsWith(".wav")));
                MusicList.ItemsSource = filesList;
                Media.Source = new Uri(files[0]);
                Media.Play();
            }


        }
        public void nextTrack()
        {
   
            Media.Source = new Uri(filesList[filesList.IndexOf(Media.Source.ToString().Replace("file:///", "").Replace("/", "\\")) + 1]);
        }
        public void prewTrack()
        {

            Media.Source = new Uri(filesList[filesList.IndexOf(Media.Source.ToString().Replace("file:///", "").Replace("/", "\\")) - 1]);
        }
        public void save()
        {
            history.Add(filesList[filesList.IndexOf(Media.Source.ToString().Replace("file:///", "").Replace("/", "\\"))]);
        }



        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (Media.NaturalDuration.HasTimeSpan)
            {
                Track.Maximum = Media.NaturalDuration.TimeSpan.Ticks;
                save();
                Thread slideMoving = new Thread(_ =>
                {
                    while (true)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Track.Value = Media.Position.Ticks;
                            if (Media.NaturalDuration.HasTimeSpan)
                            {
                                if (Media.Position == Media.NaturalDuration.TimeSpan && infinite == false)
                                {
                                    nextTrack();
                                }
                                else if (Media.Position == Media.NaturalDuration.TimeSpan && infinite == true)
                                {
                                    nextTrack();
                                    prewTrack();
                                }
                            }
                        });
                        Thread.Sleep(1000);
                    }
                });
                slideMoving.Start();
            }
            
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
            {

            Media.Position = TimeSpan.FromTicks((long)Track.Value);
            }

        private void Sound_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (filesList != null)
            {
            Media.Volume = Sound.Value;
            }
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            History window = new History();
            window.Istoria.ItemsSource = history;
            if (window.ShowDialog() == true)
            {
                Media.Source = new Uri (window.music);
                Media.Play();
            }
        }

    }
    } 
