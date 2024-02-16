using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewYPF
{
    class FileInfo
    {
        byte[] filename = new byte[32];
        uint startIdx = 0;
        uint endIdx = 0;
        byte[] payload = null;

        public string Filename 
        {
            get {
                //string temp = Encoding.Default.GetString(filename);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                string temp = Encoding.GetEncoding(51949).GetString(filename);
                temp = temp.Remove(temp.IndexOf('\0'));
                return temp;
            }
        }

        public byte[] Payload { get => payload; }


        public int FillData(byte[] datas, int idx)
        {
            //1 번째 파일--파일위치:3280 
            //2 번째 파일--파일위치:18091 
            //90 번째 파일--파일위치:1160926 

            startIdx = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;

            Buffer.BlockCopy(datas, idx, filename, 0, filename.Length);
            idx = idx + 32;
             
            if (datas.Length <= idx)
            {
                Console.WriteLine("END!");
                return idx;
            }
            else endIdx = BitConverter.ToUInt32(datas, idx);

            Console.WriteLine("datas.Len:" + datas.Length + " StartIDX :" + startIdx + " Filename:"+Filename+" EndIdx:" + endIdx);

            payload = new byte[endIdx - startIdx];

            Console.WriteLine("datas.Len:" + datas.Length + " StartIDX :" + startIdx + " EndIdx:"+ endIdx+" Payload.Len:" + payload.Length);
            Buffer.BlockCopy(datas, (int)startIdx, payload, 0, payload.Length);

            return idx;
        }

    }
    class DATFormat
    {
        uint fileCount = 0;
        FileInfo[] fileInfos = null;

        public DATFormat(byte[] datas)
        {
            int idx = 0;
            fileCount = BitConverter.ToUInt32(datas, idx) - 1;
            idx = idx + 4;
            Console.WriteLine("FileCount:" + fileCount);

           
            fileInfos = new FileInfo[fileCount];
            for(int i=0;i<fileInfos.Length;i++)
            {
                fileInfos[i] = new FileInfo();
                idx = fileInfos[i].FillData(datas, idx);
            } 
        } 

        public void Save()
        {
            DirectoryInfo di = new DirectoryInfo(".\\DAT_OUT");
            if (!di.Exists) di.Create();

            foreach (var fileInfo in fileInfos)
            {
                Console.WriteLine(fileInfo.Filename+" Len:"+fileInfo.Payload.Length);
                File.WriteAllBytes(".\\DAT_OUT\\" + fileInfo.Filename, fileInfo.Payload);
            }
        }
    }

   
}
