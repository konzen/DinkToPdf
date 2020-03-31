using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf.Contracts;
using System.IO;

namespace DinkToPdf.TestWebServer.Controllers
{
    [Route("api/values")]
    public class ConvertController : Controller
    {
        private DinkToPdf.Contracts.IConverter _converter;

        public ConvertController(DinkToPdf.Contracts.IConverter converter)
        {
            _converter = converter;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A3,
                    Orientation = Orientation.Landscape,
                },

                Objects = {
                    new ObjectSettings()
                    {
                        Page = "https://google.com/"
                    },
                     new ObjectSettings()
                    {
                        Page = "https://github.com/"
                    }
                }
            };
           
            byte[] pdf = _converter.Convert(doc);


            return new FileContentResult(pdf, "application/pdf");
        }
    }
}
