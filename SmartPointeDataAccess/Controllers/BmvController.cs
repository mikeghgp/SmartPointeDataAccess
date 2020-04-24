using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartPointe.DataAccess;
using SmartPointe.DTOs;
using SmartPointe.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SmartPointe.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BmvController : ControllerBase
    {
        private readonly NtreisDataContext _ndb;
        private IOptionsMonitor<SPOptions> _optionsAccessor;
        public BmvController(NtreisDataContext ndb, IOptionsMonitor<SPOptions> opt)
        {
            _ndb = ndb;
            _optionsAccessor = opt;
        }

        [HttpGet]
        [Route("ntreis/active/properties/{id}/image/large/{img}")]
        [Produces("image/jpeg")]
        public IActionResult GetActivePropertyImageLarge(int id, int img = 0)
        {
            return GetActivePropertyImage(id, img, 1);
        }

        [HttpGet]
        [Route("ntreis/active/properties/{id}/image/high/{img}")]
        [Produces("image/jpeg")]
        public IActionResult GetActivePropertyImageHigh(int id, int img = 0)
        {
            return GetActivePropertyImage(id, img, 2);
        }

        [HttpGet]
        [Route("ntreis/active/properties/{id}/image/{img}")]
        [Produces("image/jpeg")]
        public IActionResult GetActivePropertyImage(int id, int img = 0)
        {
            return GetActivePropertyImage(id, img, 0);
        }

        private IActionResult GetActivePropertyImage(int id, int img = 0,int size = 0)
        {
            StringBuilder baseUrl = new StringBuilder(_optionsAccessor.CurrentValue.Option3);
            if(size == 0)
                baseUrl.Append(_optionsAccessor.CurrentValue.Option4);
            else if(size == 1)
                baseUrl.Append(_optionsAccessor.CurrentValue.Option5);
            else if(size==2)
                baseUrl.Append(_optionsAccessor.CurrentValue.Option6);

            baseUrl.Append(id);
            baseUrl.Append(":");
            baseUrl.Append(img);

            NtreisRETsOptions ntreisOptions = new NtreisRETsOptions();
            ntreisOptions.BaseUrl = baseUrl.ToString();

            NtreisRETS ntreisRETS = new NtreisRETS(_optionsAccessor.CurrentValue.Option1, _optionsAccessor.CurrentValue.Option2);


            return File(ntreisRETS.GetImage(ntreisOptions).GetAwaiter().GetResult(), "image/jpeg");
        }

        [HttpGet]
        [Route("ntreis/active/properties/{id}")]
        public IActionResult GetActiveProperty(int id)
        {
            return Ok("GetActiveProperty");
        }

        [HttpGet]
        [Route("ntreis/active/properties")]
        public IActionResult GetActive(int page = 1)
        {
            return Ok(

                GetNtreisProps(true, page)
            ) ;
        }

        [HttpGet]
        [Route("properties/active/{id}")]
        public IActionResult GetActiveByMlsID(int id)
        {
            //return GetProps(true);
            bool active = true;
            NtreisRETsOptions ntreisOptions = new NtreisRETsOptions();
            ntreisOptions.BaseUrl = _optionsAccessor.CurrentValue.Option3;

            ntreisOptions.QueryActive = active;
            ntreisOptions.QueryMlsId = id;

            NtreisRETS ntreisRETS = new NtreisRETS(_optionsAccessor.CurrentValue.Option1, _optionsAccessor.CurrentValue.Option2);

            return Ok(new
            {
                Data = ntreisRETS.Search(ntreisOptions).GetAwaiter().GetResult()
            });
        }

        
        private PagedResult<ResidentialProperty> GetNtreisProps(bool active, int page)
        {
            NtreisRETsOptions ntreisOptions = new NtreisRETsOptions();
            ntreisOptions.BaseUrl = _optionsAccessor.CurrentValue.Option3;

            ntreisOptions.QueryActive = active;
            ntreisOptions.Offset = page;
            NtreisRETS ntreisRETS = new NtreisRETS(_optionsAccessor.CurrentValue.Option1, _optionsAccessor.CurrentValue.Option2);
            return ntreisRETS.Search(ntreisOptions).GetAwaiter().GetResult();
        }
        
    }
}
