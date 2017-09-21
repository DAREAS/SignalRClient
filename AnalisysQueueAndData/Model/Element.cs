using System;
using System.Collections.Generic;
using System.Text;

namespace AnalisysQueueAndData.Model
{
    public class Element
    {
        public Element()
        {
           Id = new Guid().ToString(); 
           DataMessage = new List<DataMessage>(); 
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string BodyElement { get; set; }

        public List<DataMessage> DataMessage { get; set; }
    }
}
