using System.Buffers.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IServices
{
    public interface IAnnexesService
    {
        /// <summary>
        /// 查看文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns>二进制文件</returns>
        public string ViewFile(Guid id, string path, string fileName);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileBinary">文件二进制</param>
        /// <returns></returns>
        public bool UploadFile(string path, string fileName, byte fileBinary);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns>二进制文件</returns>
        public string DownloadFile(Guid id, string path, string fileName);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public bool DeleteFile(Guid id, string path, string fileName);
    }
}