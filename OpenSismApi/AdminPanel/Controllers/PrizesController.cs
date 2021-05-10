using AutoMapper;
using DBContext.Models;
using DBContext.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace AdminPanel.Controllers
{
    public class PrizesController : BaseController
    {
        private readonly OpenSismDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PrizesController(OpenSismDBContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

    }
}
