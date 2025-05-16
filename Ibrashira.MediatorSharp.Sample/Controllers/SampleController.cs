using MediatorSharp.lib.Interfaces;
using MediatorSharp.lib.Models;
using MediatorSharp.Mediator;
using MediatorSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediatorSharp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SampleController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet("test1")]
        public Result<Test> Get()
        {
            var req = new TestRequest(1);
            return _mediator.Send(req);            
        }

        [HttpGet("test2")]
        public Result GetTest2()
        {
            var req = new Test2Request(1);
            return _mediator.Send(req);
        }
    }
}
