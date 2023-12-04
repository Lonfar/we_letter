using BasicClass;
using IServices;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services
{
    public class LayoutService : ILayoutService
    {
        private readonly Context _context;
        public LayoutService(Context context)
        {
            _context = context;
        }


        #region 信函窗口布局
        /// <summary>
        /// 获取信函列表(for rep and ref)
        /// </summary>
        /// <returns></returns>
        public List<L_LetterMain> GetLetterID_For_Rep_Ref()
        {
            return _context.L_LetterMain.Where(x => x.Virtual == false).OrderBy(orderby => orderby.NO_AlWaha).ToList();
        }

        /// <summary>
        /// 加载所有标签
        /// </summary>
        /// <returns></returns>
        public List<L_Tags> GetTags()
        {
            return _context.L_Tags.OrderBy(orderby => orderby.Title.Length).ToList();
        }
        #endregion

        #region 主窗口布局
        /// <summary>
        /// 获得已被信函引用标签
        /// </summary>
        /// <returns></returns>
        public dynamic GetLetterTags()
        {
            var entity = (from letter_tags in _context.L_Letter_Tags
                          join letter in _context.L_LetterMain on letter_tags.Letter_ID equals letter.Id
                          join tags in _context.L_Tags on letter_tags.Tags_ID equals tags.Id
                          orderby tags.Title.Length
                          select new { tags.Id, tags.Title }).Distinct().ToList();
            return entity;
        }
        /// <summary>
        /// 根据选择的标签Id获得其他可选择的标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public dynamic GetFilterTags(dynamic obj)
        {
            var json = JsonConvert.DeserializeObject(obj.ToString());
            List<L_Tags> tagsJson_entity = JsonConvert.DeserializeObject<List<L_Tags>>(json["L_Tags"].ToString());
            var Letter_ID = (from letter_tags in _context.L_Letter_Tags
                             join tags in _context.L_Tags on letter_tags.Tags_ID equals tags.Id
                             where tagsJson_entity.Contains(tags)
                             select new { letter_tags.Letter_ID, letter_tags.Tags_ID })
                          .SelectMany(x => _context.L_Letter_Tags.Where(y => y.Tags_ID == x.Tags_ID).Select(z => z.Letter_ID))
                          .Distinct()
                          .ToList();
            var tags_entity = (from letter_tags in _context.L_Letter_Tags
                               join letter in _context.L_LetterMain on letter_tags.Letter_ID equals letter.Id
                               join tags in _context.L_Tags on letter_tags.Tags_ID equals tags.Id
                               orderby tags.Title.Length
                               where Letter_ID.Contains(letter_tags.Letter_ID)
                               select new { tags.Id, tags.Title }).Distinct().ToList();
            return tags_entity;

        }

        /// <summary>
        /// 获得所有信函，在Table中使用
        /// </summary>
        /// <returns></returns>
        public OperationResult Get_All_Letter()
        {
            //根据信函ID将letterTags中的Tags_ID与tags中的ID进行匹配，将匹配到的tags.Title进行拼接，拼接后的结果作为letterTags中的TagsTitle，用数组形式表现
            var entity = _context.L_LetterMain
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.type,
                    x.NO_AlWaha,
                    x.NO_MDOC,
                    x.Virtual,
                    x.date,
                    tags = _context.L_Letter_Tags
                        .Where(y => y.Letter_ID == x.Id)
                        .Select(z => _context.L_Tags.Where(a => a.Id == z.Tags_ID).Select(b => new { Id = b.Id, Title = b.Title }).FirstOrDefault())
                        .ToArray()
                })
                .OrderBy(x => x.NO_AlWaha)
                .ToList();

            if (entity.Count > 0)
            {
                return new OperationResult(OperationResultEnum.Success, "查询成功", entity);
            }
            else
            {
                return new OperationResult(OperationResultEnum.QueryNull, "没有任何数据");
            }
        }
        #endregion

        #region 选择信函后生成相关的nodes和lines数据

        /// <summary>
        /// 递归获取相关信函的ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<object> Get_Relevant_Letter(Guid id)
        {
            List<Guid> relatedLetterIds = new List<Guid>(); // 用于存储相关信函的ID
            GetRelatedLetters(id, relatedLetterIds); // 递归获取相关信函的ID

            // 在relatedLetters集合中获取相关的L_LetterMain数据，并进行进一步的操作
            var relatedLetters = _context.L_LetterMain
                    .Where(letter => relatedLetterIds.Contains(letter.Id))
                    .Select(x => new
                    {
                        x.Id,
                        x.Title,
                        x.Description,
                        x.type,
                        x.NO_AlWaha,
                        x.NO_MDOC,
                        x.Virtual,
                        x.date,
                    })
                    //.OrderBy(x => x.Id == id ? 0 : 1) // 将当前信函排在第一位
                    .ToList<object>();
            return relatedLetters;
        }

        // 递归函数，获取与给定信函ID相关的所有信函的ID
        private void GetRelatedLetters(Guid letterId, List<Guid> relatedLetterIds)
        {
            relatedLetterIds.Add(letterId); // 将当前信函ID添加到相关信函ID列表中

            var lines = _context.L_Lines
                .Where(line => line.From_ID == letterId || line.To_ID == letterId)
                .ToList();

            // 遍历当前信函的所有关联线条
            foreach (var line in lines)
            {
                Guid relatedLetterId = line.From_ID == letterId ? line.To_ID : line.From_ID;

                // 如果相关信函ID尚未添加到列表中，则递归获取与之相关的信函
                if (!relatedLetterIds.Contains(relatedLetterId))
                {
                    GetRelatedLetters(relatedLetterId, relatedLetterIds);
                }
            }
        }

        /// <summary>
        ///  根据信函ids获取相关的nodes数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<object> Get_Relevant_Lines(string ids)
        {
            var id = JsonConvert.DeserializeObject<List<Guid>>(ids);
            var entity = _context.L_Lines.Where(x => id.Contains(x.From_ID) || id.Contains(x.To_ID)).ToList<object>();
            return entity;
        }


        #endregion
    }
}
