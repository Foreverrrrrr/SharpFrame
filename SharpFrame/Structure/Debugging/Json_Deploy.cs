using System.Collections.Generic;

namespace SharpFrame.Structure.Debugging
{
    public class Json_Deploy
    {
        public List<Input_Json> Input { get; set; } = new List<Input_Json>();
        public List<Output_Json> Output { get; set; } = new List<Output_Json>();
        public List<Axis_Json> Axis { get; set; } = new List<Axis_Json>();

    }
}
