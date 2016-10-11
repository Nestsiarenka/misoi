using System;
using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Exceptions;

namespace ImageProcessingLibrary.Capacities.Structures
{
    public struct RGB : ICapacity
    {
        public byte? R;
        public byte? G;
        public byte? B;

        public RGB(byte? r = null, byte? g = null, byte? b = null)
        {
            R = r;
            G = g;
            B = b;
        }

        public bool IsEmpty()
        {
            return R == null && G == null && B == null;
        }

        public void Initialize(object value)
        {
            try
            {
                byte[] byteArrayValue = (byte[]) value;

                R = byteArrayValue[0];
                G = byteArrayValue[1];
                B = byteArrayValue[2];
            }
            catch (IndexOutOfRangeException e)
            {
                throw new CapacityException("Error in RGB initialization, byte[3] value is expected", e);
            }
            catch (InvalidCastException e)
            {
                throw new CapacityException("Error in RGB initialization, byte[3] value is expected", e);
            }
        }
    }
}