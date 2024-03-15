using ExineEffectConvert;

class Program
{
    static void Main(string[] args)
    {
        int euckrCodePage = 51949;  
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        System.Text.Encoding euckr = System.Text.Encoding.GetEncoding(euckrCodePage); 
        Console.WriteLine("test");

        string filename = "Effect00.ypf";
        Ypf.UnYpf(filename, ".");
        filename = "Effect01.ypf";
        Ypf.UnYpf(filename, ".");
        filename = "Effect02.ypf";
        Ypf.UnYpf(filename, ".");
        filename = "Effect03.ypf";
        Ypf.UnYpf(filename, ".");

    }
}