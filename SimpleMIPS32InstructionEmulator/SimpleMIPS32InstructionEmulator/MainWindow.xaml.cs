using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleMIPS32InstructionEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Instruction> instructions;
        ObservableCollection<Register> registers;

        public MainWindow()
        {
            InitializeComponent();

            //initialize containers
            InitializeRegisters(ref registers);

            //set data binding
            RegistersListView.ItemsSource = registers;
        }

        private void InitializeRegisters(ref ObservableCollection<Register> registers)
        {
            registers = new ObservableCollection<Register>
            {
                new Register(0, "zero"),
                new Register(1, "at"),
                new Register(2, "v0"),
                new Register(3, "v1"),
                new Register(4, "a0"),
                new Register(5, "a1"),
                new Register(6, "a2"),
                new Register(7, "a3"),
                new Register(8, "t0"),
                new Register(9, "t1"),
                new Register(10, "t2"),
                new Register(11, "t3"),
                new Register(12, "t4"),
                new Register(13, "t5"),
                new Register(14, "t6"),
                new Register(15, "t7"),
                new Register(16, "s0"),
                new Register(17, "s1"),
                new Register(18, "s2"),
                new Register(19, "s3"),
                new Register(20, "s4"),
                new Register(21, "s5"),
                new Register(22, "s6"),
                new Register(23, "s7"),
                new Register(24, "t8"),
                new Register(25, "t9"),
                new Register(26, "k0"),
                new Register(27, "k1"),
                new Register(28, "gp"),
                new Register(29, "sp"),
                new Register(30, "fp"),
                new Register(31, "ra")
            };
        }

        private void ImportProgrameButton_Click(object sender, RoutedEventArgs e)
        {
            string programeFilePath;

            //select file saving path
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
                DialogResult dialogResult = openFileDialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK && openFileDialog.FileName != null)
                {
                    programeFilePath = openFileDialog.FileName;
                    ProgrameFilePathTextBox.Text = programeFilePath;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("程序文件导入失败！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
