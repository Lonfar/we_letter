using Models;
using IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using BasicClass;
using System.Text.Json;
using Newtonsoft.Json;
using System.Runtime.InteropServices.JavaScript;

namespace VueApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILayoutService _LayoutService;
        private readonly ILetterService _LetterService;

        public HomeController(ILogger<HomeController> logger, ILayoutService LayoutService, ILetterService LetterService)
        {
            _logger = logger;
            _LayoutService = LayoutService;
            _LetterService = LetterService;
        }

        #region 主页面布局
        /// <summary>
        /// 获得已被信函引用标签
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLetterTags")]
        public string GetLetterTags()
        {
            return JsonConvert.SerializeObject(_LayoutService.GetLetterTags());
        }

        /// <summary>
        /// 根据选择的标签Id获得其他可选择的标签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("GetFilterTags")]
        public string GetFilterTags([FromBody] dynamic obj)
        {
            try
            {
                return JsonConvert.SerializeObject(_LayoutService.GetFilterTags(obj));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 获得所有数据给前台的Table

        /// <summary>
        /// 获得所有信函
        /// </summary>
        /// <returns></returns>
        [HttpGet("Get_All_Letter")]
        public string Get_All_Letter()
        {
            return JsonConvert.SerializeObject(_LayoutService.Get_All_Letter().AppendData);
        }

        /// <summary>
        /// 获得选择信函的相关信函
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("Get_Relevant_Letter")]
        public string Get_Relevant_Letter([FromQuery] Guid id)
        {
            try
            {
                return JsonConvert.SerializeObject(_LayoutService.Get_Relevant_Letter(id));
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 获得相关信函的Lines
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet("Get_Relevant_Lines")]
        public string Get_Relevant_Lines([FromQuery] string ids)
        {
            try
            {
                return JsonConvert.SerializeObject(_LayoutService.Get_Relevant_Lines(ids));
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 信函侧边框窗口布局
        /// <summary>
        /// 加载所有标签
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTags")]
        public string GetTags()
        {
            return JsonConvert.SerializeObject(_LayoutService.GetTags());
        }

        /// <summary>
        /// 获取信函列表(for rep and ref)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLetterID_For_Rep_Ref")]
        public string GetLetterID_For_Rep_Ref()
        {
            return JsonConvert.SerializeObject(_LayoutService.GetLetterID_For_Rep_Ref().Select(item => new { item.Id, item.NO_AlWaha, item.NO_MDOC }));
        }
        #endregion

        /// <summary>
        ///  添加信函
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("AddLetter")]
        public string Post([FromBody] dynamic obj)
        {
            try
            {
                OperationResult result = _LetterService.AddLetter(obj);
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\upload", file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { file.FileName });
        }



        // PUT <HomeController>/5 修改
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE <HomeController>/5 删除
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
