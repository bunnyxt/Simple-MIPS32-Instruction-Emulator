using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SimpleMIPS32InstructionEmulator
{
    public class RAM : INotifyPropertyChanged
    {
        private int size;

        public int Size
        {
            get { return size; }
            set { size = value; OnPropertyChanged(new PropertyChangedEventArgs("Size")); }
        }

        private uint[] storage;

        public uint[] Storage
        {
            get { return storage; }
            set { storage = value; OnPropertyChanged(new PropertyChangedEventArgs("Storage")); }
        }

        public RAM()
        {
            this.Size = 1048576;//1 MB RAM
            this.Storage = new uint[262144];
        }

        public byte Get1Bit(int address)
        {
            uint whole = Get4Bit(address);
            byte part = 0;
            switch (address % 4)
            {
                case 0:
                    part = (byte)((whole & (0x000000FF)) >> 0);
                    break;
                case 1:
                    part = (byte)((whole & (0x0000FF00)) >> 8);
                    break;
                case 2:
                    part = (byte)((whole & (0x00FF0000)) >> 16);
                    break;
                case 3:
                    part = (byte)((whole & (0xFF000000)) >> 24);
                    break;
                default:
                    break;
            }
            return part;
        }

        public uint Get4Bit(int address)
        {
            address %= this.Size;//modify those address which exceed max RAM size  
            return Storage[address / 4];//index = first address of this 4 Bit / 4
        }

        public void Set1Bit(int address, byte target)
        {
            uint whole = Get4Bit(address);
            uint part = target;
            switch (address % 4)
            {
                case 0:
                    whole = whole & (0xFFFFFF00);
                    part = part << 0;
                    whole = whole | part;
                    break;
                case 1:
                    whole = whole & (0xFFFF00FF);
                    part = part << 8;
                    whole = whole | part;
                    break;
                case 2:
                    whole = whole & (0xFF00FFFF);
                    part = part << 16;
                    whole = whole | part;
                    break;
                case 3:
                    whole = whole & (0x00FFFFFF);
                    part = part << 24;
                    whole = whole | part;
                    break;
                default:
                    break;
            }
            Set4Bit(address, whole);
        }

        public void Set4Bit(int address, uint target)
        {
            address %= this.Size;//modify those address which exceed max RAM size
            Storage[address / 4] = target;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }

    public class Instruction : INotifyPropertyChanged
    {
        private uint machineCode;

        public uint MachineCode
        {
            get { return machineCode; }
            set { machineCode = value; OnPropertyChanged(new PropertyChangedEventArgs("MachineCode")); }
        }

        private string assemblyCode;

        public string AssemblyCode
        {
            get { return assemblyCode; }
            set { assemblyCode = value; OnPropertyChanged(new PropertyChangedEventArgs("AssemblyCode")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }

    public class Register : INotifyPropertyChanged
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged(new PropertyChangedEventArgs("Id")); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
        }

        private uint value;

        public uint Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged(new PropertyChangedEventArgs("Value")); }
        }

        public Register(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Value = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }

    public class Watch : INotifyPropertyChanged
    {
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; OnPropertyChanged(new PropertyChangedEventArgs("Address")); }
        }

        private TextBlock value;

        public TextBlock Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged(new PropertyChangedEventArgs("Value")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }

}
