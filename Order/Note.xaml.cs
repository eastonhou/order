
using Order.Utility;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Order
{
    /// <summary>
    /// Interaction logic for Note.xaml
    /// </summary>
    public partial class Note : UserControl
    {
        protected string NotePath { get; private set; }
        private DispatcherTimer Timer = new DispatcherTimer();
        private bool NoteChangedFlag;
        public Note()
        {
            InitializeComponent();
            NotePath = Path.Combine(Environment.ExecutionFolder, "note.txt");
            if(File.Exists(NotePath))
                noteBox.Text = File.ReadAllText(NotePath);
            NoteChangedFlag = false;
        }

        private void Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateNote(sender, e);
        }

        private void UpdateNote(object sender, System.EventArgs e)
        {
            if(NoteChangedFlag)
            {
                File.WriteAllText(NotePath, noteBox.Text);
                NoteChangedFlag = false;
            }
        }

        private void NoteChanged(object sender, TextChangedEventArgs e)
        {
            NoteChangedFlag = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Timer.Interval = new System.TimeSpan(0, 0, 10);
            Timer.Tick += UpdateNote;
            Timer.Start();
            Window.GetWindow(this).Closing += Closing;
        }
    }
}
