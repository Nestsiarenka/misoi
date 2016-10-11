using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Exceptions
{
    class ImageException : Exception
    {
        public ImageException(string message) : base(message) {}

        public ImageException(string message, Exception innerException) : base(message, innerException){}

        protected ImageException(SerializationInfo info, StreamingContext context) : base(info, context){}
    }
}
