using BasicClass;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IServices
{
    public interface ILetterService
    {
        public L_LetterMain GetLetter(Guid id);
        public OperationResult AddLetter(dynamic form);
    }
}