using BasicClass;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServices
{
    public interface ILayoutService
    {
        public dynamic GetLetterTags();
        public dynamic GetFilterTags(dynamic obj);
        public List<L_Tags> GetTags();
        public List<L_LetterMain> GetLetterID_For_Rep_Ref();
        public OperationResult Get_All_Letter();
        public List<object> Get_Relevant_Letter(Guid id);
        public List<object> Get_Relevant_Lines(string ids);
    }
}
