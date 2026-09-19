using MediatorSharp;
using MediatorSharp.Sample.Models;
using MediatorSharp.Sample.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MediatorSharp.Sample.Controllers
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

        [HttpGet("test3")]
        public async Task<Result<Test>> GetTest3()
        {
            var req = new TestRequest(1);
            return await _mediator.SendAsync(req);
        }

        [HttpGet("test4")]
        public async Task<Result> GetTest4()
        {
            var req = new Test2Request(1);
            return await _mediator.SendAsync(req);
        }
    }
}
