using System;
using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Exceptions;

namespace ImageProcessingLibrary.Capacities.Structures
{
    public struct Gray : ICapacity
    {
        public byte? G;

        public Gray(byte? g = null)
        {
            G = g;
        }

        public void Initialize(object value)
        {
            try
            {
                G = Convert.ToByte(value);
            }
            catch (InvalidCastException e)
            {
                throw new CapacityException("Invalid value for Gray initialization, byte is expected", e);
            }
        }

        public bool IsEmpty()
        {
            return G == null;
        }
    }
}