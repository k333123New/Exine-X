using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExineUnpacker
{
    struct ByteReader
    {
        public int index;
        public byte[] buffer;

        public ByteReader(string path)
        {
            index = 0;

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
        }

        public ByteReader(in byte[] buffer)
        {
            this.buffer = buffer;
            this.index = 0;
        }

        public ByteReader(in byte[] buffer, in int index)
        {
            this.buffer = buffer;
            this.index = index;
        }


        public bool Read(out byte data)
        {
            if (buffer.Length < index + 1)
            {
                data = 0;
                Console.WriteLine("!!Error!!: over index from buffer");
                return true;
            }

            data = buffer[index];

            index += 1;
            return false;
        }

        public bool Read(out short data)
        {
            if (buffer.Length < index + 2)
            {
                data = 0;
                Console.WriteLine("!!Error!!: over index from buffer");
                return true;
            }

            data = BitConverter.ToInt16(buffer, index);

            index += 2;
            return false;
        }

        public bool Read(out ushort data)
        {
            if (buffer.Length < index + 2)
            {
                data = 0;
                Console.WriteLine("!!Error!!: over index from buffer");
                return true;
            }

            data = BitConverter.ToUInt16(buffer, index);

            index += 2;
            return false;
        }

        public bool Read(out int data)
        {
            if (buffer.Length < index + 4)
            {
                data = 0;
                Console.WriteLine("!!Error!!: over index from buffer");
                return true;
            }

            data = BitConverter.ToInt32(buffer, index);

            index += 4;
            return false;
        }

        public bool Read(out uint data)
        {
            if (buffer.Length < index + 4)
            {
                data = 0;
                Console.WriteLine("!!Error!!: over index from buffer");
                return true;
            }

            data = BitConverter.ToUInt32(buffer, index);

            index += 4;
            return false;
        }

        public bool Read(out bool data)
        {
            if (buffer.Length < index + 4)
            {
                data = false;
                Console.WriteLine("!!Error!!: over index from buffer");
                return true;
            }

            data = BitConverter.ToBoolean(buffer, index);

            index += 4;
            return false;
        }
    }
}
