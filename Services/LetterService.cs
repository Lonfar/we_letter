using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IServices;
using Models;
using BasicClass;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Services
{
    public class LetterService : ILetterService
    {
        private readonly Context _context;

        public LetterService(Context context)
        {
            _context = context;
        }
        /// <summary>
        /// 添加信函
        /// </summary>
        /// <param name="letter"></param>
        /// <param name="annexes"></param>
        /// <returns></returns>
        public OperationResult AddLetter(dynamic obj)
        {
            //将前台数据转换为JSON对象
            var json = JsonConvert.DeserializeObject(obj.ToString());

            #region 信函类的操作
            //转换为信函类
            L_LetterMain letter = JsonConvert.DeserializeObject<L_LetterMain>(json["L_LetterMain"].ToString());
            if (letter.Id == Guid.Empty)
            {
                letter.Id = Guid.NewGuid();
                if (letter.NO_MDOC != null && !letter.NO_MDOC.Contains("MDOC"))
                {
                    letter.NO_MDOC = "MDOC-" + letter.NO_MDOC;
                }
                letter.Virtual = false;
            }
            //添加信函
            _context.L_LetterMain.Add(letter);
            #endregion

            #region 标签类的操作
            //转换为标签类
            IEnumerable<L_Tags> tags = JsonConvert.DeserializeObject<IEnumerable<L_Tags>>(json["L_Tags"].ToString());
            //添加标签
            tags = tags.Select(item =>
             {
                 if (item.Id == Guid.Empty)
                 {
                     item.Id = Guid.NewGuid();
                     _context.L_Tags.Add(item);
                 }
                 return item;
             });
            //添加信函与标签的关联
            foreach (var item in tags)
            {
                _context.L_Letter_Tags.Add(new L_Letter_Tags()
                {
                    Id = Guid.NewGuid(),
                    Letter_ID = letter.Id,
                    Tags_ID = item.Id
                });
            }
            #endregion

            #region 引用类的操作
            //转换为引用类
            IEnumerable<L_LetterMain> letter_template_Rep = JsonConvert.DeserializeObject<IEnumerable<L_LetterMain>>(json["Rep"].ToString());
            if (letter_template_Rep.Count() > 0)
            {
                letter_template_Rep = letter_template_Rep.Select(item =>
                {
                    item.Id = Guid.NewGuid();
                    _context.L_Lines.Add(new L_Lines()
                    {
                        Id = Guid.NewGuid(),
                        From_ID = letter.Id,
                        To_ID = item.Id,
                        Text = "回函"
                    });
                    return item;
                });
                _context.L_LetterMain.AddRange(letter_template_Rep);
            }

            IEnumerable<L_LetterMain> letter_template_Ref = JsonConvert.DeserializeObject<IEnumerable<L_LetterMain>>(json["Ref"].ToString());
            if (letter_template_Ref.Count() > 0)
            {
                letter_template_Ref = letter_template_Ref.Select(item =>
                {
                    item.Id = Guid.NewGuid();
                    _context.L_Lines.Add(new L_Lines()
                    {
                        Id = Guid.NewGuid(),
                        From_ID = letter.Id,
                        To_ID = item.Id,
                        Text = "引用"
                    });
                    return item;
                });
                _context.L_LetterMain.AddRange(letter_template_Ref);
            }


            IEnumerable<L_Lines> lines = JsonConvert.DeserializeObject<IEnumerable<L_Lines>>(json["L_Lines"].ToString());
            if (lines.Count() > 0)
            {
                lines = lines.Select(item =>
                {
                    item.Id = Guid.NewGuid();
                    item.From_ID = letter.Id;
                    return item;
                });
                _context.L_Lines.AddRange(lines);
            }
            #endregion

            #region 附件类的操作
            //转换为附件类
            IEnumerable<L_Annexes> annexes = JsonConvert.DeserializeObject<IEnumerable<L_Annexes>>(json["L_Annexes"].ToString());
            if (annexes.Count() > 0)
            {
                annexes = annexes.Select(item =>
                {
                    item.Id = Guid.NewGuid();
                    item.Letter_Id = letter.Id;
                    item.PhysicalAddress = @"\wwwroot\upload\" + item.FileName + "." + item.FileSuffix;
                    return item;
                });
                _context.L_Annexes.AddRange(annexes);
            }
            #endregion

            if (_context.SaveChanges() > 0)
            {
                return new OperationResult(OperationResultEnum.Success, "添加成功");
            }
            else
            {
                return new OperationResult(OperationResultEnum.NoChanged, "操作没有变化");
            }
        }

        public L_LetterMain GetLetter(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}