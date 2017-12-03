using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        RAM ram;
        ObservableCollection<Instruction> instructions;
        ObservableCollection<Register> registers;
        private object account_id;

        public MainWindow()
        {
            InitializeComponent();

            //initialize parts
            ram = new RAM();
            instructions = new ObservableCollection<Instruction>();
            registers = new ObservableCollection<Register>();
            InitializeRegisters(ref registers);

            //set data binding
            RegistersListView.ItemsSource = registers;
            InstructionsListView.ItemsSource = instructions;
        }

        private void InitializeRegisters(ref ObservableCollection<Register> registers)
        {
            registers.Clear();
            registers.Add(new Register(0, "zero"));
            registers.Add(new Register(1, "at"));
            registers.Add(new Register(2, "v0"));
            registers.Add(new Register(3, "v1"));
            registers.Add(new Register(4, "a0"));
            registers.Add(new Register(5, "a1"));
            registers.Add(new Register(6, "a2"));
            registers.Add(new Register(7, "a3"));
            registers.Add(new Register(8, "t0"));
            registers.Add(new Register(9, "t1"));
            registers.Add(new Register(10, "t2"));
            registers.Add(new Register(11, "t3"));
            registers.Add(new Register(12, "t4"));
            registers.Add(new Register(13, "t5"));
            registers.Add(new Register(14, "t6"));
            registers.Add(new Register(15, "t7"));
            registers.Add(new Register(16, "s0"));
            registers.Add(new Register(17, "s1"));
            registers.Add(new Register(18, "s2"));
            registers.Add(new Register(19, "s3"));
            registers.Add(new Register(20, "s4"));
            registers.Add(new Register(21, "s5"));
            registers.Add(new Register(22, "s6"));
            registers.Add(new Register(23, "s7"));
            registers.Add(new Register(24, "t8"));
            registers.Add(new Register(25, "t9"));
            registers.Add(new Register(26, "k0"));
            registers.Add(new Register(27, "k1"));
            registers.Add(new Register(28, "gp"));
            registers.Add(new Register(29, "sp"));
            registers.Add(new Register(30, "fp"));
            registers.Add(new Register(31, "ra"));
            registers.Add(new Register(32, "PC"));
            registers.Add(new Register(33, "HI"));
            registers.Add(new Register(34, "LO"));
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

                //load programe to ram
                LoadPrograme(programeFilePath);

                //initialize PC
                registers[32].Value = 1000;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("程序文件导入失败！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                ProgrameFilePathTextBox.Text = "";
                return;
            }
        }

        private void ImportDataButton_Click(object sender, RoutedEventArgs e)
        {
            string dataFilePath;

            //select file saving path
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
                DialogResult dialogResult = openFileDialog.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK && openFileDialog.FileName != null)
                {
                    dataFilePath = openFileDialog.FileName;
                    DataFilePathTextBox.Text = dataFilePath;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("数据文件导入失败！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                DataFilePathTextBox.Text = "";
                return;
            }
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void LoadPrograme(string programeFilePath)
        {
            instructions.Clear();

            //save programe into address begin from 1000
            FileStream programeFile;
            byte[] programeFileBytes;
            string programeFileContent;
            try
            {
                programeFile = new FileStream(programeFilePath, FileMode.Open, FileAccess.Read);
                programeFileBytes = new byte[programeFile.Length];
                programeFile.Read(programeFileBytes, 0, (int)programeFile.Length);
                programeFileContent = Encoding.Default.GetString(programeFileBytes);

                //ignore annotation
                string pattern = @"(/\*)((.|\n)*)(\*/)";
                Regex rgx = new Regex(pattern);
                programeFileContent = rgx.Replace(programeFileContent, " ");

                int count = 0, address = 1000 / 4;
                StringBuilder tmpSB = new StringBuilder();
                foreach (var item in programeFileContent)
                {
                    if (item >= '0' && item <= '9')
                    {
                        tmpSB.Append(item);
                        count++;
                        if (count == 32)
                        {
                            Instruction instruction = new Instruction();
                            instruction.MachineCode = ConvertFromBinaryStringToUInt(tmpSB.ToString());
                            instruction.AssemblyCode = Decode(instruction.MachineCode);
                            instructions.Add(instruction);
                            ram.Set4Bit(address++, instruction.MachineCode);
                            count = 0;
                            tmpSB.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("加载程序文件出错！请检查文件是否有效！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                ProgrameFilePathTextBox.Text = "";
                return;
            }
        }

        public void LoadData(string dataFilePath)
        {
            //save data into address begin from 2000
            FileStream dataFile;
            byte[] dataFileBytes;
            string dataFileContent;
            try
            {
                dataFile = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read);
                dataFileBytes = new byte[dataFile.Length];
                dataFile.Read(dataFileBytes, 0, (int)dataFile.Length);
                dataFileContent = Encoding.Default.GetString(dataFileBytes);

                //ignore annotation
                string pattern = @"(/\*)((.|\n)*)(\*/)";
                Regex rgx = new Regex(pattern);
                dataFileContent = rgx.Replace(dataFileContent, " ");

                int count = 0, address = 2000 / 4;
                StringBuilder tmpSB = new StringBuilder();
                foreach (var item in dataFileContent)
                {
                    if (item >= '0' && item <= '9')
                    {
                        tmpSB.Append(item);
                        count++;
                        if (count == 32)
                        {
                            uint data = Convert.ToUInt32(tmpSB.ToString());
                            ram.Set4Bit(address++, data);
                            count = 0;
                            tmpSB.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("加载数据文件出错！请检查文件是否有效！\n详细信息：" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                DataFilePathTextBox.Text = "";
                return;
            }
        }

        private uint ConvertFromBinaryStringToUInt(string s)
        {
            uint result = 0;
            for (int i = 0; i < 32; i++)
            {
                result += (((uint)s[31 - i]) - 48) * Pow(2, i);
            }
            return result;
        }

        private uint Pow(uint x, int i)
        {
            uint result = 1;
            for (int j = 0; j < i; j++)
            {
                result *= x;
            }
            return result;
        }

        public string Decode(uint machineCode)
        {
            //TODO  decode machineCode to assemblyCode
            string assemblyCode = "";
            uint op = machineCode >> 26;
            if (op == (0x0))
            {
                //R-type instructions
                uint rs = (machineCode & (0x03E00000)) >> 21;
                uint rt = (machineCode & (0x001F0000)) >> 16;
                uint rd = (machineCode & (0x0000F800)) >> 11;
                uint shamt = (machineCode & (0x000007C0)) >> 6;
                uint func = (machineCode & (0x0000003F)) >> 0;
                switch (func)
                {
                    case (0x20):
                        //add
                        assemblyCode = String.Format("add ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x21):
                        //addu
                        assemblyCode = String.Format("addu ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x22):
                        //sub
                        assemblyCode = String.Format("sub ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x23):
                        //subu
                        assemblyCode = String.Format("subu ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x24):
                        //and
                        assemblyCode = String.Format("and ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x25):
                        //or
                        assemblyCode = String.Format("or ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x26):
                        //xor
                        assemblyCode = String.Format("xor ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x27):
                        //nor
                        assemblyCode = String.Format("nor ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x2A):
                        //slt
                        assemblyCode = String.Format("slt ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x2B):
                        //sltu
                        assemblyCode = String.Format("sltu ${0},${1},${2}", rd, rs, rt);
                        break;
                    case (0x00):
                        //sll
                        assemblyCode = String.Format("sll ${0},${1},{2}", rd, rt, shamt);
                        break;
                    case (0x02):
                        //srl
                        assemblyCode = String.Format("srl ${0},${1},{2}", rd, rt, shamt);
                        break;
                    case (0x03):
                        //sra
                        assemblyCode = String.Format("sra ${0},${1},{2}", rd, rt, shamt);
                        break;
                    case (0x04):
                        //sllv
                        assemblyCode = String.Format("sllv ${0},${1},${2}", rd, rt, rs);
                        break;
                    case (0x06):
                        //srlv
                        assemblyCode = String.Format("srlv ${0},${1},${2}", rd, rt, rs);
                        break;
                    case (0x07):
                        //srav
                        assemblyCode = String.Format("srav ${0},${1},${2}", rd, rt, rs);
                        break;
                    case (0x08):
                        //jr
                        assemblyCode = String.Format("jr ${0}", rs);
                        break;
                    default:
                        break;
                }
            }
            else if (op == (0x2))
            {
                //J-type instruction j
                uint address = (machineCode & (0x03FFFFFF)) >> 0;
                assemblyCode = String.Format("j ${0}", address);
            }
            else if (op == (0x03))
            {
                //J-type instruction jal
                uint address = (machineCode & (0x03FFFFFF)) >> 0;
                assemblyCode = String.Format("jal ${0}", address);
            }
            else
            {
                //I-type instructions
                uint rs = (machineCode & (0x03E00000)) >> 21;
                uint rt = (machineCode & (0x001F0000)) >> 16;
                uint immediate = (machineCode & (0x0000FFFF)) >> 0;
                switch (op)
                {
                    case (0x08):
                        //addi
                        assemblyCode = String.Format("addi ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x09):
                        //addiu
                        assemblyCode = String.Format("addiu ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x0C):
                        //andi
                        assemblyCode = String.Format("andi ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x0D):
                        //ori
                        assemblyCode = String.Format("ori ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x0E):
                        //xori
                        assemblyCode = String.Format("xori ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x0F):
                        //lui
                        assemblyCode = String.Format("lui ${0},{1}", rt, immediate);
                        break;
                    case (0x23):
                        //lw
                        assemblyCode = String.Format("lw ${0},{1}({2})", rt, immediate, rs);
                        break;
                    case (0x2B):
                        //sw
                        assemblyCode = String.Format("sw ${0},{1}({2})", rt, immediate, rs);
                        break;
                    case (0x04):
                        //beq
                        assemblyCode = String.Format("beq ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x05):
                        //bne
                        assemblyCode = String.Format("bne ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x0A):
                        //slti
                        assemblyCode = String.Format("slti ${0},${1},{2}", rt, rs, immediate);
                        break;
                    case (0x0B):
                        //sltiu
                        assemblyCode = String.Format("sltiu ${0},${1},{2}", rt, rs, immediate);
                        break;
                    default:
                        break;
                }
            }
            return assemblyCode;
        }

        public bool Execute(uint machineCode)
        {
            //TODO  execute instruction
            return true;
        }
    }
}
