using System.Net;
using System.Runtime.InteropServices;
using IServices;
using Models;

namespace Services
{
    public class AnnexesService : IAnnexesService
    {
        private readonly Context _context;

        public AnnexesService(Context context)
        {
            _context = context;
        }

        public bool DeleteFile(Guid id, string path, string fileName)
        {
            throw new NotImplementedException();
        }

        public string DownloadFile(Guid id, string path, string fileName)
        {
            throw new NotImplementedException();
        }

        public bool UploadFile(string path, string fileName, byte fileBinary)
        {
            System.IO.Stream stream = new System.IO.MemoryStream();
             throw new NotImplementedException();
        }

        public string ViewFile(Guid id, string path, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}