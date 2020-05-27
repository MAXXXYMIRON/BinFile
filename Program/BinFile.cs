using System;
using System.IO;
namespace Program
{
    /// <summary>
    /// У казывает на начальную, текущую или последнюю
    /// позицию в файле, соответственно.
    /// </summary>
    public enum Seek { BEG, CUR, END }

    class BinFile
    {
        /// <summary>
        /// Перемення для записи в файл.
        /// </summary>
        BinaryWriter write;
        /// <summary>
        /// Переменная для чтения из файла.
        /// </summary>
        BinaryReader read;
        /// <summary>
        /// Размер записи.
        /// </summary>
        int size;


        /// <summary>
        /// Название файла.
        /// </summary>
        public readonly string FileName;

        /// <summary>
        /// Кол-во записей в файле.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Текущая позиция в файле.
        /// </summary>
        public long Position { get; private set; }


        /// <summary>
        /// Конструктор с заданием имени файла.
        /// </summary>
        public BinFile(string fileName)
        {
            FileName = fileName;
            Length = 0;
            Position = 0;
            size = 4;
        }

        /// <summary>
        /// Проверка условия конца файла.
        /// </summary>
        public bool EOF => (Position == Length);

        /// <summary>
        /// Установить позицию в файле относительно указанной позиции.
        /// </summary>
        public void Positioning(long pos, Seek seek = Seek.CUR)
        {
            if(seek == Seek.BEG)
            {
                if (pos > Length)
                    throw new Exception("Позиция выходит за пределы файла.");
                if (pos < 0)
                    throw new Exception("Смещение не может быть отрицательным.");

                Position = pos;
            }
            else if(seek == Seek.CUR)
            {
                if((pos + Position) >= Length || (pos + Position) < 0)
                    throw new Exception("Позиция выходит за пределы файла.");

                Position += pos;

            }
            else
            {
                if ((pos + Length) < 0)
                    throw new Exception("Позиция выходит за пределы файла.");
                if (pos > 0)
                    throw new Exception("Смещение не может быть положительным.");

                Position = Length + pos;
            }
        }

        /// <summary>
        /// Очистить файл.
        /// </summary>
        public void Clear()
        {
            using (write = new BinaryWriter(new FileStream(FileName, FileMode.Truncate))) { }
            Length = 0;
            Position = 0;
        }

        /// <summary>
        /// Установить позицию в начало файла.
        /// </summary>
        public void Reset()
        {
            Position = 0;
            using (write = new BinaryWriter(new FileStream(FileName, FileMode.OpenOrCreate)))
            {
                write.BaseStream.Position = Position; 
            }
        }



        /// <summary>
        /// Записать в файл данные data, начиная с position
        /// по умолчанию position - это текущая позиция в файле.
        /// </summary>
        public void Write(int[] data, long position = -1)
        {
            if (position >= 0) Positioning(position, Seek.BEG);
            using (write = new BinaryWriter(new FileStream(FileName, FileMode.OpenOrCreate)))
            {
                write.BaseStream.Position = Position * size;
                for (int i = 0; i < data.Length; i++)
                {
                    write.Write(data[i]);
                }
                Position = write.BaseStream.Position / size;
                Length = write.BaseStream.Length / size;
            }
        }

        /// <summary>
        /// Записать в файл данные data, начиная с position
        /// по умолчанию position - это текущая позиция в файле.
        /// </summary>
        public void Write(int data, long position = -1)
        {
            if (position >= 0) Positioning(position, Seek.BEG);
            using (write = new BinaryWriter(new FileStream(FileName, FileMode.OpenOrCreate)))
            {
                write.BaseStream.Position = Position * size;
                write.Write(data);

                Position = write.BaseStream.Position / size;
                Length = write.BaseStream.Length / size;
            }
        }

        /// <summary>
        /// Читать данные, начиная с position
        /// по умолчанию position - это текущая позиция в файле.
        /// </summary>
        public int[] Read(long n, long position = -1)
        {
            if(position >= 0) Positioning(position, Seek.BEG);

            int[] data = new int[n];

            using (read = new BinaryReader(new FileStream(FileName, FileMode.OpenOrCreate)))
            {
                read.BaseStream.Position = Position * size;

                for (int i = 0; i < n; i++)
                {
                    data[i] = read.ReadInt32();
                }

                Position = read.BaseStream.Position / size;
            }
            return data;
        }

        /// <summary>
        /// Читать данные начиная с текущей позиции в файле,
        /// до конца.
        /// </summary>
        public int[] Read()
        {
            int[] data = new int[Length - Position];

            using (read = new BinaryReader(new FileStream(FileName, FileMode.OpenOrCreate)))
            {
                read.BaseStream.Position = Position * size;

                for (long i = 0; i < data.Length; i++)
                {
                    data[i] = read.ReadInt32();
                }

                Position = read.BaseStream.Position / size;
            }

            return data;
        }

        /// <summary>
        /// Читать данные, начиная с position
        /// по умолчанию position - это текущая позиция в файле.
        /// </summary>
        public int Read(long position = -1)
        {
            if (position >= 0) Positioning(position, Seek.BEG);

            int data;

            using (read = new BinaryReader(new FileStream(FileName, FileMode.OpenOrCreate)))
            {
                read.BaseStream.Position = Position * size;

                data = read.ReadInt32();

                Position = read.BaseStream.Position / size;
            }
            return data;
        }
    }
}
