using EmailService.API.Services;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos.EmailDtos;

namespace EmailService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController(IEmailService _emailService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(EmailMessage model)
        {
            await _emailService.SendMessageAsync(model);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageWithCredentials(MessageWithCredentials model)
        {
            await _emailService.SendMessageWithCredentialsAsync(model.Message, model.Settings);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageWithAttachments(MessageWithAttachment model)
        {
            await _emailService.SendMessageWithAttachmentsAsync(model.Message, model.Attachments);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageWithCredentialsAndAttachments(MessageWithCredentialsAndAttachment model)
        {
            await _emailService.SendMessageWithCredentialsAndAttachmentsAsync(model.Message, model.Attachments, model.Settings);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendBulkMessage(BulkEmailMessage model)
        {
            await _emailService.SendBulkMessageAsync(model);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendBulkMessageWithCredentials(BulkMessageWithCredentials model)
        {
            await _emailService.SendBulkMessageWithCredentialsAsync(model.Message, model.Settings);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendBulkMessageWithAttachments(BulkMessageWithAttachments model)
        {
            await _emailService.SendBulkMessageWithAttachmentsAsync(model.Message, model.Attachments);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendBulkMessageWithCredentialsAndAttachments(BulkMessageWithCredentialsAndAttachments model)
        {
            await _emailService.SendBulkMessageWithCredentialsAndAttachmentsAsync(model.Message, model.Attachments, model.Settings);
            return Ok();
        }
    }
}