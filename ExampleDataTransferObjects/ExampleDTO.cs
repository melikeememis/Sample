using System;

namespace ExampleDataTransferObjects
{
 
    [Serializable]
    public class ExampleDTO
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}