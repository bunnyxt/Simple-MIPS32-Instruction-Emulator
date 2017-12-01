using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMIPS32InstructionEmulator
{
    public class Emulator
    {


        public Emulator()
        {

        }
    }

    public class RAM
    {

    }

    public class Instruction
    {
        public int MachineCode { get; set; }
        public string AssemblyCode { get; set; }
    }

    public class Register
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public Register(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Value = 0;
        }
    }
}
